namespace UnoTableDb.Interfaces;

public interface IDatabase
{
    IEnumerable<object> ReadRecords();
    void AppendRecord(object item);
    void WriteRecords(IEnumerable<object> items);
}