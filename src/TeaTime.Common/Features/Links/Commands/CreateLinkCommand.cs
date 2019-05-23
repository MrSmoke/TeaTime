namespace TeaTime.Common.Features.Links.Commands
{
    using Abstractions;
    using Models;

    public class CreateLinkCommand : ICommand
    {
        public long ObjectId { get; }
        public LinkType LinkType { get; }
        public string Value { get; }

        public CreateLinkCommand(long objectId, LinkType linkType, string value)
        {
            ObjectId = objectId;
            LinkType = linkType;
            Value = value;
        }
    }
}
