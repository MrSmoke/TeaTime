namespace TeaTime.Common.Abstractions.Data
{
    using System.Threading.Tasks;
    using Models.Data;

    public interface IRoomRepository
    {
        Task CreateAsync(Room room);
        Task UpdateAsync(Room room);
        Task<Room?> GetAsync(long id);
    }
}
