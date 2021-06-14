using Microsoft.Reporting.WebForms;
using SubReportDemo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SubReportDemo.Controllers
{
    public class ReportController : Controller
    {
        private  IEnumerable<Department> departments = new List<Department>()
        {
            new Department() {ID = 1, Name = "Computer Science" },
            new Department() {ID = 2, Name = "Software Engineering" },
            new Department() {ID = 3, Name = "Statistics" },
            new Department() {ID = 4, Name = "Applied Mathematics" },
            new Department() {ID = 5, Name = "Physics" },
        };

        private  List<Student> students = new List<Student>()
        {
            new Student() {ID = 1, DepartmentId = 1, Name = "Robin", Gender = "male", Email="Robin@gmail.com"},
            new Student() {ID = 2, DepartmentId = 1, Name = "Tahira",Gender = "female", Email="Tahira@gmail.com" },
            new Student() {ID = 3, DepartmentId = 1, Name = "Shohag", Gender = "male", Email ="Shohag@gmail.com" },
            new Student() {ID = 4, DepartmentId = 1, Name = "Sayed", Gender = "male", Email="Sayed@gmail.com" },
            new Student() {ID = 5, DepartmentId = 2, Name = "Raju", Gender ="male", Email ="Raju@gmail.com" },
            new Student() {ID = 6, DepartmentId = 2, Name = "Emon", Gender ="male", Email ="Emon@gmail.com" },
            new Student() {ID = 7, DepartmentId = 2, Name = "Kabita", Gender ="female", Email ="Kabita@gmail.com" },
            new Student() {ID = 8, DepartmentId = 3, Name = "Mustafa", Gender ="male", Email ="Mustafa@gmail.com" },
            new Student() {ID = 9, DepartmentId = 3, Name = "Shakil", Gender ="male", Email ="Shakil@gmail.com" },
            new Student() {ID = 10, DepartmentId = 4, Name = "Nazmun", Gender ="female", Email ="Nazmun@gmail.com" },
            new Student() {ID = 11, DepartmentId = 4, Name = "Kareem", Gender ="male", Email ="Kareem@gmail.com" },
            new Student() {ID = 12, DepartmentId = 5, Name = "Mithu", Gender ="male", Email ="Mithu@gmail.com" }
        };

        // GET: Report
        public ActionResult Index()
        {
            var reportViewer = new LocalReport { EnableExternalImages = true };
            string reportPath = "~/Rdlc";
            string rdlcName = "MainReport.rdlc";
            
            reportViewer.ReportPath = Path.Combine(Server.MapPath(reportPath), rdlcName);
            reportViewer.DataSources.Clear();
            
            ReportHeader head = new ReportHeader
            {
                UniversityName = "XYZ University",
                ReportName = "Department Details",
                Logo = new Uri(Server.MapPath("~/Images/logo.jpg")).AbsoluteUri
            };
            
            // Call to Sub Report
            reportViewer.SubreportProcessing += (sender, e) => ReportViewer_SubreportProcessing(sender, e, students);

            var rptHead = new ReportDataSource("ReportHeader", ToDataTable(head));
            reportViewer.DataSources.Add(rptHead);
            
            //Match Details With Alias
            var dataSource = new ReportDataSource("Department", departments);
            reportViewer.DataSources.Add(dataSource);
            reportViewer.EnableHyperlinks = true;

            reportViewer.Refresh();

            string mimeType;
            var renderedBytes = ReportUtility.RenderedReportViewer(reportViewer, "PDF", out mimeType, head.ReportName);
            return File(renderedBytes, mimeType);
        }

        private void ReportViewer_SubreportProcessing(object sender, SubreportProcessingEventArgs e, List<Student> studentList)
        {
            var ID = Convert.ToInt32(e.Parameters[0].Values[0]);
            var students = studentList.FindAll(x => x.DepartmentId == ID);
            if (e.ReportPath == "SubStudentReport")
            {
                e.DataSources.Add(new ReportDataSource("Student", students));
            }

        }

        private DataTable ToDataTable<TSource>(TSource data)
        {
            var dataTable = new DataTable(typeof(TSource).Name);
            var props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                                                 prop.PropertyType);
            }

            var values = new object[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(data, null);
            }
            dataTable.Rows.Add(values);

            return dataTable;
        }
    }
}