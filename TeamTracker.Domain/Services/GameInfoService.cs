using System.ComponentModel.DataAnnotations;
using TeamTracker.Data;
using TeamTracker.Data.Models;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Exceptions;

namespace TeamTracker.Domain.Services;

public class GameInfoService : IGameInfoService
{
    private readonly IRepository<GameInfo> _gamesRepository;

    public GameInfoService(IRepository<GameInfo> gamesRepository)
    {
        _gamesRepository = gamesRepository;
    }

    public IReadOnlyList<GameInfo> GetAll()
    {
        return _gamesRepository.GetAll().AsReadOnly();
    }

    public GameInfo Get(Guid id)
    {
        return _gamesRepository.Get(id)
               ?? throw new EntityNotFoundException();
    }

    public void PlayGame(GameInfoCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (dto.TeamHomeId == dto.TeamAwayId)
        {
            throw new ValidationException("Teams should be different");
        }

        if (dto.Date > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ValidationException("Cannot add a game from the future");
        }
        
        if (dto.Date < new DateOnly(1900, 1,1))
        {
            throw new ValidationException("Cannot add a game held before 1/1/1900");
        }

        GameInfo game = new()
        {
            Id = Guid.NewGuid(),
            TeamHomeId = dto.TeamHomeId,
            TeamAwayId = dto.TeamAwayId,
            TeamHomeScore = dto.TeamHomeScore,
            TeamAwayScore = dto.TeamAwayScore,
            Date = dto.Date
        };
        
        _gamesRepository.Add(game);
        _gamesRepository.SaveChanges();
    }
}