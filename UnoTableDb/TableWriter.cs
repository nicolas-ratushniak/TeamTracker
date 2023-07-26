using System.Reflection;
using UnoTableDb.Attributes;
using UnoTableDb.Core;

namespace UnoTableDb;

public class TableWriter<T> : ITableWriter<T> where T : BaseModel
{
    private readonly string _filePath;
    private readonly char _separator;

    public TableWriter(string filePath, char separator)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        _filePath = filePath;
        _separator = separator;
    }

    public void WriteHeader()
    {
        var propNames = typeof(T).GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
            .Select(p => p.Name);

        using (FileStream fs = new(_filePath, FileMode.Truncate))
        using (TextWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(string.Join(_separator, propNames));
        }
    }

    public void WriteRecord(T model)
    {
        var propValues = typeof(T).GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
            .Select(p => p.GetValue(model));

        using (FileStream fs = new(_filePath, FileMode.Append))
        using (TextWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(string.Join(_separator, propValues));
        }
    }

    public void WriteRecords(IEnumerable<T> models)
    {
        using (FileStream fs = new(_filePath, FileMode.Append))
        using (TextWriter writer = new StreamWriter(fs))
        {
            foreach (var model in models)
            {
                var propValues = typeof(T).GetProperties()
                    .Where(p => !Attribute.IsDefined(p, typeof(NotTrackedAttribute)))
                    .Select(p => p.GetValue(model));
                
                writer.WriteLine(string.Join(_separator, propValues));
            }
        }
    }
}