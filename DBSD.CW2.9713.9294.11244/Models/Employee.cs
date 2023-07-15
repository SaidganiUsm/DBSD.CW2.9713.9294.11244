using System;

namespace DBSD.CW2._9713._9294._11244.Models
{
    public class Employee
    {
        public int? EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsMale { get; set; }
        public bool IsMarried { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public int SectorId { get; set; }
        public int TotalRowsCount { get; set; } = 1;
    }

}
