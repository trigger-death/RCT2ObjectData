using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects {
	/**<summary>An entry for an image.</summary>*/
	public class ImageEntry {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The start address of the image, 0 is the position after the image directory.</summary>*/
		public uint StartAddress;
		/**<summary>The width of the image, or number of colors in the palette.</summary>*/
		public short Width;
		/**<summary>The height of the image.</summary>*/
		public short Height;
		/**<summary>The x offset of the image, or the starting index of the palette.</summary>*/
		public short XOffset;
		/**<summary>The y offset of the image.</summary>*/
		public short YOffset;
		/**<summary>The image type flags.</summary>*/
		public ImageFlags Flags;
		/**<summary>Unused.</summary>*/
		public ushort Unused;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default image entry.</summary>*/
		public ImageEntry() {
			StartAddress	= 0;
			Width			= 0;
			Height			= 0;
			XOffset			= 0;
			YOffset			= 0;
			Flags			= ImageFlags.None;
			Unused			= 0;
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the image entry.</summary>*/
		public void Read(BinaryReader reader) {
			StartAddress	= reader.ReadUInt32();
			Width			= reader.ReadInt16();
			Height			= reader.ReadInt16();
			XOffset			= reader.ReadInt16();
			YOffset			= reader.ReadInt16();
			Flags			= (ImageFlags)reader.ReadUInt16();
			Unused			= reader.ReadUInt16();
		}
		/**<summary>Writes the image entry.</summary>*/
		public void Write(BinaryWriter writer) {
			writer.Write(StartAddress);
			writer.Write(Width);
			writer.Write(Height);
			writer.Write(XOffset);
			writer.Write(YOffset);
			writer.Write((ushort)Flags);
			writer.Write(Unused);
		}

		#endregion
	}
	/**<summary>A directory of image entries.</summary>*/
	public class ImageDirectory {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The length of scan lines.</summary>*/
		public int ScanLineLength;
		/**<summary>The list of image entries.</summary>*/
		internal List<ImageEntry> entries;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default image directory.</summary>*/
		public ImageDirectory() {
			ScanLineLength	= 0;
			entries			= new List<ImageEntry>();
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the number of images to read.</summary>*/
		public int NumEntries {
			get { return entries.Count; }
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the image directory.</summary>*/
		public void Read(BinaryReader reader, bool quickLoad = false) {
			int count = reader.ReadInt32();
			ScanLineLength = reader.ReadInt32();

			for (int i = 0; i < count; i++) {
				ImageEntry entry = new ImageEntry();
				entry.Read(reader);
				if (!quickLoad || i < 168)
					entries.Add(entry);
			}
		}
		/**<summary>Reads the RCT1 image directory.</summary>*/
		public void ReadCSG1(BinaryReader reader) {
			while (reader.BaseStream.Position < reader.BaseStream.Length) {
				ImageEntry entry = new ImageEntry();
				entry.Read(reader);
				entries.Add(entry);
			}
		}
		/**<summary>Writes the image directory.</summary>*/
		public void Write(BinaryWriter writer) {
			writer.Write(entries.Count);
			writer.Write(ScanLineLength);
			
			foreach (ImageEntry entry in entries) {
				entry.Write(writer);
			}
		}
		/**<summary>Writes the RCT1 image directory.</summary>*/
		public void WriteCSG1(BinaryWriter writer) {
			foreach (ImageEntry entry in entries) {
				entry.Write(writer);
			}
		}

		#endregion
	}
	/**<summary>The image type flags.</summary>*/
	[Flags]
	public enum ImageFlags : ushort {
		/**<summary>No flag selected.</summary>*/
		None = 0,
		/**<summary>The bitmap is read directly.</summary>*/
		DirectBitmap = 1 << 0,
		/**<summary>The bitmap is compacted.</summary>*/
		CompactedBitmap = 1 << 2,
		/**<summary>The image is a collection of palette entries.</summary>*/
		PaletteEntries = 1 << 3,
		/**<summary>The flag is only used with land tiles in g1.dat. It's unknown why this flag is used.</summary>*/
		LandTile = 1 << 4
	}
}
