using ManagementDashboard.Properties;
using MySql.Data.MySqlClient;
using System.Data;

namespace ManagementDashboard.Controllers
{
    public class MySqlConfig
    {
        public string Host { get; internal set; }
        public string Database { get; internal set; }
        public string Username { get; internal set; }
        public string Password { get; internal set; }
    }

    public class DBConnect
    {
        private string server;
        private string database;
        private string uid;
        private string password;
        private string connectionString;

        //Constructor
        public DBConnect()
        {
            //server = "102.130.113.170";
            server = Settings.Default.MySQLHost;
            database = "threepeaks_tpms";
            uid = Settings.Default.MySQLUsername;
            password = Settings.Default.MySQLPassword;


            Initialize();
        }
        public DBConnect(MySqlConfig config)
        {
            server = config.Host;
            database = config.Database;
            uid = config.Username;
            password = config.Password;

            Initialize();
        }

        private void Initialize()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = uid,
                Password = password,
                ConnectionTimeout = 240,
                DefaultCommandTimeout = 120,
                Pooling = true,
                MinimumPoolSize = 5,
                MaximumPoolSize = 200
            };

            connectionString = builder.ConnectionString;
        }
        internal DataSet Query(string query)
        {
            var ds = new DataSet();

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.CommandTimeout = 220;
                cmd.CommandType = CommandType.Text;

                conn.Open();

                using (var sdr = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    ds.EnforceConstraints = false;
                    ds.Tables.Add(dt);

                    dt.BeginLoadData();
                    dt.Load(sdr, LoadOption.OverwriteChanges);
                    dt.EndLoadData();
                }
            }

            return ds;
        }






    }
}
