using TeamTracker.Domain.Data;
using TeamTracker.Domain.Models;

namespace TeamTracker.Data;

public class TeamRepository : IRepository<Team>
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

    public Team? Get(Guid id)
    {
        return _teams.Find(t => t.Id == id);
    }

    public void Add(Team team)
    {
        _teams.Add(team);
        _changesSaved = false;
    }

    public void Update(Team team)
    {
        _changesSaved = false;
    }

    public void Remove(Team model)
    {
        _teams.Remove(model);
        _changesSaved = false;
    }

    public void SaveChanges()
    {
        if (_changesSaved)
        {
            return;
        }
        
        var records = _teams.Select(t => _modelConverter.ToDbRecord(t)).ToList();
        _textBasedDb.WriteRecords(records);
        _changesSaved = true;
    }

    private List<Team> GetTeamsFromDb()
    {
        return _textBasedDb.ReadRecords()
            .Select(record => _modelConverter.ParseFromDbRecord(record)).ToList();
    }
}