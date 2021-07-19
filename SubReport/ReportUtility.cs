using Microsoft.Reporting.WebForms;
using System;
using System.Web;

namespace SubReportDemo
{
    public static class ReportUtility
    {
        public static byte[] RenderedReportViewer(LocalReport reportViewer, string reportType, out string mimeType, string reportName = null)
        {
            try
            {
                Warning[] warnings;

                string[] streamids;

                string encoding;

                string extension;

                string deviceInfo;

                string contentType;

                var rType = reportType.ToUpper();

                switch (rType)

                {
                    case "PDF":
                        deviceInfo =
                            "<DeviceInfo>" +

                            " <OutputFormat>PDF</OutputFormat>" +

                            "</DeviceInfo>";
                        contentType = "application/pdf";
                        break;

                    case "EXCEL":

                        deviceInfo =

                            "<DeviceInfo>" +

                            " <SimplePageHeaders>False</SimplePageHeaders>" +

                            "</DeviceInfo>";

                        contentType = "application/vnd.ms-excel";

                        break;

                    default:

                        deviceInfo =

                            "<DeviceInfo>" +

                            " <OutputFormat>" + reportType + "</OutputFormat>" +

                            "</DeviceInfo>";

                        contentType = "application/msword";

                        break;

                }
                byte[] bytes = reportViewer.Render(reportType, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

                HttpContext.Current.Response.Clear();

                HttpContext.Current.Response.ClearHeaders();

                HttpContext.Current.Response.ContentType = contentType;

                if (rType == "EXCEL") HttpContext.Current.Response.AddHeader("Content-disposition", "attachment; filename=" + (string.IsNullOrEmpty(reportName) ? reportType : reportName) + ".xls");
                if (rType == "WORD") HttpContext.Current.Response.AddHeader("Content-disposition", "attachment; filename=" + (string.IsNullOrEmpty(reportName) ? reportType : reportName) + ".doc");
                if (rType == "PDF") HttpContext.Current.Response.AddHeader("Content-disposition", "filename=" + (string.IsNullOrEmpty(reportName) ? reportType : reportName) + ".pdf");

                HttpContext.Current.Response.BinaryWrite(bytes);

                HttpContext.Current.Response.End();
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                mimeType = "application/pdf";
                byte[] reBytes = new byte[1];
                return reBytes;
            }
        }
    }
}