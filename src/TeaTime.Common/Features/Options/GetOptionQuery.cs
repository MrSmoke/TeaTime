namespace TeaTime.Common.Features.Options
{
    using Abstractions;
    using Models.Data;

    public class GetOptionQuery : IQuery<Option?>
    {
        public long Id { get; }

        public GetOptionQuery(long id)
        {
            Id = id;
        }
    }
}
