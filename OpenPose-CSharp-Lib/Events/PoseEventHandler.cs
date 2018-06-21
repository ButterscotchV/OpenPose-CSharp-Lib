using System.Collections.Generic;

namespace OpenPose
{
	class PoseEventHandler
	{
		public List<IPoseEvent> RegisteredPoseEvents { get; } = new List<IPoseEvent>();

		public void RegisterPoseListener(IPoseEvent poseListener)
		{
			RegisteredPoseEvents.Add(poseListener);
		}

		public void ExecuteHandlers()
		{
			foreach (IPoseEvent poseEvent in RegisteredPoseEvents)
			{
				poseEvent.OnPoseGenerated(new Pose2D());
			}
		}
	}
}
