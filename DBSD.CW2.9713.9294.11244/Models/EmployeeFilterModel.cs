using System;
using System.Collections;
using System.Collections.Generic;
using X.PagedList;

namespace DBSD.CW2._9713._9294._11244.Models
{
    public class EmployeeFilterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public int SectorId { get; set; }

        public IList<Employee> Employees;
        public int TotalRows { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 2;
        public bool SortDesc { get; set; } = false;
        public string SortColumn { get; set; } = "EmployeeId";
    }
}
