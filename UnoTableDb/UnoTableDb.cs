namespace UnoTableDb;

public class UnoTableDb : IDatabase
{
    private readonly string _filePath;

    public UnoTableDb(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public IEnumerable<string> ReadRecords()
    {
        List<string> result = new();

        using (FileStream fs = new(_filePath, FileMode.Open))
        using (TextReader reader = new StreamReader(fs))
        {
            while (reader.Peek() > -1)
            {
                result.Add(reader.ReadLine());
            }
        }

        return result;
    }

    public void AppendRecord(string record)
    {
        using (FileStream fs = new(_filePath, FileMode.Append))
        using (TextWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(record);
        }
    }

    public void WriteRecords(IEnumerable<string> records)
    {
        using (FileStream fs = new(_filePath, FileMode.Truncate))
        using (TextWriter writer = new StreamWriter(fs))
        {
            foreach (var record in records)
            {
                writer.WriteLine(record);
            }
        }
    }
}