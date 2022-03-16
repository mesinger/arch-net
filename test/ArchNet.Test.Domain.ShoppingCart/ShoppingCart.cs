namespace ArchNet.Test.Domain.ShoppingCart;

public class ShoppingCart
{
    private readonly List<ShoppingCartItem> _items = new();

    public void AddItem(ShoppingCartItem item)
    {
        
    }

    public IEnumerable<ShoppingCartItem> Items() => _items;
}

public record ShoppingCartItem(string Id, string Name);
