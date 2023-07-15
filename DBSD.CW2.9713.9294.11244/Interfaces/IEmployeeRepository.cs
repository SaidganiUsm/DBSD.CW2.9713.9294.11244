using DBSD.CW2._9713._9294._11244.Models;
using System;
using System.Collections.Generic;
using X.PagedList.Mvc.Core;

namespace DBSD.CW2._9713._9294._11244.Interfaces
{
    public interface IEmployeeRepository
    {
        string ExportAsXml();
        string ExportAsJson();
        IList<Employee> ImportCsv(string csv);
        IList<Employee> ImportXml(string xml);
        IList<Employee> Insert(IEnumerable<Employee> employees);

        IList<Employee> GetAllEmployees();

        Employee GetById(int id);

        int Insert(Employee emp);

        void Update(Employee emp);

        void Delete(int id);

        IList<Employee> Filter(string firstName,
            string lastName,
            int age,
            string address,
            int sectorId,
            out int totalRows,
            int page = 1,
            int pageSize = 10,
            string sortColumn = "EmployeeId",
            bool sortDesc = false);
    }
}
