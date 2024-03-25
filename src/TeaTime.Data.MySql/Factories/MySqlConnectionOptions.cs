namespace TeaTime.Data.MySql.Factories;

using Common.Options;

public class MySqlConnectionOptions
{
    public string Host { get; set; } = "localhost";
    public ushort Port { get; set; } = 3306;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string Database { get; set; } = "teatime";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Host))
            throw new InvalidOptionException(nameof(Host), "MySql host has not been set");

        if (string.IsNullOrWhiteSpace(Database))
            throw new InvalidOptionException(nameof(Database), "MySql Database has not been set");
    }
}
