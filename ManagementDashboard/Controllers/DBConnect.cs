using ManagementDashboard.Properties;
using MySql.Data.MySqlClient;
using System.Data;

namespace ManagementDashboard.Controllers
{
    public class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        private void Initialize()
        {
            //server = "102.130.113.170";
            server = Settings.Default.MySQLHost;
            database = "threepeaks_tpms";
            uid = Settings.Default.MySQLUsername;
            password = Settings.Default.MySQLPassword;
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";Connection Timeout=120;";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandTimeout = 120;

            connection = new MySqlConnection(connectionString);
        }

        internal DataSet Query(string query)
        {

            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            //        //SQL Command ?? need this to do a query to the dB

            var cmd = new MySqlCommand(query, connection);
            cmd.CommandTimeout = 220;
            cmd.CommandType = CommandType.Text;
            using (var sdr = cmd.ExecuteReader())
            {
                DataTable dt = new DataTable();
                using (var ds = new DataSet())
                {
                    ds.EnforceConstraints = false;
                    ds.Tables.Add(dt);
                    dt.BeginLoadData();
                    dt.Clear();
                    dt.Load(sdr, LoadOption.OverwriteChanges);
                    dt.EndLoadData();
                    //ds.Tables.Remove(dt);
                    return ds;
                }

            }


            return null;
        }






    }
}