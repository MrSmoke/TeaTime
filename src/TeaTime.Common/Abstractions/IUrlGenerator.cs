namespace TeaTime.Common.Abstractions;

public interface IUrlGenerator
{
    public string CreateAbsoluteUrl(string path);
    public string CreateAbsoluteUrlByName(string name);
}
