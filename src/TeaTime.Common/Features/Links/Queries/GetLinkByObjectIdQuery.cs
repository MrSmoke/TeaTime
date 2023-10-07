namespace TeaTime.Common.Features.Links.Queries
{
    using Abstractions;
    using Models;

    public class GetObjectIdByLinkValueQuery : IQuery<long?>
    {
        public LinkType LinkType { get; }
        public string Value { get; }

        public GetObjectIdByLinkValueQuery(LinkType linkType, string value)
        {
            LinkType = linkType;
            Value = value;
        }
    }
}
