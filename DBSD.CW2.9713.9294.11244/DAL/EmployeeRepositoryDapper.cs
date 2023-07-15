using DBSD.CW2._9713._9294._11244.Interfaces;
using DBSD.CW2._9713._9294._11244.Models;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using Microsoft.SqlServer.Server;
using Microsoft.AspNetCore.Mvc;

namespace DBSD.CW2._9713._9294._11244.DAL
{
    public class EmployeeRepositoryDapper : IEmployeeRepository
    {
        public string _connStr;

        public EmployeeRepositoryDapper(string connStr)
        {
            _connStr = connStr;
        }
        private const string SQL_DELETE = @"DELETE FROM Employee 
                                            WHERE EmployeeId = @EmployeeId";

        private const string SQL_SELECT_ALL = @"SELECT [EmployeeId]
                                          ,[FirstName]
                                          ,[LastName]                                          
                                          ,[IsMale]
                                          ,[IsMarried]
                                          ,[Age]
                                          ,[Address]
                                          ,[BirthDate]
                                          ,[SectorId]
                                      FROM [dbo].[Employee]";

        private const string SQL_SELECT_BY_ID = @"SELECT [EmployeeId]
                                          ,[FirstName]
                                          ,[LastName]                                          
                                          ,[IsMale]
                                          ,[IsMarried]
                                          ,[Age]
                                          ,[Address]
                                          ,[BirthDate]
                                          ,[SectorId]
                                      FROM [dbo].[Employee]
                                      WHERE EmployeeId = @EmployeeId";

        private const string SQL_INSERT = @"INSERT INTO Employee(
                                          [FirstName]
                                          ,[LastName]                                          
                                          ,[IsMale]
                                          ,[IsMarried]
                                          ,[Age]
                                          ,[Address]
                                          ,[BirthDate]
                                          ,[SectorId]) VALUES (
                                            @FirstName,
                                            @LastName,
                                            @IsMale,
                                            @IsMarried,
                                            @Age,
                                            @Address,
                                            @BirthDate,
                                            @SectorId
                                            )";

        private const string SQL_UPDATE = @"UPDATE Employee SET
                                            [FirstName] = @FirstName
		                                    ,[LastName]  = @LastName
		                                    ,[IsMale] = @IsMale
		                                    ,[IsMarried] = @IsMarried
		                                    ,[Age] = @Age
		                                    ,[Address] = @Address
		                                    ,[BirthDate] = @BirthDate
                                            ,[SectorId] = @SectorId
	                                    WHERE EmployeeId = @EmployeeId";

        private const string SQL_FILTER = @"SELECT [EmployeeId]
                                          ,[FirstName]
                                          ,[LastName]
                                          ,[Age]
                                          ,[Address]
                                          ,[BirthDate]
                                          ,[SectorId]
	                                      ,count(*) over() TotalRowsCount
                                            FROM [dbo].[Employee]
                                            WHERE FirstName LIKE coalesce(@FirstName, '') + '%'
	                                            AND LastName LIKE coalesce(@Lastname, '') + '%'
                                                AND Age >= @Age
                                                AND Address LIKE coalesce(@Address, '') + '%'
                                                AND SectorId >= @SectorId
                                            ORDER BY {0}
                                            offset @OffsetRows rows
                                            fetch next @PageSize rows only";


        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Execute(SQL_DELETE, new { EmployeeId = id });
            }
        }

        public Employee GetById(int id)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<Employee>(SQL_SELECT_BY_ID, new { EmployeeId = id });
            }
        }

        public IList<Employee> GetAllEmployees()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.Query<Employee>(SQL_SELECT_ALL).ToList();
            }
        }

        public int Insert(Employee emp)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<int>(SQL_INSERT, emp);
            }
        }

        public void Update(Employee emp)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Execute(SQL_UPDATE, emp);
            }
        }

        public IList<Employee> ImportXml(string xml)
        {
            using var conn = new SqlConnection(_connStr);
            return conn.Query<Employee>("udpImportEmployeeFromXml", 
                commandType: System.Data.CommandType.StoredProcedure,
                param: new {xml = xml}).AsList();
        }

        public IList<Employee> ImportCsv(string csv)
        {
            using var conn = new SqlConnection(_connStr);
            return conn.Query<Employee>("udpEmployeeCsvImport",
                commandType: System.Data.CommandType.StoredProcedure,
                param: new {csv = csv }).AsList();
        }

        public IList<Employee> Insert(IEnumerable<Employee> employees)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                SqlMetaData[] recordDef = {
                    new SqlMetaData("FirstName", SqlDbType.VarChar, 30),
                    new SqlMetaData("LastName", SqlDbType.VarChar, 30),
                    new SqlMetaData("IsMale", SqlDbType.Bit),
                    new SqlMetaData("IsMarried", SqlDbType.Bit),
                    new SqlMetaData("Age", SqlDbType.Int),
                    new SqlMetaData("Address", SqlDbType.VarChar, 50),
                    new SqlMetaData("BirthDate", SqlDbType.DateTime)
                };

                var records = new List<SqlDataRecord>();

                foreach (var e in employees)
                {
                    var r = new SqlDataRecord(recordDef);

                    r.SetString(0, e.FirstName);
                    r.SetString(1, e.LastName);
                    r.SetBoolean(2, e.IsMale);
                    r.SetBoolean(3, e.IsMarried);
                    r.SetInt32(4, e.Age);
                    r.SetString(5, e.Address);
                    if (e.BirthDate.HasValue)
                        r.SetDateTime(6, e.BirthDate.Value);
                    else
                        r.SetDBNull(6);

                    records.Add(r);
                }

                return conn.Query<Employee>("procBulkInsertEmployee",
                 commandType: System.Data.CommandType.StoredProcedure, param: new { Emps = records.AsTableValuedParameter() }
                ).AsList();
            }

        }

        public IList<Employee> Filter(string firstName, string lastName, int age, string address, 
            int sectorId, out int totalRows, int page = 1, int pageSize = 10, 
            string sortColumn = "EmployeeId", bool sortDesc = false)
        {
            if (page <= 0)
                page = 1;

            var sort = "EmployeeId";
            if ("EmployeeId".Equals(sortColumn))
                sort = "EmployeeId";
            else if ("FirstName".Equals(sortColumn))
                sort = "FirstName";

            if (sortDesc)
                sort += " DESC ";

            string sql = string.Format(SQL_FILTER, sort);

            using var conn = new SqlConnection(_connStr);
            var employees = conn.Query<Employee>(
                sql,
                new
                {
                    FirstName = firstName,
                    Lastname = lastName,
                    Age = age,
                    Address = address,
                    SectorId = sectorId,
                    OffsetRows = (page - 1) * pageSize,
                    PageSize = pageSize
                }).AsList();

            totalRows = employees.FirstOrDefault()?.TotalRowsCount ?? 0;

            return employees;
        }

        public string ExportAsXml()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.ExecuteScalar<string>("select dbo.udfEmployeeExportAsXml()");
            }
        }

        public string ExportAsJson()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.ExecuteScalar<string>("udpEmployeeExportAsJson");
            }
        }
    }
}
