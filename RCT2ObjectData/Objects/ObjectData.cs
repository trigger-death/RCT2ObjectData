using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RCT2ObjectData.Objects.Types;
using RCT2ObjectData.Drawing;

namespace RCT2ObjectData.Objects {
	/**<summary>An exception thrown by object data.</summary>*/
	public class ObjectDataException : Exception {
		/**<summary>The object data throwing the exception.</summary>*/
		public ObjectData ObjectData { get; private set; }

		/**<summary>Constructs the object data exception.</summary>*/
		internal ObjectDataException(ObjectData obj) {
			ObjectData = obj;
		}
		/**<summary>Constructs the object data exception.</summary>*/
		internal ObjectDataException(ObjectData obj, string message)
			: base (message) {
			ObjectData = obj;
		}
		/**<summary>Constructs the object data exception.</summary>*/
		internal ObjectDataException(ObjectData obj, string message, Exception innerException)
			: base(message, innerException) {
			ObjectData = obj;
		}
	}

	/**<summary>The info header for all objects.</summary>*/
	public struct ObjectDataInfo {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The 8 character file name of the object.</summary>*/
		public string FileName;
		/**<summary>The flags of the object.</summary>*/
		public uint Flags;
		/**<summary>The name of the object.</summary>*/
		public string Name;
		/**<summary>The subtype of the object.</summary>*/
		public ObjectSubtypes Subtype;
		/**<summary>The header of the object info.</summary>*/
		public ObjectTypeHeader Header;
		/**<summary>The 8 checksum of the file.</summary>*/
		public uint Checksum;

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the type of the object.</summary>*/
		public SourceTypes Source {
			get { return (SourceTypes)((Flags >> 4) & 0xF); }
		}
		/**<summary>Gets the type of the object.</summary>*/
		public ObjectTypes Type {
			get { return (ObjectTypes)(Flags & 0xF); }
		}
		/**<summary>Returns true if the object is invalid.</summary>*/
		public bool Invalid {
			get { return Flags == 0xFFFFFFFF || (ObjectTypes)(Flags & 0xF) == ObjectTypes.None || (ObjectTypes)(Flags & 0xF) > ObjectTypes.ScenarioText; }
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		/**<summary>Returns an object info structure loaded from the specified stream.</summary>*/
		public static ObjectDataInfo FromStream(Stream stream, bool readExtended, Languages language = Languages.British) {
			ObjectDataInfo objInfo = new ObjectDataInfo();

			try {
				BinaryReader reader = new BinaryReader(stream);
				// Read the object data header
				objInfo.Flags = reader.ReadUInt32();
				objInfo.FileName = "";
				for (int i = 0; i < 8; i++) {
					char c = (char)reader.ReadByte();
					if (c != ' ')
						objInfo.FileName += c;
				}
				objInfo.Checksum = reader.ReadUInt32();
				objInfo.Name = "";

				if (readExtended && !objInfo.Invalid) {
					// Decode the chunk to get more info
					ReadInfoFromChunk(reader, ref objInfo, language);
				}
			}
			catch (Exception) {
				objInfo.Flags = 0xFFFFFFFF;
			}

			return objInfo;
		}
		/**<summary>Returns an object info structure loaded from the specified file path.</summary>*/
		public static ObjectDataInfo FromFile(string path, bool readExtended, Languages language = Languages.British) {
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				return FromStream(stream, readExtended, language);
			}
		}
		/**<summary>Returns an object info structure loaded from the specified buffer.</summary>*/
		public static ObjectDataInfo FromBytes(byte[] buffer, bool readExtended, Languages language = Languages.British) {
			using (MemoryStream stream = new MemoryStream(buffer)) {
				return FromStream(stream, readExtended, language);
			}
		}
		
		#endregion
		//=========== HELPERS ============
		#region Helpers

