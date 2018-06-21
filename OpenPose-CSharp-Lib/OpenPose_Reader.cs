using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OpenPose
{
    public class OpenPose_Reader
    {
		public string JSONFolderPath { get; private set; }
		public int QueueCheckDelay = 10; // In milliseconds
		public bool StopQueue = false;

		public readonly List<string> PoseQueue = new List<string>(); // The queue of files to process
		public readonly List<string> RemainingQeueudFiles = new List<string>(); // Files that have been queued

		public readonly PoseEventHandler poseEventHandler = new PoseEventHandler();

		public OpenPose_Reader(string jsonFolderPath, IPoseEvent poseEvent)
		{
			poseEventHandler.RegisterPoseListener(poseEvent);

			JSONFolderPath = jsonFolderPath;
			JSONDirWatcher();
		}

		public void SynchronousQueueReader()
		{
			while (!StopQueue)
			{
				// If the queue is empty, run the delay for checking queue
				if (PoseQueue.Count <= 0)
				{
					Thread.Sleep(QueueCheckDelay);
					continue;
				}

				JObject parsedPose = JObject.Parse(PoseQueue[0]);

				Console.WriteLine(parsedPose.ToString());

				poseEventHandler.ExecuteHandlers(new Pose2D(new KeyPoint2D[] { new KeyPoint2D(0, 1f, 2f, 3f) }));
			}
		}

		private void JSONDirWatcher()
		{
			FileSystemWatcher watcher = new FileSystemWatcher
			{
				Path = JSONFolderPath,
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = "*.*"
			};
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			string[] filePaths = Directory.GetFiles(JSONFolderPath);

			if (filePaths.Length > 0)
			{
				foreach (string file in filePaths)
				{
					if (!RemainingQeueudFiles.Contains(file))
					{
						RemainingQeueudFiles.Add(file);
						PoseQueue.Add(File.ReadAllText(file));
						File.Delete(file);
					}
				}
			}
		}
	}
}
