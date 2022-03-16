using System.Reflection;
using ArchNet.Model;
using Module = ArchNet.Model.Module;

namespace ArchNet.Loader;

public class ArchitectureLoader : IArchitectureLoader
{
    private readonly Dictionary<string, ClassLevelContract> _cache = new();
    private readonly ArchitectureLoaderOptions _options;

    public ArchitectureLoader(): this(options =>
    {
        options.IgnoredNamespaces = new[] {"System", "Microsoft"};
    })
    {
    }

    public ArchitectureLoader(Action<ArchitectureLoaderOptions> options)
    {
        _options = new ArchitectureLoaderOptions();
        options(_options);
    }
        
    /// <inheritdoc />
    public IContract LoadContractFromAssembly(Assembly assembly)
    {
        var contracts = new List<IContract>();
        foreach (var type in assembly.ExportedTypes)
        {
            contracts.Add(LoadContractFromType(type));
        }

        var contract = new ModuleLevelContract(assembly.GetName().Name!, new Module(assembly.FullName!));
        contracts.ForEach(c => contract.AddChildContract(c));
        return contract;
    }

    /// <inheritdoc />
    public ClassLevelContract LoadContractFromType(Type type)
    {
        if (_cache.ContainsKey(type.FullName!))
        {
            return _cache[type.FullName!];
        }

        if (ShouldIgnoreCapabilities(type))
        {
            var classContract = new ClassLevelContract(new Module(type.Assembly.FullName!), type.FullName!,
                type, Enumerable.Empty<ExposedCapability>());
            _cache[type.FullName!] = classContract;
                
            return classContract;
        }
            
        var module = new Module(type.Assembly.FullName!);

        var runtimeMethodInfos = type.GetMethods()
            .Where(m => m.IsPublic)
            .Where(m => !IsStandardMethod(m.Name))
            .Where(m => !IsRecordMethod(m.Name))
            // .Where(m => m.Name.StartsWith("get_") || m.Name.StartsWith("set_"))
            .ToList();

        var capabilities = runtimeMethodInfos.Select(m =>
        {
            return new ExposedCapability(m.Name, ExposureLevel.Public,
                m.GetParameters().Select(p => p.ParameterType), m.ReturnType);
        });
            
        var contract = new ClassLevelContract(module, type.FullName!, type, capabilities);
        _cache[type.FullName!] = contract;

        // usages -> method params, method returns, from getmembers, private things from declaredfields

        var returnTypes = runtimeMethodInfos
            .Select(methodInfo => methodInfo.ReturnType)
            .Select(HandleCollections);

        var parameterTypes = runtimeMethodInfos
            .SelectMany(methodInfo =>
                methodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType))
            .Select(HandleCollections);
            
        // var paremterTypes = type.GetMembers().Where(m => m.MemberType == MemberTypes.Property).Select(property => property.typ)

        var fieldTypes = type.GetTypeInfo().DeclaredFields.Select(field => field.FieldType).Select(HandleCollections);

        var interfaceTypes = type.GetInterfaces();

        var localVariableTypes = runtimeMethodInfos
            .SelectMany(m => m.GetMethodBody()?.LocalVariables ?? Enumerable.Empty<LocalVariableInfo>())
            .Select(l => l.LocalType);

        var allUsedTypes = new HashSet<Type>(
            returnTypes.Concat(parameterTypes).Concat(fieldTypes).Concat(interfaceTypes).Concat(localVariableTypes)
                .Where(t => t.Name != "IEquatable`1")
                .Where(t => t != typeof(void))
                .Select(FilterTask));

        foreach (var usedType in allUsedTypes)
        {
            if (string.IsNullOrWhiteSpace(usedType.FullName))
            {
                continue;
            }
                
            contract.AddUsedContract(LoadContractFromType(usedType));
        }

        _cache[type.FullName!] = contract;
        return contract;
    }

    private bool ShouldIgnoreCapabilities(Type type)
    {
        return _options.IgnoredNamespaces.Any(ns => type.FullName!.StartsWith(ns));
    }

    private bool IsStandardMethod(string name)
    {
        return name is "Equals" or "ToString" or "GetHashCode" or "GetType";
    }
        
    private bool IsRecordMethod(string name)
    {
        return name is "op_Inequality" or "op_Equality" or "<Clone>$" or "Deconstruct";
    }
        
    private Type HandleCollections(Type type)
    {
        type = HandleIEnumerable(type);
        type = HandleList(type);
        return type;
    }

    private Type HandleIEnumerable(Type type)
    {
        if (type.AssemblyQualifiedName?.StartsWith("System.Collections.Generic.List") ?? false)
        {
            var actualType = type.GenericTypeArguments.FirstOrDefault();
            return actualType ?? type;
        }
            
        return type;
    }
        
    private Type HandleList(Type type)
    {
        if (type.AssemblyQualifiedName?.StartsWith("System.Collections.Generic.IEnumerable") ?? false)
        {
            var actualType = type.GenericTypeArguments.FirstOrDefault();
            return actualType ?? type;
        }
            
        return type;
    }

    private Type FilterTask(Type type)
    {
        if (type == typeof(Task))
        {
            return typeof(void);
        }

        if (type.FullName?.StartsWith("System.Threading.Tasks.Task") ?? false)
        {
            var generic = type.GetGenericArguments().FirstOrDefault();
            return generic ?? type;
        }
            
        return type;
    }
}