namespace Web.Utilities
{
    public interface IFileUtility
    {
        Task SaveFileAsync(string fileName, string content, bool overwrite);
    }
}