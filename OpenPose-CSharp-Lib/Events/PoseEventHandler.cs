using System.Collections.Generic;

namespace OpenPose
{
	public class PoseEventHandler
	{
		public List<IPoseEvent> RegisteredPoseEvents { get; } = new List<IPoseEvent>();

		public void RegisterPoseListener(IPoseEvent poseListener)
		{
			RegisteredPoseEvents.Add(poseListener);
		}

		public void ExecuteHandlers(Pose2D pose)
		{
			foreach (IPoseEvent poseEvent in RegisteredPoseEvents)
			{
				poseEvent.OnPoseGenerated(pose);
			}
		}
	}
}
