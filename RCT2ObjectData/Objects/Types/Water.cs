﻿using RCT2ObjectData.Drawing;
using RCT2ObjectData.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects.Types {
	/**<summary>A water object.</summary>*/
	public class Water : ObjectData {
		//========== CONSTANTS ===========
		#region Constants

		/**<summary>The size of the header for this object type.</summary>*/
		public const uint HeaderSize = 0x10;
		/**<summary>The sprites used for drawing water.</summary>*/
		private static PaletteImage[] WaterSprites;

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The object header.</summary>*/
		public WaterHeader Header;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Loads the water resources.</summary>*/
		static Water() {
			GraphicsData graphicsData = GraphicsData.FromBytes(Resources.Water);
			WaterSprites = new PaletteImage[5];
			graphicsData.CopyTo(WaterSprites);
		}
		/**<summary>Constructs the default object.</summary>*/
		public Water() : base() {
			Header = new WaterHeader();
		}
		/**<summary>Constructs the default object.</summary>*/
		internal Water(ObjectDataHeader objectHeader, ChunkHeader chunkHeader)
			: base(objectHeader, chunkHeader) {
			Header = new WaterHeader();
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
			get { return false; }
		}

		#endregion
		//--------------------------------
		#region Information

		/**<summary>Gets the subtype of the object.</summary>*/
		public override ObjectSubtypes Subtype {
			get { return ObjectSubtypes.Water; }
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
			get { return false; }
		}
		/**<summary>Gets the number of frames in the animation.</summary>*/
		public override int AnimationFrames {
			get { return 15; }
		}
		/**<summary>Gets the palette to draw the object with.</summary>*/
		public override Palette GetPalette(DrawSettings drawSettings) {
			Palette palette = new Palette(255);
			for (int i = 0; i < 255; i++) {
				if (i >= 190 && i < 202)
					palette.Colors[i] = graphicsData.palettes[0].Colors[i - 10];
				else
					palette.Colors[i] = Palette.DefaultPalette.Colors[i];
			}
			for (int i = 0; i < 5; i++) {
				palette.Colors[230 + i] = graphicsData.palettes[1 + drawSettings.Darkness].Colors[(i * 3 + drawSettings.Frame) % 15];
				palette.Colors[235 + i] = graphicsData.palettes[4 + drawSettings.Darkness].Colors[(i * 3 + drawSettings.Frame) % 15];
			}

			return palette;
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
				WaterSprites[2].DrawWithOffset(p, Point.Add(position, new Size(0, 0)), drawSettings.Darkness, false);
				WaterSprites[3].DrawWithOffset(p, Point.Add(position, new Size(2, 0)), drawSettings.Darkness, false);
				WaterSprites[4].DrawWithOffset(p, Point.Add(position, new Size(-2, 0)), drawSettings.Darkness, false);
				WaterSprites[3].DrawWithOffset(p, Point.Add(position, new Size(-30, 16)), drawSettings.Darkness, false);
				WaterSprites[4].DrawWithOffset(p, Point.Add(position, new Size(30, 16)), drawSettings.Darkness, false);

				WaterSprites[0].DrawWithOffset(p, Point.Add(position, new Size(0, -15)), drawSettings.Darkness, true);
				WaterSprites[1].DrawWithOffset(p, Point.Add(position, new Size(0, -15)), drawSettings.Darkness, false);
			}
			catch (IndexOutOfRangeException) { return false; }
			catch (ArgumentOutOfRangeException) { return false; }
			return true;
		}
		/**<summary>Draws the object data in the dialog.</summary>*/
		public override bool DrawDialog(PaletteImage p, Point position, Size dialogSize, DrawSettings drawSettings) {
			try {
				position = Point.Add(position, new Size(dialogSize.Width / 2, dialogSize.Height / 2));
				WaterSprites[2].DrawWithOffset(p, Point.Add(position, new Size(0, 0)), drawSettings.Darkness, false);
				WaterSprites[3].DrawWithOffset(p, Point.Add(position, new Size(2, 0)), drawSettings.Darkness, false);
				WaterSprites[4].DrawWithOffset(p, Point.Add(position, new Size(-2, 0)), drawSettings.Darkness, false);
				WaterSprites[3].DrawWithOffset(p, Point.Add(position, new Size(-30, 16)), drawSettings.Darkness, false);
				WaterSprites[4].DrawWithOffset(p, Point.Add(position, new Size(30, 16)), drawSettings.Darkness, false);

				WaterSprites[0].DrawWithOffset(p, Point.Add(position, new Size(0, -15)), drawSettings.Darkness, true);
				WaterSprites[1].DrawWithOffset(p, Point.Add(position, new Size(0, -15)), drawSettings.Darkness, false);
			}
			catch (IndexOutOfRangeException) { return false; }
			catch (ArgumentOutOfRangeException) { return false; }
			return true;
		}

		#endregion
	}
	/**<summary>The header used for water objects.</summary>*/
	public class WaterHeader : ObjectTypeHeader {
		//=========== MEMBERS ============
		#region Members

		/**<summary>14 bytes that are always zero in dat files.</summary>*/
		public byte[] Reserved0;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default object header.</summary>*/
		public WaterHeader() {
			Reserved0	= new byte[16];
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the size of the object type header.</summary>*/
		internal override uint HeaderSize {
			get { return Water.HeaderSize; }
		}
		/**<summary>Gets the basic subtype of the object.</summary>*/
		internal override ObjectSubtypes ObjectSubtype {
			get {
				return ObjectSubtypes.Water;
			}
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object header.</summary>*/
		internal override void Read(BinaryReader reader) {
			reader.Read(Reserved0, 0, Reserved0.Length);
			if (Reserved0[Reserved0.Length - 1] != 0) {
				reader.BaseStream.Position -= 2;
				Reserved0[Reserved0.Length - 1] = 0;
				Reserved0[Reserved0.Length - 2] = 0;
			}
		}
		/**<summary>Writes the object header.</summary>*/
		internal override void Write(BinaryWriter writer) {
			writer.Write(Reserved0);
		}

		#endregion
	}
}
