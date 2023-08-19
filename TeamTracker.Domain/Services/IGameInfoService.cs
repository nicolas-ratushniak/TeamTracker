using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Models;

namespace TeamTracker.Domain.Services;

public interface IGameInfoService
{
    public IReadOnlyList<GameInfo> GetAll();
    public GameInfo Get(Guid id);
    public void PlayGame(GameInfoCreateDto dto);
}