using System;
using System.Threading;

namespace PhotoSyncLib.Interface
{
    public interface IPhotoSyncEngine
    {
        void AddImagePath(string path);
        void LoadImageMetadata(IProgress<IProgressValue> progress, CancellationToken ct);
    }

    public interface IPhotoSet
    {
        
    }
}