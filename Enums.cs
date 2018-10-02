using System.ComponentModel;
namespace LiveSplit.NightInTheWoods {
	public enum LogObject {
		CurrentSplit,
		SceneName,
		Loading,
		Info,
		ScreenState
	}
	public enum SplitName {
		[Description("Manual Split"), ToolTip("Does not split automatically. Use this for custom splits not yet defined.")]
		ManualSplit,
		[Description("Act 1 - Day 1"), ToolTip("Splits when Act 1 Day 1 is complete")]
		A1D1,
		[Description("Part 1 - Home Again"), ToolTip("Splits when Part 1 shows")]
		Part1,
		[Description("Act 1 - Day 2"), ToolTip("Splits when Act 1 Day 2 is complete")]
		A1D2,
		[Description("Act 1 - Day 3"), ToolTip("Splits when Act 1 Day 3 is complete")]
		A1D3,
		[Description("The Party"), ToolTip("Splits when The Party shows")]
		Party,
		[Description("Act 1 - Day 4"), ToolTip("Splits when Act 1 Day 4 is complete")]
		A1D4,
		[Description("Part 2 - Weird Autumn"), ToolTip("Splits when Part 2 shows")]
		Part2,
		[Description("Old Gods Fort Lucenne Mall"), ToolTip("Splits when Old Gods of the Fort Lucenne Mall shows")]
		OldGods,
		[Description("Act 2 - Day 1"), ToolTip("Splits when Act 2 Day 1 is complete")]
		A2D1,
		[Description("Act 2 - Day 2"), ToolTip("Splits when Act 2 Day 2 is complete")]
		A2D2,
		[Description("Act 2 - Day 3"), ToolTip("Splits when Act 2 Day 3 is complete")]
		A2D3,
		[Description("Act 2 - Day 4"), ToolTip("Splits when Act 2 Day 4 is complete")]
		A2D4,
		[Description("Act 2 - Day 5"), ToolTip("Splits when Act 2 Day 5 is complete")]
		A2D5,
		[Description("Act 3 - Day 1"), ToolTip("Splits when Act 3 Day 1 is complete")]
		A3D1,
		[Description("Act 3 - Day 2"), ToolTip("Splits when Act 3 Day 2 is complete")]
		A3D2,
		[Description("Act 3 - Day 3"), ToolTip("Splits when Act 3 Day 3 is complete")]
		A3D3,
		[Description("Act 3 - Day 4"), ToolTip("Splits when Act 3 Day 4 is complete")]
		A3D4,
		[Description("Act 3 - Day 5"), ToolTip("Splits when Act 3 Day 5 is complete")]
		A3D5,
		[Description("Act 4 - Day 1"), ToolTip("Splits when Act 4 Day 1 is complete")]
		A4D1,
		[Description("Act 4 - Day 2"), ToolTip("Splits when Act 4 Day 2 is complete")]
		A4D2
	}
	public enum TransitionState {
		None,
		In,
		Out
	}
}