using Microsoft.Data.SqlClient;

namespace StarApp1.Services
{
    public interface IResetPassword
    {
        bool ChangePass(string userName, string password);
    }
    public class ResetPassword:IResetPassword
    {
        public readonly IRegistration _registration;
        private readonly IDbConnection _connection;
        public ResetPassword(IRegistration registration,IDbConnection dbConnection)
        {
            _connection=dbConnection;
            _registration = registration;
        }

        public bool ChangePass(string userName, string password)
        {
            password = _registration.HashPassword(password);
            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            string querry = "update UserInfo set Password=@Password where UserId=@UserId";
            SqlCommand cmd = new SqlCommand(querry, cn);

            cmd.Parameters.AddWithValue("@UserId", userName);
            cmd.Parameters.AddWithValue("@Password", password);
            return cmd.ExecuteNonQuery() == 1;
        }
    }
}
