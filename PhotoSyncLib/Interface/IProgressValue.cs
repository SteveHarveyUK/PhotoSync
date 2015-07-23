namespace PhotoSyncLib.Interface
{
    public interface IProgressValue
    {
        int ThreadId { get; }
        int Current { get; }
        int Max { get; }
        string Message { get; }
    }
}