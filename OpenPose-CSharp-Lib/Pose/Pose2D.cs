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
				// Left hand Z axis and rotation estimation
				if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Left_Wrist)
				{
					KeyPoint2D leftElbow = GetKeyPoint2D(BodyPoint.Left_Elbow);
					KeyPoint2D leftShoulder = GetKeyPoint2D(BodyPoint.Left_Shoulder);

					//double distScale = 1;
					double upperArmZ = 0;
					double lowerArmZ = 0;

					double leftHandYaw = 0;
					double leftHandPitch = 0;
					double leftHandRoll = 0;

					if (leftElbow != null && leftElbow.IsValid)
					{
						if (leftShoulder != null && leftShoulder.IsValid)
						{
							upperArmZ += CalculateZ(leftShoulder, leftElbow, Simulated3DSettings.UpperLeftArmLength);
						}

						lowerArmZ += CalculateZ(keypoint, leftElbow, Simulated3DSettings.LowerLeftArmLength);

						// - 90 degrees to aim forward for both of these
						leftHandYaw += CalculateXAngle(keypoint, leftElbow, Simulated3DSettings.LowerLeftArmLength) - 90;
						leftHandPitch += CalculateYAngle(leftElbow, keypoint, Simulated3DSettings.LowerLeftArmLength) - 90;
					}

					temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, upperArmZ + lowerArmZ, keypoint.Score, leftHandYaw, leftHandPitch, leftHandRoll));
				}

				// Right hand Z axis and rotation estimation
				else if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Right_Wrist)
				{
					KeyPoint2D rightElbow = GetKeyPoint2D(BodyPoint.Right_Elbow);
					KeyPoint2D rightShoulder = GetKeyPoint2D(BodyPoint.Right_Shoulder);

					//double distScale = 1;
					double upperArmZ = 0;
					double lowerArmZ = 0;

					double rightHandYaw = 0;
					double rightHandPitch = 0;
					double rightHandRoll = 0;

					if (rightElbow != null && rightElbow.IsValid)
					{
						if (rightShoulder != null && rightShoulder.IsValid)
						{
							upperArmZ += CalculateZ(rightShoulder, rightElbow, Simulated3DSettings.UpperRightArmLength);
							//distScale += armDisp;
						}

						lowerArmZ += CalculateZ(keypoint, rightElbow, Simulated3DSettings.LowerRightArmLength);

						// - 90 degrees to aim forward for both of these
						rightHandYaw += CalculateXAngle(keypoint, rightElbow, Simulated3DSettings.LowerLeftArmLength) - 90;
						rightHandPitch += CalculateYAngle(rightElbow, keypoint, Simulated3DSettings.LowerLeftArmLength) - 90;
					}

					temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, upperArmZ + lowerArmZ, keypoint.Score, rightHandYaw, rightHandPitch, rightHandRoll));
				}

				// Head Z axis and rotation estimation
				else if (keypoint.IsValid && keypoint.BodyPoint == BodyPoint.Nose_or_Top_Head)
				{
					double headZ = 0;
					double headYaw = 0;
					double headPitch = 0;
					double headRoll = 0;

					KeyPoint2D neck = GetKeyPoint2D(BodyPoint.Bottom_Neck);

					if (neck != null && neck.IsValid)
					{
						headZ = CalculateZ(keypoint, neck, (OpenPose_Reader.Model == Model.COCO ? Simulated3DSettings.NeckLength_COCO : Simulated3DSettings.NeckLength_MPI));
					}

					if (OpenPose_Reader.Model == Model.COCO)
					{
						KeyPoint2D leftEye = GetKeyPoint2D(BodyPoint.Left_Eye);
						KeyPoint2D rightEye = GetKeyPoint2D(BodyPoint.Right_Eye_or_Center_Body);

						if (leftEye != null && leftEye.IsValid && rightEye != null && rightEye.IsValid)
						{
							headYaw = CalculateZAngle(leftEye, rightEye, (Simulated3DSettings.EyeDist));

							KeyPoint2D leftEar = GetKeyPoint2D(BodyPoint.Left_Ear);
							KeyPoint2D rightEar = GetKeyPoint2D(BodyPoint.Right_Ear);

							headYaw = (leftEar != null && leftEar.IsValid) || (rightEar == null || !rightEar.IsValid) ? headYaw : -headYaw;

							KeyPoint2D averageEyes = new KeyPoint2D(BodyPoint.Left_Eye, (leftEye.X + rightEye.X) / 2, (leftEye.Y + rightEye.Y) / 2, (leftEye.Score + rightEye.Score));
							headPitch = CalculateZAngle(averageEyes, keypoint, Simulated3DSettings.NoseToBetweenEyes);
						}
					}

					temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, headZ, keypoint.Score, headYaw, headPitch, headRoll));
				}
				else
				{
					temp.Add(new KeyPoint3D(keypoint.BodyPoint, keypoint.Raw_X, keypoint.Raw_Y, 0, keypoint.Score));
				}
			}

			return new Pose3D(temp.ToArray());
		}

		private double CalculateZ(double x_one, double x_two, double y_one, double y_two, double length)
		{
			//   /|
			//  / | = 1m
			// /__|

			// Known:
			//  - X diff
			//  - Y diff
			//  - Base length [sqrt((|X diff| ^ 2) + (|Y diff| ^ 2))]
			//  - Hypotenuse length (double length)
			// Need to get:
			//  - Z diff

			// Calculation:
			//  Z diff = sqrt(Hypotenuse length ^ 2 - [(|X diff| ^ 2) + (|Y diff| ^ 2)])

			double x_diff = Math.Abs(x_one - x_two);
			double y_diff = Math.Abs(y_one - y_two);

			double z_diff = Math.Sqrt(Math.Pow(length, 2) - (Math.Pow(x_diff, 2) + Math.Pow(y_diff, 2)));

			return Double.IsNaN(z_diff) ? 0 : z_diff;
		}

		private double CalculateZ(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, double length)
		{
			return CalculateZ(keyPointOne.X, keyPointTwo.X, keyPointOne.Y, keyPointTwo.Y, length);
		}

		private double CalculateZAngle(double x_one, double x_two, double y_one, double y_two, double length)
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

			double x_diff = Math.Abs(x_one - x_two);
			double y_diff = Math.Abs(y_one - y_two);

			double z_angle = MathHelpers.RadToDeg(Math.Acos(Math.Sqrt(Math.Pow(x_diff, 2) + Math.Pow(y_diff, 2)) / length));

			return Double.IsNaN(z_angle) ? 0 : z_angle;
		}

		private double CalculateZAngle(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, double length)
		{
			return CalculateZAngle(keyPointOne.X, keyPointTwo.X, keyPointOne.Y, keyPointTwo.Y, length);
		}

		private double CalculateBaseAngle(double x_one, double x_two, double y_one, double y_two)
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
			// Base angle = Quadrant angle + Tan^-1(|Y diff| / |X diff|)

			double x_diff = x_one - x_two;
			double y_diff = y_one - y_two;

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

			// Normalize angle to smallest relative position
			while (base_angle > 360)
			{
				base_angle -= 360;
			}

			return Double.IsNaN(base_angle) ? 0 : base_angle;
		}

		private double CalculateBaseAngle(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo)
		{
			return CalculateBaseAngle(keyPointOne.X, keyPointTwo.X, keyPointOne.Y, keyPointTwo.Y);
		}

		private double CalculateXAngle(double x_one, double x_two, double length)
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

			double x_diff = x_one - x_two;

			double x_angle = MathHelpers.RadToDeg(Math.Acos(x_diff / length));

			return Double.IsNaN(x_angle) ? 0 : x_angle;
		}

		private double CalculateXAngle(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, double length)
		{
			return CalculateXAngle(keyPointOne.X, keyPointTwo.X, length);
		}

		private double CalculateYAngle(double y_one, double y_two, double length)
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

			double y_diff = y_one - y_two;

			double y_angle = MathHelpers.RadToDeg(Math.Acos(y_diff / length));

			return Double.IsNaN(y_angle) ? 0 : y_angle;
		}

		private double CalculateYAngle(KeyPoint2D keyPointOne, KeyPoint2D keyPointTwo, double length)
		{
			return CalculateYAngle(keyPointOne.Y, keyPointTwo.Y, length);
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
