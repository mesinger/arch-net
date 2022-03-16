using System;
using System.Linq;
using ArchNet.Loader;
using ArchNet.Model;
using FluentAssertions;
using Xunit;

namespace ArchNet.Test;

class Bar
{
    public string Id { get; } = null!;

    public void Baz(Foo foo)
    {
        // using var http = new HttpClient();
    }
}

record Foo(string Id);
    
    
public class LoaderTest
{
    [Fact]
    public void ItShallLoadClass()
    {
        IArchitectureLoader loader = new ArchitectureLoader();
        var contract = loader.LoadContractFromType(typeof(Bar));

        contract.Should().NotBeNull();

        contract.Module().Should().Be(new Module("ArchNet.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

        contract.Capabilities().Should()
            .NotBeEmpty()
            .And.HaveCount(2)
            .And.Equal(
                new ExposedCapability("get_Id", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(string)), 
                new ExposedCapability("Baz", ExposureLevel.Public, new []{typeof(Foo)}, typeof(void)));

        contract.Usages().Select(usage => usage.ToString()).Should()
            .NotBeEmpty()
            .And.HaveCount(2)
            .And.Equal(
                "ArchNet.Test.Bar -> System.String",
                "ArchNet.Test.Bar -> ArchNet.Test.Foo");
    }
        
    [Fact]
    public void ItShallLoadRecord()
    {
        IArchitectureLoader loader = new ArchitectureLoader();
        var contract = loader.LoadContractFromType(typeof(Foo));

        contract.Should().NotBeNull();

        contract.Module().Should().Be(new Module("ArchNet.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));

        contract.Capabilities().Should()
            .NotBeEmpty()
            .And.HaveCount(2)
            .And.Equal(
                new ExposedCapability("get_Id", ExposureLevel.Public, Enumerable.Empty<Type>(), typeof(string)),
                new ExposedCapability("set_Id", ExposureLevel.Public, new []{typeof(string)}, typeof(void)));

        contract.Usages().Select(usage => usage.ToString()).Should()
            .NotBeEmpty()
            .And.HaveCount(1)
            .And.Equal(
                "ArchNet.Test.Foo -> System.String");
    }
}