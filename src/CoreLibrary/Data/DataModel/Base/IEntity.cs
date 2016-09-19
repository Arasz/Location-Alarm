namespace CoreLibrary.Data.DataModel.Base
{
    public interface IEntity
    {
        int Id { get; set; }

        IEntity Clone();
    }
}