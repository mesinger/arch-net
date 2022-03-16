using ArchNet.Model;

namespace ArchNet.Loader;

public interface IArchitectureTestContext
{
    public IArchitectureLoader Loader { get; }
    public IArchitectureTestContextHistory History { get; }
}

public class DefaultArchitectureTestContext : IArchitectureTestContext
{
    public IArchitectureLoader Loader { get; init; } = new ArchitectureLoader();
    public IArchitectureTestContextHistory History { get; init; } = new ArchitectureTestContextHistory();
}

public interface IArchitectureTestContextHistory
{
    void TestedCapability(Type type, ExposedCapability exposedCapability);
    IEnumerable<ExposedCapability> ExposedCapabilitiesByType(Type type);
}

public class ArchitectureTestContextHistory : IArchitectureTestContextHistory
{
    private readonly Dictionary<string, List<ExposedCapability>> _testedCapabilities = new();

    public void TestedCapability(Type type, ExposedCapability exposedCapability)
    {
        var typeName = type.FullName!;
        
        if (!_testedCapabilities.ContainsKey(typeName))
        {
            _testedCapabilities[typeName] = new List<ExposedCapability>();
        }
        
        _testedCapabilities[typeName].Add(exposedCapability);
    }

    public IEnumerable<ExposedCapability> ExposedCapabilitiesByType(Type type)
    {
        var typeName = type.FullName!;
        
        return _testedCapabilities.ContainsKey(typeName)
            ? _testedCapabilities[typeName]
            : Enumerable.Empty<ExposedCapability>();
    }
}
