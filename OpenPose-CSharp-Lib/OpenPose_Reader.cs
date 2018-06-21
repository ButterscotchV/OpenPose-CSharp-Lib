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
		}

		public void SynchronousQueueReader()
		{
			while (!StopQueue)
			{
				// If the queue is empty, run the delay for checking queue
				if (PoseQueue.Count <= 0)
				{
					Thread.Sleep(QueueCheckDelay);
					ProcessFiles();

					continue;
				}

				// Parse saved text
				JObject parsedPose = JObject.Parse(PoseQueue[0]);
				PoseQueue.RemoveAt(0);

				// Do JSON stuff to grab keypoints

				//Console.WriteLine(parsedPose.ToString());

				if (parsedPose["people"].HasValues && parsedPose["people"][0].HasValues)
				{
					//Console.WriteLine(parsedPose["people"].ToString());

					//Console.WriteLine(parsedPose["people"][0].Value<JArray>("pose_keypoints_2d"));

					//foreach (float f in parsedPose["people"][0].Value<JArray>("pose_keypoints_2d").Values<float>())
					//{
					//	Console.WriteLine(f);
					//}

					float[] keypoints = new List<float> (parsedPose["people"][0].Value<JArray>("pose_keypoints_2d").Values<float>()).ToArray();

					poseEventHandler.ExecuteHandlers(Pose2D.ParseFloatArray(keypoints));
				}
			}
		}

		public Thread AsynchronousQueueReader()
		{
			Thread thread = new Thread(new ThreadStart(SynchronousQueueReader));
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
					}
				}
			}
		}
	}
}
