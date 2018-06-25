using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPose.Pose
{
	public static class Simulated3DSettings
	{
		// These default values are measurements for me

		public static double LengthScale = 1;

		public static double NeckLength_MPI = 0.233 * LengthScale;
		public static double NeckLength_COCO = 0.195 * LengthScale;

		public static double EyeDist = 0.06 * LengthScale;
		public static double NoseToBetweenEyes = 0.055 * LengthScale;

		public static double UpperBodyLength = 0.36 * LengthScale;
		public static double LowerBodyLength = 0.285 * LengthScale;

		public static double BodyLength
		{
			get
			{
				return UpperBodyLength + LowerBodyLength;
			}

			set
			{
				UpperBodyLength = (value / 2) * LengthScale;
				LowerBodyLength = (value / 2) * LengthScale;
			}
		}

		public static double UpperLeftArmLength = 0.305 * LengthScale;
		public static double UpperRightArmLength = 0.305 * LengthScale;

		public static double UpperArmLength
		{
			get
			{
				return (UpperLeftArmLength + UpperRightArmLength) / 2;
			}

			set
			{
				UpperLeftArmLength = value * LengthScale;
				UpperRightArmLength = value * LengthScale;
			}
		}

		public static double LowerLeftArmLength = 0.285 * LengthScale;
		public static double LowerRightArmLength = 0.285 * LengthScale;

		public static double LowerArmLength
		{
			get
			{
				return (LowerLeftArmLength + LowerRightArmLength) / 2;
			}

			set
			{
				LowerLeftArmLength = value * LengthScale;
				LowerRightArmLength = value * LengthScale;
			}
		}

		public static double UpperLeftLegLength = 0.395 * LengthScale;
		public static double UpperRightLegLength = 0.395 * LengthScale;

		public static double UppLegLength
		{
			get
			{
				return (UpperLeftLegLength + UpperRightLegLength) / 2;
			}

			set
			{
				UpperLeftLegLength = value * LengthScale;
				UpperRightLegLength = value * LengthScale;
			}
		}

		public static double LowerLeftLegLength = 0.43 * LengthScale;
		public static double LowerRightLegLength = 0.43 * LengthScale;

		public static double LowerLegLength
		{
			get
			{
				return (LowerLeftLegLength + LowerRightLegLength) / 2;
			}

			set
			{
				LowerLeftLegLength = value * LengthScale;
				LowerRightLegLength = value * LengthScale;
			}
		}
	}
}
