using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhotoSyncLib.Interface;

namespace PhotoSyncLib.Model
{
    internal class PhotoSyncEngine : IPhotoSyncEngine
    {
        private readonly ICollection<string> _pathCollection = new Collection<string>();
        private ICollection<IPhoto> _photos;

        public void AddImagePath(string path)
        {
            if (Directory.Exists(path))
            {
                _pathCollection.Add(path);
            }
        }

        public async void LoadImageMetadata(IProgress<IProgressValue> progress, CancellationToken ct)
        {
            var sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("LoadImageMetadata[" + Thread.CurrentThread.ManagedThreadId + "]() started...");
            await Task.Run(() =>
            {
                progress.Report(new ProgressValue { Current = -1, Max = -1, Message = "Searching for photo files..."} );
                var files = _pathCollection.Select(path => new DirectoryInfo(path))
                    .Select(di => di.GetFiles("*.jpg", SearchOption.TopDirectoryOnly))
                    .SelectMany(images => images.Select(fileInfo => fileInfo.FullName))
                    .ToList();

                var count = 0;
                var max = files.Count();
                try
                {
                    _photos =
                        files
                            //.AsParallel().WithDegreeOfParallelism(2)
                            .Select(p =>
                            {
                                ct.ThrowIfCancellationRequested();

                                progress.Report(new ProgressValue
                                {
                                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                                    Current = count,
                                    Max = max,
                                    Message = string.Format("Loading '{0}'...", Path.GetFileName(p))
                                });
                                count++;
                                return (IPhoto) new Photo(p);
                            })
                            .ToList();

                    progress.Report(new ProgressValue()
                    {
                        ThreadId = Thread.CurrentThread.ManagedThreadId,
                        Current = -1,
                        Max = -1,
                        Message = string.Format("Operation Complete. [{0}]", sw.Elapsed),
                    });
                }
                catch (OperationCanceledException)
                {
                    progress.Report(new ProgressValue()
                    {
                        ThreadId = Thread.CurrentThread.ManagedThreadId,
                        Current = -1,
                        Max = -1,
                        Message = string.Format("Operation Aborted. [{0}]", sw.Elapsed),
                    });
                }
            }, ct);
            sw.Stop();
            Debug.WriteLine("LoadImageMetadata[" + Thread.CurrentThread.ManagedThreadId + "]() complete. " + sw.Elapsed);
        }
    } 
}