using MaterialDesignThemes.Wpf.Converters;
using Microsoft.ReportingServices.Diagnostics.Internal;
using Microsoft.VisualBasic;
using Npgsql;
using System.Configuration;
using System.Security.Cryptography;
using System.Windows.Forms;
using WorkMonitor_360.Models;
using WorkMonitor_360.Models.WorkMonitor_360.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WorkMonitor_360.Services
{
    public class DatabaseService
    {        
        public readonly string _connectionString =
           ConfigurationManager.ConnectionStrings["WorkMonitorDB"].ConnectionString;  
        public async Task InsertSessionAsync(WorkSessionModel session)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            await conn.OpenAsync();            

            session.UserName = Environment.UserName;
            session.UserName = Environment.UserName;
            session.MachineName = Environment.MachineName;


            string query = @"
                INSERT INTO work_sessions
                (MachineName,UserName,CheckInTime, CheckOutTime, Status, Synced)
                VALUES
                (@machinename,@username,@checkin, @checkout, @status, @synced)";

            using var cmd = new NpgsqlCommand(query, conn);


            cmd.Parameters.AddWithValue("@machinename", session.MachineName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@username", session.UserName ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@checkin", session.CheckInTime ?? (object)DBNull.Value);
            
            cmd.Parameters.AddWithValue("@checkout",
                session.CheckOutTime ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@status", session.Status ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@synced", session.Synced ?? (object)DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<WorkSessionModel>> GetSessionsAsync()
        {
            var sessions = new List<WorkSessionModel>();

            using var conn = new NpgsqlConnection(_connectionString);

            await conn.OpenAsync();

            string query = "SELECT * FROM work_sessions ORDER BY SerialNo";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sessions.Add(new WorkSessionModel
                {
                    SerialNo = Convert.ToInt32(reader["SerialNo"]),
                    MachineName = reader["MachineName"].ToString(),
                    UserName = reader["UserName"].ToString(),

                    CheckInTime = Convert.ToDateTime(reader["CheckInTime"]),
                    CheckOutTime = reader["CheckOutTime"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["CheckOutTime"]),
                    Status = reader["Status"].ToString(),
                    Synced = Convert.ToBoolean(reader["Synced"])
                });
            }

            return sessions;
        }

        public async Task<List<WorkSessionModel>> GetUnsyncedSessionsAsync()
        {
            var sessions = new List<WorkSessionModel>();

            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            string query = "SELECT * FROM work_sessions WHERE Synced=false";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sessions.Add(new WorkSessionModel
                {
                    SerialNo = Convert.ToInt32(reader["SerialNo"]),
                    MachineName = reader["MachineName"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    CheckInTime = Convert.ToDateTime(reader["CheckInTime"]),
                    CheckOutTime = reader["CheckOutTime"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["CheckOutTime"]),
                    Status = reader["Status"].ToString(),
                    Synced = Convert.ToBoolean(reader["Synced"])
                });
            }

            return sessions;
        }

        public async Task MarkAsSyncedAsync(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);

            await conn.OpenAsync();

            string query = "UPDATE work_sessions SET synced=true WHERE SerialNo=@id";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await cmd.ExecuteNonQueryAsync();
        }
        


        public List<string> LoadHostNames()
        {


            List<string> hostNames = new List<string>();

            string query = "SELECT distinct machinename FROM work_sessions";

            using (var conn =new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        hostNames.Add(reader["machinename"]?.ToString() ?? string.Empty);
                    }    
                }                    
            }

            return hostNames;
        }




        public List<string> LoadUserNames(string machineName)
        {


            List<string> userNames = new List<string>();

            //string query = "SELECT distinct username FROM work_sessions  order by username asc";

            string query = @"SELECT distinct username FROM work_sessions where machinename=@MachineName  order by username asc";

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MachineName", machineName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            userNames.Add(reader["username"]?.ToString() ?? string.Empty);
                        }
                    }
                }
            }

            return userNames;
        }        

        //public void GetEmployeeWorkSummary(string machineName, string userName, DateTime fromDate, DateTime toDate)
        public AIAnalyticalReportModel? GetEmployeeWorkSummary(string machineName, string userName, DateTime fromDate, DateTime toDate)
        {
            string machine_Name = string.Empty;
            string user_Name = string.Empty;
            decimal total_TimeSpentHours = 0;
            decimal idle_Time = 0;

            string query = @"
            SELECT
                machinename,
                username,
                ROUND(SUM(EXTRACT(EPOCH FROM (checkouttime - checkintime)) / 3600)::numeric, 2) AS tot_TimeSpent_Hours,
                9-ROUND(SUM(EXTRACT(EPOCH FROM (checkouttime - checkintime)) / 3600)::numeric, 2) as idletime
            FROM work_sessions
            WHERE machinename = @MachineName
              AND username = @UserName 
            and  checkintime>=@CheckInTime  and checkouttime <=@CheckOutTime  
            GROUP BY machinename, username;";


            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@MachineName", machineName);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@CheckInTime", fromDate);
                    cmd.Parameters.AddWithValue("@CheckOutTime", toDate);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            machine_Name = reader["machinename"]?.ToString() ?? string.Empty;
                            user_Name = reader["username"]?.ToString() ?? string.Empty;
                            total_TimeSpentHours = reader.GetDecimal(reader.GetOrdinal("tot_timespent_hours"));
                            idle_Time = reader.GetDecimal(reader.GetOrdinal("idletime"));

                            return new AIAnalyticalReportModel
                            {
                                HostName = machine_Name,
                                UserName = user_Name,
                                TotalTimeSpent = total_TimeSpentHours,
                                IdleTime = idle_Time

                            };

                        }
                    }
                }
            }  

            return null;
           
        }
    }
}