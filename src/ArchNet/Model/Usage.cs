namespace ArchNet.Model;

public class Usage
{
    public ClassLevelContract Dependent { get; }
    public ClassLevelContract DependsOn { get; }
        
    public Usage(ClassLevelContract dependent, ClassLevelContract dependsOn)
    {
        Dependent = dependent;
        DependsOn = dependsOn;
    }

    public bool IsModuleDependency()
    {
        return Dependent.Module().Equals(DependsOn.Module());
    }

    public bool IsPublicDependency()
    {
        return !IsModuleDependency();
    }

    public override string ToString()
    {
        return $"{Dependent.Name} -> {DependsOn.Name}";
    }

    protected bool Equals(Usage other)
    {
        return Dependent.Equals(other.Dependent) && DependsOn.Equals(other.DependsOn);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Usage) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Dependent.GetHashCode() * 397) ^ DependsOn.GetHashCode();
        }
    }
}