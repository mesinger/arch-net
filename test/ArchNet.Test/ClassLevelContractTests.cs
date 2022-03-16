using System;
using System.Linq;
using ArchNet.Model;
using FluentAssertions;
using Xunit;

namespace ArchNet.Test;

public class ClassLevelContractTests
{
    [Fact]
    public void ItShallBeInModule()
    {
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string),
            Enumerable.Empty<ExposedCapability>());

        contract.Module().Should().Be(new Module("123"));
    }
    
    [Fact]
    public void ItShallExposeCapabilities()
    {
        var capabilities = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities);

        contract.Capabilities().Should().Equal(
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)), 
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)));
    }
    
    [Fact]
    public void ItShallExposeCapability()
    {
        var capabilities = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities);

        contract.Exposes(new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void))).Should().BeTrue();
    }
    
    [Fact]
    public void ItShallNotExposeOtherCapability()
    {
        var capabilities = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities);

        contract.Exposes(new ExposedCapability("Other", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void))).Should().BeFalse();
    }
    
    [Fact]
    public void ItShallExposeExactly()
    {
        var capabilities = new[]
        {
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)),
        };
        
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string), capabilities);

        contract.ExposesExactly(
            new ExposedCapability("Bar", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void)), 
            new ExposedCapability("Baz", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(void))).Should().BeTrue();
    }
    
    [Fact]
    public void ItShallUseModule()
    {
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());

        contract.UsesModule(new Module("456")).Should().BeFalse();
        contract.UsesModuleNot(new Module("456")).Should().BeTrue();
        
        contract.AddUsedContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>()));
        
        contract.UsesModule(new Module("456")).Should().BeTrue();
        contract.UsesModuleNot(new Module("456")).Should().BeFalse();
    }
    
    [Fact]
    public void ItShallUseExactlyTheseModules()
    {
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string),
            Enumerable.Empty<ExposedCapability>());
        contract.AddUsedContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>()));
        contract.AddUsedContract(new ClassLevelContract(new Module("789"), "Baz", typeof(string), Enumerable.Empty<ExposedCapability>()));

        contract.UsesExactlyTheseModules(new Module("456"), new Module("789"));
    }
    
    [Fact]
    public void ItShallUseContract()
    {
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string), Enumerable.Empty<ExposedCapability>());

        contract.UsesContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>())).Should().BeFalse();
        contract.UsesContractNot(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>())).Should().BeTrue();
        
        contract.AddUsedContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>()));
        
        contract.UsesContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>())).Should().BeTrue();
        contract.UsesContractNot(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>())).Should().BeFalse();
    }
    
    [Fact]
    public void ItShallUseExactlyTheseContracts()
    {
        var contract = new ClassLevelContract(new Module("123"), "Foo", typeof(string),
            Enumerable.Empty<ExposedCapability>());
        contract.AddUsedContract(new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>()));
        contract.AddUsedContract(new ClassLevelContract(new Module("456"), "Baz", typeof(string), Enumerable.Empty<ExposedCapability>()));

        contract.UsesExactlyTheseContracts(
            new ClassLevelContract(new Module("456"), "Bar", typeof(string), Enumerable.Empty<ExposedCapability>()),
            new ClassLevelContract(new Module("456"), "Baz", typeof(string), Enumerable.Empty<ExposedCapability>())
        );
    }
}