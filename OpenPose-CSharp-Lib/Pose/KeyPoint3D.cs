using System;
using System.Collections.Generic;
using System.Linq;
namespace OpenPose.Pose
{
	public class KeyPoint3D : KeyPoint
	{
		public double Raw_X { get; private set; }
		public double Raw_Y { get; private set; }
		public double Raw_Z { get; private set; }

		public double X
		{
			get
			{
				return (Max_X - Raw_X) * X_Scale_MPP;
			}
		}

		public double Y
		{
			get
			{
				return (Max_Y - Raw_Y) * Y_Scale_MPP;
			}
		}

		public double Z
		{
			get
			{
				return (Max_Z - Raw_Z) * Z_Scale_MPP;
			}
		}

		// Maximums are used for inverting the values
		public static double Max_X = 0;
		public static double Max_Y = 720;
		public static double Max_Z = 0;

		// These should be in the unit meters per pixel (m/p)
		public static double X_Scale_MPP = 1;
		public static double Y_Scale_MPP = 1;
		public static double Z_Scale_MPP = 1;

		// Scales but in pixels per meter (p/m)
		public static double X_Scale_PPM
		{
			get
			{
				return 1 / X_Scale_MPP;
			}

			set
			{
				X_Scale_MPP = 1 / value;
			}
		}

		public static double Y_Scale_PPM
		{
			get
			{
				return 1 / Y_Scale_MPP;
			}

			set
			{
				Y_Scale_MPP = 1 / value;
			}
		}

		public static double Z_Scale_PPM
		{
			get
			{
				return 1 / Z_Scale_MPP;
			}

			set
			{
				Z_Scale_MPP = 1 / value;
			}
		}

		public KeyPoint3D(int pointNum, double x, double y, double z, double score)
		{
			Raw_X = x;
			Raw_Y = y;
			Raw_Z = z;
			Score = score;

			BodyPoint = (BodyPoint)pointNum;
		}

		public KeyPoint3D(BodyPoint bodyPoint, double x, double y, double z, double score)
		{
			Raw_X = x;
			Raw_Y = y;
			Raw_Z = z;
			Score = score;

			BodyPoint = bodyPoint;
		}

		public KeyPoint3D(int pointNum, double x, double y, double z, double score, double yaw, double pitch, double roll)
		{
			Raw_X = x;
			Raw_Y = y;
			Raw_Z = z;
			Score = score;

			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;

			BodyPoint = (BodyPoint)pointNum;
		}

		public KeyPoint3D(BodyPoint bodyPoint, double x, double y, double z, double score, double yaw, double pitch, double roll)
		{
			Raw_X = x;
			Raw_Y = y;
			Raw_Z = z;
			Score = score;

			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;

			BodyPoint = bodyPoint;
		}

		override
		public string ToString()
		{
			return "[" + BodyPoint + "]{(" + X + "," + Y + "," + Z + "), " + Score + "}";
		}
	}
}
