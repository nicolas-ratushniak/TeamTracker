using TeamTracker.Data.Models;

namespace TeamTracker.Data;

public class Repository<TModel> : IRepository<TModel> where TModel : BaseModel
{
    private readonly ITextBasedStorage _textBasedStorage;
    private readonly IModelToRecordConverter<TModel> _modelToRecordConverter;
    private readonly List<TModel> _entities;
    
    private bool _changesSaved;

    public Repository(ITextBasedStorage textBasedStorage, IModelToRecordConverter<TModel> modelToRecordConverter)
    {
        _textBasedStorage = textBasedStorage;
        _modelToRecordConverter = modelToRecordConverter;
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
        
        var records = _entities.Select(t => _modelToRecordConverter.ToDbRecord(t)).ToList();
        _textBasedStorage.WriteRecords(records);
        _changesSaved = true;
    }
    
    private List<TModel> GetTeamsFromDb()
    {
        return _textBasedStorage.ReadRecords()
            .Select(record => _modelToRecordConverter.ParseFromDbRecord(record)).ToList();
    }
}