namespace LibLayout.Entities;

public sealed class MenuItem
{
    public string Name { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsEnable { get; set; } = true;
    
    public override string ToString()
    {
        return Name;
    }
}