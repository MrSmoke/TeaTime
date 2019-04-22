namespace TeaTime.Data.MySql
{
    using global::MySql.Data.MySqlClient;

    public interface IMySqlConnectionFactory
    {
        MySqlConnection GetConnection();
    }
}