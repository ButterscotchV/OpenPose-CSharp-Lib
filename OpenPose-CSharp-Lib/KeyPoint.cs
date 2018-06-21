using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPose
{
	public enum BodyPoint
	{
		/*
		 * Head
		 */
		Nose = 0,
		Bottom_Neck = 1,
		
		// Right Head
		Right_Eye = 14,
		Right_Ear = 16,

		// Left Head
		Left_Eye = 15,
		Left_Ear = 17,

		/*
		 * Left
		 */
		// Upper Left
		Left_Shoulder = 5,
		Left_Elbow = 6,
		Left_Wrist = 7,

		// Lower Left
		Left_Hip = 11,
		Left_Knee = 12,
		Left_Heel = 13,

		/*
		 * Right
		 */
		// Upper Right
		Right_Shoulder = 2,
		Right_Elbow = 3,
		Right_Wrist = 4,

		// Lower Right
		Right_Hip = 8,
		Right_Knee = 9,
		Right_Heel = 10,
	}

	public class KeyPoint2D
	{
		public float X { get; private set; }
		public float Y { get; private set; }
		public float Score { get; private set; }

		public BodyPoint BodyPoint { get; private set; }

		public KeyPoint2D(int pointNum, float x, float y, float score)
		{
			X = x;
			Y = y;
			Score = score;

			BodyPoint = (BodyPoint)pointNum;
		}
	}

	public class KeyPoint3D
	{
		public float X { get; private set; }
		public float Y { get; private set; }
		public float Z { get; private set; }
		public float Score { get; private set; }

		public BodyPoint BodyPoint { get; private set; }

		public KeyPoint3D(int pointNum, float x, float y, float z, float score)
		{
			X = x;
			Y = y;
			Z = z;
			Score = score;

			BodyPoint = (BodyPoint)pointNum;
		}
	}
}
