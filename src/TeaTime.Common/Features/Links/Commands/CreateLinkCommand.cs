namespace TeaTime.Common.Features.Links.Commands;

using Abstractions;
using Models;

public record CreateLinkCommand(long ObjectId, LinkType LinkType, string Value) : ICommand;
