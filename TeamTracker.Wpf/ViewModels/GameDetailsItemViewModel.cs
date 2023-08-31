﻿namespace TeamTracker.Wpf.ViewModels;

public class GameDetailsItemViewModel
{
    public Guid Id { get; set; }
    public string Date { get; set; }
    public string HomeTeamName { get; set; }
    public string HomeTeamOriginCity { get; set; }
    public int HomeTeamScore { get; set; }
    public string AwayTeamName { get; set; }
    public string AwayTeamOriginCity { get; set; }
    public int AwayTeamScore { get; set; }
}