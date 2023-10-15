using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using TeamTracker.Wpf.ViewModels.Inners;

namespace TeamTracker.Wpf.ViewModels.Components;

public class TeamSelectorComponent : BaseViewModel
{
    public event EventHandler SelectedTeamChanged;
    
    private TeamDropdownListItemViewModel? _selectedTeam;
    private string _teamNameFilter = string.Empty;
    private bool _isTeamListVisible;
    private string _placeholder;

    public ICollectionView TeamCollectionView { get; }

    public ObservableCollection<TeamDropdownListItemViewModel> Teams { get; }

    public string Placeholder
    {
        get => _placeholder;
        set
        {
            if (value == _placeholder) return;
            _placeholder = value;
            OnPropertyChanged();
        }
    }

    public TeamDropdownListItemViewModel? SelectedTeam
    {
        get => _selectedTeam;
        set
        {
            if (Equals(value, _selectedTeam)) return;
            _selectedTeam = value;
            
            OnPropertyChanged();
            OnSelectedTeamChanged();
        }
    }

    public string TeamNameFilter
    {
        get => _teamNameFilter;
        set
        {
            if (value == _teamNameFilter) return;
            _teamNameFilter = value;
            
            OnPropertyChanged();
            OnTeamFilterChanged();
        }
    }

    public bool IsTeamListVisible
    {
        get => _isTeamListVisible;
        set
        {
            if (value == _isTeamListVisible) return;
            _isTeamListVisible = value;
            OnPropertyChanged();
        }
    }

    public TeamSelectorComponent(string placeholder)
    {
        _placeholder = placeholder;
        Teams = new ObservableCollection<TeamDropdownListItemViewModel>();
        
        TeamCollectionView = CollectionViewSource.GetDefaultView(Teams);
        
        TeamCollectionView.SortDescriptions.Add(
            new SortDescription(nameof(TeamDropdownListItemViewModel.FullName),
                ListSortDirection.Ascending));
    }
    
    private void OnSelectedTeamChanged()
    {
        if (_selectedTeam != null)
        {
            TeamNameFilter = _selectedTeam.FullName;
            IsTeamListVisible = false;
        }

        SelectedTeamChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnTeamFilterChanged()
    {
        var filter = TeamNameFilter;

        if (_selectedTeam is null)
        {
            IsTeamListVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedTeam.FullName)
            {
                IsTeamListVisible = false;
                return;
            }

            IsTeamListVisible = true;
            _selectedTeam = null;
        }

        FilterTeam();
    }
    
    private void FilterTeam()
    {
        TeamCollectionView.Filter = o =>
        {
            if (o is not TeamDropdownListItemViewModel team)
            {
                return false;
            }

            var lowerFilter = TeamNameFilter.ToLower();

            return team.FullName.ToLower().StartsWith(lowerFilter) ||
                   team.OriginCity.ToLower().StartsWith(lowerFilter);
        };
    }
}