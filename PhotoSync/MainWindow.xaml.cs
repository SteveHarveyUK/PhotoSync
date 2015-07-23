using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using PhotoSyncLib;
using PhotoSyncLib.Interface;

namespace PhotoSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ObservableCollection<DirectoryInfo> _imagePaths = new ObservableCollection<DirectoryInfo>();

        private readonly IPhotoSyncEngine _photoSyncEngine = Factory.CreateEngine();

        public MainWindow()
        {
            InitializeComponent();

            AddPath(@"D:\Users\StephenH\OneDrive\Pictures\Camera Roll\2012-07-08 Wedding");

            ListviewPaths.ItemsSource = _imagePaths;
        }

        private void buttonAddPath_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = Properties.Resources.Select_image_folder,
                // RootFolder = Environment.SpecialFolder.Personal,
            };

            var result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var selectedPath = dlg.SelectedPath;
                AddPath(selectedPath);
            }
        }

        private void AddPath(string selectedPath)
        {
            var di = new DirectoryInfo(selectedPath);
            if (!_imagePaths.Contains(di))
            {
                _imagePaths.Add(di);
            }
        }

        private void ButtonProcess_Click(object sender, RoutedEventArgs e)
        {
            foreach (var di in _imagePaths)
            {
                _photoSyncEngine.AddImagePath(di.FullName);
            }

            var progressIndicator = new Progress<IProgressValue>(ReportProgress);
            _cancellationTokenSource = new CancellationTokenSource();
            _photoSyncEngine.LoadImageMetadata( progressIndicator, _cancellationTokenSource.Token );
        }

        private readonly Dictionary<int, string> _threadToMessage = new Dictionary<int, string>();
        private CancellationTokenSource _cancellationTokenSource;

        private void ReportProgress(IProgressValue obj)
        {
            if (obj.Max > -1)
            {
                if (obj.Current == 0)
                {
                    _threadToMessage.Clear();
                }
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.Maximum = obj.Max;
                ProgressBar.Minimum = 0;
                ProgressBar.Value = obj.Current;
                _threadToMessage[obj.ThreadId] = obj.Message;
                LabelProgressMessage.Content = _threadToMessage.OrderBy(p => p.Key)
                    .Aggregate(new StringBuilder(),
                        (sb, p) => sb.AppendFormat("{0}{1}", sb.Length == 0 ? "" : "\n", p.Value));
            }
            else
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                LabelProgressMessage.Content = obj.Message;
            }
        }

        private void ProgressBar_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}
