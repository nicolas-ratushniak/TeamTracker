using TeamTracker.Data.Exceptions;
using TeamTracker.Domain.Models;

namespace TeamTracker.Data;

public class TeamRepository : ITeamRepository
{
    private readonly ITextBasedDb _textBasedDb;
    private readonly IModelConverter<Team> _modelConverter;

    private readonly List<Team> _teams;
    private bool _changesSaved;

    public TeamRepository(ITextBasedDb textBasedDb, IModelConverter<Team> modelConverter)
    {
        _textBasedDb = textBasedDb;
        _modelConverter = modelConverter;
        _teams = GetTeamsFromDb();
        _changesSaved = true;
    }

    public IReadOnlyList<Team> GetAll()
    {
        return _teams.AsReadOnly();
    }

    public Team Get(Guid id)
    {
        return _teams.Find(t => t.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public void Add(Team team)
    {
        _textBasedDb.AppendRecord(_modelConverter.ToDbRecord(team));
        _teams.Add(team);
    }

    public void Update(Team team)
    {
        if (!_teams.Contains(team))
        {
            throw new EntityNotFoundException();
        }

        _changesSaved = false;
    }

    public void Remove(Guid id)
    {
        var team = Get(id);
        
        _teams.Remove(team);
        _changesSaved = false;
    }

    public void SaveChanges()
    {
        if (!_changesSaved)
        {
            var records = _teams.Select(t => _modelConverter.ToDbRecord(t)).ToList();
            _textBasedDb.WriteRecords(records);
        }

        _changesSaved = true;
    }

    private List<Team> GetTeamsFromDb()
    {
        return _textBasedDb.ReadRecords()
            .Select(record => _modelConverter.ParseFromDbRecord(record)).ToList();
    }
}