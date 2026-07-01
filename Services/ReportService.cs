using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO;

using WorkMonitor_360.Helpers;

using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace WorkMonitor_360.Services
{
    public class ReportService
    {
        private DatabaseService connectionString;

        public ReportService()
        {
            connectionString=new DatabaseService();
            //connectionString._connectionString;
        }
        public DataTable GetEmployeeData(DateTime startDate,DateTime endDate)
        {


            //using Report report = new Report();
            DataTable dt = new DataTable();

            //using (var conn = new NpgsqlConnection(connectionString._connectionString))
            //{

            //    conn.Open();
            
            //}

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString._connectionString))
            {
                conn.Open();              

                string query = @"
              SELECT machinename,username,checkintime,checkouttime FROM work_sessions where checkouttime >= @StartDate  and checkouttime <=@EndDate ORDER BY  SerialNo";
               
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {                    

                    cmd.Parameters.AddWithValue("StartDate", startDate);
                    cmd.Parameters.AddWithValue("EndDate", endDate);
                
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                }      
            
            }
            return dt;
        }


        public void GenerateExpenseReport(DataTable dt)
        {
            //string pdfPath = Path.Combine(
            //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            //    "EmployeeReport.pdf");

            string pdfPath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
               $"EmployeeReport_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header()
                    .Border(1)
                    .Padding(10)
                        .Text("Employee Report")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter()
                        ;

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Border(1).Padding(5).Text("Machine Name").Bold();
                            header.Cell().Border(1).Padding(5).Text("User Name").Bold();
                            header.Cell().Border(1).Padding(5).Text("Checkin Time").Bold();
                            header.Cell().Border(1).Padding(5).Text("checkout Time").Bold();
                        });


                        foreach (DataRow row in dt.Rows)
                        {                            

                      //      table.Cell().Border(1)
                      //.Padding(5).Text(Helpers.RelayCommand.MaskMiddle(row["machinename"]?.ToString()));

                            //table.Cell().Border(1)
                      //.Padding(5).Text(Helper.StringHelper.MaskMiddle(row["machinename"]?.ToString()));

                            table.Cell().Border(1)
                      .Padding(5).Text(row["machinename"]?.ToString());


                            //table.Cell().Border(1)
                            // .Padding(5).Text(
                            //   Helper.StringHelper.MaskMiddle(row["username"]?.ToString() ?? ""));

                            table.Cell().Border(1)
                         .Padding(5).Text((row["username"]?.ToString() ?? ""));

                            table.Cell().Border(1)
                             .Padding(5).Text(
                                Convert.ToDateTime(row["checkintime"])
                                       .ToString("dd-MMM-yyyy HH:mm:ss"));                           

                            table.Cell().Border(1)
                             .Padding(5).Text(
                          Convert.ToDateTime(row["checkouttime"])
                                 .ToString("dd-MMM-yyyy HH:mm:ss"));
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(pdfPath);

            Process.Start(new ProcessStartInfo
            {
                FileName = pdfPath,
                UseShellExecute = true
            });
        }
    }
}
