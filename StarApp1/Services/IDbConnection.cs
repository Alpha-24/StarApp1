using Microsoft.Data.SqlClient;

namespace StarApp1.Services
{
    public interface IDbConnection
    {
        SqlConnection ConnectToSql();
    }
    public class DbConnection : IDbConnection
    {
        private readonly IConfiguration _configuration;
        public DbConnection(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        public SqlConnection ConnectToSql()
        {
            string constr = this._configuration.GetConnectionString("MYConnector");
            SqlConnection con = new SqlConnection(constr);

            return con;
        }
    }
}
