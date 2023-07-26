namespace UnoTableDb;

public interface ITableReader<T>
{
    IEnumerable<T> ReadRecords();
}