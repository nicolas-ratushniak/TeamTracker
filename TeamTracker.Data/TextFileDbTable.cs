using System.Text.RegularExpressions;

namespace TeamTracker.Data;

public class TextFileDbTable : ITextBasedStorage
{
    private readonly string _filePath;

    public TextFileDbTable(string folderPath, string tableName)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException();
        }

        if (!IsValidFileName(tableName))
        {
            throw new ArgumentException("Invalid table name");
        }

        var fileName = tableName + ".txt";
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