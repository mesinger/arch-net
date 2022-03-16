using System;
using System.Linq;
using ArchNet.Model;
using FluentAssertions;
using Xunit;

namespace ArchNet.Test;

public class ModuleLevelContractTests
{
    [Fact]
    public void ItShallBeInModule()
    {
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.Module().Should().Be(new Module("123"));
    }
    
    [Fact]
    public void ItShallExposeCapabilities()
    {
        var capabilities1 = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities1);
        
        var capabilities2 = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract2 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);
        contract.AddChildContract(contract2);

        contract.Capabilities().Should().Equal(
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)), 
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)));
    }
    
    [Fact]
    public void ItShallExposesExactlyTheseCapabilities()
    {
        var capabilities1 = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities1);
        
        var capabilities2 = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract2 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);
        contract.AddChildContract(contract2);

        contract.ExposesExactly(
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)), 
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void))).Should().BeTrue();
    }
    
    [Fact]
    public void ItShallExpose()
    {
        var capabilities1 = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities1);
        
        var capabilities2 = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract2 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);
        contract.AddChildContract(contract2);

        contract.Exposes(
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void))).Should().BeTrue();
    }
    
    [Fact]
    public void ItShallReturnPublicUsage()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.Usages().Should().HaveCount(1).And.Equal(new Usage(contract1, contract2));
    }
    
    [Fact]
    public void ItShallNotReturnInternalUsage()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("123"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.Usages().Should().HaveCount(0);
    }
    
    [Fact]
    public void ItShallUseModuleFromPublicUsage()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.UsesModule(new Module("456")).Should().BeTrue();
    }
    
    [Fact]
    public void ItShallNotUseSameModule()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("123"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.UsesModule(new Module("123")).Should().BeFalse();
    }
    
    [Fact]
    public void ItShallUseContractFromOtherModule()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.UsesContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string),
            Enumerable.Empty<ExposedCapability>())).Should().BeTrue();
    }
    
    [Fact]
    public void ItShallNotUseContractFromSameModule()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("123"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.UsesContract(new ClassLevelContract(new Module("123"), "Bar", typeof(string),
            Enumerable.Empty<ExposedCapability>())).Should().BeFalse();
    }
    
    [Fact]
    public void ItShallUseExactlyModules()
    {
        var contract1 = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());
        var contract2 = new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>());
        contract1.AddUsedContract(contract2);
        
        var contract = new ModuleLevelContract("Foo", new Module("123"));
        contract.AddChildContract(contract1);

        contract.UsesExactlyTheseModules(new Module("456")).Should().BeTrue();
    }
}