using TeamTracker.Data.Models;

namespace TeamTracker.Data;

public class Repository<TModel> : IRepository<TModel> where TModel : ModelBase
{
    private readonly ITextBasedStorage _textBasedStorage;
    private readonly IModelConverter<TModel> _modelConverter;
    private readonly List<TModel> _entities;
    
    private bool _changesSaved;

    public Repository(ITextBasedStorage textBasedStorage, IModelConverter<TModel> modelConverter)
    {
        _textBasedStorage = textBasedStorage;
        _modelConverter = modelConverter;
        _entities = GetTeamsFromDb();
        _changesSaved = true;
    }
    
    public List<TModel> GetAll()
    {
        return _entities;
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
        _textBasedStorage.WriteRecords(records);
        _changesSaved = true;
    }
    
    private List<TModel> GetTeamsFromDb()
    {
        return _textBasedStorage.ReadRecords()
            .Select(record => _modelConverter.ParseFromDbRecord(record)).ToList();
    }
}