		/**<summary>Reads and decodes the chunk.</summary>*/
		private static void ReadInfoFromChunk(BinaryReader chunkReader, ref ObjectDataInfo objInfo, Languages language) {
			ChunkHeader chunkHeader = new ChunkHeader();

			chunkHeader.Read(chunkReader);
			byte[] decodedChunkData = Chunk.Read(chunkReader, chunkHeader);
			
			using (MemoryStream stream = new MemoryStream(decodedChunkData)) {
				BinaryReader reader = new BinaryReader(stream);
				switch (objInfo.Type) {
				case ObjectTypes.Attraction:	objInfo.Header = new AttractionHeader(); break;
				case ObjectTypes.SmallScenery:	objInfo.Header = new SmallSceneryHeader(); break;
				case ObjectTypes.LargeScenery:	objInfo.Header = new LargeSceneryHeader(); break;
				case ObjectTypes.Wall:			objInfo.Header = new WallHeader(); break;
				case ObjectTypes.PathBanner:	objInfo.Header = new PathBannerHeader(); break;
				case ObjectTypes.Footpath:		objInfo.Header = new FootpathHeader(); break;
				case ObjectTypes.PathAddition:	objInfo.Header = new PathAdditionHeader(); break;
				case ObjectTypes.SceneryGroup:	objInfo.Header = new SceneryGroupHeader(); break;
				case ObjectTypes.ParkEntrance:	objInfo.Header = new ParkEntranceHeader(); break;
				case ObjectTypes.Water:			objInfo.Header = new WaterHeader(); break;
				case ObjectTypes.ScenarioText:	objInfo.Header = new ScenarioTextHeader(); break;
				}

				// Get the object's header
				objInfo.Header.Read(reader);

				// Get the object's subtype for better classification
				objInfo.Subtype = objInfo.Header.ObjectSubtype;

				// Get the object's name in the specified language
				StringEntry entry = new StringEntry();
				entry.Read(reader);
				objInfo.Name = entry.GetWithFallback(language);
			}
		}

		#endregion
	}
	/**<summary>The header for all objects.</summary>*/
	public class ObjectDataHeader {
		//========== CONSTANTS ===========
		#region Constants

		/**<summary>The size of the object data header.</summary>*/
		public const uint HeaderSize = 0x10;

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The flags of the object.</summary>*/
		public uint Flags;
		/**<summary>The 8 character file name of the object.</summary>*/
		public string FileName;
		/**<summary>The checksum of the object header.</summary>*/
		public uint Checksum;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the object data header.</summary>*/
		public ObjectDataHeader() {
			Flags = 0;
			FileName = "";
			Checksum = 0;
		}
		/**<summary>Constructs the object data header.</summary>*/
		public ObjectDataHeader(uint flags, string fileName, uint checksum) {
			Flags = flags;
			FileName = fileName;
			Checksum = checksum;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the source game of the object header.</summary>*/
		public SourceTypes Source {
			get { return (SourceTypes)((Flags >> 4) & 0xF); }
			set { Flags &= 0xFFFFFF0F; Flags |= (uint)((byte)value << 4) & 0xF0; }
		}
		/**<summary>Gets the type of the object header.</summary>*/
		public ObjectTypes Type {
			get { return (ObjectTypes)(Flags & 0xF); }
		}
		/**<summary>Returns true if the object header is invalid.</summary>*/
		public bool Invalid {
			get { return (ObjectTypes)(Flags & 0xF) == ObjectTypes.None || (ObjectTypes)(Flags & 0xF) > ObjectTypes.ScenarioText; }
		}
		
		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object data header.</summary>*/
		public void Read(BinaryReader reader) {
			Flags = reader.ReadUInt32();
			FileName = "";
			for (int i = 0; i < 8; i++) {
				char c = (char)reader.ReadByte();
				if (c != ' ')
					FileName += c;
			}
			Checksum = reader.ReadUInt32();
		}
		/**<summary>Writes the object data header.</summary>*/
		public void Write(BinaryWriter writer) {
			writer.Write(Flags);
			for (int i = 0; i < 8; i++) {
				if (i < FileName.Length)
					writer.Write((byte)FileName[i]);
				else
					writer.Write((byte)' ');
			}
			writer.Write(Checksum);
		}
		/**<summary>Saves the object data to the specified file path.</summary>*/
		public static ObjectDataHeader FromFile(string path) {
			ObjectDataHeader obj = new ObjectDataHeader();
			BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
			obj.Read(reader);
			return obj;
		}

		#endregion
	}
	/**<summary>The base header for all object types.</summary>*/
	public abstract class ObjectTypeHeader {
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the size of the object type header.</summary>*/
		internal abstract uint HeaderSize { get; }
		/**<summary>Gets the basic subtype of the object.</summary>*/
		internal abstract ObjectSubtypes ObjectSubtype { get; }

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object type header.</summary>*/
		internal abstract void Read(BinaryReader reader);
		/**<summary>Writes the object type header.</summary>*/
		internal abstract void Write(BinaryWriter writer);

