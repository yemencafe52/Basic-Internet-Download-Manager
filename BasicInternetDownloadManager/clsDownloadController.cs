namespace BasicInternetDownloadManager
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class DownloadController
    {
        private static List<FileDownloader> files;
        private static int fileNumber = 0;
        //================================
        private static object obj = new object();

        internal delegate void UpdateViewDlg(FileDownloader fd);
        internal static event UpdateViewDlg UpdateView;
        internal DownloadController()
        {
            if (files is null)
            {
                files = new List<FileDownloader>();
            }
        }
        internal void Add(string link)
        {
            files.Add(new FileDownloader(++fileNumber, link));
            files[files.Count - 1].Start();
            UpdateView(files[files.Count - 1]);
        }
        internal void Stop(int number)
        {
            FileDownloader fd = files.Find(p => p.Number == number);

            if(!(fd is null))
            {
                fd.Stop();
            }

            UpdateView(fd);
        }
        internal void Resume(int number)
        {
            FileDownloader fd = files.Find(p => p.Number == number);

            if (!(fd is null))
            {
                fd.Start();
            }

            UpdateView(fd);
        }
        internal void Cancel(int number)
        {
            FileDownloader fd = files.Find(p => p.Number == number);

            if (!(fd is null))
            {
                fd.Cencel();
            }

            files.Remove(fd);
        }
        internal static void UpdateViewPrograss(FileDownloader fd)
        {
            lock (obj)
            {
                Task.Run(() => UpdateView(fd));
            }
        }
    }
}
