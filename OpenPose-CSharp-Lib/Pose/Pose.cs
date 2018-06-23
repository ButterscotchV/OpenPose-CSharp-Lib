using System.Collections.Generic;

namespace OpenPose.Pose
{
	public abstract class Pose
	{
		public List<KeyPoint> KeyPoints { get; } = new List<KeyPoint>();

		public Pose(KeyPoint[] keyPoints)
		{
			KeyPoints.AddRange(keyPoints);
		}

		public KeyPoint GetKeyPoint(BodyPoint bodyPoint)
		{
			if ((int)bodyPoint >= 0 && (int)bodyPoint < KeyPoints.Count)
			{
				return KeyPoints[(int)bodyPoint];
			}

			return null;
		}
	}
}
