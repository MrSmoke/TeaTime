namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;
    using Models;
    using Models.Results;

    public class RunSerivice : IRunService
    {
        public Task<Run> Start(Room room, User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<Run> Start(Room room, User user, string @group)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Join(Room room, User user, string option)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Leave(Room room, User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Volunteer(Room room, User user)
        {
            throw new System.NotImplementedException();
        }

        public Task<RunEndResult> End(Room room, User user)
        {
            throw new System.NotImplementedException();
        }
    }
}
