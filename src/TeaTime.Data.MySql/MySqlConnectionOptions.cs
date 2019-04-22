namespace TeaTime.Data.MySql
{
    public class MySqlConnectionOptions
    {
        public string Host { get; set; } = "localhost";
        public ushort Port { get; set; } = 3306;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; } = "teatime";
    }
}