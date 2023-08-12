namespace TeamTracker.Data;

public interface ITextBasedDb
{
    IEnumerable<string> ReadRecords();
    void WriteRecords(IEnumerable<string> records);
}