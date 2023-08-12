using System.Text.RegularExpressions;

namespace TeamTracker.Data;

public class TextFiledDb : ITextBasedDb
{
    private readonly string _filePath;

    public TextFiledDb(string folderPath, string dbName)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException();
        }

        if (!IsValidFileName(dbName))
        {
            throw new ArgumentException("Invalid DB name");
        }

        var fileName = dbName + ".txt";
        _filePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath);
        }
    }

    public IEnumerable<string> ReadRecords()
    {
        return File.ReadLines(_filePath);;
    }

    public void WriteRecords(IEnumerable<string> records)
    {
        File.WriteAllLines(_filePath, records);
    }

    private static bool IsValidFileName(string? fileName)
    {
        return fileName is not null && Regex.IsMatch(fileName, "[^\"<>:/\\|?*]");
    }
}