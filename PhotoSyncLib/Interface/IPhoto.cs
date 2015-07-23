namespace PhotoSyncLib.Interface
{
    public interface IPhoto
    {
        ICamera Camera { get; }
        string FullPath { get; }
    }
}