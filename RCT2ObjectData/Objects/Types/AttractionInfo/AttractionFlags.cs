using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects.Types.AttractionInfo {
	/**<summary>All flags usable with attraction objects.</summary>*/
	[Flags]
	public enum AttractionFlags : uint {
		/**<summary>No flags are set.</summary>*/
		None = 0,

		/**<summary>TODO: Test.</summary>*/
		VehicleTabHalfScale = 1 << 0,
		/**<summary>Throws an error when trying to open the ride stating the inverted track is unsuitable for that type of train.</summary>*/
		NoInversions = 1 << 1,
		/**<summary>Throws an error when trying to open the ride stating the banked track is unsuitable for that type of train.</summary>*/
		NoBankedTrack = 1 << 2,
		/**<summary>Plays a tram or train sound when a train departs the entrance. Also hides first remap color on the track.</summary>*/
		PlayTrainDepartSound = 1 << 3,
		/**<summary>Modifies swing state. Set on magic carpet and swinging inverted ship.</summary>*/
		AlternativeSwingMode1 = 1 << 4,
		/**<summary>Set for rotation rides like twist and snow cups. Modifies time to sprite map.</summary>*/
		AlternativeRotationMode1 = 1 << 5,
		/**<summary>Set for lifting arm rotation rides like enterprise. Modifies time to sprite map.</summary>*/
		AlternativeRotationMode2 = 1 << 6,
		/**<summary>Used for updating boat hire vehicle locations. Has an effect when unset.</summary>*/
		UnknownBoatHireFlag = 1 << 7,
		/**<summary>Plays a splash sound during track progress.</summary>*/
		PlaySplashSound = 1 << 8,
		/**<summary>Plays a splash sound when entering water. Also slows boats down in water boats do not slow down in water. Only set on coaster boats.</summary>*/
		PlaySplashSoundSlowInWater = 1 << 9,
		/**<summary>Ride is covered, guests won't care if it's raining.</summary>*/
		Covered = 1 << 10,
		/**<summary>Ride is covered, guests won't care if it's raining.</summary>*/
		LimitAirtimeRatingBonus = 1 << 11,
		/**<summary>If set, this ride shows as a seperate ride instead of an alternate train type. If the track
		 * style doesn't support alternate train types, then this flag must be set or the ride won't show at all.</summary>*/
		SeparateRide1 = 1 << 12,
		/**<summary>If set, this ride shows as a seperate ride instead of an alternate train type. If the track
		 * style doesn't support alternate train types, then this flag must be set or the ride won't show at all.
		 * Set if SeparateRide1 is set.</summary>*/
		SeparateRide2 = 1 << 13,
		/**<summary>This ride will not breakdown. Set on rowing boats, canoes and elevator.</summary>*/
		CannotBreakDown = 1 << 14,
		/**<summary>Hides the last operating mode of the ride.</summary>*/
		DisableLastOperatingMode = 1 << 15,
		/**<summary>Prevents doors from being placed over the track. Set on spinning wild mouse.</summary>*/
		DisableDoors = 1 << 16,
		/**<summary>Hides the first two available operating modes. Set on inverted shuttle and inverted vertical shuttle.</summary>*/
		DisableFirstTwoOperatingModes = 1 << 17,
		/**<summary>Effects vehicle track motion. Has an effect when unset.</summary>*/
		UnknownVehicleTrackMotionFlag = 1 << 18,
		/**<summary>Hides the coloring tab. Set only on sunglasses stall.</summary>*/
		DisableColorTab = 1 << 19,
		/**<summary>Modifies swing state. Only works when AlternativeSwingMode1 is set too.</summary>*/
		AlternativeSwingMode2 = 1 << 20,

		UnusedFlag21 = 1 << 21,
		UnusedFlag22 = 1 << 22,
		UnusedFlag23 = 1 << 23,

		UnusedFlag24 = 1 << 24,
		UnusedFlag25 = 1 << 25,
		UnusedFlag26 = 1 << 26,
		UnusedFlag27 = 1 << 27,

		UnusedFlag28 = 1 << 28,
		UnusedFlag29 = 1 << 29,
		UnusedFlag30 = 1 << 30,
		UnusedFlag31 = 1U << 31
	}
	/**<summary>The different types of rides.</summary>*/
	public enum RideCategories : byte {
		/**<summary>No ride type is set.</summary>*/
		None = 0xFF,
		/**<summary>The attraction is a transport ride.</summary>*/
		Transport = 0x00,
		/**<summary>The attraction is a gentle ride.</summary>*/
		Gentle = 0x01,
		/**<summary>The attraction is a rollercoaster.</summary>*/
		Rollercoaster = 0x02,
		/**<summary>The attraction is a thrill ride.</summary>*/
		Thrill = 0x03,
		/**<summary>The attraction is a water ride.</summary>*/
		Water = 0x04,
		/**<summary>The attraction is a stall.</summary>*/
		Stall = 0x05,
	}
	/**<summary>The different types of stalls.</summary>*/
	public enum StallTypes : byte {
		/**<summary>No stall type is set.</summary>*/
		None = 0xFF,
		/**<summary>The stall is a food stall.</summary>*/
		Food = 0x1C,
		/**<summary>The stall is a drinks stall.</summary>*/
		Drinks = 0x1E,
		/**<summary>The stall is a souvenir stall.</summary>*/
		Souvenir = 0x20,
		/**<summary>The stall is an information kiosk.</summary>*/
		InfoKiosk = 0x23,
		/**<summary>The stall is a restroom.</summary>*/
		Restroom = 0x24,
		/**<summary>The stall is a cash machine.</summary>*/
		CashMachine = 0x2D,
		/**<summary>The stall is a first aid room.</summary>*/
		FirstAid = 0x30,
	}
	/**<summary>The different types of tracks.</summary>*/
	public enum TrackTypes : byte {
		/**<summary>No track type is set.</summary>*/
		None = 0xFF,
		/**<summary>The track type is for sprial rollercoasters.</summary>*/
		SpiralRollerCoaster = 0x00,
		/**<summary>The track type is for stand-up rollercoasters.</summary>*/
		StandUpRollerCoaster = 0x01,
		/**<summary>The track type is for suspended swinging rollercoasters.</summary>*/
		SuspendedSwingingCoaster = 0x02,
		/**<summary>The track type is for inverted rollercoasters.</summary>*/
		InvertedRollerCoaster = 0x03,
		/**<summary>The track type is for junior rollercoasters.</summary>*/
		JuniorRollerCoaster = 0x04,
		/**<summary>The track type is for railroads.</summary>*/
		Railroad = 0x05,
		/**<summary>The track type is for monorails.</summary>*/
		Monorail = 0x06,
		/**<summary>The track type is for mini suspended rollercoasters.</summary>*/
		MiniSuspendedCoaster = 0x07,
		/**<summary>The track type is for boar hires.</summary>*/
		BoatHire = 0x08,
		/**<summary>The track type is for wooden wild rollercoasters.</summary>*/
		WoodenWildRide = 0x09,
		/**<summary>The track type is for single rail rollercoasters.</summary>*/
		SingleRailCoaster = 0x0A,
		/**<summary>The track type is for car rides.</summary>*/
		CarRide = 0x0B,
		/**<summary>The track type is for launched freefalls.</summary>*/
		LaunchedFreefall = 0x0C,
		/**<summary>The track type is for bobsled rollercoasters.</summary>*/
		BobsledCoaster = 0x0D,
		/**<summary>The track type is for observation tower.</summary>*/
		ObservationTower = 0x0E,
		/**<summary>The track type is for looping rollercoasters.</summary>*/
		LoopingRollerCoaster = 0x0F,
		/**<summary>The track type is for dinghy slides.</summary>*/
		DinghySlide = 0x10,
		/**<summary>The track type is for mine train rollercoasters.</summary>*/
		MineTrainCoaster = 0x11,
		/**<summary>The track type is for chair lifts.</summary>*/
		ChairLift = 0x12,
		/**<summary>The track type is for corkscrew rollercoasters.</summary>*/
		CorkscrewRollerCoaster = 0x13,
		/**<summary>The track type is for hedge mazes.</summary>*/
		HedgeMaze = 0x14,
		/**<summary>The track type is for spiral slides.</summary>*/
		SpiralSlide = 0x15,
		/**<summary>The track type is for go karts.</summary>*/
		GoKarts = 0x16,
		/**<summary>The track type is for log flumes.</summary>*/
		LogFlume = 0x17,
		/**<summary>The track type is for river rapids.</summary>*/
		RiverRapids = 0x18,
		/**<summary>The track type is for dodgems, or bumper cars.</summary>*/
		Dodgems = 0x19,
		/**<summary>The track type is for swinging ships.</summary>*/
		SwingingShip = 0x1A,
		/**<summary>The track type is for swinging inverted ships.</summary>*/
		SwingingInvertedShip = 0x1B,
		/**<summary>The track type is a food stall.</summary>*/
		FoodStall = 0x1C,
		Unused0x1D = 0x1D,
		/**<summary>The track type is a drinks stall.</summary>*/
		DrinksStall = 0x1E,
		Unused0x1F = 0x1F,
		/**<summary>The track type is a souvenir stall.</summary>*/
		SouvenirStall = 0x20,
		/**<summary>The track type is for merry-go-rounds.</summary>*/
		MerryGoRound = 0x21,
		Unused0x22 = 0x22,
		/**<summary>The track type is an information kiosk.</summary>*/
		InfoKiosk = 0x23,
		/**<summary>The track type is a restroom.</summary>*/
		Restroom = 0x24,
		/**<summary>The track type is for ferris wheels.</summary>*/
		FerrisWheel = 0x25,
		/**<summary>The track type is for motion simulator.</summary>*/
		MotionSimulator = 0x26,
		/**<summary>The track type is for 3D cinemas.</summary>*/
		Cinema3D = 0x27,
		/**<summary>The track type is for top spins.</summary>*/
		TopSpin = 0x28,
		/**<summary>The track type is for space rings.</summary>*/
		SpaceRings = 0x29,
		/**<summary>The track type is for reverse freefall rollercoasters.</summary>*/
		ReverseFreefallCoaster = 0x2A,
		/**<summary>The track type is for elevators.</summary>*/
		Elevator = 0x2B,
		/**<summary>The track type is for vertical drop rollercoasters.</summary>*/
		VerticalDropRollerCoaster = 0x2C,
		/**<summary>The track type is a cash machine.</summary>*/
		CashMachine = 0x2D,
		/**<summary>The track type is for twists.</summary>*/
		Twist = 0x2E,
		/**<summary>The track type is for rhaunted houses.</summary>*/
		HauntedHouse = 0x2F,
		/**<summary>The track type is a first aid room.</summary>*/
		FirstAid = 0x30,
		/**<summary>The track type is for circuses.</summary>*/
		Circus = 0x31,
		/**<summary>The track type is for haunted rides.</summary>*/
		HauntedRide = 0x32,
		/**<summary>The track type is for twister rollercoasters.</summary>*/
		TwisterRollerCoaster = 0x33,
		/**<summary>The track type is for wooden rollercoasters.</summary>*/
		WoodenRollerCoaster = 0x34,
		/**<summary>The track type is for side friction rollercoasters.</summary>*/
		SideFrictionRollerCoaster = 0x35,
		/**<summary>The track type is for wild mouse rollercoasters.</summary>*/
		WildMouse = 0x36,
		/**<summary>The track type is for multi-dimension rollercoasters.</summary>*/
		MultiDimensionRollerCoaster = 0x37,
		Unused0x38 = 0x38,
		/**<summary>The track type is for flying rollercoasters.</summary>*/
		FlyingRollerCoaster = 0x39,
		Unused0x3A = 0x3A,
		/**<summary>The track type is for virginia reels.</summary>*/
		VirginiaReel = 0x3B,
		/**<summary>The track type is for splash boats.</summary>*/
		SplashBoats = 0x3C,
		/**<summary>The track type is for mini helicopters.</summary>*/
		MiniHelicopters = 0x3D,
		/**<summary>The track type is for laydown rollercoasters.</summary>*/
		LaydownRollerCoaster = 0x3E,
		/**<summary>The track type is for suspended monorails.</summary>*/
		SuspendedMonorail = 0x3F,
		Unused0x40 = 0x40,
		/**<summary>The track type is for reverser rollercoasters.</summary>*/
		ReverserRollerCoaster = 0x41,
		/**<summary>The track type is for heartline twister rollercoasters.</summary>*/
		HeartlineTwisterCoaster = 0x42,
		/**<summary>The track type is for mini golf.</summary>*/
		MiniGolf = 0x43,
		/**<summary>The track type is for giga rollercoasters.</summary>*/
		GigaCoaster = 0x44,
		/**<summary>The track type is for roto drops.</summary>*/
		RotoDrop = 0x45,
		/**<summary>The track type is for flying saucers.</summary>*/
		FlyingSaucers = 0x46,
		/**<summary>The track type is for crooked houses.</summary>*/
		CrookedHouse = 0x47,
		/**<summary>The track type is for monorail cycles.</summary>*/
		MonorailCycles = 0x48,
		/**<summary>The track type is for inverted shuttle rollercoasters.</summary>*/
		InvertedShuttleCoaster = 0x49,
		/**<summary>The track type is for water rollercoasters.</summary>*/
		WaterCoaster = 0x4A,
		/**<summary>The track type is for air powered vertical rollercoasters.</summary>*/
		AirPoweredVerticalCoaster = 0x4B,
		/**<summary>The track type is for inverted hairpin rollercoasters.</summary>*/
		InvertedHairpinCoaster = 0x4C,
		/**<summary>The track type is for magic carpets.</summary>*/
		MagicCarpet = 0x4D,
		/**<summary>The track type is for submarine rides.</summary>*/
		SubmarineRide = 0x4E,
		/**<summary>The track type is for river rafts.</summary>*/
		RiverRafts = 0x4F,
		Unused0x50 = 0x50,
		/**<summary>The track type is for enterprises.</summary>*/
		Enterprise = 0x51,
		Unused0x52 = 0x52,
		Unused0x53 = 0x53,
		Unused0x54 = 0x54,
		Unused0x55 = 0x55,
		/**<summary>The track type is for inverted himpulse rollercoasters.</summary>*/
		InvertedImpulseCoaster = 0x56,
		/**<summary>The track type is for mini rollercoasters.</summary>*/
		MiniRollerCoaster = 0x57,
		/**<summary>The track type is for mine ride rollercoasters.</summary>*/
		MineRide = 0x58,
		Unused0x59 = 0x59,
		/**<summary>The track type is for LIM launched rollercoasters.</summary>*/
		LIMLaunchedRollerCoaster = 0x5A
	}
}
