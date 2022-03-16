using System;
using System.Collections.Generic;
using System.Linq;
using ArchNet.Loader;
using ArchNet.Patterns;
using ArchNet.Test.Domain.Logistics;
using ArchNet.Test.Domain.ShoppingCart;
using Xunit;

namespace ArchNet.Test;

public class PatternTest
{
    [Fact]
    public void AggregatePattern()
    {
        var context = new DefaultArchitectureTestContext();

        typeof(ShoppingCart).AsAnAggregate(context)
            .ShallExpose("AddItem", new [] {typeof(ShoppingCartItem)}, typeof(void))
            .Verify();
        
        typeof(ShoppingCart).AsAnAggregate(context)
            .ShallExpose("Items", Enumerable.Empty<Type>(), typeof(IEnumerable<ShoppingCartItem>))
            .Verify();

        typeof(ShoppingCart).AsAnAggregate(context).AndNothingElse().Verify();
    }

    [Fact]
    public void BoundedContextPattern()
    {
        var testContext = new DefaultArchitectureTestContext();

        var shoppingCartContext = typeof(ShoppingCart).Assembly.AsBoundedContext(testContext);
        var logisticsContext = typeof(Delivery).Assembly.AsBoundedContext(testContext);

        shoppingCartContext.ShouldNotKnow(logisticsContext).Verify();
    }

    [Fact]
    public void DependencyPattern()
    {
        var context = new DefaultArchitectureTestContext();

        typeof(ShoppingCart).AndItsDependencies(context)
            .ShouldOnlyHave(typeof(ShoppingCartItem))
            .Verify();
    }
}