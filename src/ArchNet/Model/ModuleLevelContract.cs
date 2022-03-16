namespace ArchNet.Model;

public class ModuleLevelContract : IContract
{
    public string Name { get; }

    private readonly Module _module;
    private readonly List<IContract> _children;

    public ModuleLevelContract(string name, Module module)
    {
        Name = name;
        _module = module;
        _children = new List<IContract>();
    }

    public void AddChildContract(IContract contract)
    {
        _children.Add(contract);
    }
        
    /// <inheritdoc />
    public Module Module()
    {
        return _module;
    }

    /// <inheritdoc />
    public IEnumerable<ExposedCapability> Capabilities()
    {
        return _children.SelectMany(child => child.Capabilities());
    }

    /// <inheritdoc />
    public bool Exposes(ExposedCapability capability)
    {
        return Capabilities().Any(childCapability => childCapability.Equals(capability));
    }

    /// <inheritdoc />
    public bool ExposesExactly(params ExposedCapability[] capabilities)
    {
        return Capabilities().SequenceEqual(capabilities);
    }

    /// <inheritdoc />
    public IEnumerable<Usage> Usages()
    {
        return _children
            .SelectMany(child => child.Usages()
                .Where(childUsage => childUsage.IsPublicDependency()));
    }

    /// <inheritdoc />
    public bool UsesContract(IContract contract)
    {
        if (this.Equals(contract))
        {
            return true;
        }

        var usages = Usages().ToList();
            
        if (contract is ModuleLevelContract moduleLevelContract)
        {
            return usages.Any(usage => usage.DependsOn.Module().Equals(moduleLevelContract.Module()));
        }
            
        return usages.Any(usage => usage.DependsOn.Equals(contract));
    }

    /// <inheritdoc />
    public bool UsesModule(Module module)
    {
        return Usages().Any(usage => usage.DependsOn.Module().Equals(module));
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
        var usageContracts =
            Usages().Select(usage => usage.DependsOn).ToList();
        return usageContracts.SequenceEqual(contracts);
    }

    /// <inheritdoc />
    public bool UsesExactlyTheseModules(params Module[] modules)
    {
        return Usages().Select(usage => usage.DependsOn.Module()).SequenceEqual(modules);
    }

    protected bool Equals(ModuleLevelContract other)
    {
        return _module.Equals(other._module) && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ModuleLevelContract) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (_module.GetHashCode() * 397) ^ Name.GetHashCode();
        }
    }

    public override string ToString()
    {
        return Name;
    }
}