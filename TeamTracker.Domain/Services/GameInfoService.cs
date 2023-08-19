﻿using System.ComponentModel.DataAnnotations;
using TeamTracker.Domain.Data;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Exceptions;
using TeamTracker.Domain.Models;

namespace TeamTracker.Domain.Services;

public class GameInfoService : IGameInfoService
{
    private readonly IRepository<GameInfo> _gamesRepository;
    private readonly IRepository<Team> _teamsRepository;

    public GameInfoService(IRepository<GameInfo> gamesRepository, IRepository<Team> teamsRepository)
    {
        _gamesRepository = gamesRepository;
        _teamsRepository = teamsRepository;
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
        Validator.ValidateObject(dto, new ValidationContext(dto));

        if (dto.TeamHomeId == dto.TeamAwayId)
        {
            throw new ValidationException("Teams should be different");
        }

        GameInfo game = new()
        {
            Id = Guid.NewGuid(),
            TeamHomeId = dto.TeamHomeId,
            TeamAwayId = dto.TeamAwayId,
            TeamHomeScore = dto.TeamHomeScore,
            TeamAwayScore = dto.TeamAwayScore
        };
        
        _gamesRepository.Add(game);
        UpdateTeamsStats(game);
    }

    private void UpdateTeamsStats(GameInfo game)
    {
        var teamHome = _teamsRepository.Get(game.TeamHomeId) ?? throw new InvalidOperationException();
        var teamAway = _teamsRepository.Get(game.TeamAwayId) ?? throw new InvalidOperationException();

        if (game.TeamHomeScore == game.TeamAwayScore)
        {
            teamHome.GamesDrawn++;
            teamAway.GamesDrawn++;
        }
        else if (game.TeamHomeScore > game.TeamAwayScore)
        {
            teamHome.GamesWon++;
            teamAway.GamesLost++;
        }
        else
        {
            teamHome.GamesLost++;
            teamAway.GamesWon++;
        }
        
        _teamsRepository.Update(teamHome);
        _teamsRepository.Update(teamAway);
        _teamsRepository.SaveChanges();
    }
}