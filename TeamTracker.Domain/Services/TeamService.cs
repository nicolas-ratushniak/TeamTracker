using System.ComponentModel.DataAnnotations;
using TeamTracker.Domain.Data;
using TeamTracker.Domain.Dto;
using TeamTracker.Domain.Exceptions;
using TeamTracker.Domain.Models;

namespace TeamTracker.Domain.Services;

public class TeamService : ITeamService
{
    private readonly IRepository<Team> _repository;

    public TeamService(IRepository<Team> repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Team> GetAll()
    {
        return _repository.GetAll().AsReadOnly();
    }

    public Team Get(Guid id)
    {
        return _repository.Get(id)
               ?? throw new EntityNotFoundException();
    }

    public void Add(TeamCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto));

        if (GetAll().Any(t => t.Name == dto.Name && t.OriginCity == dto.OriginCity))
        {
            throw new ValidationException("No way to have two identical teams in one city");
        }

        Team team = new()
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            OriginCity = dto.OriginCity,
            MembersCount = dto.MembersCount
        };
        
        _repository.Add(team);
        _repository.SaveChanges();
    }

    public void Update(TeamUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto));

        if (_repository.GetAll().Any(t => t.Name == dto.Name && t.OriginCity == dto.OriginCity && t.Id == dto.Id))
        {
            throw new ValidationException("No way to have two identical teams in one city");
        }

        var team = Get(dto.Id);
        
        _repository.Update(team);
        _repository.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var team = Get(id);

        if (GetTotalGames(team) > 0)
        {
            throw new InvalidOperationException("Cannot delete a team having played once");
        }
        
        _repository.Remove(team);
        _repository.SaveChanges();
    }

    public int CalculatePoints(Team team)
    {
        return team.GamesWon * 3 + team.GamesDrawn;
    }

    public int GetTotalGames(Team team)
    {
        return team.GamesWon + team.GamesDrawn + team.GamesLost;
    }
}