namespace TeaTime.Data.MySql.Factories;

using System.ComponentModel.DataAnnotations;

public class MySqlConnectionOptions
{
    [Required]
    public string Host { get; set; } = "localhost";

    public ushort Port { get; set; } = 3306;
    public string? Username { get; set; }
    public string? Password { get; set; }

    [Required]
    public string Database { get; set; } = "teatime";
}
