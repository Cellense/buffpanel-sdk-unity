using System;
using System.Collections;
using System.Data;
using Mono.Data.SqliteClient;

namespace BuffPanel
{
    public class CookieExtractor
    {
        public static ArrayList ReadChromeCookies()
        {
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cookies";
            if (!System.IO.File.Exists(dbPath))
            {
                throw new System.IO.FileNotFoundException("Cant find cookie store", dbPath); // race condition, but i'll risk it
            }

            IDbConnection connection = new SqliteConnection("URI=file:" + dbPath);
            connection.Open();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM cookies WHERE host_key LIKE '%" + BuffPanel.serviceHostname + "%';";
            IDataReader reader = command.ExecuteReader();

            ArrayList result = new ArrayList();

            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            connection.Close();

            return result;

            /*
            // TODO decrypt from a single cookie value.
            using (var conn = new System.Data.SQLite.SQLiteConnection(connectionString))
            using (var cmd = conn.CreateCommand())
            {                   
                var prm = cmd.CreateParameter();
                prm.ParameterName = "hostName";
                prm.Value = hostName;
                cmd.Parameters.Add(prm);

                cmd.CommandText = "SELECT name, encrypted_value FROM cookies WHERE host_key = @hostName";

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {                    
                    var encryptedData = (byte[])reader[1];
                    var decodedData = System.Security.Cryptography.ProtectedData.Unprotect(encryptedData, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                    var plainText = Encoding.ASCII.GetString(decodedData); // Looks like ASCII
                }

                conn.Close();                
            }
            */
        }
    }
}
