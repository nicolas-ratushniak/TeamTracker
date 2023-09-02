namespace TeamTracker.Wpf.ViewModels;

public class SelectTeamItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string OriginCity { get; set; }
    
    public string FullName => $"{Name}-{OriginCity}";
}