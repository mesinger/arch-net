# arch-net
An architecture testing (assertion) framework for dotnet.

It should allow to write pattern driven tests like this. The idea is highly driven by DDD patterns, but should also be extensible for any other pattern available in OO programming languages.

```csharp
[Fact]
public void ShoppingCartAggregateExposesOnlyAddItem()
{
  var context = new DefaultArchitectureTestContext();
  
  typeof(ShoppingCart).AsAnAggregate(context)
    .ShallExpose("AddItem", new[] {typeof(ShoppingCartItem)}, typeof(void))
    .Verify();
    
  typeof(ShoppingCart).AsAnAggregate(context).AndNothingElse().Verify();
}
```

```csharp
[Fact]
public void WebShopShouldNotKnowAboutShipping()
{
  var context = new DefaultArchitectureTestContext();

  var webShopContext = typeof(ShoppingCart).Assembly.AsBoundedContext(context);
  var shippingContext = typeof(Shipping).Assembly.AsBoundedContext(context);
        
  webShopContext.ShouldNotKnow(shippingContext).Verify();
}
```

It requires no additional test execution environment but can be used in regular unit testing frameworks like xUnit.
