using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MapleStory.IO.GifDownloader
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			saveToTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		}

		private void SaveToBrowseClick(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (dialog.SelectedPath.Length > 0)
				{
					saveToTextBox.Text = dialog.SelectedPath;
				}
			}

		}

		private void StartClick(object sender, RoutedEventArgs e)
		{
			string characterUrl = characterUrlTextBox.Text;
			string savePath = saveToTextBox.Text;

			if (!Directory.Exists(savePath))
			{
				ConsoleWriteLine("Invalid Save Path. Process aborted.");
				return;
			}

			bool showFloraEars = (bool)floraEarsCheckBox.IsChecked;
			bool showHighFloraEars = (bool)highFloraEarsCheckBox.IsChecked;
			bool showMercedesEars = (bool)mercedesEarsCheckBox.IsChecked;
			bool flipHorizontally = (bool)flipHorizontallyCheckBox.IsChecked;

			startButton.IsEnabled = false;
			Task.Run(() => Start(characterUrl, savePath, showFloraEars, showHighFloraEars, showMercedesEars, flipHorizontally));
		}

		private void Start(string characterUrl, string savePath, bool showFloraEars, bool showHighFloraEars, bool showMercedesEars, bool flipHorizontally)
		{
			GifDownloader.DownloadGifs(characterUrl, savePath, showFloraEars, showHighFloraEars, showMercedesEars, flipHorizontally, this);

			Dispatcher.Invoke(() =>
			{
				startButton.IsEnabled = true;
			});
		}

		public void ConsoleWriteLine(string line)
		{
			Dispatcher.Invoke(() =>
			{
				consoleTextBox.Text += line + "\n";
				consoleScroleViwer.ScrollToEnd();
			});
		}

		private void InputChanged(object sender, TextChangedEventArgs e)
		{
			if (characterUrlTextBox.Text != string.Empty && saveToTextBox.Text != string.Empty)
			{
				startButton.IsEnabled = true;
			}
			else
			{
				startButton.IsEnabled = false;
			}
		}
	}
}