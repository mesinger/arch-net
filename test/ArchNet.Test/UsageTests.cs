using System.Linq;
using ArchNet.Model;
using FluentAssertions;
using Xunit;

namespace ArchNet.Test;

public class UsageTests
{
    [Fact]
    public void ItShallHaveModuleDependency()
    {
        var dependent = new ClassLevelContract(new Module("123"), "Dependent", typeof(string),
            Enumerable.Empty<ExposedCapability>());        
        
        var dependsOn = new ClassLevelContract(new Module("123"), "DependsOn", typeof(string),
            Enumerable.Empty<ExposedCapability>());

        var usage = new Usage(dependent, dependsOn);

        usage.IsModuleDependency().Should().BeTrue();
        usage.IsPublicDependency().Should().BeFalse();
    }
    
    [Fact]
    public void ItShallHavePublicDependency()
    {
        var dependent = new ClassLevelContract(new Module("123"), "Dependent", typeof(string),
            Enumerable.Empty<ExposedCapability>());        
        
        var dependsOn = new ClassLevelContract(new Module("456"), "DependsOn", typeof(string),
            Enumerable.Empty<ExposedCapability>());

        var usage = new Usage(dependent, dependsOn);

        usage.IsModuleDependency().Should().BeFalse();
        usage.IsPublicDependency().Should().BeTrue();
    }
}