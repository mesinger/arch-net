namespace ArchNet.Loader;

public record ArchitectureLoaderOptions
{
    public string[] IgnoredNamespaces { get; set; } = { };
}