using TeamTracker.Data.Abstract;
using TeamTracker.Data.Models;

namespace TeamTracker.Data;

public class Repository<TModel> : IRepository<TModel> 
    where TModel : BaseModel
{
    private readonly ITextBasedStorage _textBasedStorage;
    private readonly IModelToRecordConverter<TModel> _modelToRecordConverter;
    private List<TModel>? _entities;

    private bool _changesSaved;

    public Repository(ITextBasedStorage textBasedStorage, IModelToRecordConverter<TModel> modelToRecordConverter)
    {
        _textBasedStorage = textBasedStorage;
        _modelToRecordConverter = modelToRecordConverter;
    }

    public List<TModel> GetAll()
    {
        if (_entities is null)
        {
            _entities = ReadRecords();
            _changesSaved = true;
        }

        return _entities;
    }

    public TModel? Get(Guid id)
    {
        if (_entities is null)
        {
            _entities = ReadRecords();
            _changesSaved = true;
        }

        return _entities.Find(t => t.Id == id);
    }

    public void Add(TModel model)
    {
        if (_entities is null)
        {
            _entities = ReadRecords();
            _changesSaved = true;
        }

        _entities.Add(model);
        _changesSaved = false;
    }

    public void Update(TModel model)
    {
        _changesSaved = false;
    }

    public void Remove(TModel model)
    {
        if (_entities is null)
        {
            _entities = ReadRecords();
            _changesSaved = true;
        }

        _entities.Remove(model);
        _changesSaved = false;
    }

    public void SaveChanges()
    {
        if (_changesSaved || _entities == null)
        {
            return;
        }

        var records = _entities
            .Select(t => _modelToRecordConverter.ToDbRecord(t))
            .ToList();
        
        _textBasedStorage.WriteRecords(records);
        _changesSaved = true;
    }

    private List<TModel> ReadRecords()
    {
        try
        {
            return _textBasedStorage.ReadRecords()
                .Select(record => _modelToRecordConverter.ParseFromDbRecord(record))
                .ToList();
        }
        catch (Exception ex)
        {
            if (ex is FormatException or ArgumentException)
            {
                throw new InvalidDataException("Failed to read records from file");
            }

            throw;
        }
    }
}