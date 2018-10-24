namespace Geone.Utiliy.Library
{
    public interface IJsonFileAccess
    {
        string GetValue(string path, string file, params string[] Sections);

        T GetFullPath<T>(string path, string file, params string[] Sections);

        T GetFullRoot<T>(string path, string file, params string[] Sections);

        bool SetValue(string path, string file, string value);

        bool SetFullPath<T>(string path, string file, T model);

        bool SetFullRoot<T>(string path, string file, T model);
    }
}