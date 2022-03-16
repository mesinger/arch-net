namespace ArchNet.Model;

public class Module
{
    public Module(string id)
    {
        Id = id;
    }

    public string Id { get; }

    protected bool Equals(Module other)
    {
        return Id == other.Id;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Module) obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return Id;
    }
}