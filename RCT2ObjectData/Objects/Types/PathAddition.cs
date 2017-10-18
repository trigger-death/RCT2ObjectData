using RCT2ObjectData.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects.Types {
	/**<summary>A path addition scenery object.</summary>*/
	public class PathAddition : ObjectData {
		//========== CONSTANTS ===========
		#region Constants

		/**<summary>The size of the header for this object type.</summary>*/
		public const uint HeaderSize = 0x0E;

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The object header.</summary>*/
		public PathAdditionHeader Header;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default object.</summary>*/
		public PathAddition() : base() {
			Header = new PathAdditionHeader();
		}
		/**<summary>Constructs the default object.</summary>*/
		internal PathAddition(ObjectDataHeader objectHeader, ChunkHeader chunkHeader)
			: base(objectHeader, chunkHeader) {
			Header = new PathAdditionHeader();
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties
		//--------------------------------
		#region Reading

		/**<summary>Gets the number of string table entries in the object.</summary>*/
		public override int NumStringTableEntries {
			get { return 1; }
		}
		/**<summary>Returns true if the object has a group info section.</summary>*/
		public override bool HasGroupInfo {
			get { return true; }
		}

		#endregion
		//--------------------------------
		#region Information

		/**<summary>Gets the subtype of the object.</summary>*/
		public override ObjectSubtypes Subtype {
			get {
				if (Header.Flags.HasFlag(PathAdditionFlags.QueueTV))
					return ObjectSubtypes.QueueTV;
				if (Header.Subtype.HasFlag(PathAdditionSubtypes.Bench))
					return ObjectSubtypes.Bench;
				if (Header.Subtype.HasFlag(PathAdditionSubtypes.LitterBin))
					return ObjectSubtypes.LitterBin;
				if (Header.Subtype.HasFlag(PathAdditionSubtypes.Lamp))
					return ObjectSubtypes.Lamp;
				if (Header.Subtype.HasFlag(PathAdditionSubtypes.JumpFountain))
					return ObjectSubtypes.JumpingFountain;
				return ObjectSubtypes.Basic;
			}
		}
		/**<summary>True if the object can be placed on a slope.</summary>*/
		public override bool CanSlope {
			get { return false; }
		}
		/**<summary>Gets the number of color remaps.</summary>*/
		public override int ColorRemaps {
			get { return 0; }
		}
		/**<summary>Gets if the dialog view has color remaps.</summary>*/
		public override bool HasDialogColorRemaps {
			get { return true; }
		}
		/**<summary>Gets the number of frames in the animation.</summary>*/
		public override int AnimationFrames {
			get { return 1; }
		}

		#endregion
		//--------------------------------
		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object header.</summary>*/
		protected override void ReadHeader(BinaryReader reader) {
			Header.Read(reader);
		}
		/**<summary>Writes the object.</summary>*/
		protected override void WriteHeader(BinaryWriter writer) {
			Header.Write(writer);
		}
	
		#endregion
		//=========== DRAWING ============
		#region Drawing

		/**<summary>Constructs the default object.</summary>*/
		public override bool Draw(PaletteImage p, Point position, DrawSettings drawSettings) {
			try {
				if (Header.Flags.HasFlag(PathAdditionFlags.JumpFountain) || Header.Flags.HasFlag(PathAdditionFlags.JumpSnowball)) {
					graphicsData.paletteImages[1 + 0].DrawWithOffset(p, position, drawSettings.Darkness, false);
					graphicsData.paletteImages[1 + 1].DrawWithOffset(p, position, drawSettings.Darkness, false);
					graphicsData.paletteImages[1 + 2].DrawWithOffset(p, position, drawSettings.Darkness, false);
					graphicsData.paletteImages[1 + 3].DrawWithOffset(p, position, drawSettings.Darkness, false);
				}
				else {
					Size offset = Size.Empty;
					if (Header.Subtype == PathAdditionSubtypes.Bench || Header.Subtype == PathAdditionSubtypes.LitterBin) {
						switch (drawSettings.Rotation) {
						case 0: offset = new Size(16 - 4, 8 + 2); break;
						case 1: offset = new Size(16 - 4, 24 - 4); break;
						case 2: offset = new Size(-16 + 4, 24 - 4); break;
						case 3: offset = new Size(-16 + 4, 8 + 4); break;
						}
					}
					else {
						switch (drawSettings.Rotation) {
						case 0: offset = new Size(16, 8); break;
						case 1: offset = new Size(16, 24); break;
						case 2: offset = new Size(-16, 24); break;
						case 3: offset = new Size(-16, 8); break;
						}
					}
					graphicsData.paletteImages[1 + drawSettings.Rotation].DrawWithOffset(p, Point.Add(position, offset), drawSettings.Darkness, false);
				}
			}
			catch (IndexOutOfRangeException) { return false; }
			catch (ArgumentOutOfRangeException) { return false; }
			return true;
		}
		/**<summary>Draws the object data in the dialog.</summary>*/
		public override bool DrawDialog(PaletteImage p, Point position, Size dialogSize, DrawSettings drawSettings) {
			try {
				position = Point.Add(position, new Size(dialogSize.Width / 2, dialogSize.Height / 2));
				graphicsData.paletteImages[0].DrawWithOffset(p, Point.Add(position, new Size(-20, -16)), drawSettings.Darkness, false);
			}
			catch (IndexOutOfRangeException) { return false; }
			catch (ArgumentOutOfRangeException) { return false; }
			return true;
		}
	
		#endregion
	}
	/**<summary>The header used for path banner scenery objects.</summary>*/
	public class PathAdditionHeader : ObjectTypeHeader {
		//=========== MEMBERS ============
		#region Members

		/**<summary>Always zero in files.</summary>*/
		public ushort Reserved0;
		/**<summary>Always zero in files.</summary>*/
		public uint Reserved1;
		/**<summary>The flags used by the object.</summary>*/
		public PathAdditionFlags Flags;
		/**<summary>The subtype of the path addition such as bench or litterbin.</summary>*/
		public PathAdditionSubtypes Subtype;
		/**<summary>The cursor to use when placing the object.</summary>*/
		public byte Cursor;
		/**<summary>X 10.</summary>*/
		public ushort BuildCost;
		/**<summary>Always zero in files.</summary>*/
		public byte Reserved2;
		/**<summary>Always zero in files.</summary>*/
		public byte Reserved3;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default object header.</summary>*/
		public PathAdditionHeader() {
			Reserved0	= 0;
			Reserved1	= 0;
			Flags		= PathAdditionFlags.None;
			Subtype		= PathAdditionSubtypes.Lamp;
			Cursor		= 0;
			BuildCost	= 0;
			Reserved2	= 0;
			Reserved3	= 0;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the size of the object type header.</summary>*/
		internal override uint HeaderSize {
			get { return PathAddition.HeaderSize; }
		}
		/**<summary>Gets the basic subtype of the object.</summary>*/
		internal override ObjectSubtypes ObjectSubtype {
			get {
				if (Flags.HasFlag(PathAdditionFlags.QueueTV))
					return ObjectSubtypes.QueueTV;
				if (Subtype.HasFlag(PathAdditionSubtypes.Bench))
					return ObjectSubtypes.Bench;
				if (Subtype.HasFlag(PathAdditionSubtypes.LitterBin))
					return ObjectSubtypes.LitterBin;
				if (Subtype.HasFlag(PathAdditionSubtypes.Lamp))
					return ObjectSubtypes.Lamp;
				if (Subtype.HasFlag(PathAdditionSubtypes.JumpFountain))
					return ObjectSubtypes.JumpingFountain;
				return ObjectSubtypes.Basic;
			}
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object header.</summary>*/
		internal override void Read(BinaryReader reader) {
			Reserved0	= reader.ReadUInt16();
			Reserved1	= reader.ReadUInt32();
			Flags		= (PathAdditionFlags)reader.ReadUInt16();
			Subtype		= (PathAdditionSubtypes)reader.ReadByte();
			Cursor		= reader.ReadByte();
			BuildCost	= reader.ReadUInt16();
			Reserved2	= reader.ReadByte();
			Reserved3	= reader.ReadByte();
		}
		/**<summary>Writes the object header.</summary>*/
		internal override void Write(BinaryWriter writer) {
			writer.Write(Reserved0);
			writer.Write(Reserved1);
			writer.Write((ushort)Flags);
			writer.Write((byte)Subtype);
			writer.Write(Cursor);
			writer.Write(BuildCost);
			writer.Write(Reserved2);
			writer.Write(Reserved3);
		}

		#endregion
	}
	/**<summary>All flags usable with path banner scenery objects.</summary>*/
	[Flags]
	public enum PathAdditionFlags : ushort {
		/**<summary>No flags are set.</summary>*/
		None = 0,
		/**<summary>The addition can hold trash (has an extra static frame).</summary>*/
		HoldTrash = 1 << 0,
		/**<summary>Guests can sit on this addition.</summary>*/
		CanSit = 1 << 1,
		/**<summary>The addition can be vandilized.</summary>*/
		CanVandal = 1 << 2,
		/**<summary>The addition is a light.</summary>*/
		Light = 1 << 3,
		/**<summary>The addition is a jump fountain.</summary>*/
		JumpFountain = 1 << 4,
		/**<summary>The addition is a jump snowball fountain.</summary>*/
		JumpSnowball = 1 << 5,
		/**<summary>Set for benches and jumping fountains/snowballs and Litter bins.</summary>*/
		Unknown1 = 1 << 6,
		/**<summary>Set for benches and jumping fountains/snowballs.</summary>*/
		Unknown2 = 1 << 7,
		/**<summary>The addition is a queue line TV.</summary>*/
		QueueTV = 1 <<8
	}
	/**<summary>All the subtypes of the path addition scenery objects.</summary>*/
	public enum PathAdditionSubtypes : byte {
		/**<summary>(edge centered, 1 dialog view, 2 frames of static (normal and vandalized).</summary>*/
		Lamp = 0x00,
		/**<summary>(edge centered/inset, 1 dialog view, 3 frames of static (normal and vandalized, and full).</summary>*/
		LitterBin = 0x01,
		/**<summary>(edge centered/inset, 1 dialog view, 2 frames of static (normal and vandalized).</summary>*/
		Bench = 0x02,
		/**<summary>(corners, 1 dialog view, 1 frame of static).</summary>*/
		JumpFountain = 0x03
	}
}
