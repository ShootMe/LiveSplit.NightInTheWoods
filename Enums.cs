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
		[Description("Part 1 - Home Again"), ToolTip("Splits when Part 1 - Home Again shows")]
		Part1,
		[Description("Act 1 - Day 2"), ToolTip("Splits when Act 1 Day 2 is complete")]
		A1D2,
		[Description("Act 1 - Day 3"), ToolTip("Splits when Act 1 Day 3 is complete")]
		A1D3,
		[Description("The Party"), ToolTip("Splits when The Party shows")]
		Party,
		[Description("Act 1 - Day 4"), ToolTip("Splits when Act 1 Day 4 is complete")]
		A1D4,
		[Description("Part 2 - Weird Autumn"), ToolTip("Splits when Part 2 - Weird Autumn shows")]
		Part2,
		[Description("Old Gods Fort Lucenne Mall"), ToolTip("Splits when Old Gods of the Fort Lucenne Mall shows")]
		OldGods,
		[Description("Act 2 - Day 1"), ToolTip("Splits when Act 2 Day 1 is complete")]
		A2D1,
		[Description("Act 2 - Day 1 Dream"), ToolTip("Splits when Act 2 Day 1 Dream is complete")]
		A2D1Dream,
		[Description("Mechanics"), ToolTip("Splits when Mechanics shows")]
		Mechanics,
		[Description("Act 2 - Day 2"), ToolTip("Splits when Act 2 Day 2 is complete")]
		A2D2,
		[Description("Act 2 - Day 2 Dream"), ToolTip("Splits when Act 2 Day 2 Dream is complete")]
		A2D2Dream,
		[Description("Dinner At Bea's"), ToolTip("Splits when Dinner At Beas shows")]
		DinnerAtBeas,
		[Description("Act 2 - Day 3"), ToolTip("Splits when Act 2 Day 3 is complete")]
		A2D3,
		[Description("Act 2 - Day 3 Dream"), ToolTip("Splits when Act 2 Day 3 Dream is complete")]
		A2D3Dream,
		[Description("Harfest"), ToolTip("Splits when Harfest shows")]
		Harfest,
		[Description("Part 3 - The Long Fall"), ToolTip("Splits when Part 3 - The Long Fall shows")]
		Part3,
		[Description("The Library"), ToolTip("Splits when The Library shows")]
		Library,
		[Description("Act 3 - Day 1"), ToolTip("Splits when Act 3 Day 1 is complete")]
		A3D1,
		[Description("Act 3 - Day 1 Dream"), ToolTip("Splits when Act 3 Day 1 Dream is complete")]
		A3D1Dream,
		[Description("The Graveyard"), ToolTip("Splits when The Graveyard shows")]
		Graveyard,
		[Description("Act 3 - Day 2"), ToolTip("Splits when Act 3 Day 2 is complete")]
		A3D2,
		[Description("Act 3 - Day 2 Dream"), ToolTip("Splits when Act 3 Day 2 Dream is complete")]
		A3D2Dream,
		[Description("Proximity"), ToolTip("Splits when Proximity shows")]
		Proximity,
		[Description("Act 3 - Day 3"), ToolTip("Splits when Act 3 Day 3 is complete")]
		A3D3,
		[Description("The Historical Society"), ToolTip("Splits when The Historical Society Shows")]
		HistSociety,
		[Description("Part 4 - The End Of Everything"), ToolTip("Splits when Part 4 - The End Of Everything Shows")]
		Part4,
		[Description("Act 4 - Day 1"), ToolTip("Splits when Act 4 Day 1 is complete")]
		A4D1,
		[Description("The Hole In The Center Of Everything"), ToolTip("Splits when The Hole In The Center Of Everything Shows")]
		HoleInTheCenter,
		[Description("Epilogue - Stars"), ToolTip("Splits when Epilogue - Stars Shows")]
		Part5,
		[Description("Good Enough"), ToolTip("Splits when you finish dialog of Good Enough")]
		GoodEnough
	}
	public enum TransitionState {
		None,
		In,
		Out
	}
}