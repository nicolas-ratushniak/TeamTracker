namespace TeamTracker.Data;

public interface IModelConverter<TModel>
{
    public TModel ParseFromDbRecord(string record);
    public string ToDbRecord(TModel model);
}