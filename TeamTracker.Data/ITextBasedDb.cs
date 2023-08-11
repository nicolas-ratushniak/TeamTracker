namespace TeamTracker.Data;

public interface ITextBasedDb
{
    IEnumerable<string> ReadRecords();
    void AppendRecord(string record);
    void WriteRecords(IEnumerable<string> records);
}