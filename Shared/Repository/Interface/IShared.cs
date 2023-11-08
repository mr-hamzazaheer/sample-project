namespace Shared.Repository.Interface
{
    public interface IShared
    {
        Task<T> CallAsync<T>(string url, object objectToPost = null, Method httpMethod = Method.Get);
    }
}
