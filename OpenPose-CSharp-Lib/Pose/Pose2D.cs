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

		public KeyPoint2D GetKeyPoint2D(int index)
		{
			return (KeyPoint2D)base.GetKeyPoint(index);
		}

		public static Pose2D ParseDoubleArray(double[] points)
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
				throw new Exception("Pose2D#ParseDoubleArray() error: Double array is not divisible by 3.");
			}
		}

		public Pose3D Simulate3D()
		{
			//KeyPoint2D centerPoint = GetKeyPoint2D(BodyPoint.Center_Body);

			List<KeyPoint3D> temp = new List<KeyPoint3D>();

			foreach (KeyPoint2D keypoint in KeyPoints)
			{
				if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Left_Wrist)
				{
					KeyPoint2D leftElbow = GetKeyPoint2D(BodyPoint.Left_Elbow);
					KeyPoint2D leftShoulder = GetKeyPoint2D(BodyPoint.Left_Shoulder);

					//double distScale = 1;
					double armDisp = 0;

					if (leftElbow != null && leftElbow.IsValid)
					{
						if (leftShoulder != null && leftShoulder.IsValid)
						{
							armDisp = CalculateZ(leftShoulder, leftElbow, Simulated3DSettings.UpperLeftArmLength);
						}

						double armZ = CalculateZ(keypoint, leftElbow, Simulated3DSettings.LowerLeftArmLength) + armDisp;

						temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, armZ, keypoint.Score));
					}
				}
				else if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Right_Wrist)
				{
					KeyPoint2D rightElbow = GetKeyPoint2D(BodyPoint.Right_Elbow);
					KeyPoint2D rightShoulder = GetKeyPoint2D(BodyPoint.Right_Shoulder);

					//double distScale = 1;
					double armDisp = 0;

					if (rightElbow != null && rightElbow.IsValid)
					{
						if (rightShoulder != null && rightShoulder.IsValid)
						{
							armDisp = CalculateZ(rightShoulder, rightElbow, Simulated3DSettings.UpperRightArmLength);
							//distScale += armDisp;
						}

						double armZ = CalculateZ(keypoint, rightElbow, Simulated3DSettings.LowerRightArmLength) + armDisp;

						temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, armZ, keypoint.Score));
					}
				}
				else if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Nose_or_Top_Head)
				{
					KeyPoint2D neck = GetKeyPoint2D(BodyPoint.Bottom_Neck);

					if (neck != null && neck.IsValid)
					{
						double headZ = CalculateZ(keypoint, neck, (OpenPose_Reader.Model == Model.COCO ? Simulated3DSettings.NeckLength_COCO : Simulated3DSettings.NeckLength_MPI));

						temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, headZ, keypoint.Score));
					}
				}
				else
				{
					temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, 0, keypoint.Score));
				}
			}

			return new Pose3D(temp.ToArray());
		}

		private double CalculateZ(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, double length)
		{
			//  _____
			// /     \
			// |   ->| = 0 degrees
			// \_____/

			// Known:
			//  - X diff
			//  - Y diff
			//  - Base length [sqrt((|X diff| ^ 2) + (|Y diff| ^ 2))]
			//  - Hypotenuse length (double length)
			// Need to get:
			//  - Z diff

			// Calculation:
			//  Z diff = sqrt(Hypotenuse length ^ 2 - [(|X diff| ^ 2) + (|Y diff| ^ 2)])

			double x_diff = Math.Abs(keyPointOne.X - keyPointTwo.X);
			double y_diff = Math.Abs(keyPointOne.Y - keyPointTwo.Y);

			double z_diff = Math.Sqrt(Math.Pow(length, 2) - (Math.Pow(x_diff, 2) + Math.Pow(y_diff, 2)));

			return z_diff;
		}

		private double CalculateZAngle(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, double length)
		{
			//              ^|
			//             / |
			// 75 degrees  ->|

			// Known:
			//  - X diff
			//  - Y diff
			//  - Base length [sqrt((|X diff| ^ 2) + (|Y diff| ^ 2))]
			//  - Hypotenuse length (double length)
			// Need to get:
			//  - Z angle

			// Calculation:
			//  Z angle = Cos^-1(Sqrt((|X diff| ^ 2) + (|Y diff| ^ 2)) / Hypotenuse length)

			double x_diff = Math.Abs(keyPointOne.X - keyPointTwo.X);
			double y_diff = Math.Abs(keyPointOne.Y - keyPointTwo.Y);

			double z_angle = MathHelpers.RadToDeg(Math.Acos(Math.Sqrt(Math.Pow(x_diff, 2) + Math.Pow(y_diff, 2)) / length));

			return z_angle;
		}

		private double CalculateBaseAngle(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo)
		{
			//  _____
			// /     \
			// |   ->| = 0 degrees
			// \_____/

			// Desc: Angle from front facing view

			// Known:
			//  - X diff
			//  - Y diff
			//  - Base length [sqrt((|X diff| ^ 2) + (|Y diff| ^ 2))]
			//  - Hypotenuse length (double length)
			// Need to get:
			//  - Z angle

			// Calculation:
			//  Z angle = Quadrant angle + Tan^-1(|Y diff| / |X diff|)

			double x_diff = keyPointOne.X - keyPointTwo.X;
			double y_diff = keyPointOne.Y - keyPointTwo.Y;

			double base_angle = MathHelpers.RadToDeg(Math.Atan(Math.Abs(y_diff) / Math.Abs(x_diff)));

			// Handle quadrants other than quadrant 1
			if (IsPositive(y_diff) && !IsPositive(x_diff))
			{
				base_angle += 90;
			}
			else if (!IsPositive(y_diff) && !IsPositive(x_diff))
			{
				base_angle += 180;
			}
			else if (!IsPositive(y_diff) && IsPositive(x_diff))
			{
				base_angle += 270;
			}

			return base_angle;
		}

		public bool IsPositive(double num)
		{
			return num >= 0;
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
