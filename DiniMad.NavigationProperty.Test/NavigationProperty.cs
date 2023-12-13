namespace DiniMad.NavigationProperty.Test;

public class NavigationProperty
{
    [Fact]
    public void SingleLayerInclude()
    {
        var navigation = Root.Navigations.First();

        var expectation = new[]
        {
            $"{nameof(Root.First)}",
        };

        navigation.Includes.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public void MultilayerInclude()
    {
        var navigation =
            Root.Navigations.First(First.Navigations.Second(Second.Navigations.Third1(Third.Navigations.Forth()),
                                                            Second.Navigations.Forth()),
                                   First.Navigations.Third2());

        var expectation = new[]
        {
            $"{nameof(Root.First)}",
            $"{nameof(Root.First)}.{nameof(First.Second)}",
            $"{nameof(Root.First)}.{nameof(First.Second)}.{nameof(Second.Third1)}",
            $"{nameof(Root.First)}.{nameof(First.Second)}.{nameof(Second.Third1)}.{nameof(Third.Forth)}",
            $"{nameof(Root.First)}.{nameof(First.Second)}.{nameof(Second.Forth)}",
            $"{nameof(Root.First)}.{nameof(First.Third2)}"
        };

        navigation.Includes.Should().BeEquivalentTo(expectation);
    }
}