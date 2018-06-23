namespace OpenPose
{
	public interface IPoseEvent
	{
		void OnPoseGenerated(Pose.Pose pose);
	}
}
