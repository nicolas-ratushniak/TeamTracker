namespace TeamTracker.Wpf.ViewModels;

public class SelectTeamItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    
    public string FullName => $"{Name}-{OriginCity}";
}