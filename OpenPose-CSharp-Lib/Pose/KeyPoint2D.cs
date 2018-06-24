namespace OpenPose.Pose
{
	public class KeyPoint2D : KeyPoint
	{
		public double Raw_X { get; private set; }
		public double Raw_Y { get; private set; }

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

		// Maximums are used for inverting the values
		public static double Max_X = 0;
		public static double Max_Y = 720;

		// These should be in the unit meters per pixel (m/p)
		public static double X_Scale_MPP = 1;
		public static double Y_Scale_MPP = 1;

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

		public KeyPoint2D(int pointNum, double x, double y, double score)
		{
			Raw_X = x;
			Raw_Y = y;
			Score = score;

			BodyPoint = (BodyPoint)pointNum;
		}

		public KeyPoint2D(BodyPoint bodyPoint, double x, double y, double score)
		{
			Raw_X = x;
			Raw_Y = y;
			Score = score;

			BodyPoint = bodyPoint;
		}

		public KeyPoint2D(int pointNum, double x, double y, double score, double yaw, double pitch, double roll)
		{
			Raw_X = x;
			Raw_Y = y;
			Score = score;

			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;

			BodyPoint = (BodyPoint)pointNum;
		}

		public KeyPoint2D(BodyPoint bodyPoint, double x, double y, double score, double yaw, double pitch, double roll)
		{
			Raw_X = x;
			Raw_Y = y;
			Score = score;

			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;

			BodyPoint = bodyPoint;
		}

		override
		public string ToString()
		{
			return "[" + BodyPoint + "]{(" + X + "," + Y + "), " + Score + "}";
		}
	}
}
