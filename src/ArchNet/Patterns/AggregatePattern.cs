using ArchNet.Loader;
using ArchNet.Model;

namespace ArchNet.Patterns;

public interface IAggregatePattern
{
    public IVerify ShallExpose(string methodName, IEnumerable<Type> paramTypes, Type returnType);
    public IVerify AndNothingElse();
}

public class AggregatePattern : IAggregatePattern, IVerify
{
    private readonly Type _type;
    private readonly IArchitectureTestContext _context;
    private readonly IContract _typeContract;
    private readonly Dictionary<string, bool> _results = new();

    public AggregatePattern(Type type, IArchitectureTestContext context)
    {
        _type = type;
        _context = context;
        _typeContract = context.Loader.LoadContractFromType(type);
    }
    
    public IVerify ShallExpose(string methodName, IEnumerable<Type> paramTypes, Type returnType)
    {
        var exposedCapability = new ExposedCapability(methodName, ExposureLevel.Public, paramTypes, returnType);
        var capabilityExits = _typeContract.Exposes(exposedCapability);
        
        _results[$"{nameof(ShallExpose)}:{nameof(methodName)}"] = capabilityExits;
        _context.History.TestedCapability(_type, exposedCapability);
        
        return this;
    }

    public IVerify AndNothingElse()
    {
        return new NothingElseVerify(_typeContract, _context.History.ExposedCapabilitiesByType(_type));
    }

    public void Verify()
    {
        if (_results.Any(r => r.Value == false))
        {
            throw new Exception("Error in contract");
        }
    }
}

public class NothingElseVerify : IVerify
{
    private readonly IContract _contract;
    private readonly IEnumerable<ExposedCapability> _exposedCapabilities;

    public NothingElseVerify(IContract contract, IEnumerable<ExposedCapability> exposedCapabilities)
    {
        _contract = contract;
        _exposedCapabilities = exposedCapabilities;
    }
    
    public void Verify()
    {
        var onlyThese = _contract.ExposesExactly(_exposedCapabilities.ToArray());

        if (!onlyThese)
        {
            throw new Exception("Exposes not exactly this");
        }
    }
}

public static partial class TypeExtensions
{
    public static IAggregatePattern AsAnAggregate(this Type type, IArchitectureTestContext context)
    {
        return new AggregatePattern(type, context);
    }
}
