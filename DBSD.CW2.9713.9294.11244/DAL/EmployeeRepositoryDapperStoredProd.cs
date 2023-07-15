using Dapper;
using DBSD.CW2._9713._9294._11244.Interfaces;
using DBSD.CW2._9713._9294._11244.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DBSD.CW2._9713._9294._11244.DAL
{
    public class EmployeeRepositoryDapperStoredProd : IEmployeeRepository
    {
        private const string SQL_DELETE = @"delete from employee 
                                            where employeeid = @EmployeeId";
        private readonly string _connStr;
        public EmployeeRepositoryDapperStoredProd(string connStr)
        {
            _connStr = connStr;
        }
        public EmployeeRepositoryDapperStoredProd()
        {
        }

        public IList<Employee> GetAllEmployees()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.Query<Employee>("dbo.udpGetEmployees").AsList();
            }
        }


        public Employee GetById(int id)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.Query<Employee>("dbo.udpGetById",
                    new { Id = id },
                    commandType: CommandType.StoredProcedure
                    ).FirstOrDefault();
            }
        }

        public int Insert(Employee emp)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                return conn.QueryFirstOrDefault<int>(
                    "dbo.udpInsert",
                    new
                    {
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        IsMale = emp.IsMale,
                        IsMarried = emp.IsMarried,
                        Age = emp.Age,
                        Address = emp.Address,
                        BirthDate = emp.BirthDate,
                        SectorId = emp.SectorId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }


        public void Update(Employee emp)
        {

            using (var conn = new SqlConnection(_connStr))
            {
                var p = new DynamicParameters();
                p.Add("EmployeeId", emp.EmployeeId, DbType.Int32);
                p.Add("FirstName", emp.FirstName);
                p.Add("LastName", emp.LastName);
                p.Add("IsMale", emp.IsMale, DbType.Boolean);
                p.Add("Age", emp.Age, DbType.Int32);
                p.Add("Address", emp.Address);
                p.Add("BirthDate", emp.BirthDate);
                p.Add("SectorId", emp.SectorId, DbType.Int32);
                p.Add("Errors", dbType: DbType.String, size: 1000, direction: ParameterDirection.Output);
                p.Add("RetVal", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                conn.Execute("dbo.udpUpdateEmployee", p, commandType: CommandType.StoredProcedure);

                string errors = p.Get<string>("Errors");
                int retVal = p.Get<int>("RetVal");

                if (retVal != 0)
                    throw new Exception($"Updated failed! status={retVal}, errors={errors}");
            }

        }

        public void Delete(int id)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Execute(SQL_DELETE, new { EmployeeId = id });
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
            using (var conn = new SqlConnection(_connStr))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FirstName", firstName);
                parameters.Add("@LastName", lastName);
                parameters.Add("@Age", age);
                parameters.Add("@Address", address);
                parameters.Add("ASectorId", sectorId);
                parameters.Add("@TotalCount",
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);
                parameters.Add("@Page", page);
                parameters.Add("@PageSize", pageSize);

                var employees = conn.Query<Employee>(
                    "udpEmployeeFilter",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                totalRows = parameters.Get<int>("@TotalCount");
                return employees.AsList();

            }
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
                return conn.Execute("udpEmployeeExportAsJson").ToString();
            }
        }

        public IList<Employee> ImportCsv(string csv)
        {
            throw new NotImplementedException();
        }
    }
}
