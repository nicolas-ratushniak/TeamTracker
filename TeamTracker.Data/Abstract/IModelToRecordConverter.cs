namespace TeamTracker.Data.Abstract;

public interface IModelToRecordConverter<TModel>
{
    public TModel ParseFromDbRecord(string record);
    public string ToDbRecord(TModel model);
}