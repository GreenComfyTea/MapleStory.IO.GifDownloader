using System;
using System.IO;
using System.Net;

namespace MapleStory.IO.GifDownloader
{
    public class GifDownloader
    {
		public static void DownloadGifs(string characterUrl, string savePath, bool showFloraEars, bool showHighFloraEars, bool showMercedesEars, bool flipHorizontally, MainWindow mainWindow)
        {

			if(characterUrl.Equals(string.Empty))
            {
				mainWindow.ConsoleWriteLine("Incorrect Character URL. Process Aborted.");
				return;
			}

			if (!characterUrl.StartsWith("http"))
			{
				characterUrl = "https://" + characterUrl;
			}

			// Decoding URL into a readable format
			Uri uri;
			try
            {
				uri = new Uri(characterUrl, true);
			}
			catch (ArgumentNullException ex1)
			{
				mainWindow.ConsoleWriteLine("Incorrect Character URL. Process Aborted.");
				return;
			}
			catch (UriFormatException ex2)
            {
				mainWindow.ConsoleWriteLine("Incorrect Character URL. Process Aborted.");
				return;
			}

			// Trying to check that the URL is correct
			string characterUrlDecoded = uri.ToString();
			string[] characterUrlSplit = characterUrlDecoded.Split('/');

			if (characterUrlSplit.Length != 8)
			{
				mainWindow.ConsoleWriteLine("Incorrect Character URL. Process Aborted.");
				return;
			}

			// Still trying to check that the URL is correct
			string characterData;
			if (characterUrlSplit[0].ToLower().Contains("http"))
            {
				characterData = characterUrlSplit[5];
			}
			else
            {
				characterData = characterUrlSplit[4];
			}

			// Looking for the expression string and marking it's place
			foreach (string expression in Data.Expressions)
            {
				if (characterData.Contains(expression))
				{
					characterData.Replace(expression, "EXPRESSION");
					break;
				}
            }

			// Assembling parameters into URL format.
			string linkBase = "https://maplestory.io/api/character/";
			string linkEnd = "/animated?showears=" + showMercedesEars.ToString() + "&showLefEars=" + showFloraEars.ToString() + "&showHighLefEars=" + showHighFloraEars.ToString() + "&name=&flipX=" + flipHorizontally.ToString();

			// Hashing character URL for naming the folder to avoid collisions
			int characterHash = characterData.GetStableHashCode();

			// Making string of parameters for naming the folder
			string floraEarsString = showFloraEars ? ", Flora Ears" : "";
			string highFloraEarsString = showHighFloraEars ? ", High Flora Ears" : "";
			string mercedesEarsString = showMercedesEars ? ", Mercedes Ears" : "";
			string flipString = flipHorizontally ? ", Flipped" : "";
			string defaultSavePathSubFolder = Path.Combine(savePath, string.Format("Maple Story GIFs ({0}{1}{2}{3}{4})", characterHash, floraEarsString, highFloraEarsString, mercedesEarsString, flipString));
			string savePathSubFolder = defaultSavePathSubFolder;

			// Creating the folder
			bool success = false;
			int index = 0;
			while(!success)
            {
				if (!Directory.Exists(savePathSubFolder))
				{
					Directory.CreateDirectory(savePathSubFolder);
					success = true;
				}
				else
				{
					savePathSubFolder = defaultSavePathSubFolder + " - " + index.ToString();
					index++;
				}
			}

			// Fun begins
			mainWindow.ConsoleWriteLine("Starting downloading...");

			WebClient webClient = new WebClient();

			foreach (string expression in Data.Expressions)
			{
				string expressionFolder = Path.Combine(savePathSubFolder, expression);
				if (!Directory.Exists(expressionFolder))
				{
					Directory.CreateDirectory(expressionFolder);
				}

				foreach (string action in Data.Actions)
				{
					// Assembling file names
					string url = linkBase + characterData.Replace("EXPRESSION", expression) + "/" + action + linkEnd;
					string saveFileName = expression + "_" + action + ".gif";
					string saveFileFullName = Path.Combine(expressionFolder, saveFileName);

					// Downloading
					try
                    {
						webClient.DownloadFile(url, saveFileFullName);
					}
					catch(WebException ex)
                    {
						mainWindow.ConsoleWriteLine(saveFileName + ": " + ex.Message);
						continue;
					}
					
					// Was the download successful?
					if (File.Exists(saveFileFullName))
                    {
						mainWindow.ConsoleWriteLine(saveFileName + " downloaded!");
					}
					else
                    {
						mainWindow.ConsoleWriteLine(saveFileName + " downloading failed!");
					}
					
				}
			}

			webClient.Dispose();

			mainWindow.ConsoleWriteLine("Done!");
			mainWindow.ConsoleWriteLine("Saved to: " + savePathSubFolder);
		}
	}
}