		#endregion
	}
	/**<summary>The base object data class.</summary>*/
	public class ObjectData {
		//========== CONSTANTS ===========
		#region Constants

		/**<summary>The initial value of the checksum before rotation.</summary>*/
		public const uint InitialChecksum = 0xF369A75B;

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The header of the object data.</summary>*/
		protected ObjectDataHeader objectHeader;
		/**<summary>The header of the chunk.</summary>*/
		protected ChunkHeader chunkHeader;
		/**<summary>The string table of the object.</summary>*/
		protected StringTable stringTable;
		/**<summary>The information about the object's group.</summary>*/
		protected GroupInfo groupInfo;
		/**<summary>The image directory of the object.</summary>*/
		protected ImageDirectory imageDirectory;
		/**<summary>The graphics data of the object.</summary>*/
		protected GraphicsData graphicsData;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the base object data.</summary>*/
		public ObjectData() {
			objectHeader	= new ObjectDataHeader();
			chunkHeader		= new ChunkHeader();
			stringTable		= new StringTable();
			groupInfo		= new GroupInfo();
			imageDirectory	= new ImageDirectory();
			graphicsData	= new GraphicsData(this.imageDirectory);
		}
		/**<summary>Constructs the base object data.</summary>*/
		public ObjectData(ObjectDataHeader objectHeader, ChunkHeader chunkHeader) {
			this.objectHeader	= objectHeader;
			this.chunkHeader	= chunkHeader;
			stringTable			= new StringTable();
			groupInfo			= new GroupInfo();
			imageDirectory		= new ImageDirectory();
			graphicsData		= new GraphicsData(this.imageDirectory);
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties
		//--------------------------------
		#region Base

		/**<summary>Gets the header of the object.</summary>*/
		public ObjectDataHeader ObjectHeader {
			get { return objectHeader; }
			set { objectHeader = value; }
		}
		/**<summary>Gets the header of the chunk.</summary>*/
		public ChunkHeader ChunkHeader {
			get { return chunkHeader; }
			set { chunkHeader = value; }
		}
		/**<summary>The string table of the object.</summary>*/
		public StringTable StringTable {
			get { return stringTable; }
		}
		/**<summary>The information about the object's group.</summary>*/
		public GroupInfo GroupInfo {
			get { return groupInfo; }
		}
		/**<summary>The image directory of the object.</summary>*/
		public ImageDirectory ImageDirectory {
			get { return imageDirectory; }
		}
		/**<summary>The graphics data of the object.</summary>*/
		public GraphicsData GraphicsData {
			get { return graphicsData; }
		}
		/**<summary>Gets the type of the object.</summary>*/
		public SourceTypes Source {
			get { return (SourceTypes)((objectHeader.Flags >> 4) & 0xF); }
			set { objectHeader.Flags &= 0xFFFFFF0F; objectHeader.Flags |= (uint)((byte)value << 4) & 0xF0; }
		}
		/**<summary>Gets the type of the object.</summary>*/
		public ObjectTypes Type {
			get { return (ObjectTypes)(objectHeader.Flags & 0xF); }
		}
		/**<summary>Returns true if the object is invalid.</summary>*/
		public bool Invalid {
			get { return (ObjectTypes)(objectHeader.Flags & 0xF) == ObjectTypes.None; }
		}

		#endregion
		//--------------------------------
		#region Reading

		/**<summary>Gets the number of string table entries in the object.</summary>*/
		public virtual int NumStringTableEntries {
			get { return 1; }
		}
		/**<summary>Returns true if the object has a group info section.</summary>*/
		public virtual bool HasGroupInfo {
			get { return false; }
		}
		/**<summary>Returns true if the object has an image directory and graphics data section.</summary>*/
		public virtual bool HasGraphics {
			get { return true; }
		}

		#endregion
		//--------------------------------
		#region Information

		/**<summary>Gets the subtype of the object.</summary>*/
		public virtual ObjectSubtypes Subtype {
			get { return ObjectSubtypes.Basic; }
		}
		/**<summary>True if the object can be placed on a slope.</summary>*/
		public virtual bool CanSlope {
			get { return false; }
		}
		/**<summary>Gets the number of color remaps.</summary>*/
		public virtual int ColorRemaps {
			get { return 0; }
		}
		/**<summary>Gets if the dialog view has color remaps.</summary>*/
		public virtual bool HasDialogColorRemaps {
			get { return false; }
		}
		/**<summary>Gets the number of frames in the animation.</summary>*/
		public virtual int AnimationFrames {
			get { return 1; }
		}
		/**<summary>Gets the palette to draw the object with.</summary>*/
		public virtual Palette GetPalette(DrawSettings drawSettings) {
			return Palette.DefaultPalette;
		}

		#endregion
		//--------------------------------
		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object data.</summary>*/
		protected void Read(BinaryReader reader, bool quickLoad = false) {
			// Read the header
			ReadHeader(reader);

			// Read the string table entries
			stringTable.Read(reader, NumStringTableEntries);

			// Read the group info
			if (HasGroupInfo)
				groupInfo.Read(reader);

			// Read the optional section
			ReadOptional(reader);

			if (HasGraphics) {
				// Read the image directory
				imageDirectory.Read(reader, quickLoad);
				// Read the graphics data
				graphicsData.Read(reader);
			}
		}
		/**<summary>Writes the object data.</summary>*/
		protected void Write(BinaryWriter writer) {
			// Write the header
			WriteHeader(writer);

			// Write the string table entries
			stringTable.Write(writer);

			// Write the group info
			if (HasGroupInfo)
				groupInfo.Write(writer);

			// Write the optional section
			WriteOptional(writer);
			
			if (HasGraphics) {
				long imageDirectoryPosition = writer.BaseStream.Position;

				// Write the image directory and graphics data
				imageDirectory.Write(writer);
				graphicsData.Write(writer);

				// Rewrite the image directory after the image addresses are known
				long finalPosition = writer.BaseStream.Position;
				writer.BaseStream.Position = imageDirectoryPosition;
				imageDirectory.Write(writer);

				// Set the position to the end of the file so the file size is known
				writer.BaseStream.Position = finalPosition;
			}
		}
		/**<summary>Reads the object data header.</summary>*/
		protected virtual void ReadHeader(BinaryReader reader) {
		
		}
		/**<summary>Reads the object data optional section.</summary>*/
		protected virtual void ReadOptional(BinaryReader reader) {

		}
		/**<summary>Writes the object data header.</summary>*/
		protected virtual void WriteHeader(BinaryWriter writer) {

		}
		/**<summary>Writes the object data optional section.</summary>*/
		protected virtual void WriteOptional(BinaryWriter writer) {

		}

		#endregion
		//=========== DRAWING ============
		#region Drawing

		/**<summary>Draws the object as it is in game.</summary>*/
		public virtual bool Draw(PaletteImage p, Point position, DrawSettings drawSettings) {
			return false;
		}
		/**<summary>Draws the object as it is in the dialog.</summary>*/
		public virtual bool DrawDialog(PaletteImage p, Point position, Size dialogSize, DrawSettings drawSettings) {
			return false;
		}

		#endregion
		//============ SAVING ============
		#region Saving

		/**<summary>Saves the object data to the specified stream.</summary>*/
		public void Save(Stream stream) {
			uint fileSize;
			byte[] encodedChunkData;
			BinaryWriter writer;

			using (MemoryStream chunkStream = new MemoryStream()) {
				writer = new BinaryWriter(chunkStream);
				objectHeader.Write(writer);
				chunkHeader.Write(writer);

				long chunkStartPosition = writer.BaseStream.Position;
				Write(writer);

				// Set the chunk size
				chunkHeader.ChunkSize = (uint)(writer.BaseStream.Position - chunkStartPosition);
				// Get the file size
				fileSize = (uint)writer.BaseStream.Position;

				// Calculate the checksum
				chunkStream.Position = 0;
				BinaryReader reader = new BinaryReader(chunkStream);
				uint checksum = InitialChecksum;
				checksum = RotateChecksum(checksum, reader.ReadByte());
				reader.ReadBytes(3);
				for (int i = 0; i < 8; i++)
					checksum = RotateChecksum(checksum, reader.ReadByte());
				reader.ReadBytes(9);
				for (int i = 16 + 5; i < (int)fileSize; i++)
					checksum = RotateChecksum(checksum, reader.ReadByte());

				objectHeader.Checksum = checksum;

				byte[] chunkData = new byte[fileSize - 21];
				Array.Copy(chunkStream.GetBuffer(), 21, chunkData, 0, fileSize - 21);

				encodedChunkData = Chunk.Write(chunkData, chunkHeader);
			}
			writer = new BinaryWriter(stream);
			objectHeader.Write(writer);
			chunkHeader.Write(writer);
			writer.Write(encodedChunkData);
		}
		/**<summary>Saves the object data to the specified file.</summary>*/
		public void Save(string path, bool setHeaderFileName = false) {
			using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
				stream.SetLength(0);
				if (setHeaderFileName) {
					string fileName = Path.GetFileNameWithoutExtension(path);
					objectHeader.FileName = fileName;
				}
				Save(stream);
			}
		}
		/**<summary>Saves the object data to a new buffer.</summary>*/
		public byte[] ToBytes(string path) {
			using (MemoryStream stream = new MemoryStream()) {
				stream.SetLength(0);
				Save(stream);
				return stream.ToArray();
			}
		}

