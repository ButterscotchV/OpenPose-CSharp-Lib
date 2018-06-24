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
			if (bodyPoint >= 0 && (int)bodyPoint < KeyPoints.Count)
			{
				foreach (KeyPoint keyPoint in KeyPoints)
				{
					if (keyPoint.BodyPoint == bodyPoint)
					{
						return keyPoint;
					}
				}
			}

			return null;
		}

		public KeyPoint GetKeyPoint(int index)
		{
			if (index >= 0 && index < KeyPoints.Count)
			{
				return KeyPoints[index];
			}

			return null;
		}
	}
}
