using TeamTracker.Wpf.Models;

namespace TeamTracker.Wpf.Data;

public interface ITeamRepository
{
    IReadOnlyList<Team> GetAllTeams();
    Team GetTeamById(Guid id);
    void AddTeam(Team team);
    void UpdateTeam(Team team);
    void RemoveTeam(Guid id);
    void SaveChanges();
}