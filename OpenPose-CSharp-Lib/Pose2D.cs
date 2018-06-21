using System;
using System.Collections.Generic;

namespace OpenPose
{
	class Pose2D
	{
		List<KeyPoint2D> KeyPoints { get; } = new List<KeyPoint2D>();

		public Pose2D(KeyPoint2D[] keyPoints)
		{
			KeyPoints.AddRange(keyPoints);
		}

		public static Pose2D ParseFloatArray(float[] points)
		{
			if (points.Length % 3 == 0)
			{
				List<KeyPoint2D> keyPoints = new List<KeyPoint2D>();

				for (int i = 0; i < points.Length; i += 3)
				{
					// pointNum = 0 if less than 3, otherwise pointNum = current index divided by 3
					keyPoints.Add(new KeyPoint2D((i < 3 ? 0 : i / 3), points[i], points[i + 1], points[i + 2]));
				}
			}
			else
			{
				throw new Exception("Pose2D#ParseFloatArray() error: Float array is not divisible by 3.");
			}

			return null;
		}
	}
}
