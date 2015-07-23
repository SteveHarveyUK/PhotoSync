// #define STORE_SHELLOBJECT

using System;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using PhotoSyncLib.Interface;

namespace PhotoSyncLib.Model
{
    internal class Photo : IPhoto
    {
#if STORE_SHELLOBJECT
        private readonly ShellObject _shellObject;
        private ICamera _camera;
        private DateTime? _dateTaken;
        private int _threadId;
#endif

        public Photo(string path)
        {
            // Debug.WriteLine("Photo[" + Thread.CurrentThread.ManagedThreadId + "](" + path + ") started...");
            FullPath = path;
#if STORE_SHELLOBJECT
            _threadId = Thread.CurrentThread.ManagedThreadId;
            _shellObject = ShellObject.FromParsingName(path);
            var x = DateTaken;
            var xx = Camera;
#else
            // var shellObject = ShellObject.FromParsingName(path);
            using (var shellObject = ShellObject.FromParsingName(path))
            {
                DateTaken = shellObject.Properties.GetProperty<DateTime?>(SystemProperties.System.Photo.DateTaken).Value;
                Camera = new Camera(shellObject);
            }
#endif
            // Debug.WriteLine("Photo[" + Thread.CurrentThread.ManagedThreadId + "](" + path + ") complete.");
        }

        public DateTime? DateTaken
        {
#if STORE_SHELLOBJECT
            get
            {
                Debug.Assert(_threadId == Thread.CurrentThread.ManagedThreadId,"Accessing ShellObject on different thread from creation!");

                return _dateTaken ??
                       (_dateTaken = _shellObject.Properties.GetProperty<DateTime?>(SystemProperties.System.Photo.DateTaken).Value);
            }
            set { _dateTaken = value; }
#else
            get; set;
#endif
        }

        public ICamera Camera
        {
#if STORE_SHELLOBJECT
            get { return _camera ?? (_camera = new Camera(_shellObject)); }
            private set { _camera = value; }
#else
            get; private set;
#endif
        }

        public string FullPath { get; private set; }
    }
}