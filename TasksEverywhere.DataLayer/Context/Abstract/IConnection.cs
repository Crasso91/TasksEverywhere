namespace TasksEverywhere.DataLayer.Context.Abstract
{
    public interface IConnection
    {
        T GetProperty<T>(string key);
    }
}