using Dapper;
using DBSD.CW2._9713._9294._11244.Interfaces;
using DBSD.CW2._9713._9294._11244.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace DBSD.CW2._9713._9294._11244.DAL
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public string _connStr;

        public EmployeeRepository(string connStr) 
        {
            _connStr= connStr;
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
                                      FROM [dbo].[Employee]";

        private const string SQL_SELECT_BY_ID = @"SELECT [EmployeeId]
                                          ,[FirstName]
                                          ,[LastName]                                          
                                          ,[IsMale]
                                          ,[IsMarried]
                                          ,[Age]
                                          ,[Address]
                                          ,[BirthDate]
                                      FROM [dbo].[Employee]
                                      WHERE EmployeeId = @EmployeeId";

        private const string SQL_INSERT = @"INSERT INTO Employee(
                                          ,[FirstName]
                                          ,[LastName]                                          
                                          ,[IsMale]
                                          ,[IsMarried]
                                          ,[Age]
                                          ,[Address]
                                          ,[BirthDate]) VALUES (
                                            @FirstName,
                                            @LastName,
                                            @IsMale,
                                            @IsMarried,
                                            @Age,
                                            @Address,
                                            @BirthDate
                                            )";

        private const string SQL_UPDATE = @"UPDATE Employee SET
                                            [FirstName] = @FirstName
		                                    ,[LastName]  = @LastName
		                                    ,[IsMale] = @IsMale
		                                    ,[IsMarried] = @IsMarried
		                                    ,[Age] = @Age
		                                    ,[Address] = @Address
		                                    ,[BirthDate] = @BirthDate 
	                                    where EmployeeId = @EmployeeId";


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

        public int Insert(Employee employee)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<int>(SQL_INSERT, employee);
            }
        }

        public void Update(Employee Employee)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Execute(SQL_UPDATE, Employee);
            }
        }

        public IList<Employee> ImportXml(string xml)
        {
            throw new NotImplementedException();
        }

        public IList<Employee> Insert(IEnumerable<Employee> employees)
        {
            throw new NotImplementedException();
        }

        public IList<Employee> Filter(string firstName, string lastName, int age, string address, int sectorId, out int totalRows, int page = 1, int pageSize = 10, string sortColumn = "EmployeeId", bool sortDesc = false)
        {
            throw new NotImplementedException();
        }

        public string ExportAsXml()
        {
            throw new NotImplementedException();
        }

        public string ExportAsJson()
        {
            throw new NotImplementedException();
        }

        public IList<Employee> ImportCsv(string csv)
        {
            throw new NotImplementedException();
        }
    }
}
