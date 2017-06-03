namespace TeaTime.Data.MySql.Repositories
{
    using Common.Abstractions.Data;

    public class RunRepository : BaseRepository, IRunRepository
    {
        public RunRepository(ConnectionFactory factory) : base(factory)
        {
        }
    }
}
