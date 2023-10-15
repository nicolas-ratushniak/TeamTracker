using TeamTracker.Data.Models;
using TeamTracker.Domain.Dto;

namespace TeamTracker.Domain.Services;

public interface ITeamService
{
    public IReadOnlyList<Team> GetAll();
    public Team Get(Guid id);
    public void Add(TeamCreateDto dto); 
    public void Update(TeamUpdateDto dto);
    public void Delete(Guid id);
    public int GetPoints(Guid id);
    public int GetGamesWon(Guid id);
    public int GetGamesDrawn(Guid id);
    public int GetGamesLost(Guid id);
    public int GetTotalGames(Guid id);
}