using MySqlConnector;

namespace Kutuphane.DAL
{
    public static class DbConnection
    {
        // XAMPP MySQL (localhost)
        private const string Server = "127.0.0.1";
        private const string Database = "kutuphane_db";

        // XAMPP default kullanıcı
        private const string User = "root";
        private const string Password = ""; // XAMPP'ta genelde boş

        public static MySqlConnection GetConnection()
        {
            string cs =
                $"Server={Server};Database={Database};Uid={User};Pwd={Password};Port=3306;SslMode=None;";

            return new MySqlConnection(cs);
        }

        public static bool TestConnection(out string message)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                }

                message = "✅ Bağlantı başarılı! (Localhost)";
                return true;
            }
            catch (System.Exception ex)
            {
                message = "❌ Bağlantı hatası: " + ex.Message;
                return false;
            }
        }
    }
}
