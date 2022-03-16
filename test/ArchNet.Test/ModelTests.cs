using ArchNet.Model;
using FluentAssertions;
using Xunit;

namespace ArchNet.Test;

public class ModelTests
{
    [Fact]
    public void TestExposedCapabilityEquality()
    {
        var a = new ExposedCapability("SubStr", ExposureLevel.Public, new[] {typeof(int), typeof(int)}, typeof(string));
        var b = new ExposedCapability("SubStr", ExposureLevel.Public, new[] {typeof(int), typeof(int)}, typeof(string));

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void TestModuleEquality()
    {
        var a = new Module("123456");
        var b = new Module("123456");

        a.Equals(b).Should().BeTrue();
    }
}