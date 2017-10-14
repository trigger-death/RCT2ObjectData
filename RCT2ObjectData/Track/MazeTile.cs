using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Track {
	/**<summary>A single tile in a maze.</summary>*/
	public class MazeTile {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The x position of the maze tile.</summary>*/
		public sbyte X;
		/**<summary>The y position of the maze tile.</summary>*/
		public sbyte Y;
		/**<summary>The walls which are set on the maze tile.</summary>*/
		public MazeWalls Walls;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default maze tile.</summary>*/
		public MazeTile() {
			this.X = 0;
			this.Y = 0;
			this.Walls = MazeWalls.All;
		}
		/**<summary>Constructs the default maze tile.</summary>*/
		public MazeTile(int x, int y, MazeWalls walls) {
			this.X = (sbyte)x;
			this.Y = (sbyte)y;
			this.Walls = walls;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>True if the entrance is on this tile.</summary>*/
		public bool IsEntrance {
			get {
				return (Walls == MazeWalls.EntranceWest ||
						Walls == MazeWalls.EntranceNorth ||
						Walls == MazeWalls.EntranceEast ||
						Walls == MazeWalls.EntranceSouth);
			}
		}
		/**<summary>True if the exit is on this tile.</summary>*/
		public bool IsExit {
			get {
				return (Walls == MazeWalls.ExitWest ||
						Walls == MazeWalls.ExitNorth ||
						Walls == MazeWalls.ExitEast ||
						Walls == MazeWalls.ExitSouth);
			}
		}
		/**<summary>True if the exit is on this tile.</summary>*/
		public bool IsBuilding {
			get {
				return (IsEntrance || IsExit);
			}
		}
		/**<summary>Gets the direction the building on this tile is facing.</summary>*/
		public MazeBuildingDirections BuildingDirection {
			get {
				if (!IsEntrance && !IsExit)
					return MazeBuildingDirections.None;
				return (MazeBuildingDirections)((uint)Walls & 0x000F);
			}
		}
		/**<summary>True if this is the end maze tile.</summary>*/
		public bool IsEnd {
			get { return X == 0 && Y == 0 && Walls == MazeWalls.None; }
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the maze tile.</summary>*/
		public void Read(BinaryReader reader) {
			this.X = reader.ReadSByte();
			this.Y = reader.ReadSByte();
			this.Walls = (MazeWalls)reader.ReadUInt16();
		}
		/**<summary>Writes the maze tile.</summary>*/
		public void Write(BinaryWriter writer) {
			writer.Write(this.X);
			writer.Write(this.Y);
			writer.Write((ushort)this.Walls);
		}

		#endregion

	}
	/**<summary>The flags for maze walls, a set flag means a wall exists there.</summary>*/
	[Flags]
	public enum MazeWalls : ushort {
		None = 0x0000,
		All = 0xFFFF,

		Entrance = 0x0800,
		Exit = 0x8000,
		BuildingMask = 0xFFFC,
		Building = 0x8800,

		BuildingSouth = 0x0003,
		BuildingWest = 0x0002,
		BuildingNorth = 0x00001,
		BuildingEast = 0x0000,
		BuildingDirections = 0x0003,

		EntranceSouth = 0x0803,
		EntranceWest = 0x0802,
		EntranceNorth = 0x0801,
		EntranceEast = 0x0800,

		ExitSouth = 0x8003,
		ExitWest = 0x8002,
		ExitNorth = 0x08001,
		ExitEast = 0x8000,

		QuadrantNorthWestAll = 0x400F,
		QuadrantSouthWestAll = 0x00F4,
		QuadrantSouthEastAll = 0x0F40,
		QuadrantNorthEastAll = 0xF400,

		NorthLeft = 0x0001,
		WestTop = 0x0002,
		WestMiddle = 0x0004,
		QuadrantNorthWest = 0x0008,

		WestBottom = 0x0010,
		SouthLeft = 0x0020,
		SouthMiddle = 0x0040,
		QuadrantSouthWest = 0x0080,

		SouthRight = 0x0100,
		EastBottom = 0x0200,
		EastMiddle = 0x0400,
		QuadrantSouthEast = 0x0800,

		EastTop = 0x1000,
		NorthRight = 0x2000,
		NorthMiddle = 0x4000,
		QuadrantNorthEast = 0x8000
	}
	/**<summary>The directions an entrance/exit can face.</summary>*/
	public enum MazeBuildingDirections {
		None = -1,
		East = 0,
		North = 1,
		West = 2,
		South = 3
	}
	/**<summary>The different types of maze wall styles.</summary>*/
	public enum MazeWallStyles {
		None = -1,
		BrickWalls = 0,
		Hedges = 1,
		IceBlocks = 2,
		WoodenFences = 3
	}
}
