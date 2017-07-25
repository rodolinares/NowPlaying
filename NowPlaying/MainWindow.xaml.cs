using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NowPlaying
{
	public partial class MainWindow
	{
		private readonly RotateTransform _rotateTransform;

		public MainWindow()
		{
			InitializeComponent();

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

			var info = GetTitleAndArtist();

			if (info == null)
			{
				return;
			}

			TbTitle.Text = info.Item2.TrimStart(' ');
			TbArtist.Text = info.Item1.TrimEnd(' ');

			//CommandManager.InvalidateRequerySuggested();

			#endregion
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			var info = GetTitleAndArtist();

			if (info == null)
			{
				return;
			}

			TbTitle.Text = info.Item2.TrimStart(' ');
			TbArtist.Text = info.Item1.TrimEnd(' ');
			_rotateTransform.Angle += 2f;
			ImgIcon.RenderTransform = _rotateTransform;

			CommandManager.InvalidateRequerySuggested();
		}

		private static string GetSpotifyTrackInfo()
		{
			var proc = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));

			if (proc == null)
			{
				return "Spotify is not running!";
			}

			return string.Equals(proc.MainWindowTitle, "Spotify", StringComparison.InvariantCultureIgnoreCase) ? "No track is playing" : proc.MainWindowTitle;
		}

		private static Tuple<string, string> GetTitleAndArtist()
		{
			var trackInfo = GetSpotifyTrackInfo();

			if (!trackInfo.Contains(" - "))
			{
				return null;
			}

			//var info = trackInfo.Split('-');
			var separator = new[] { " - " };
			var info = trackInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			return new Tuple<string, string>(info[0], info[1]);
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
