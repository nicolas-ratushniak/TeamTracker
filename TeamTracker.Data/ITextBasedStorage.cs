namespace TeamTracker.Data;

public interface ITextBasedStorage
{
    IEnumerable<string> ReadRecords();
    void WriteRecords(IEnumerable<string> records);
}