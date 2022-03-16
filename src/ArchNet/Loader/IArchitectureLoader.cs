using System.Reflection;
using ArchNet.Model;

namespace ArchNet.Loader;

public interface IArchitectureLoader
{
    IContract LoadContractFromAssembly(Assembly assembly);
    ClassLevelContract LoadContractFromType(Type type);
}