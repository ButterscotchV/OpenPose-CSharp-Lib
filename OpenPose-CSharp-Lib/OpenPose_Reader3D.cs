using Newtonsoft.Json.Linq;
using OpenPose.Pose;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OpenPose
{
	public class OpenPose_Reader3D : OpenPose_Reader
	{
		public OpenPose_Reader3D(string jsonFolderPath, IPoseEvent poseEvent) : base(jsonFolderPath, poseEvent)
		{
		}

		override
		public void QueueReader()
		{
			while (!StopQueue)
			{
				// If the queue is empty, run the delay for checking queue
				if (PoseQueue.Count <= 0)
				{
					Thread.Sleep(QueueCheckDelay);
					try
					{
						ProcessFiles();
					}
					catch (IOException)
					{
						Console.WriteLine("IOException. File is probably in-use by another program.");
					}
					catch (UnauthorizedAccessException)
					{
						Console.WriteLine("UnauthorizedAccessException. File is probably in-use by another program.");
					}

					continue;
				}
				else if (PoseQueue.Count > 20)
				{
					PoseQueue.RemoveRange(0, PoseQueue.Count - 1);
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

					//foreach (double f in parsedPose["people"][0].Value<JArray>("pose_keypoints_2d").Values<double>())
					//{
					//	Console.WriteLine(f);
					//}

					double[] keypoints = new List<double>(parsedPose["people"][0].Value<JArray>("pose_keypoints_3d").Values<double>()).ToArray();

					poseEventHandler.ExecuteHandlers(Pose3D.ParseDoubleArray(keypoints));
				}
			}
		}
	}
}
