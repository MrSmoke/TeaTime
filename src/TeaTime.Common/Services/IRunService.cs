namespace TeaTime.Common.Services
{
    using System.Threading.Tasks;

    public interface IRunService
    {
        Task Start();
        Task Join();
        Task Leave();
        Task Volunteer();
        Task End();
    }
}