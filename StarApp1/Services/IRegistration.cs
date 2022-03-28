using Microsoft.Data.SqlClient;
using StarApp1.Models;
using System.Text;

namespace StarApp1.Services
{
    public interface IRegistration
    {
        bool IsEmailExist(string email);

        string HashPassword(string value);
        int Register(RegisterViewModel model);
        
    }

    public class Registration : IRegistration
    {
        private readonly IDbConnection _connection;
        
        public Registration(IDbConnection dbConnection)
        {
            _connection=dbConnection;
            
        }
        bool IRegistration.IsEmailExist(string email)
        {
            SqlConnection con = _connection.ConnectToSql();
            con.Open();
            string SqlQuery = "select UserId from UserInfo where UserId=@UserId";
            SqlCommand cmd = new SqlCommand(SqlQuery, con);
            cmd.Parameters.AddWithValue("@UserId", email);

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr.HasRows ? true : false;
        }

        public string HashPassword(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                );
        }

        int IRegistration.Register(RegisterViewModel model)
        {
            SqlConnection cn = _connection.ConnectToSql();
            cn.Open();
            string querry = "insert into UserInfo values(@userId,@Name,@RoleId,3,CURRENT_TIMESTAMP,NULL,@activatedby,@password)";
            SqlCommand cmd = new SqlCommand(querry, cn);

            cmd.Parameters.AddWithValue("@userId", model.UserName);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@RoleId", model.RoleId);
            cmd.Parameters.AddWithValue("@activatedby", model.Name);
            cmd.Parameters.AddWithValue("@password", model.Password);

            int count = cmd.ExecuteNonQuery();
            cn.Close();
            return count;
        }
        
    }
}
