namespace LibLayout.Entities;

public abstract class Menu()
{
    public string Settings { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public MenuItem[]? Items { get; set; } = [];

    public ColorText Color { get; set; } = new ();
    
    public bool IsVisble { get; set; }
    
    public string Icon { get; set; } = string.Empty;
}