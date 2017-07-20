﻿using System;
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

			_rotateTransform = new RotateTransform(0);

			#region SetWindowPosition

			Left = SystemParameters.PrimaryScreenWidth - Width - 30;
			Top = SystemParameters.PrimaryScreenHeight - Height - 65; // 40

			#endregion

			#region SetTimer

			var dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
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
			_rotateTransform.Angle += 5f;
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
			var separators = new[] { " - " };
			var info = trackInfo.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			return new Tuple<string, string>(info[0], info[1]);
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			var window = (Window)sender;
			window.Topmost = true;
		}
	}
}
