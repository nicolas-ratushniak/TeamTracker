namespace UnoTableDb;

public interface IRepository<TItem, TId>
{
    IEnumerable<TItem> GetAll();
    TItem GetById(TId id);
    void Add(TItem item);
    void Remove(TId id);
    void Save();
}