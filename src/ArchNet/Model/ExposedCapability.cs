namespace ArchNet.Model;

public class ExposedCapability
{
    public string Name { get; }
    public ExposureLevel Level { get; }
    public Type[] Params { get; }
    public Type ReturnType { get; }

    public ExposedCapability(string name, ExposureLevel level, IEnumerable<Type> @params, Type returnType)
    {
        Name = name;
        Level = level;
        Params = @params.ToArray();
        ReturnType = returnType;
    }

    protected bool Equals(ExposedCapability other)
    {
        return Name == other.Name && Level == other.Level && Params.SequenceEqual(other.Params) && ReturnType == other.ReturnType;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ExposedCapability) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ (int) Level;
            hashCode = (hashCode * 397) ^ Params.GetHashCode();
            hashCode = (hashCode * 397) ^ ReturnType.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        return Name;
    }
}