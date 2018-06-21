namespace OpenPose
{
	public interface IPoseEvent
	{
		void OnPoseGenerated(Pose2D pose);
	}
}
