using Microsoft.Data.SqlClient;

namespace StarApp1.Services
{
    public interface IButton
    {
        void ApproveBtn(String UserName);
        void DeclineBtn(String UserName);

    }
    public class Button : IButton
    {
        private readonly IDbConnection _connection;
        public Button(IDbConnection dbConnection)
        {
            _connection=dbConnection;
        }

        void IButton.DeclineBtn(string UserName)
        {
            SqlConnection con = _connection.ConnectToSql();
            con.Open();
            string querry = "Update UserInfo set StatusId = 2, ActiveTime = default  where UserId=@UserName";
            SqlCommand cmd = new SqlCommand(querry, con);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        void IButton.ApproveBtn(string UserName)
        {
            SqlConnection con = _connection.ConnectToSql();
            con.Open();
            string querry = "Update UserInfo set StatusId = 1,ActiveTime = CURRENT_TIMESTAMP where UserId=@UserName";
            SqlCommand cmd = new SqlCommand(querry, con);
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.ExecuteNonQuery();
            con.Close();

           
        }
    }
}
