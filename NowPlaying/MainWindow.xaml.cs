using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SpotifyAPI.Local;

namespace NowPlaying
{
	public partial class MainWindow
	{
		private readonly RotateTransform _rotateTransform;
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

		    #endregion

            #region IconSpin

            _rotateTransform = new RotateTransform(0);

			#endregion

			#region SetWindowPosition

			Left = SystemParameters.PrimaryScreenWidth - Width - 30;
			Top = SystemParameters.PrimaryScreenHeight - Height - 65; // 40

			#endregion

			#region SetTimer

			var dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
			dispatcherTimer.Start();

            #endregion

            #region SetInitialInfo

		    var status = _spotify.GetStatus();
		    TbTitle.Text = status.Track.TrackResource.Name;
		    TbArtist.Text = status.Track.ArtistResource.Name;

            //CommandManager.InvalidateRequerySuggested();

            #endregion
        }

		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
		    var status = _spotify.GetStatus();
		    TbTitle.Text = status.Track.TrackResource.Name;
		    TbArtist.Text = status.Track.ArtistResource.Name;
			_rotateTransform.Angle += 2f;
			ImgIcon.RenderTransform = _rotateTransform;

			CommandManager.InvalidateRequerySuggested();
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
