using UnoTableDb.Core;

namespace UnoTableDb;

public class UnoTableReader<T> where T : BaseModel
{
    private readonly string _filePath;
    private readonly char _separator;
    private SimpleParser<T> _parser;

    public UnoTableReader(string filePath, char separator)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _separator = separator;
        _parser = new SimpleParser<T>(_separator);
    }

}