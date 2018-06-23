using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OpenPose
{
	public enum Model
	{
		COCO = 0,
		MPI = 1
	}

    public abstract class OpenPose_Reader
    {
		public string JSONFolderPath { get; private set; }
		public int QueueCheckDelay = 10; // In milliseconds
		public bool StopQueue = false;

		public static Model Model = Model.COCO;

		public readonly List<string> PoseQueue = new List<string>(); // The queue of files to process
		public readonly List<string> RemainingQeueudFiles = new List<string>(); // Files that have been queued

		public readonly PoseEventHandler poseEventHandler = new PoseEventHandler();

		public OpenPose_Reader(string jsonFolderPath, IPoseEvent poseEvent)
		{
			poseEventHandler.RegisterPoseListener(poseEvent);
			JSONFolderPath = jsonFolderPath;
		}

		public abstract void QueueReader();

		public Thread AsynchronousQueueReader()
		{
			Thread thread = new Thread(new ThreadStart(QueueReader));
			thread.Start();

			return thread;
		}

		public void ProcessFiles()
		{
			string[] filePaths = Directory.GetFiles(JSONFolderPath);

			if (filePaths.Length > 0)
			{
				foreach (string file in filePaths)
				{
					if (!RemainingQeueudFiles.Contains(file))
					{
						try
						{
							PoseQueue.Add(File.ReadAllText(file));
							RemainingQeueudFiles.Add(file);
							File.Delete(file);
						}
						catch (IOException)
						{
							Console.WriteLine("IOException on \"" + file + "\". File is probably in-use by another program.");
						}
						catch (UnauthorizedAccessException)
						{
							Console.WriteLine("UnauthorizedAccessException on \"" + file + "\". File is probably in-use by another program.");
						}
					}
					else
					{
						try
						{
							File.Delete(file);
							RemainingQeueudFiles.Remove(file);
						}
						catch (IOException)
						{
							Console.WriteLine("IOException on \"" + file + "\". File is probably in-use by another program.");
						}
						catch (UnauthorizedAccessException)
						{
							Console.WriteLine("UnauthorizedAccessException on \"" + file + "\". File is probably in-use by another program.");
						}
					}
				}
			}
		}
	}
}
