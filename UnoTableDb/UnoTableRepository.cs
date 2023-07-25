using UnoTableDb.Core;
using UnoTableDb.Exceptions;

namespace UnoTableDb;

public class UnoTableRepository<T> where T : BaseModel
{
    private string _filePath;

    public UnoTableRepository(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        if (!BaseModel.IsModelTypeValid(typeof(T)))
        {
            throw new InvalidModelTypeException("The model is invalid");
        }
    }

    // public IEnumerable<T> GetAll()
    // {
    //     
    // }
}