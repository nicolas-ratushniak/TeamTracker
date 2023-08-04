namespace TeamTracker.Wpf.Data;

public interface IModelConverter<TModel>
{
    public TModel ParseFromDbRecord(string record);
    public bool TryParseFromDbRecord(string record, out TModel result);
    public string ToDbRecord(TModel model);
}