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
    }

    public IEnumerable<string> ReadRecords()
    {
        List<string> result = new();

        using (TextReader reader = new StreamReader(File.Open(_filePath, FileMode.OpenOrCreate)))
        {
            while (reader.Peek() > -1)
            {
                result.Add(reader.ReadLine()!);
            }
        }

        return result;
    }

    public void WriteRecords(IEnumerable<string> records)
    {
        File.WriteAllLines(_filePath, records);
    }

    private static bool IsValidFileName(string? fileName)
    {
        return fileName is not null && !Regex.IsMatch(fileName, "[\"<>:/\\|?*]");
    }
}