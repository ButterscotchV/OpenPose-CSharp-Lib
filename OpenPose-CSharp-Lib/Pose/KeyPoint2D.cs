namespace OpenPose.Pose
{
	public class KeyPoint2D : KeyPoint
	{
		public float Raw_X { get; private set; }
		public float Raw_Y { get; private set; }

		public float X
		{
			get
			{
				return (Max_X - Raw_X) * X_Scale_MPP;
			}
		}

		public float Y
		{
			get
			{
				return (Max_Y - Raw_Y) * Y_Scale_MPP;
			}
		}

		// Maximums are used for inverting the values
		public static float Max_X = 0;
		public static float Max_Y = 720;

		// These should be in the unit meters per pixel (m/p)
		public static float X_Scale_MPP = 1;
		public static float Y_Scale_MPP = 1;

		// Scales but in pixels per meter (p/m)
		public static float X_Scale_PPM
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

		public static float Y_Scale_PPM
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

		public KeyPoint2D(int pointNum, float x, float y, float score)
		{
			Raw_X = x;
			Raw_Y = y;
			Score = score;

			BodyPoint = (BodyPoint)pointNum;
		}

		override
		public string ToString()
		{
			return "[" + BodyPoint + "]{(" + X + "," + Y + "), " + Score + "}";
		}
	}
}
