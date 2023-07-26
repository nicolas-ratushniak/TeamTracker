namespace UnoTableDb.Interfaces;

public interface ITableWriter<T>
{
    public void WriteHeader();
    public void WriteRecord(T model);
    public void WriteRecords(IEnumerable<T> models);
}