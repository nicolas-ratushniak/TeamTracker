using TeamTracker.Wpf.Data.Exceptions;
using TeamTracker.Wpf.Models;
using UnoTableDb;

namespace TeamTracker.Wpf.Data;

public class TeamRepository : ITeamRepository
{
    private readonly IDbProvider _dbProvider;
    private readonly IModelConverter<Team> _modelConverter;

    private readonly List<Team> _teams;
    private bool _changesSaved;

    public TeamRepository(IDbProvider dbProvider, IModelConverter<Team> modelConverter)
    {
        _dbProvider = dbProvider;
        _modelConverter = modelConverter;
        _teams = GetTeamsFromDb();
        _changesSaved = true;
    }

    public IReadOnlyList<Team> GetAllTeams()
    {
        return _teams.AsReadOnly();
    }

    public Team GetTeamById(Guid id)
    {
        return _teams.Find(t => t.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public void AddTeam(Team team)
    {
        if (team.Id == Guid.Empty)
        {
            team.Id = Guid.NewGuid();
        }
        
        _dbProvider.AppendRecord(_modelConverter.ToDbRecord(team));
        _teams.Add(team);
    }

    public void UpdateTeam(Team team)
    {
        if (!_teams.Contains(team))
        {
            throw new EntityNotFoundException();
        }

        _changesSaved = false;
    }

    public void RemoveTeam(Guid id)
    {
        var team = GetTeamById(id);
        
        _teams.Remove(team);
        _changesSaved = false;
    }

    public void SaveChanges()
    {
        if (!_changesSaved)
        {
            var records = _teams.Select(t => _modelConverter.ToDbRecord(t));
            _dbProvider.WriteRecords(records);
        }

        _changesSaved = true;
    }

    private List<Team> GetTeamsFromDb()
    {
        return _dbProvider.ReadRecords()
            .Select(record => _modelConverter.ParseFromDbRecord(record)).ToList();
    }
}