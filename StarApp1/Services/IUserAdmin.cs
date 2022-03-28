using Microsoft.Data.SqlClient;
using StarApp1.Models;

namespace StarApp1.Services
{
    public interface IUserAdmin
    {
        List<UserAdmin> DisplayData();
        int DeleteAccess(string userName);
        int UpdateAccess(string userName,int RoleId);
    }
    public class User : IUserAdmin
    {
        private readonly IDbConnection _connection;

        public User(IDbConnection dbConnection)
        {
            _connection=dbConnection;
        }

        List<UserAdmin> IUserAdmin.DisplayData()
        {
            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            string querry = "EXEC sp_UserAdmin";
            SqlCommand cmd = new SqlCommand(querry, cn);
            List<UserAdmin> listEmployee = new List<UserAdmin>();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    var employee = new UserAdmin();
                    employee.UserName = Convert.ToString(sdr["UserId"]);
                    employee.Name = Convert.ToString(sdr["Name"]);
                    employee.ActiveFrom = Convert.ToString(sdr["ActiveTime"]);
                    employee.Role = Convert.ToString(sdr["RoleName"]);
                    employee.Status = Convert.ToString(sdr["Status"]);
                    listEmployee.Add(employee);

                }
                cn.Close();
            }
            return listEmployee;
        }
        public int DeleteAccess(string userName)
        {
            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            string querry = "delete from UserInfo where UserId = @UserId";
            SqlCommand cmd = new SqlCommand(querry, cn);
            cmd.Parameters.AddWithValue("@UserId", userName);
            int count = cmd.ExecuteNonQuery();
            cn.Close();
            return count;

        }
        public int UpdateAccess(string userName, int RoleId)
        {
            int count = 0;
            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            string querry = "update UserInfo set RoleId=@RoleId where UserId =@UserId";
            SqlCommand cmd = new SqlCommand(querry, cn);
            cmd.Parameters.AddWithValue("@RoleId", RoleId);
            cmd.Parameters.AddWithValue("@UserId", userName);
            count = cmd.ExecuteNonQuery();
            cn.Close();
            return count;

        }
    }
}
