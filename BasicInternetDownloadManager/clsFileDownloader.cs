namespace BasicInternetDownloadManager
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using System.Security.Cryptography;
    using System.IO;
    using System.Net;
    using System.Threading;
    internal enum State : byte
    {
        STOPPED = 0,
        CONNECTING,
        QUERY,
        DOWNLOADING,
        COMPLETE,
        DISCONNECTED
    }
    internal class FileDownloader
    {
        private int number;
        private string link;
        private double precent;
        private State state;
        //------------------
        private long fileSize;
        private bool isRunning;
        private string filePath;

        public int Number { get => number; }
        public string Link { get => link; }
        public double Precent { get => precent; }
        internal State State { get => state; }

        internal FileDownloader(int number, string link)
        {
            this.number = number;
            this.link = link;
            GetTempFilePath();

            this.state = State.QUERY;
            this.fileSize = QueryFileSize();
        }
        internal async void Start()
        {
            this.isRunning = true;
            while (
                (this.isRunning)
                &&
                (this.state != State.COMPLETE)
                )
            {
                try
                {

                    await Task.Run(() =>
                    {
                        this.state = State.CONNECTING;

                        if (!(fileSize > 0))
                        {
                            this.state = State.QUERY;
                            this.fileSize = QueryFileSize();
                        }

                        HttpWebRequest wTX = (HttpWebRequest)WebRequest.Create(this.link);
                        wTX.AddRange(GetDownloadedFileSize());

                        HttpWebResponse wRX = (HttpWebResponse)wTX.GetResponse();
                        if ((((HttpWebResponse)wRX).StatusCode == HttpStatusCode.OK) || (((HttpWebResponse)wRX).StatusCode) == HttpStatusCode.PartialContent)
                        {
                            Stream s = wRX.GetResponseStream();
                            byte[] buffer = new byte[1024 * 1024 * 8];
                            int len = 0;

                            this.state = State.DOWNLOADING;
                            while (
                            ((len = s.Read(buffer, 0, buffer.Length)) > 0)
                            &&
                            (this.isRunning)
                            )
                            {
                                FileStream fs = new FileStream(this.filePath, FileMode.Append);
                                fs.Write(buffer, 0, len);

                                if ((fileSize > 0))
                                {
                                    this.precent = ((double)fs.Length / (double)fileSize) * (double)100;
                                }

                                fs.Close();

                                DownloadController.UpdateViewPrograss(this);
                                Thread.Sleep(10);
                            }

                            s.Close();

                            if (isRunning)
                            {
                                if (new FileInfo(this.filePath).Length >= this.fileSize)
                                {
                                    this.state = State.COMPLETE;
                                    this.isRunning = false;

                                    try
                                    {
                                        if (!(System.IO.Directory.Exists(Constants.DownloadFolder)))
                                        {
                                            System.IO.Directory.CreateDirectory(Constants.DownloadFolder);
                                        }

                                        File.Copy(this.filePath, Constants.DownloadFolder + "\\" + GetFileNameFromLink(), false);
                                        File.Delete(this.filePath);

                                        DownloadController.UpdateViewPrograss(this);
                                    }
                                    catch { }
                                }
                            }
                        }
                    });
                }

                catch
                {
                    this.state = State.DISCONNECTED;
                    Thread.Sleep(1000);

                }
            }
        }
        internal void Stop()
        {
            try
            {
                this.isRunning = false;
                this.state = State.STOPPED;
            }
            catch { }
        }
        internal void Cencel()
        {
            Stop();

            try
            {
                this.isRunning = false;
                this.state = State.STOPPED;
                File.Delete(this.filePath);
            }
            catch { }
        }
        private long QueryFileSize()
        {
            long res = 0;

            try
            {
                WebRequest wTX = WebRequest.Create(this.link);
                HttpWebResponse httpWebRespone = (HttpWebResponse)wTX.GetResponse();

                if (httpWebRespone.StatusCode == HttpStatusCode.OK)
                {
                    res = httpWebRespone.ContentLength;
                }

                httpWebRespone.Close();
            }
            catch { }
            return res;
        }
        private long GetDownloadedFileSize()
        {
            long res = 0;

            try
            {
                if (File.Exists(this.filePath))
                {
                    res = new FileInfo(this.filePath).Length;
                }
            }
            catch { }
            return res;
        }
        private string GetTempFilePath()
        {
            string res = string.Empty;

            try
            {
                if (this.filePath is null)
                {
                    if (!(System.IO.Directory.Exists(Constants.TempFolder)))
                    {
                        System.IO.Directory.CreateDirectory(Constants.TempFolder);
                    }

                    this.filePath = Constants.TempFolder + "\\" + GetHashTempFileName();

                    FileStream fs = new FileStream(this.filePath, FileMode.OpenOrCreate);
                    fs.Close();
                }

                res = this.filePath;

            }

            catch { }
            return res;
        }
        private string GetHashTempFileName()
        {
            string res = string.Empty;
            using (MD5 md5 = MD5CryptoServiceProvider.Create())
            {
                byte[] ar = md5.ComputeHash(Encoding.UTF8.GetBytes(this.number.ToString() + this.link));
                res = Convert.ToBase64String(ar);
            }
            return res;
        }
        private string GetFileNameFromLink()
        {
            string res = string.Empty;

            try
            {
                Uri url = new Uri(this.link);
                res =  this.number.ToString() + "_" + url.Segments[url.Segments.Length - 1];
            }
            catch { }

            return res;
        }
    }
}
