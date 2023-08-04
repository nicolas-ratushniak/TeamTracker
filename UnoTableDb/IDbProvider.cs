namespace UnoTableDb;

public interface IDbProvider
{
    IEnumerable<string> ReadRecords();
    void AppendRecord(string record);
    void WriteRecords(IEnumerable<string> records);
}