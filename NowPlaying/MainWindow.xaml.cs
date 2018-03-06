using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;

namespace NowPlaying
{
    public partial class MainWindow
    {
        private static SpotifyLocalAPI _spotify;

        public MainWindow()
        {
            InitializeComponent();

            #region Init Spotify

            _spotify = new SpotifyLocalAPI();

            if (!SpotifyLocalAPI.IsSpotifyInstalled())
            {
                return;
            }

            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                return;
            }

            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                return;
            }

            if (!_spotify.Connect())
            {
                return;
            }

            _spotify.ListenForEvents = true;
            _spotify.OnTrackChange += OnTrackChange;

            #endregion

            #region SetWindowPosition

            Left = SystemParameters.PrimaryScreenWidth - Width - 30;
            Top = SystemParameters.PrimaryScreenHeight - Height - 65; // 40

            #endregion

            #region SetInitialInfo

            var status = _spotify.GetStatus();
            TbTitle.Text = status.Track.TrackResource.Name;
            TbArtist.Text = status.Track.ArtistResource.Name;
            ImgCover.Source = new BitmapImage(new Uri(status.Track.GetAlbumArtUrl(AlbumArtSize.Size160)));

            #endregion
        }

        private void OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var status = _spotify.GetStatus();
                TbTitle.Text = status.Track.TrackResource.Name;
                TbArtist.Text = status.Track.ArtistResource.Name;
                ImgCover.Source = new BitmapImage(new Uri(status.Track.GetAlbumArtUrl(AlbumArtSize.Size160)));
            });
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}