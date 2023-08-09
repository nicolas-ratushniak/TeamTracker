using TeamTracker.Wpf.Models;

namespace TeamTracker.Wpf.Data;

public interface ITeamRepository
{
    IReadOnlyList<Team> GetAll();
    Team Get(Guid id);
    void Add(Team team);
    void Update(Team team);
    void Remove(Guid id);
    void SaveChanges();
}