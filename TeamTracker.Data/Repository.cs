using TeamTracker.Domain.Data;
using TeamTracker.Domain.Models;

namespace TeamTracker.Data;

public class Repository<TModel> : IRepository<TModel> where TModel : ModelBase
{
    private readonly ITextBasedDb _textBasedDb;
    private readonly IModelConverter<TModel> _modelConverter;
    private readonly List<TModel> _entities;
    
    private bool _changesSaved;

    public Repository(ITextBasedDb textBasedDb, IModelConverter<TModel> modelConverter)
    {
        _textBasedDb = textBasedDb;
        _modelConverter = modelConverter;
        _entities = GetTeamsFromDb();
        _changesSaved = true;
    }
    
    public IReadOnlyList<TModel> GetAll()
    {
        return _entities.AsReadOnly();
    }

    public TModel? Get(Guid id)
    {
        return _entities.Find(t => t.Id == id);
    }

    public void Add(TModel model)
    {
        _entities.Add(model);
        _changesSaved = false;
    }

    public void Update(TModel model)
    {
        _changesSaved = false;
    }

    public void Remove(TModel model)
    {
        _entities.Remove(model);
        _changesSaved = false;
    }

    public void SaveChanges()
    {
        if (_changesSaved)
        {
            return;
        }
        
        var records = _entities.Select(t => _modelConverter.ToDbRecord(t)).ToList();
        _textBasedDb.WriteRecords(records);
        _changesSaved = true;
    }
    
    private List<TModel> GetTeamsFromDb()
    {
        return _textBasedDb.ReadRecords()
            .Select(record => _modelConverter.ParseFromDbRecord(record)).ToList();
    }
}