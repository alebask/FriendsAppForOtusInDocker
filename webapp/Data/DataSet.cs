using System.Threading.Tasks;
using MySqlConnector;

namespace FriendsAppNoORM.Data{

    public class DataSet{
    
        private string _connectionString;

        public DataSet(string connectionString)
        {
              _connectionString = connectionString;
        }

        protected MySqlConnection CreateOpenConnection()
        {
            MySqlConnection c = new MySqlConnection(_connectionString);
            c.Open();
            return c;
        }

        protected async Task<MySqlConnection> CreateOpenConnectionAsync()
        {
            MySqlConnection c = new MySqlConnection(_connectionString);
            
            await c.OpenAsync();

            return c;
        }
        
    }

}