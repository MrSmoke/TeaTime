namespace TeaTime.Data.MySql
{
    using global::MySql.Data.MySqlClient;

    public abstract class BaseRepository
    {
        private readonly ConnectionFactory _factory;

        protected BaseRepository(ConnectionFactory factory)
        {
            _factory = factory;
        }

        protected MySqlConnection GetConnection()
        {
            return _factory.GetConnection();
        }
    }
}
