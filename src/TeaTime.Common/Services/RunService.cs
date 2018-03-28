namespace TeaTime.Common.Services
{
    using System;
    using System.Threading.Tasks;
    using Models;
    using Models.Data;
    using Models.Results;

    public class RunService : IRunService
    {
        public Task<Run> Start(Room room, User user, RoomGroup group)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Join(Room room, User user)
        {
            var run = await GetCurrent(room);
            if (run == null)
                throw new Exception("No run");

            throw new System.NotImplementedException();
        }

        public Task<bool> Join(Room room, User user, Option option)
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

        public async Task<RunEndResult> End(Room room, User user)
        {
            var run = await GetCurrent(room);
            if(run == null)
                throw new Exception("No run");

            if(run.UserId != user.Id)
                throw new Exception("You cannot end this run");

            throw new System.NotImplementedException();
        }

        public Task<Run> GetCurrent(Room room)
        {
            throw new System.NotImplementedException();
        }
    }
}
