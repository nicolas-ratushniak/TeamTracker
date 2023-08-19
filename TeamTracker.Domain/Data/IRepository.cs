namespace TeamTracker.Domain.Data;

public interface IRepository<TModel>
{
    List<TModel> GetAll();
    TModel? Get(Guid id);
    void Add(TModel model);
    void Update(TModel model);
    void Remove(TModel model);
    void SaveChanges();
}