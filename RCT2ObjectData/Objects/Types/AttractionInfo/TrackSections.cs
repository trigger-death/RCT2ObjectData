using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects.Types.AttractionInfo {
	/**<summary>All flags usable for available track sections.
	 * <para> Each of these pieces will only be enabled if the track type supports it. </para>
	 * <para> Rides with roll banking or sloped turns should have the large turn enabled by default.
	 * Otherwise, the editor will still select it as the default even if the user cannot select it. </para>
	 *</summary>*/
	[Flags]
	public enum TrackSections : ulong {
		/**<summary>No track sections are available.</summary>*/
		None = 0x0000000000000000,
		/**<summary>All track sections are available.</summary>*/
		All = 0xFFFFFFFFFFFFFFFF,

		/**<summary>Enables the flat track piece.</summary>*/
		Flat = 1 << 0,
		/**<summary>Enables the straight track piece.</summary>*/
		Straight = 1 << 1,
		/**<summary>Enables the station platform track piece.</summary>*/
		StationPlatform = 1 << 2,
		/**<summary>Enables the chainlift hill for flat and gentle slope surfaces. Downwards steep slope chainlift seems to be available as well.</summary>*/
		ChainLift = 1 << 3,
		/**<summary>Enables the chainlift hill for steep slope surfaces.</summary>*/
		SteepSlopeChainLift = 1 << 4,
		/**<summary>Enables the chainlift hill for gentle slope turns.</summary>*/
		GentleSlopeTurnChainLift = 1 << 5,
		/**<summary>Enables flat roll banking.</summary>*/
		FlatRollBanking = 1 << 6,
		/**<summary>Enables the verticle small vertical loop pieces.</summary>*/
		VerticalLoop = 1 << 7,
		/**<summary>Enables the gentle slope track pieces.</summary>*/
		GentleSlope = 1 << 8,
		/**<summary>Enables the steep slope track pieces.</summary>*/
		SteepSlope = 1 << 9,
		/**<summary>Enables the short flat to steep slope transition track piece.</summary>*/
		FlatToSteepSlopeShort = 1 << 10,
		/**<summary>Enables the gentle slope turn track pieces.</summary>*/
		GentleSlopeTurns = 1 << 11,
		/**<summary>Enables the steep slope turn track pieces.</summary>*/
		SteepSlopeTurns = 1 << 12,
		/**<summary>Enables the s-bend track pieces.</summary>*/
		SBend = 1 << 13,
		/**<summary>Enables the 1x1 turn track pieces. Flat turns are enabled by default with this flag but other turn types require a separate flag as well.</summary>*/
		TinyTurns = 1 << 14,
		/**<summary>Enables the 2x2 turn track pieces. Flat turns are enabled by default with this flag but other turn types require a separate flag as well.</summary>*/
		SmallTurns = 1 << 15,
		/**<summary>Enables the 3x3 turn track pieces. Flat turns are enabled by default with this flag but other turn types require a separate flag as well.</summary>*/
		LargeTurns = 1 << 16,
		/**<summary>Enables the inline twist track pieces. These pieces do not support inversions.</summary>*/
		InlineTwists = 1 << 17,
		/**<summary>Enables the half loop track piece. This piece does not support inversions.</summary>*/
		HalfLoop = 1 << 18,
		/**<summary>Enables the half corkscrew track pieces. These pieces support inversions and no inversions.</summary>*/
		HalfCorkscrew = 1 << 19,
		/**<summary>Enables the verticle track piece. This is used for all tower rides.</summary>*/
		VerticalTower = 1 << 20,
		/**<summary>Enables the half helix track pieces. This is only available for right-side-up track pieces.</summary>*/
		HalfHelixes = 1 << 21,
		/**<summary>Enables the quarter helix track pieces. This is only available for hanging track pieces.</summary>*/
		QuarterHelixes = 1 << 22,
		/**<summary>Enables the no roll banking quarter helix track pieces. This is only available for hanging track pieces and is exlusive to the suspended swinging coaster.</summary>*/
		QuarterHelixesNoBanking = 1 << 23,
		/**<summary>Enables the brakes track piece.</summary>*/
		Brakes = 1 << 24,

		UnknownFlag25 = 1 << 25,

		/**<summary>Enables the on-ride photo section track piece.</summary>*/
		OnRidePhotoSection = 11 << 26,
		/**<summary>Enables the water splash track piece. This piece is exclusive to the wooden coaster.</summary>*/
		WaterSplash = 1 << 27,
		/**<summary>Enables the vertical slope track pieces.</summary>*/
		VerticalSlopes = 1 << 28,
		/**<summary>Enables the barrel roll track pieces. These pieces do not support inversions.</summary>*/
		BarrelRoll = 1 << 29,
		/**<summary>Enables the launched lift hill track pieces.</summary>*/
		LaunchedLiftHill = 1 << 30,
		/**<summary>Enables the large half loop track pieces. These pieces do not support inversions.</summary>*/
		LargeHalfLoop = 1U << 31,
		/**<summary>Enables the flat banked to gentle slope turn track pieces.
		 * When using gentle slope down, click small turn and click flat.
		 * When using flat roll banking, click small turn and click gentle slope up.
		 * There seems to be a downward version of this but it doesn't work in game. This is also enabled by this flag.
		 *</summary>*/
		FlatBankedToGentleSlopeTurn = 1UL << 32,
		/**<summary>Enables the reverser turntable track piece. This piece is exclusive to the log flume.</summary>*/
		ReverserTurntable = 1UL << 33,
		/**<summary>Enables the heartline roll track pieces. These pieces are exclusive to the heartline twister coaster.</summary>*/
		HeartlineRoll = 1UL << 34,
		/**<summary>Enables the reverser track pieces. These pieces are exclusive to the reverser coaster.</summary>*/
		Reverser = 1UL << 35,
		/**<summary>Enables the slope to vertical and vertical track pieces. These pieces are exclusive to the air powered vertical coaster and reverse freefall coaster.</summary>*/
		SlopeToVertical = 1UL << 36,
		/**<summary>Enables the slope to flat, top section, and vertical track pieces. These pieces are exclusive to the air powered vertical coaster.</summary>*/
		SlopeToFlat = 1UL << 37,
		/**<summary>Enables the black brakes track piece.</summary>*/
		BlockBrakes = 1UL << 38,
		/**<summary>Enables the gentle slope roll banking track pieces. This also enables all available turns.</summary>*/
		GentleSlopeRollBanking = 1UL << 39,
		/**<summary>Enables the long flat to steep slope track pieces.</summary>*/
		FlatToSteepSlopeLong = 1UL << 40,
		/**<summary>Enables the vertical slope turn track pieces.</summary>*/
		VerticalSlopeTurns = 1UL << 41,

		UnknownFlag42 = 1UL << 42,

		/**<summary>Enables the cable lift hill track piece. This piece is exclusive to the giga coaster.</summary>*/
		CableLiftHill = 1UL << 43,
		/**<summary>Enables the curved lift hill track pieces. These pieces are exclusive to the spiral coaster.</summary>*/
		CurvedLiftHill = 1UL << 44,
		/**<summary>Enables the quarter loop track pieces. These pieces do not support inversions.</summary>*/
		QuarterLoop = 1UL << 45,
		/**<summary>Enables the quarter loop track piece. This piece is exclusive to the car ride and ghost train.</summary>*/
		SpinningTunnel = 1UL << 46,
		/**<summary>Enables the spinning toggle control track piece. This piece is exclusive to the spinning wild mouse.</summary>*/
		SpinningToggleControl = 1UL << 47,
		/**<summary>Enables the booster track piece. This flag is the same as spinning toggle control.</summary>*/
		Booster = 1UL << 47,
		/**<summary>Enables the uninverted in-line twist track pieces. These pieces support inversions to inverted track.</summary>*/
		InlineTwistUninverted = 1UL << 48,
		/**<summary>Enables the inverted in-line twist track pieces. These pieces support inversions to uninverted track.</summary>*/
		InlineTwistInverted = 1UL << 49,
		/**<summary>Enables the uninverted quarter loop track pieces. These pieces support inversions to inverted track.</summary>*/
		QuarterLoopUninverted = 1UL << 50,
		/**<summary>Enables the inverted quarter loop track pieces. These pieces support inversions to uninverted track.</summary>*/
		QuarterLoopInverted = 1UL << 51,
		/**<summary>Enables the rapids and log bump track pieces. These pieces are exclusive to the river rapids and car ride.</summary>*/
		RapidsAndLogBumps = 1UL << 52,
		/**<summary>Enables the uninverted half loop track pieces. These pieces support inversions to inverted track.</summary>*/
		HalfLoopUninverted = 1UL << 53,
		/**<summary>Enables the inverted half loop track pieces. These pieces support inversions to uninverted track.</summary>*/
		HalfLoopInverted = 1UL << 54,

		UnknownFlag55 = 1UL << 55,

		UnusedFlag56 = 1UL << 56,
		UnusedFlag57 = 1UL << 57,
		UnusedFlag58 = 1UL << 58,
		UnusedFlag59 = 1UL << 59,
		UnusedFlag60 = 1UL << 60,
		UnusedFlag61 = 1UL << 61,
		UnusedFlag62 = 1UL << 62,
		UnusedFlag63 = 1UL << 63
	}
}
