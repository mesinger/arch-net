using ArchNet.Loader;
using ArchNet.Model;

namespace ArchNet.Patterns;

public interface IDependencyPattern
{
    IVerify ShouldOnlyHave(params Type[] types);
}

public class DependencyPattern : IDependencyPattern, IVerify
{
    private readonly Type _type;
    private readonly IArchitectureTestContext _context;
    private readonly IContract _contract;
    private bool _result = true;

    public DependencyPattern(Type type, IArchitectureTestContext context)
    {
        _type = type;
        _context = context;
        _contract = context.Loader.LoadContractFromType(type);
    }

    public IVerify ShouldOnlyHave(params Type[] types)
    {
        IEnumerable<IContract> contracts = types.Select(t => _context.Loader.LoadContractFromType(t));
        var result = _contract.UsesExactlyTheseContracts(contracts.ToArray());

        _result &= result;
        return this;
    }

    public void Verify()
    {
        if (_result == false)
        {
            throw new Exception("Error in usages");
        }
    }
}

public static partial class TypeExtensions
{
    public static IDependencyPattern AndItsDependencies(this Type type, IArchitectureTestContext context)
    {
        return new DependencyPattern(type, context);
    }
}