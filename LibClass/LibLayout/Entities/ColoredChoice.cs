namespace LibLayout.Entities;

public class ColoredChoice<T>(T value, string color)
{
    public T Value { get; } = value;
    public string FormattedText { get; } = $"[{color}]{value}[/]";

    public override string ToString()
    {
        return FormattedText;
    }
}