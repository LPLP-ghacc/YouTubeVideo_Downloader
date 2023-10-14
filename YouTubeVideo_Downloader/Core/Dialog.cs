using System.Windows.Forms;

namespace YouTubeVideo_Downloader.Core
{
    public static class Dialog
    {
        public static string OpenDialog()
        {
            string path = "";

            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                }
            }

            return path;
        }
    }
}
