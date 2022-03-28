using Microsoft.Data.SqlClient;
using StarApp1.Models;

namespace StarApp1.Services
{
    public interface IDashboard
    {
        List<AllowanceDashboardViewModel> DisplayUsersData(string querry, AllowanceDashboardViewModel model,string ProjectId,string PeriodStart);

        string SortQuerrySelector(string tempProjectId, string tempPeriodStart, string sortValue);

        int UpdateShiftsData(int LogId, int Asd, int Nsd, int Tsd, string UserName);
    }
    public class UserDashboard:IDashboard
    {
        private readonly IDbConnection _connection;
        public UserDashboard(IDbConnection dbConnection)
        {
            _connection=dbConnection;
        }

       public string SortQuerrySelector(string tempProjectId, string tempPeriodStart, string sortValue)
        {
            string querry = "";
            if (tempPeriodStart != null && tempProjectId != null)
            {
                switch (sortValue)
                {
                    case "Name":
                        querry = "Select LogId, Resource_name , Resource_Id, Hours, Approval_Status,Project_Id,Period_Start  ,updated_asd, updated_nsd from export_data where DATENAME(MONTH,Period_Start)= @periodStart  AND Project_Id = @ProjectId order by Resource_name ";
                        break;
                    case "SAPId":
                        querry = "Select LogId, Resource_name , Resource_Id, Hours, Approval_Status,Project_Id,Period_Start ,updated_asd, updated_nsd from export_data where Period_Start = @periodStart  AND Project_Id = @ProjectId order by Resource_Id ";
                        break;
                    default:
                        querry = "Select LogId,Resource_name , Resource_Id, Hours, Approval_Status,Project_Id,Period_Start ,updated_asd, updated_nsd from export_data where DATENAME(MONTH,Period_Start)= @periodStart  AND Project_Id = @ProjectId ";
                        break;
                }
            }
            else
            {
                switch (sortValue)
                {
                    case "Name":
                        querry = "Select  LogId, Resource_name , Resource_Id, Hours, Approval_Status,Project_Id,Period_Start ,updated_asd, updated_nsd from export_data where Resource_Id is not null order by Resource_name ";
                        break;
                    case "SAPId":
                        querry = "Select LogId, Resource_name , Resource_Id, Hours, Approval_Status,Project_Id,Period_Start ,updated_asd, updated_nsd from export_data where Resource_Id is not null order by Resource_Id ";
                        break;
                    default:
                        querry = "Select LogId,Resource_name , Resource_Id, Hours, Approval_Status,Project_Id,Period_Start ,updated_asd, updated_nsd from export_data where Resource_Id is not null ";
                        break;
                }
            }
            return querry;
        }
        List<AllowanceDashboardViewModel> IDashboard.DisplayUsersData(string querry, AllowanceDashboardViewModel model, string ProjectId, string PeriodStart)
        {
            List<AllowanceDashboardViewModel> listEmployee = new List<AllowanceDashboardViewModel>();

            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            SqlCommand cmd = new SqlCommand(querry, cn);
            if (PeriodStart != null && ProjectId != null)
            {
                cmd.Parameters.AddWithValue("@periodStart", PeriodStart);
                cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
            }
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {

                while (sdr.Read())
                {

                    var employee = new AllowanceDashboardViewModel();
                    employee.Name = Convert.ToString(sdr["Resource_Name"]);
                    employee.SAPid = Convert.ToInt32(sdr["Resource_Id"]);
                    employee.Hours = Convert.ToInt32(sdr["Hours"]);
                    employee.LeaveHours = 40- employee.Hours;
                    if(!(sdr["updated_asd"] is DBNull))
                    {
                        employee.AfternoonShiftDays = Convert.ToInt32(sdr["updated_asd"]);
                    }
                    else
                    {
                        employee.AfternoonShiftDays = Convert.ToInt32(sdr["Hours"]) / 8;
                    }
                    if (!(sdr["updated_nsd"] is DBNull))
                    {
                        employee.NightShiftDays = Convert.ToInt32(sdr["updated_nsd"]);
                    }
                    else
                    {
                        employee.NightShiftDays = 0;
                    }
                    employee.TotalDays = employee.AfternoonShiftDays + employee.NightShiftDays;
                    if(model.TransportAllowance>0)
                    {
                        employee.TransportAllowance = employee.TotalDays * model.TransportAllowance;
                    }
                    else
                    {
                        employee.TransportAllowance = employee.TotalDays * 150;
                    }
                   if(model.AfternoonShiftAllowance>0 && model.NightShiftAllowance>0)
                    {
                        employee.TotalAllowance = employee.AfternoonShiftDays * model.AfternoonShiftAllowance + employee.NightShiftDays * model.NightShiftAllowance + employee.TransportAllowance;

                    }
                    else
                    {
                        employee.TotalAllowance = employee.TotalDays*300 + employee.TransportAllowance;

                    }
                    employee.ApprovalStatus = Convert.ToString(sdr["Approval_Status"]);
                    employee.PeriodStart = model.PeriodStart;
                    employee.ProjectId = model.ProjectId;
                    employee.LogId = Convert.ToInt32(sdr["LogId"]);

                    listEmployee.Add(employee);

                }
                cn.Close();
            }
            return listEmployee;
        }

        public int UpdateShiftsData(int LogId,int Asd, int Nsd, int Tsd,string UserName)
        {
            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            string querry = "EXEC sp_updtShiftDays @LogId,@updated_nsd ,@updated_asd ,@updated_by";
            string querry1 = "insert into log_shiftdays values(@LogId,@updated_asd ,@updated_nsd ,@updated_tsd ,@updated_by,CURRENT_TIMESTAMP) ";
            SqlCommand cmd = new SqlCommand(querry, cn);
            
            cmd.Parameters.AddWithValue("@LogId", LogId);
            cmd.Parameters.AddWithValue("@updated_nsd", Nsd);
            cmd.Parameters.AddWithValue("@updated_asd", Asd);
            cmd.Parameters.AddWithValue("@updated_by", UserName);
            int count = cmd.ExecuteNonQuery();


            SqlCommand cmd1 = new SqlCommand(querry1, cn);
            cmd1.Parameters.AddWithValue("@LogId", LogId);
            cmd1.Parameters.AddWithValue("@updated_nsd", Nsd);
            cmd1.Parameters.AddWithValue("@updated_asd", Asd);
            cmd1.Parameters.AddWithValue("@updated_tsd", Tsd);
            cmd1.Parameters.AddWithValue("@updated_by", UserName);
            cmd1.ExecuteNonQuery();

            cn.Close();
            return count;
        }
    }
}
