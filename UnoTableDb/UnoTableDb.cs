using UnoTableDb.Core;
using UnoTableDb.Exceptions;
using UnoTableDb.Interfaces;

namespace UnoTableDb;

public class UnoTableDb<T> : IDatabase where T : BaseModel
{
    private readonly string _filePath;

    public UnoTableDb(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        if (!BaseModel.IsModelTypeValid(typeof(T)))
        {
            throw new InvalidModelTypeException();
        }
    }

    public IEnumerable<object> ReadRecords()
    {
        List<BaseModel> result = new();

        using (FileStream fs = new(_filePath, FileMode.Open))
        using (TextReader reader = new StreamReader(fs))
        {
            while (reader.Peek() > -1)
            {
                if (BaseModel.TryParse(reader.ReadLine(), out T item))
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }

    public void AppendRecord(object item)
    {
        using (FileStream fs = new(_filePath, FileMode.Append))
        using (TextWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(item.ToString());
        }
    }

    public void WriteRecords(IEnumerable<object> items)
    {
        using (FileStream fs = new(_filePath, FileMode.Truncate))
        using (TextWriter writer = new StreamWriter(fs))
        {
            foreach (var item in items)
            {
                writer.WriteLine(item.ToString());
            }
        }
    }
}