		#endregion
		//=========== LOADING ============
		#region Loading

		/**<summary>Returns an object loaded from the specified stream.</summary>*/
		public static ObjectData FromStream(Stream stream, bool quickLoad = false) {
			ObjectData obj = null;
			ObjectDataHeader objectHeader = new ObjectDataHeader();
			ChunkHeader chunkHeader = new ChunkHeader();
			byte[] decodedChunkData;

			BinaryReader reader = new BinaryReader(stream);
			objectHeader.Read(reader);
			chunkHeader.Read(reader);
			decodedChunkData = Chunk.Read(reader, chunkHeader);

			using (MemoryStream chunkStream = new MemoryStream(decodedChunkData)) {
				reader = new BinaryReader(chunkStream);
				switch ((ObjectTypes)(objectHeader.Flags & 0xF)) {
				case ObjectTypes.Attraction: obj = new Attraction(objectHeader, chunkHeader); break;
				case ObjectTypes.SmallScenery: obj = new SmallScenery(objectHeader, chunkHeader); break;
				case ObjectTypes.LargeScenery: obj = new LargeScenery(objectHeader, chunkHeader); break;
				case ObjectTypes.Wall: obj = new Wall(objectHeader, chunkHeader); break;
				case ObjectTypes.PathBanner: obj = new PathBanner(objectHeader, chunkHeader); break;
				case ObjectTypes.Footpath: obj = new Footpath(objectHeader, chunkHeader); break;
				case ObjectTypes.PathAddition: obj = new PathAddition(objectHeader, chunkHeader); break;
				case ObjectTypes.SceneryGroup: obj = new SceneryGroup(objectHeader, chunkHeader); break;
				case ObjectTypes.ParkEntrance: obj = new ParkEntrance(objectHeader, chunkHeader); break;
				case ObjectTypes.Water: obj = new Water(objectHeader, chunkHeader); break;
				case ObjectTypes.ScenarioText: obj = new ScenarioText(objectHeader, chunkHeader); break;
				default: objectHeader.Flags = (uint)ObjectTypes.None; break; // Set as invalid
				}
				if (obj != null) {
					obj.Read(reader, quickLoad);
				}
			}
			return obj;
		}
		/**<summary>Returns an object loaded from the specified file path.</summary>*/
		public static ObjectData FromFile(string path, bool quickLoad = false) {
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				return FromStream(stream, quickLoad);
			}
		}
		/**<summary>Returns an object loaded from the specified buffer.</summary>*/
		public static ObjectData FromBytes(byte[] data, bool quickLoad = false) {
			using (MemoryStream stream = new MemoryStream(data)) {
				return FromStream(stream, quickLoad);
			}
		}

