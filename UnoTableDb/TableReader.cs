using UnoTableDb.Core;
using UnoTableDb.Interfaces;

namespace UnoTableDb;

public class TableReader<T> : ITableReader<T> where T : BaseModel
{
    private readonly string _filePath;
    private readonly IPropertyParser<T> _parser;

    public TableReader(string filePath, char separator)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        _filePath = filePath;
        _parser = new PropertyParser<T>(separator);
    }

    public IEnumerable<T> ReadRecords()
    {
        var result = new List<T>();
        
        using (FileStream fs = File.OpenRead(_filePath))
        using (TextReader reader = new StreamReader(fs))
        {
            if (!_parser.TryParsePropertyNames(reader.ReadLine()))
            {
                throw new FormatException("Cannot parse a header line.");
            }
            while (reader.Peek() > -1)
            {
                if (_parser.TryParseModelFromLine(reader.ReadLine(), out T model))
                {
                    result.Add(model);
                }
                else
                {
                    throw new FormatException("Cannot parse some records from a file.");
                }
            }
        }
        return result;
    }
}