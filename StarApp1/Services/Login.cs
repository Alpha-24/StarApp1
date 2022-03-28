using Microsoft.Data.SqlClient;
using StarApp1.Models;

namespace StarApp1.Services
{
    public class Login : ILogin
    {
        private readonly IDbConnection _connection;
        private readonly IRegistration _registration;

        public Login(IRegistration registration,IDbConnection dbConnection)
        {
            _registration = registration;
            _connection = dbConnection;
            
        }
        
        public LoginViewModel CheckLogin(LoginViewModel model)
        {
            
            
            SqlConnection con = _connection.ConnectToSql();
            con.Open();
            string SqlQuery = "EXEC sp_SignIn @UserId = @UserId ,@Password = @Password;";
                SqlCommand cmd = new SqlCommand(SqlQuery, con);
                cmd.Parameters.AddWithValue("@UserId", model.UserName);
                cmd.Parameters.AddWithValue("@Password", _registration.HashPassword(model.Password));
                SqlDataReader sdr = cmd.ExecuteReader();
                var user = new LoginViewModel();
                while (sdr.Read())
                {
                   
                    user.UserName = (string)sdr["UserId"];
                    user.Password = (string)sdr["Password"];
                    user.RoleId = Convert.ToInt32(sdr["RoleId"]);
                    user.StatusId = Convert.ToInt32(sdr["StatusId"]);
                    user.Name = (string)sdr["Name"];

                    
                }
            con.Close();


            return user;

        }
        
    }
}