		#endregion
		//=========== HELPERS ============
		#region Helpers

		/**<summary>Rotates the checksum with the specified byte.</summary>*/
		private static uint RotateChecksum(uint checksum, byte data) {
			byte checkSumByte = (byte)(checksum & 0xFF);
			checksum &= 0xFFFFFF00;
			checksum |= (uint)(checkSumByte ^ data);
			checksum = (checksum << 11) | (checksum >> (32 - 11));
			return checksum;
		}

		#endregion
	}
	/**<summary>The type of objects.</summary>*/
	public enum ObjectTypes : byte {
		/**<summary>No object type.</summary>*/
		None = 0xF,
		/**<summary>The object is a ride or shop.</summary>*/
		Attraction = 0,
		/**<summary>The object is a small scenery.</summary>*/
		SmallScenery = 1,
		/**<summary>The object is a large scenery.</summary>*/
		LargeScenery = 2,
		/**<summary>The object is a wall.</summary>*/
		Wall = 3,
		/**<summary>The object is a path banner.</summary>*/
		PathBanner = 4,
		/**<summary>The object is a path.</summary>*/
		Footpath = 5,
		/**<summary>The object is a path addition.</summary>*/
		PathAddition = 6,
		/**<summary>The object is a scenery group.</summary>*/
		SceneryGroup = 7,
		/**<summary>The object is a park entrance.</summary>*/
		ParkEntrance = 8,
		/**<summary>The object is a water palette.</summary>*/
		Water = 9,
		/**<summary>The object is scenario text.</summary>*/
		ScenarioText = 10
	}
	/**<summary>The subtype of objects.</summary>*/
	public enum ObjectSubtypes : byte {
		//--------------------------------
		#region Basic

