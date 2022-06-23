namespace TeaTime.Data.MySql
{
    using MySqlConnector;

    public interface IMySqlConnectionFactory
    {
        MySqlConnection GetConnection();
    }
}
