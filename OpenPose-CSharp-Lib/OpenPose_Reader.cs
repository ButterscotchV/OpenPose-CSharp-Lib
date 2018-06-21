using System.Collections.Generic;
using System.IO;

namespace OpenPose
{
    public class OpenPose_Reader
    {
		public string JSONFolderPath { get; private set; }
		public int QueueCheckDelay = 5;

		public List<string> PoseQueue { get; } = new List<string>();
		public List<string> RemainingQeueudFiles { get; } = new List<string>(); // Files that have been queued

		public OpenPose_Reader(string jsonFolderPath)
		{
			JSONFolderPath = jsonFolderPath;
			JSONDirWatcher();
		}

		private void JSONDirWatcher()
		{
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = JSONFolderPath;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = "*.*";
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			string[] filePaths = Directory.GetFiles(JSONFolderPath);

			if (filePaths.Length > 0)
			{
				
			}
		}
	}
}
