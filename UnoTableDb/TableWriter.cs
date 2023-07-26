using UnoTableDb.Core;
using UnoTableDb.Interfaces;

namespace UnoTableDb;

public class TableWriter<T> : ITableWriter<T> where T : BaseModel
{
    private readonly string _filePath;
    private readonly IPropertyFormatter<T> _formatter;

    public TableWriter(string filePath, char separator)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }
        
        _filePath = filePath;
        _formatter = new PropertyFormatter<T>(separator);
    }

    public void WriteHeader()
    {
        using (FileStream fs = new(_filePath, FileMode.Truncate))
        using (TextWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(_formatter.FormatPropertyNames(typeof(T)));
        }
    }

    public void WriteRecord(T model)
    {
        using (FileStream fs = new(_filePath, FileMode.Append))
        using (TextWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(_formatter.FormatProperties(model));
        }
    }

    public void WriteRecords(IEnumerable<T> models)
    {
        using (FileStream fs = new(_filePath, FileMode.Append))
        using (TextWriter writer = new StreamWriter(fs))
        {
            foreach (var model in models)
            {
                writer.WriteLine(_formatter.FormatProperties(model));
            }
        }
    }
}