		None,
		Basic,

		Water,
		Entrance,
		Group,
		Path,
		Text,
	
		#endregion
		//--------------------------------
		#region Attractions

		TransportRide,
		GentleRide,
		Rollercoaster,
		ThrillRide,
		WaterRide,
		Stall,

		#endregion
		//--------------------------------
		#region Small Scenery

		Fountain,
		Clock,
		Garden,
	
		#endregion
		//--------------------------------
		#region Walls
	
		Door,
	
		#endregion
		//--------------------------------
		#region Multiple Types
	
		Animation,
		Glass,
	
		#endregion
		//--------------------------------
		#region Path Additions

		Lamp,
		LitterBin,
		Bench,
		JumpingFountain,
		QueueTV,

		#endregion
		//--------------------------------
		#region Signs

		Text3D,
		TextScrolling,
		Photogenic,
	
		#endregion
	}
	/**<summary>The type of source this object came from.</summary>*/
	public enum SourceTypes : byte {
		/**<summary>No object type.</summary>*/
		RCT2 = 8,
		/**<summary>The object is a ride or shop.</summary>*/
		WW = 1,
		/**<summary>The object is a small scenery.</summary>*/
		TT = 2,
		/**<summary>The object is a large scenery.</summary>*/
		Custom = 0
	}
}
