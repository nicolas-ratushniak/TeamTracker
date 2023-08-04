namespace UnoTableDb;

public interface IDatabase
{
    IEnumerable<string> ReadRecords();
    void AppendRecord(string record);
    void WriteRecords(IEnumerable<string> records);
}