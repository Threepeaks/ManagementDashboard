using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManagementDashboard.Models
{
    public class BirthdayMessageVM
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public bool IsLandmark { get; set; }
        public int Age { get; set; }
    }

    public class EmployeeBirthdayDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirthday { get; set; }

    }
}