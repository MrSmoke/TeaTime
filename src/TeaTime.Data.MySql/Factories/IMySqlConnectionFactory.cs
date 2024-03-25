namespace TeaTime.Data.MySql.Factories;

using MySqlConnector;

public interface IMySqlConnectionFactory
{
    MySqlConnection GetConnection();
}
