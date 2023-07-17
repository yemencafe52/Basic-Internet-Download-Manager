namespace BasicInternetDownloadManager
{
    using System;
    class Constants
    {
        private static string tempFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BIDMTemp";
        private static string downloadFolder = System.IO.Directory.GetCurrentDirectory() + "\\Downloads";
        internal static string TempFolder
        {
            get
            {
                return tempFolder;
            }
        }
        internal static string DownloadFolder
        {
            get
            {
                return downloadFolder;
            }
        }
    }
}
