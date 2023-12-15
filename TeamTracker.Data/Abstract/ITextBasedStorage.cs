namespace TeamTracker.Data.Abstract;

public interface ITextBasedStorage
{
    IEnumerable<string> ReadRecords();
    void WriteRecords(IEnumerable<string> records);
}