namespace ArchNet.Model;

public class ClassLevelContract : IContract
{
    public string Name { get; }
    public Type Type { get; }
        
    private readonly Module _module;
    private readonly List<ExposedCapability> _exposedCapabilities;
    private readonly List<Usage> _usages = new();

    public ClassLevelContract(Module module, string name, Type type, IEnumerable<ExposedCapability> exposedCapabilities)
    {
        Name = name;
        Type = type;
        _module = module;
        _exposedCapabilities = exposedCapabilities.ToList();
    }

    public void AddUsedContract(ClassLevelContract contract)
    {
        var usage = new Usage(this, contract);
        _usages.Add(usage);
    }
        
    /// <inheritdoc />
    public Module Module()
    {
        return _module;
    }

    /// <inheritdoc />
    public IEnumerable<ExposedCapability> Capabilities()
    {
        return _exposedCapabilities;
    }

    /// <inheritdoc />
    public bool Exposes(ExposedCapability capability)
    {
        return _exposedCapabilities.Any(c => Equals(c, capability));
    }

    /// <inheritdoc />
    public bool ExposesExactly(params ExposedCapability[] capabilities)
    {
        return _exposedCapabilities.SequenceEqual(capabilities);
    }

    /// <inheritdoc />
    public IEnumerable<Usage> Usages()
    {
        return _usages;
    }

    /// <inheritdoc />
    public bool UsesContract(IContract contract)
    {
        return _usages.Any(usage => usage.DependsOn.Equals(contract));
    }

    /// <inheritdoc />
    public bool UsesModule(Module module)
    {
        if (_module.Equals(module))
        {
            return true;
        }
            
        return _usages.Any(usage => usage.DependsOn.Module().Equals(module));
    }

    /// <inheritdoc />
    public bool UsesContractNot(IContract contract)
    {
        return !UsesContract(contract);
    }

    /// <inheritdoc />
    public bool UsesModuleNot(Module module)
    {
        return !UsesModule(module);
    }

    /// <inheritdoc />
    public bool UsesExactlyTheseContracts(params IContract[] contracts)
    {
        return _usages.Select(usage => usage.DependsOn).SequenceEqual(contracts);
    }

    /// <inheritdoc />
    public bool UsesExactlyTheseModules(params Module[] modules)
    {
        return _usages.Select(usage => usage.DependsOn.Module()).SequenceEqual(modules);
    }

    protected bool Equals(ClassLevelContract other)
    {
        return _module.Equals(other._module) && Name == other.Name && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ClassLevelContract) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = _module.GetHashCode();
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            hashCode = (hashCode * 397) ^ Type.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}