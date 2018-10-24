namespace Geone.Utiliy.Library
{
    public interface IFileReadWrite
    {
        string Read(string path, string file);

        bool Write(string path, string file, string content);
    }
}