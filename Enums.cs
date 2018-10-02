using System.ComponentModel;
namespace LiveSplit.NightInTheWoods {
	public enum LogObject {
		CurrentSplit,
		SceneName,
		Loading
	}
	public enum SplitName {
		[Description("Manual Split (Not Automatic)"), ToolTip("Does not split automatically. Use this for custom splits not yet defined.")]
		ManualSplit,

		[Description("Intro (Completed)"), ToolTip("Splits when completing the intro")]
		Level_0_1
	}
}