using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTubeVideo_Downloader
{
    public partial class MainWindow : Window
    {
        //when value is "" video will be download in application folder 
        private string downloadPath = "";

        private string defaultPath = Environment.CurrentDirectory + "/videos";

        public MainWindow()
        {
            InitializeComponent();

            if(!Directory.Exists(Environment.CurrentDirectory + "/videos"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "/videos");
        }

        private void download_button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Foreground = Brushes.Green;
            button.Effect.SetValue(DropShadowEffect.OpacityProperty, 0.0 as object);

            if (link_tb.Text.Length > 0)
            {
                var youtube = new YoutubeClient();

                DownloadVideo(youtube, link_tb.Text, downloadPath);
            }
            else MessageBox.Show("Put the video link first");
        }

        private async void DownloadVideo(YoutubeClient youtube, string url, string path)
        {
            top_tb.Text = "Downloading...";

            var video = await youtube.Videos.GetAsync(url);

            title_tb.Text = video.Title;

            duration_tb.Text = video.Duration.Value.ToString();

            author_tb.Text = video.Author.Title;

            try
            {
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Url);

                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                List<Task> tasks = new List<Task>();

                Task t = Task.Run(async () =>
                {
                    if (path == "")
                    {
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{defaultPath}/{video.Title}.{streamInfo.Container}");
                    }
                    else
                    {
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{path}/{video.Title}.{streamInfo.Container}");
                    }
                });
                tasks.Add(t);

                Task.WaitAll(tasks.ToArray());

                foreach (Task task in tasks)
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        top_tb.Text = "Download Link";

                        title_tb.Text = "";

                        duration_tb.Text = "";

                        author_tb.Text = "";
                    }
                }
            }
            catch
            {
                MessageBox.Show("Something went wrong, try again");
            }             
        }

        private void pathLink_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (pathLink_tb.Text.Length > 0 && e.Key == Key.Enter)
            {
                EnterEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 64, 119, 0));

                downloadPath = pathLink_tb.Text;
            }
            else MessageBox.Show("Put the directory path here");
        }

        private void download_button_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.Foreground = Brushes.Red;
        }

        private void download_button_MouseLeave(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            button.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            button.Effect.SetValue(DropShadowEffect.OpacityProperty, 0.48 as object);
        }

        //DragMove Window
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        #region Exit Button
        private void exit_button_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void exit_button_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as Button;

            var elliple = button.Content as Ellipse;

            elliple.Height = 21;
            elliple.Width = 21;
            elliple.Fill = new SolidColorBrush(Color.FromArgb(255, 237, 23, 23));
        }

        private void exit_button_MouseLeave(object sender, MouseEventArgs e)
        {
            var button = sender as Button;

            var elliple = button.Content as Ellipse;

            elliple.Height = 18;
            elliple.Width = 18;
            elliple.Fill = new SolidColorBrush(Color.FromArgb(255, 177, 30, 30));
        }
        #endregion

        #region Enter Ellipse
        private void EnterEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (pathLink_tb.Text.Length > 0)
            {
                EnterEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 64, 119, 0));

                downloadPath = pathLink_tb.Text;
            }
            else MessageBox.Show("Put the directory path here");
        }

        private void EnterEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            var ellipse = sender as Ellipse;

            ellipse.Height = 21;
            ellipse.Width = 21;
            ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 237, 23, 23));
        }

        private void EnterEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            var ellipse = sender as Ellipse;

            ellipse.Height = 18;
            ellipse.Width = 18;
            ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, 177, 30, 30));
        }
        #endregion 
    }
}
