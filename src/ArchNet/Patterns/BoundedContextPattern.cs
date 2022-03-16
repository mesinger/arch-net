using System.Reflection;
using ArchNet.Loader;
using ArchNet.Model;

namespace ArchNet.Patterns;

public interface IBoundedContextPattern
{
    IContract Contract();
    IVerify ShouldNotKnow(IBoundedContextPattern boundedContext);
}

public class BoundedContextPattern : IBoundedContextPattern
{
    private readonly Assembly _assembly;
    private readonly IArchitectureTestContext _context;
    private readonly IContract _contract;

    public BoundedContextPattern(Assembly assembly, IArchitectureTestContext context)
    {
        _assembly = assembly;
        _context = context;
        _contract = context.Loader.LoadContractFromAssembly(assembly);
    }

    public IContract Contract()
    {
        return _contract;
    }

    public IVerify ShouldNotKnow(IBoundedContextPattern boundedContext)
    {
        var dependingModule = boundedContext.Contract().Module();
        var usedModules = _contract.Usages().Select(usage => usage.DependsOn.Module());
        var foundUsage = usedModules.Any(module => Equals(dependingModule, module));

        if (foundUsage)
        {
            return BoundedContextVerify.WithError($"Bounded Context {_contract.Module().Id} knows of Bounded Context {dependingModule.Id}");
        }
        
        return BoundedContextVerify.WithNoError();
    }
}

internal class BoundedContextVerify : IVerify
{
    private readonly bool _isValid;
    private readonly string _message;
    
    protected BoundedContextVerify(bool isValid, string message)
    {
        _isValid = isValid;
        _message = message;
    }

    public static BoundedContextVerify WithNoError() => new BoundedContextVerify(true, "");
    public static BoundedContextVerify WithError(string error) => new BoundedContextVerify(false, error);
    
    public void Verify()
    {
        if (_isValid == false)
        {
            throw new Exception(_message);
        }
    }
}

public static partial class TypeExtensions
{
    public static IBoundedContextPattern AsBoundedContext(this Assembly assembly, IArchitectureTestContext context)
    {
        return new BoundedContextPattern(assembly, context);
    }
}
