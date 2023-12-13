namespace DiniMad.NavigationProperty.Test;

public partial class Root : IWithNavigationProperties
{
    public virtual First  First  { get; set; }
    public virtual Second Second { get; set; }
    public virtual Third  Third  { get; set; }
    public virtual Forth  Forth  { get; set; }
}

public partial class First : IWithNavigationProperties
{
    public virtual Second Second  { get; set; }
    public virtual Second Second2 { get; set; }
    public virtual Third  Third   { get; set; }
    public virtual Third  Third2  { get; set; }
    public virtual Forth  Forth   { get; set; }
    public virtual Forth  Forth2  { get; set; }
}

public partial class Second : IWithNavigationProperties
{
    public virtual Third Third1 { get; set; }
    public virtual Third Third2 { get; set; }
    public virtual Forth Forth  { get; set; }
}

public partial class Third : IWithNavigationProperties
{
    public virtual Forth Forth  { get; set; }
    public virtual Forth Forth2 { get; set; }
}

public class Forth
{
}