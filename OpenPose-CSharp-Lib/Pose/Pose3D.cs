using System;
using System.Collections.Generic;

namespace OpenPose.Pose
{
	public class Pose3D : Pose
	{
		public Pose3D(KeyPoint[] keyPoints) : base(keyPoints)
		{
		}

		public KeyPoint3D GetKeyPoint3D(BodyPoint bodyPoint)
		{
			return (KeyPoint3D)base.GetKeyPoint(bodyPoint);
		}

		public static Pose3D ParseFloatArray(float[] points)
		{
			if (points.Length % 4 == 0)
			{
				List<KeyPoint3D> keyPoints = new List<KeyPoint3D>();

				for (int i = 0; i < points.Length; i += 4)
				{
					// pointNum = 0 if less than 4, otherwise pointNum = current index divided by 4
					keyPoints.Add(new KeyPoint3D((i < 4 ? 0 : i / 3), points[i], points[i + 1], points[i + 2], points[i + 3]));
				}

				return new Pose3D(keyPoints.ToArray());
			}
			else
			{
				throw new Exception("Pose3D#ParseFloatArray() error: Float array is not divisible by 4.");
			}
		}
	}
}
