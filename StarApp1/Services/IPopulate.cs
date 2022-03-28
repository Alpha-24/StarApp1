using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;


namespace StarApp1.Services
{
    public interface IPopulate
    {
        List<SelectListItem> PopulateRole();
        List<SelectListItem> PopulatePeriod();
        List<SelectListItem> PopulateProject();
    }
    public class Populate:IPopulate
    {
        private readonly IDbConnection _connection;
        public Populate(IDbConnection dbConnection)
        {
            _connection=dbConnection;
        }
        public List<SelectListItem> PopulateRole()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (SqlConnection con = _connection.ConnectToSql())
            {
                string query = " SELECT RoleName, RoleId FROM LqpRoles";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["RoleName"].ToString(),
                                    Value = sdr["RoleId"].ToString()
                                });
                            }

                        }

                    }
                    con.Close();
                }
            }
            return items;

        }

        public List<SelectListItem> PopulatePeriod()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            using (SqlConnection con = _connection.ConnectToSql())
            {
                //string query = "select distinct concat (Period_Start,' TO  ',Period_End ) as Period , Period_Start from export_data where Period_Start is not null";
                string query = "select distinct DATENAME(MONTH,Period_Start) as Period from export_data where Period_Start is not null";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows && items != null)
                        {
                            while (sdr.Read())
                            {

                                items.Add(new SelectListItem
                                {
                                    Value = sdr["Period"].ToString(),
                                    Text = sdr["Period"].ToString()
                                });


                            }

                        }
                    }
                    con.Close();
                }
            }
            return items;

        }
        public List<SelectListItem> PopulateProject()
        {
            List<SelectListItem> items1 = new List<SelectListItem>();

            using (SqlConnection con = _connection.ConnectToSql())
            {
                string query = "select distinct Project_Id , Project_Name from export_data where Project_Name is not null";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows && items1 != null)
                        {
                            while (sdr.Read())
                            {

                                items1.Add(new SelectListItem
                                {
                                    Value = sdr["Project_Id"].ToString(),
                                    Text = sdr["Project_Name"].ToString()
                                });


                            }

                        }
                    }
                    con.Close();
                }
            }
            return items1;

        }
    }
}
