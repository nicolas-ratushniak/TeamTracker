using TeamTracker.Data.Models;
using TeamTracker.Domain.Dto;

namespace TeamTracker.Domain.Services;

public interface IGameInfoService
{
    public IReadOnlyList<GameInfo> GetAll();
    public GameInfo Get(Guid id);
    public void PlayGame(GameInfoCreateDto dto);
}