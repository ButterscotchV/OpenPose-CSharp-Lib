using System;
using System.Collections.Generic;

namespace OpenPose.Pose
{
	public class Pose2D : Pose
	{
		public Pose2D(KeyPoint[] keyPoints) : base(keyPoints)
		{
		}

		public KeyPoint2D GetKeyPoint2D(BodyPoint bodyPoint)
		{
			return (KeyPoint2D)base.GetKeyPoint(bodyPoint);
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

				return new Pose2D(keyPoints.ToArray());
			}
			else
			{
				throw new Exception("Pose2D#ParseFloatArray() error: Float array is not divisible by 3.");
			}
		}

		public Pose3D Simulate3D()
		{
			if (OpenPose_Reader.Model == Model.COCO)
			{

			}
			else if (OpenPose_Reader.Model == Model.MPI)
			{
				//KeyPoint2D centerPoint = GetKeyPoint2D(BodyPoint.Center_Body);

				List<KeyPoint3D> temp = new List<KeyPoint3D>();

				foreach (KeyPoint2D keypoint in KeyPoints)
				{
					if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Left_Wrist)
					{
						temp.Add(new KeyPoint3D((int)keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, CalculateZ(keypoint, GetKeyPoint2D(BodyPoint.Left_Elbow), 0.285f), keypoint.Score));
					}
					else
					{
						temp.Add(new KeyPoint3D((int)keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, 0, keypoint.Score));
					}
				}

				return new Pose3D(temp.ToArray());
			}

			return null;
		}

		private float CalculateZ(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, float length)
		{
			//  _____
			// /     \
			// |   ->| = 0 degrees
			// \_____/

			// Known:
			//  - X diff
			//  - Y diff
			//  - Base length [sqrt((|X diff| ^ 2) + (|Y diff| ^ 2))]
			//  - Hypotenuse length (float length)
			// Need to get:
			//  - Z diff

			// Calculation:
			//  Z diff = sqrt(Hypotenuse length ^ 2 - [(|X diff| ^ 2) + (|Y diff| ^ 2)])

			float x_diff = Math.Abs(keyPointOne.X - keyPointTwo.X);
			float y_diff = Math.Abs(keyPointOne.Y - keyPointTwo.Y);

			float z_diff = (float) Math.Sqrt(Math.Pow(length, 2) - (Math.Pow(x_diff, 2) + Math.Pow(y_diff, 2)));

			return z_diff;
		}
	}

	public static class MathHelpers
	{
		public static double RadToDeg(double rad)
		{
			return rad * (180.0 / Math.PI);
		}

		public static double DegToRad(double deg)
		{
			return Math.PI * deg / 180.0;
		}
	}
}
