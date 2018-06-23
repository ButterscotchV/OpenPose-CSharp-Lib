﻿namespace OpenPose.Pose
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
}