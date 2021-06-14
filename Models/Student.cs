using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubReportDemo.Models
{
    public class Student
    {
        public int ID { get; set; }

        public int DepartmentId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }
    }
}