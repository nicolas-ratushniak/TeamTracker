using System.ComponentModel.DataAnnotations;
using TeamTracker.Data;
using TeamTracker.Data.Models;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Exceptions;

namespace TeamTracker.Domain.Services;

public class TeamService : ITeamService
{
    private readonly IRepository<Team> _teamRepository;
    private readonly IRepository<GameInfo> _gameRepository;

    public TeamService(IRepository<Team> teamRepository, IRepository<GameInfo> gameRepository)
    {
        _teamRepository = teamRepository;
        _gameRepository = gameRepository;
    }

    public IReadOnlyList<Team> GetAll()
    {
        return _teamRepository.GetAll().AsReadOnly();
    }

    public Team Get(Guid id)
    {
        return _teamRepository.Get(id)
               ?? throw new EntityNotFoundException();
    }

    public void Add(TeamCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (GetAll().Any(t => t.Name == dto.Name && t.OriginCity == dto.OriginCity))
        {
            throw new ValidationException("No way having two identical teams in one city");
        }

        Team team = new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            OriginCity = dto.OriginCity,
            MembersCount = dto.MembersCount
        };
        
        _teamRepository.Add(team);
        _teamRepository.SaveChanges();
    }

    public void Update(TeamUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_teamRepository.GetAll().Any(t => t.Name == dto.Name && t.OriginCity == dto.OriginCity && t.Id != dto.Id))
        {
            throw new ValidationException("No way having two identical teams in one city");
        }

        var team = Get(dto.Id);

        team.Name = dto.Name;
        team.OriginCity = dto.OriginCity;
        team.MembersCount = dto.MembersCount;
        
        _teamRepository.Update(team);
        _teamRepository.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var team = Get(id);

        if (GetTotalGames(id) > 0)
        {
            throw new InvalidOperationException("Cannot delete a team having played once");
        }
        
        _teamRepository.Remove(team);
        _teamRepository.SaveChanges();
    }

    public int GetGamesWon(Guid id)
    {
        var team = Get(id);
        
        var gamesWonAtHome = _gameRepository.GetAll()
            .Count(g => g.TeamHomeId == team.Id && g.TeamHomeScore > g.TeamAwayScore);
        
        var gamesWonAway = _gameRepository.GetAll()
            .Count(g => g.TeamAwayId == team.Id && g.TeamHomeScore < g.TeamAwayScore);

        return gamesWonAtHome + gamesWonAway;
    }
    
    public int GetGamesDrawn(Guid id)
    {
        var team = Get(id);
        
        return _gameRepository.GetAll()
            .Count(g => (g.TeamHomeId == team.Id || g.TeamAwayId == team.Id) && 
                        g.TeamHomeScore == g.TeamAwayScore);
    }
    
    public int GetGamesLost(Guid id)
    {
        var team = Get(id);
        
        var gamesLostAtHome = _gameRepository.GetAll()
            .Count(g => g.TeamHomeId == team.Id && g.TeamHomeScore < g.TeamAwayScore);
        
        var gamesLostAway = _gameRepository.GetAll()
            .Count(g => g.TeamAwayId == team.Id && g.TeamHomeScore > g.TeamAwayScore);

        return gamesLostAtHome + gamesLostAway;
    }

    public int GetPoints(Guid id)
    {
        return GetGamesWon(id) * 3 + GetGamesDrawn(id);
    }

    public int GetTotalGames(Guid id)
    {
        var team = Get(id);
        
        return _gameRepository.GetAll()
            .Count(g => g.TeamHomeId == team.Id || g.TeamAwayId == team.Id);
    }
}