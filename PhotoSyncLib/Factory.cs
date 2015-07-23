using PhotoSyncLib.Interface;
using PhotoSyncLib.Model;

namespace PhotoSyncLib
{
    public static class Factory
    {
        public static IPhotoSyncEngine CreateEngine()
        {
            return new PhotoSyncEngine();
        }
    }
}
