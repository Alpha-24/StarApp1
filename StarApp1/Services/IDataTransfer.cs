using Microsoft.Data.SqlClient;
using Microsoft.Extensions.FileProviders;
using StarApp1.Models;
using System.Data.OleDb;
using System.Text;

using System.Data;

namespace StarApp1.Services
{
    public interface IDataTransfer
    {
        void ImportExcel(IFormFile file);
        StringBuilder ExportCsv(List<AllowanceDashboardViewModel> listEmployee2);

        
    }
    public class DataTransfer : IDataTransfer
    {
        private readonly IDbConnection _connection;
        public DataTransfer(IDbConnection dbConnection)
        {
            _connection=dbConnection;
        }

        StringBuilder IDataTransfer.ExportCsv(List<AllowanceDashboardViewModel> listEmployee2)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Name,SAPid,Hours,Leave Hours,Afternoon Shift Days,Night Shift Days ,Total Days,Transport Allowance,Total Allowance");
            foreach (var user in listEmployee2)
            {
                if (user.ApprovalStatus == "Approved")
                    builder.AppendLine($"{user.Name},{user.SAPid},{user.Hours},{user.LeaveHours},{user.AfternoonShiftDays},{user.NightShiftDays},{user.TotalDays},{user.TransportAllowance},{user.TotalAllowance}");
                else if (user.ApprovalStatus == "Awaiting Approval")
                {
                    SqlConnection con = _connection.ConnectToSql();
                    con.Open();
                    string querry = "Update export_data set Approval_Status = 'Approved'  where Resource_name=@UserName and Period_Start = @periodStart And Project_Id=@ProjectId and Approval_Status='Awaiting Approval'";
                    SqlCommand cmd = new SqlCommand(querry, con);
                    cmd.Parameters.AddWithValue("@UserName", user.Name);
                    cmd.Parameters.AddWithValue("@PeriodStart", user.PeriodStart);
                    cmd.Parameters.AddWithValue("@ProjectId", user.ProjectId);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    builder.AppendLine($"{user.Name},{user.SAPid},{user.Hours},{user.LeaveHours},{user.AfternoonShiftDays},{user.NightShiftDays},{user.TotalDays},{user.TransportAllowance},{user.TotalAllowance}");
                }

            }
            return builder;
        }

        void IDataTransfer.ImportExcel(IFormFile file)
        {
            String Resoursename;
            String ResourseId;
            DateTime PeriodStart;
            DateTime PeriodEnd;
            int Hours;
            String ApprovalStatus;
            String TimesheetNumber;
            String Vertical;
            String Horizontal;
            String SubHorizontal;
            String CoustmerId;
            String CoustmerName;
            String ProjectId;
            String ProjectName;
            String ProjectManager;
            try
            {
                if(file!=null)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    //Assigning Unique Filename (Guid)
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Getting file Extension
                    var fileExtension = Path.GetExtension(fileName);
                    if (fileExtension == ".xlsx" || fileExtension == ".XLSX")
                    {
                        var newFileName = String.Concat(myUniqueFileName, fileExtension);

                        // Combines two strings into a path.
                        var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Imports")).Root + $@"\{newFileName}";

                        
                        
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }

                        OleDbConnection myCon = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = '" + filePath + "'; Extended Properties =\"Excel 12.0; HDR=YES;\"");

                        myCon.Open();
                        List<string> listSheet = new List<string>();
                        System.Data.DataTable dtSheet = myCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        foreach (DataRow drSheet in dtSheet.Rows)
                        {
                            if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                            {
                                listSheet.Add(drSheet["TABLE_NAME"].ToString());
                            }
                        }
                        int i = 0;
                        while(listSheet[i+1]!=null)
                        {
                            OleDbCommand cmd = new OleDbCommand("select * from [" + listSheet[i] + "]", myCon);
                            
                            OleDbDataReader dr = cmd.ExecuteReader();
                            while (dr.Read())
                            {
                                // Response.Write("<br/>"+dr[0].ToString());
                                Resoursename = dr[0].ToString();
                                ResourseId = dr[1].ToString();
                                PeriodStart = Convert.ToDateTime(dr[2].ToString());
                                PeriodEnd = Convert.ToDateTime(dr[3].ToString());
                                Hours = Convert.ToInt32(dr[4]);
                                ApprovalStatus = dr[5].ToString();
                                TimesheetNumber = dr[6].ToString();
                                Vertical = dr[7].ToString();
                                Horizontal = dr[8].ToString();
                                SubHorizontal = dr[9].ToString();
                                CoustmerId = dr[10].ToString();
                                CoustmerName = dr[11].ToString();
                                ProjectId = dr[12].ToString();
                                ProjectName = dr[13].ToString();
                                ProjectManager = dr[14].ToString();


                                savedata(Resoursename, ResourseId, PeriodStart, PeriodEnd, Hours, ApprovalStatus, TimesheetNumber, Vertical, Horizontal, SubHorizontal, CoustmerId, CoustmerName, ProjectId, ProjectName, ProjectManager);
                            }
                            i++;
                        }
                        DeleteDuplicates();

                        myCon.Close();
                    }
                }
                
            }
            catch (Exception ex)
            {
                
            }
            
            // concatenating FileName + FileExtension
           
        }
        public void DeleteDuplicates()
        {
            SqlConnection con = _connection.ConnectToSql();
            con.Open();
            SqlCommand cmd = new SqlCommand();
            String querry = "EXEC DELETE_DUPLICATES";
            cmd.CommandText = querry;
            cmd.Connection = con;
        }
        private void savedata(String Resoursename,
                        String ResourseId,
                        DateTime PeriodStart,
                        DateTime PeriodEnd,
                        int Hours,
                        String ApprovalStatus,
                        String TimesheetNumber,
                        String Vertical,
                        String Horizontal,
                        String SubHorizontal,
                        String CoustmerId,
                        String CoustmerName,
                        String ProjectId,
                        String ProjectName,
                        String ProjectManager)
        {
            String query = "insert into export_data values(@Resoursename, @ResourseId, @PeriodStart, @PeriodEnd, @Hours, @ApprovalStatus, @TimesheetNumber, @Vertical, @Horizontal, @SubHorizontal,@CustmerId, @CustmerName,@ProjectId, @ProjectName, @ProjectManager)";

            SqlConnection con = _connection.ConnectToSql();
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Resoursename", Resoursename);
            cmd.Parameters.AddWithValue("@ResourseId", ResourseId);
            cmd.Parameters.AddWithValue("@PeriodStart", PeriodStart);
            cmd.Parameters.AddWithValue("@PeriodEnd", PeriodEnd);
            cmd.Parameters.AddWithValue("@Hours", Hours);
            cmd.Parameters.AddWithValue("@ApprovalStatus", ApprovalStatus);
            cmd.Parameters.AddWithValue("@TimesheetNumber", TimesheetNumber);
            cmd.Parameters.AddWithValue("@Vertical", Vertical);
            cmd.Parameters.AddWithValue("@Horizontal", Horizontal);
            cmd.Parameters.AddWithValue("@SubHorizontal", SubHorizontal);
            cmd.Parameters.AddWithValue("@CustmerId", CoustmerId);
            cmd.Parameters.AddWithValue("@CustmerName", CoustmerName);
            cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
            cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
            cmd.Parameters.AddWithValue("@ProjectManager", ProjectManager);

            cmd.ExecuteNonQuery();

            
            con.Close();

        }
    }
}
