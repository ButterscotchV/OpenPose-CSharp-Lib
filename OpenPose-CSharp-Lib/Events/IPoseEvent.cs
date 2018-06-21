namespace OpenPose
{
	interface IPoseEvent
	{
		void OnPoseGenerated(Pose2D pose);
	}
}
