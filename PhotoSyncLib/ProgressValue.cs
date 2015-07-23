using PhotoSyncLib.Interface;

namespace PhotoSyncLib
{
    internal class ProgressValue : IProgressValue
    {
        public int ThreadId { get; internal set; }
        public int Current { get; internal set; }
        public int Max { get; internal set; }
        public string Message { get; internal set; }
    }
}