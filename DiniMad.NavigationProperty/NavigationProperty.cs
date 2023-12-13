namespace DiniMad.NavigationProperty;

public class NavigationProperty<T>
{
    public IReadOnlyCollection<string> Includes { get; private set; } = null!;

    protected NavigationProperty()
    {
    }

    protected static NavigationProperty<T> Create(string name, params string[] thenIncludes)
    {
        var      includes = thenIncludes.Select(then => $"{name}.{then}").Append(name);
        return new() {Includes = includes.ToArray()};
    }
}