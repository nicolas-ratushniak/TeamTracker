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
    public int CountPoints(Guid id);
    public int CountGamesWon(Guid id);
    public int CountGamesDrawn(Guid id);
    public int CountGamesLost(Guid id);
    public int CountTotalGames(Guid id);
}