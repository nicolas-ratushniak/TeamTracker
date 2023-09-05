using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Models;

namespace TeamTracker.Domain.Services;

public interface ITeamService
{
    public IReadOnlyList<Team> GetAll();
    public Team Get(Guid id);
    public void Add(TeamCreateDto dto); 
    public void Update(TeamUpdateDto dto);
    public void Delete(Guid id);
    public int GetPoints(Team team);
    public int GetGamesWon(Team team);
    public int GetGamesDrawn(Team team);
    public int GetGamesLost(Team team);
    public int GetTotalGames(Team team);
}