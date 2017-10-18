using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCT2ObjectData.Objects.Types.AttractionInfo;
using RCT2ObjectData.Drawing;

namespace RCT2ObjectData.Objects.Types {
	/**<summary>A attraction object.</summary>*/
	public class Attraction : ObjectData {
		//========== CONSTANTS ===========
		#region Constants

		/**<summary>The size of the header for this object type.</summary>*/
		public const uint HeaderSize = 0x1C2;

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The object header.</summary>*/
		public AttractionHeader Header;
		/**<summary>The list of 3 colors for each car.</summary>*/
		public List<RemapColors[]> CarColors;
		/**<summary>The list of peep positions on the ride.</summary>*/
		public List<byte[]> RiderPositions;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default object.</summary>*/
		public Attraction() : base() {
			Header			= new AttractionHeader();
			CarColors		= new List<RemapColors[]>();
			RiderPositions	= new List<byte[]>();
		}
		/**<summary>Constructs the default object.</summary>*/
		internal Attraction(ObjectDataHeader objectHeader, ChunkHeader chunkHeader)
			: base(objectHeader, chunkHeader) {
			Header			= new AttractionHeader();
			CarColors		= new List<RemapColors[]>();
			RiderPositions	= new List<byte[]>();
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties
		//--------------------------------
		#region Reading

		/**<summary>Gets the number of string table entries in the object.</summary>*/
		public override int NumStringTableEntries {
			get { return 3; }
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
			get {
				if (Header.RideCategory == RideCategories.Stall)
					return ObjectSubtypes.Stall;
				else if (Header.RideCategory == RideCategories.Transport)
					return ObjectSubtypes.TransportRide;
				else if (Header.RideCategory == RideCategories.Gentle)
					return ObjectSubtypes.GentleRide;
				else if (Header.RideCategory == RideCategories.Rollercoaster)
					return ObjectSubtypes.Rollercoaster;
				else if (Header.RideCategory == RideCategories.Thrill)
					return ObjectSubtypes.ThrillRide;
				else if (Header.RideCategory == RideCategories.Water)
					return ObjectSubtypes.WaterRide;
				return ObjectSubtypes.Basic;
			}
		}
		/**<summary>True if the object can be placed on a slope.</summary>*/
		public override bool CanSlope {
			get { return false; }
		}
		/**<summary>Gets the number of color remaps.</summary>*/
		public override int ColorRemaps {
			get {
				if (Header.RideCategory == RideCategories.Stall) {
					return 1;
				}
				return (!Header.CarTypeList[0].Flags.HasFlag(CarFlags.NoRemap3) ? 3 :
					(Header.CarTypeList[0].Flags.HasFlag(CarFlags.Remap2) ? 2 : 1));
			}
		}
		/**<summary>Gets if the dialog view has color remaps.</summary>*/
		public override bool HasDialogColorRemaps {
			get { return false; }
		}
		/**<summary>Gets the number of frames in the animation.</summary>*/
		public override int AnimationFrames {
			get {
				if (Header.RideCategory == RideCategories.Stall) {
					return 1;
				}
				int frameOffset = 1;
				CarHeader car = Header.CarTypeList[0];
				if (car.Flags.HasFlag(CarFlags.Spinning)) {
					if (car.Flags.HasFlag(CarFlags.SpinningIndependantWheels))
						frameOffset *= (car.LastRotationFrame + 1);
				}
				if (car.Flags.HasFlag(CarFlags.Swinging)) {
					int swingingFrames = 5;
					if (car.Flags.HasFlag(CarFlags.SwingingMoreFrames))
						swingingFrames += 2;
					if (car.Flags.HasFlag(CarFlags.SwingingSlide))
						swingingFrames += 2;
					if (car.Flags.HasFlag(CarFlags.SwingingTilting))
						swingingFrames -= 2;
					frameOffset *= swingingFrames;
				}
				if (car.SpecialFrames != 0)
					frameOffset *= car.SpecialFrames;
				if (car.IsAnimated)
					frameOffset *= 4;
				return frameOffset;
			}
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
		/**<summary>Reads the object data optional.</summary>*/
		protected override void ReadOptional(BinaryReader reader) {
			// Read the 3 byte car color structures
			byte b = reader.ReadByte();
			int numStructures = ((b == 0xFF) ? 32 : b);
			for (int i = 0; i < numStructures; i++) {
				CarColors.Add(new RemapColors[3]);
				for (int j = 0; j < 3; j++) {
					CarColors[CarColors.Count - 1][j] = (RemapColors)reader.ReadByte();
				}
			}

			// Read the 4 variable-length rider position structures for each car type
			for (int i = 0; i < 4; i++) {
				byte b2 = reader.ReadByte();
				int structureLength = (b2 == 0xFF ? reader.ReadUInt16() : b2);
				RiderPositions.Add(new byte[structureLength]);
				for (int j = 0; j < structureLength; j++) {
					RiderPositions[i][j] = reader.ReadByte();
				}
			}
		}
		/**<summary>Writes the object data optional.</summary>*/
		protected override void WriteOptional(BinaryWriter writer) {
			// Write the number of car color structures
			if (CarColors.Count == 32)
				writer.Write((byte)0xFF);
			else
				writer.Write((byte)(CarColors.Count));

			// Write each car color structure
			for (int i = 0; i < CarColors.Count; i++) {
				for (int j = 0; j < 3; j++) {
					writer.Write((byte)CarColors[i][j]);
				}
			}

			// Write the rider positions for each car type
			for (int i = 0; i < RiderPositions.Count; i++) {
				if ((RiderPositions[i].Length & 0x00FF) == RiderPositions[i].Length && (ushort)RiderPositions[i].Length != 0x00FF) {
					// If the rider positions structure length is less than 255, write it as a single byte
					writer.Write((byte)RiderPositions[i].Length);
				}
				else {
					// If the rider positions structure length is greater than or equal 255 write it as an unsigned short
					writer.Write((byte)0xFF);
					writer.Write((ushort)RiderPositions[i].Length);
				}
				// Write the rider positions structure
				writer.Write(RiderPositions[i]);
			}
		}
	
		#endregion
		//=========== DRAWING ============
		#region Drawing

		/**<summary>Constructs the default object.</summary>*/
		public override bool Draw(PaletteImage p, Point position, DrawSettings drawSettings) {
			try {
				if (Header.RideCategory == RideCategories.Stall) {
					graphicsData.paletteImages[3 + drawSettings.Rotation].DrawWithOffset(p, position, drawSettings.Darkness, false,
						drawSettings.Remap1, RemapColors.None, RemapColors.None
					);
					if ((drawSettings.Rotation == 0 || drawSettings.Rotation == 3) && (Header.TrackType == TrackTypes.Restroom || Header.TrackType == TrackTypes.FirstAid)) {
						graphicsData.paletteImages[3 + 4 + drawSettings.Rotation / 3].DrawWithOffset(p, position, drawSettings.Darkness, false,
							drawSettings.Remap1,
							RemapColors.None,
							RemapColors.None
						);
					}
				}
				else {
					int nextCarOffset = 0;
					for (int i = 0; i <= (int)drawSettings.CurrentCar; i++) {
						CarHeader car = Header.CarTypeList[i];
						int C = 0; // Offset for this car
						int R = car.LastRotationFrame + 1; // number of rotation frames
						int F = 1; // Number of frames per rotation
						int P = car.RiderSprites; // number of rider sprites
						int A = 1; // number of animation frames
						if (car.IsAnimated)
							A = 4;
						if (car.Flags.HasFlag(CarFlags.Spinning)) {
							if (car.Flags.HasFlag(CarFlags.SpinningIndependantWheels))
								F *= (car.LastRotationFrame + 1);
						}
						if (car.Flags.HasFlag(CarFlags.Swinging)) {
							int swingingFrames = 5;
							if (car.Flags.HasFlag(CarFlags.SwingingMoreFrames))
								swingingFrames += 2;
							if (car.Flags.HasFlag(CarFlags.SwingingSlide))
								swingingFrames += 2;
							if (car.Flags.HasFlag(CarFlags.SwingingTilting))
								swingingFrames -= 2;
							F *= swingingFrames;
						}
						if (car.SpecialFrames != 0) {
							F *= car.SpecialFrames;
						}

						if (i == (int)drawSettings.CurrentCar) {
							graphicsData.paletteImages[3 + nextCarOffset + drawSettings.Rotation * F * A + drawSettings.Frame].DrawWithOffset(p,
								position, drawSettings.Darkness, false,
								drawSettings.Remap1,
								(car.Flags.HasFlag(CarFlags.Remap2) ? drawSettings.Remap2 : RemapColors.None),
								(!car.Flags.HasFlag(CarFlags.NoRemap3) ? drawSettings.Remap3 : RemapColors.None));
						}
						else {
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.Flat))
								C += (R * F);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.GentleSlopes))
								C += ((4 * F) * 2) + ((R * F) * 2);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.SteepSlopes))
								C += ((8 * F) * 2) + ((R * F) * 2);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.VerticalSlopes))
								C += ((4 * F) * 2) + ((R * F) * 2) + (((4 * F) * 2) * 5) + (4 * F);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.DiagonalSlopes))
								C += ((4 * F) * 2) + ((4 * F) * 2) + ((4 * F) * 2);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.FlatBanked))
								C += ((8 * F) * 2) + ((R * F) * 2);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.InlineTwists))
								C += ((4 * F) * 10);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.FlatToGentleSlopeBankedTransitions))
								C += ((R * F) * 4);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.DiagonalGentleSlopeBankedTransitions))
								C += ((4 * F) * 4);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.GentleSlopeBankedTransitions))
								C += ((4 * F) * 4);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.GentleSlopeBankedTurns))
								C += ((R * F) * 4);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.FlatToGentleSlopeWhileBankedTransitions))
								C += ((4 * F) * 4);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.Corkscrews))
								C += ((4 * F) * 20);
							if (car.SpriteFlags.HasFlag(CarSpriteFlags.RestraintAnimation))
								C += ((4 * F) * 3);

							C *= (P + 1 * A);
							nextCarOffset += C;
						}
					}

				}
			}
			catch (IndexOutOfRangeException) { return false; }
			catch (ArgumentOutOfRangeException) { return false; }
			return true;
		}
		/**<summary>Draws the object data in the dialog.</summary>*/
		public override bool DrawDialog(PaletteImage p, Point position, Size dialogSize, DrawSettings drawSettings) {
			try {
				graphicsData.paletteImages[Header.PreviewIndex].Draw(p, position, drawSettings.Darkness, false);
			}
			catch (IndexOutOfRangeException) { return false; }
			catch (ArgumentOutOfRangeException) { return false; }
			return true;
		}
	
		#endregion
	}
	/**<summary>The header used for attraction objects.</summary>*/
	public class AttractionHeader : ObjectTypeHeader {
		//=========== MEMBERS ============
		#region Members

		/**<summary>This value is always zero in dat files.</summary>*/
		public ulong Reserved0x00;

		/**<summary>The flags used for the attraction.</summary>*/
		public AttractionFlags Flags;
		/**<summary>The list of track types used by the ride.</summary>*/
		public TrackTypes[] TrackTypeList;

		/**<summary>The minimum number of cars per train.</summary>*/
		public byte MinCarsPerTrain;
		/**<summary>The maximum number of cars per train.</summary>*/
		public byte MaxCarsPerTrain;
		/**<summary>The number of cars per flat ride. 0xFF if ride is tracked.</summary>*/
		public byte CarsPerFlatRide;

		/**<summary>The number of zero cars in the train. This value is subtracted from the total number of cars.</summary>*/
		public byte ZeroCars;
		/**<summary>The index of the car shown in the car tab menu.</summary>*/
		public byte CarTabIndex;

		/**<summary>The index of the default car used when no specified type fits.</summary>*/
		public CarTypes DefaultCarType;
		/**<summary>The index of the front car type.</summary>*/
		public CarTypes FrontCarType;
		/**<summary>The index of the second car type.</summary>*/
		public CarTypes SecondCarType;
		/**<summary>The index of the rear car type.</summary>*/
		public CarTypes RearCarType;
		/**<summary>The index of the third car type.</summary>*/
		public CarTypes ThirdCarType;

		/**<summary>The value is always zero in dat files.</summary>*/
		public byte Padding0x19;

		/**<summary>The list of defined car types.</summary>*/
		public CarHeader[] CarTypeList;

		/**<summary>The additional excitement percentage of the ride.</summary>*/
		public byte Excitement;
		/**<summary>The additional intensity percentage of the ride.</summary>*/
		public byte Intensity;
		/**<summary>The additional nausea percentage of the ride.</summary>*/
		public byte Nausea;

		/**<summary>Increases the maximum height of the ride. Theres a base value determined by the track type, and this only adds to it.</summary>*/
		public byte MaxHeight;

		/**<summary>The main category of the ride.</summary>*/
		public RideCategories RideCategory;
		/**<summary>The alternate category of the ride.</summary>*/
		public RideCategories RideCategoryAlternate;

		/**<summary>The available track sections that this ride supports in the editor.</summary>*/
		public TrackSections AvailableTrackSections;

		/**<summary>The first item being sold by the stall. This can be specified for rides as well.</summary>*/
		public ItemTypes SoldItem1;
		/**<summary>The second item being sold by the stall. This can be specified for rides as well.</summary>*/
		public ItemTypes SoldItem2;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default object header.</summary>*/
		public AttractionHeader() {
			Reserved0x00			= 0;

			Flags					= AttractionFlags.None;
			TrackTypeList			= new TrackTypes[3];
			for (int i = 0; i < TrackTypeList.Length; i++)
				TrackTypeList[i]	= TrackTypes.None;

			MinCarsPerTrain			= 0;
			MaxCarsPerTrain			= 0;
			CarsPerFlatRide			= 0;

			ZeroCars				= 0;
			CarTabIndex				= 0;

			DefaultCarType			= CarTypes.CarType0;
			FrontCarType			= CarTypes.None;
			SecondCarType			= CarTypes.None;
			RearCarType				= CarTypes.None;
			ThirdCarType			= CarTypes.None;

			Padding0x19				= 0;

			CarTypeList				= new CarHeader[4];
			for (int i = 0; i < CarTypeList.Length; i++)
				CarTypeList[i]		= new CarHeader();

			Excitement				= 0;
			Intensity				= 0;
			Nausea					= 0;
			MaxHeight				= 0;

			AvailableTrackSections	= TrackSections.None;

			RideCategory			= RideCategories.None;
			RideCategoryAlternate	= RideCategories.None;
			SoldItem1				= ItemTypes.None;
			SoldItem2				= ItemTypes.None;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the number of car types used.</summary>*/
		public int CarCount {
			get {
				int count = (int)DefaultCarType;
				count = Math.Max(count, (int)(FrontCarType != CarTypes.None ? FrontCarType : 0));
				count = Math.Max(count, (int)(SecondCarType != CarTypes.None ? SecondCarType : 0));
				count = Math.Max(count, (int)(RearCarType != CarTypes.None ? RearCarType : 0));
				count = Math.Max(count, (int)(ThirdCarType != CarTypes.None ? ThirdCarType : 0));
				return count + 1;
			}
		}
		/**<summary>Gets or sets the primary type of track used by the ride.</summary>*/
		public TrackTypes TrackType {
			get { return TrackTypeList[PreviewIndex]; }
			set { TrackTypeList[PreviewIndex] = value; }
		}
		/**<summary>Gets or sets the index of the primary type of track used by the ride. Do not set this property if you are using more than one track type.</summary>*/
		public int PreviewIndex {
			get {
				for (int i = 0; i < 3; i++) {
					if (TrackTypeList[i] != TrackTypes.None)
						return i;
				}
				return 0;
			}
			set {
				if (value >= 0 && value < 2) {
					int previewIndex = 0;
					TrackTypes trackType = TrackTypes.None;
					for (int i = 0; i < 3; i++) {
						if (TrackTypeList[i] != TrackTypes.None) {
							if (trackType == TrackTypes.None) {
								previewIndex = i;
								trackType = TrackTypeList[i];
							}
							TrackTypeList[i] = TrackTypes.None;
						}
					}
					TrackTypeList[value] = trackType;
				}
			}
		}

		/**<summary>Gets the size of the object type header.</summary>*/
		internal override uint HeaderSize {
			get { return Attraction.HeaderSize; }
		}
		/**<summary>Gets the basic subtype of the object.</summary>*/
		internal override ObjectSubtypes ObjectSubtype {
			get {
				if (RideCategory == RideCategories.Stall)
					return ObjectSubtypes.Stall;
				else if (RideCategory == RideCategories.Transport)
					return ObjectSubtypes.TransportRide;
				else if (RideCategory == RideCategories.Gentle)
					return ObjectSubtypes.GentleRide;
				else if (RideCategory == RideCategories.Rollercoaster)
					return ObjectSubtypes.Rollercoaster;
				else if (RideCategory == RideCategories.Thrill)
					return ObjectSubtypes.ThrillRide;
				else if (RideCategory == RideCategories.Water)
					return ObjectSubtypes.WaterRide;
				return ObjectSubtypes.Basic;
			}
		}

		/**<summary>Gets the subtype of the object.</summary>*/
		public static ObjectSubtypes ReadSubtype(BinaryReader reader) {
			AttractionHeader header = new AttractionHeader();
			header.Read(reader);
			if (header.RideCategory == RideCategories.Stall)
				return ObjectSubtypes.Stall;
			else if (header.RideCategory == RideCategories.Transport)
				return ObjectSubtypes.TransportRide;
			else if (header.RideCategory == RideCategories.Gentle)
				return ObjectSubtypes.GentleRide;
			else if (header.RideCategory == RideCategories.Rollercoaster)
				return ObjectSubtypes.Rollercoaster;
			else if (header.RideCategory == RideCategories.Thrill)
				return ObjectSubtypes.ThrillRide;
			else if (header.RideCategory == RideCategories.Water)
				return ObjectSubtypes.WaterRide;
			return ObjectSubtypes.Basic;
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object header.</summary>*/
		internal override void Read(BinaryReader reader) {
			long startPosition = reader.BaseStream.Position;

			Reserved0x00 = reader.ReadUInt64();
			Flags = (AttractionFlags)reader.ReadUInt32();

			for (int i = 0; i < 3; i++) {
				TrackTypeList[i] = (TrackTypes)reader.ReadByte();
			}

			MinCarsPerTrain = reader.ReadByte();
			MaxCarsPerTrain = reader.ReadByte();
			CarsPerFlatRide = reader.ReadByte();

			ZeroCars = reader.ReadByte();
			CarTabIndex = reader.ReadByte();

			DefaultCarType = (CarTypes)reader.ReadByte();
			FrontCarType = (CarTypes)reader.ReadByte();
			SecondCarType = (CarTypes)reader.ReadByte();
			RearCarType = (CarTypes)reader.ReadByte();
			ThirdCarType = (CarTypes)reader.ReadByte();

			Padding0x19 = reader.ReadByte();

			for (int i = 0; i < 4; i++) {
				CarTypeList[i].Read(reader);
			}

			reader.BaseStream.Position = startPosition + 0x1B2;

			Excitement = reader.ReadByte();
			Intensity = reader.ReadByte();
			Nausea = reader.ReadByte();
			MaxHeight = reader.ReadByte();

			AvailableTrackSections = (TrackSections)reader.ReadUInt64();

			RideCategory = (RideCategories)reader.ReadByte();
			RideCategoryAlternate = (RideCategories)reader.ReadByte();
			SoldItem1 = (ItemTypes)reader.ReadByte();
			SoldItem2 = (ItemTypes)reader.ReadByte();
		}
		/**<summary>Writes the object header.</summary>*/
		internal override void Write(BinaryWriter writer) {
			long startPosition = writer.BaseStream.Position;

			writer.Write(Reserved0x00);
			writer.Write((uint)Flags);

			for (int i = 0; i < 3; i++) {
				writer.Write((byte)TrackTypeList[i]);
			}

			writer.Write(MinCarsPerTrain);
			writer.Write(MaxCarsPerTrain);
			writer.Write(CarsPerFlatRide);

			writer.Write(ZeroCars);
			writer.Write(CarTabIndex);

			writer.Write((byte)DefaultCarType);
			writer.Write((byte)FrontCarType);
			writer.Write((byte)SecondCarType);
			writer.Write((byte)RearCarType);
			writer.Write((byte)ThirdCarType);

			writer.Write(Padding0x19);

			for (int i = 0; i < 4; i++) {
				CarTypeList[i].Write(writer);
			}

			writer.BaseStream.Position = startPosition + 0x1B2;

			writer.Write(Excitement);
			writer.Write(Intensity);
			writer.Write(Nausea);
			writer.Write(MaxHeight);

			writer.Write((ulong)AvailableTrackSections);

			writer.Write((byte)RideCategory);
			writer.Write((byte)RideCategoryAlternate);
			writer.Write((byte)SoldItem1);
			writer.Write((byte)SoldItem2);
		}

		#endregion
	}
	/**<summary>The header used for attraction car structures.</summary>*/
	public class CarHeader {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The index of the last frame used in full rotations.</summary>*/
		public ushort LastRotationFrame;
		public byte NumVerticalFramesUnused;
		public byte NumHorizontalFramesUnused;

		/**<summary>Gives the spacing between the car and the next.</summary>*/
		public uint Spacing;
		/**<summary>The friction of the car, higher values equals less friction. This value must be above zero on non-flat rides.</summary>*/
		public ushort CarFriction;
		/**<summary>The height of the car to display in the cars tab and in the train length preview. A higher value brings the car up.</summary>*/
		public sbyte CarTabHeight;
		/**<summary>Specifies the amount of riders per car and if they ride in pairs.</summary>*/
		public byte RiderSettings;
		/**<summary>The flags used to specify which car sprites are present in the images.</summary>*/
		public CarSpriteFlags SpriteFlags;

		/**<summary>These 3 values effect how many flat and tower rides are drawn. Changing the values causes certain tiles to draw over each other or not cleanup.
		 * Setting all 3 bytes to 0xFF always seems to work for rides that normally have non-zero values.</summary>*/
		public byte SpriteWidth;
		/**<summary>These 3 values effect how many flat and tower rides are drawn. Changing the values causes certain tiles to draw over each other or not cleanup.
		 * Setting all 3 bytes to 0xFF always seems to work for rides that normally have non-zero values.</summary>*/
		public byte SpriteHeightPositive;
		/**<summary>These 3 values effect how many flat and tower rides are drawn. Changing the values causes certain tiles to draw over each other or not cleanup.
		 * Setting all 3 bytes to 0xFF always seems to work for rides that normally have non-zero values.</summary>*/
		public byte SpriteHeightNegative;

		/**<summary>The first bit seems to be set when the car is animated.</summary>*/
		public byte Animation;
		/**<summary>The flags for the car type.</summary>*/
		public CarFlags Flags;
		public ushort BaseNumFrames;
		/**<summary>These values are always zero in dat files. This has a length of 60 bytes.</summary>*/
		public byte[] Reserved0x18;
		/**<summary>The number of rider sprites for this car type. This is equal to the number of riders if riders do not ride in pairs, else it is half the number of riders.
		 * This is also used for certain rider animations, such as with canoes and rowing boats.</summary>*/
		public byte RiderSprites;
		/**<summary>A higher value results in a smaller change in angular velocity for the same turn radius.</summary>*/
		public byte SpinningInertia;
		/**<summary>A higher value will cause the spin to slow down faster.</summary>*/
		public byte SpinningFriction;

		/**<summary>These 4 values have unknown effects. Bytes 0x57 and 0x59 are often the same value. Bytes 0x58 and 0x5A are only non-zero on a few rides,
		 * such as go karts and reverser. Stalls and many other rides have default values of FF 00 FF 00. 
		 * It's possible that this may be two 2-byte structures.</summary>*/
		public byte FrictionSoundID;
		/**<summary>Only non-zero on a few rides, such as go karts and reverser.</summary>*/
		public byte Unknown0x58;
		/**<summary>These 4 values have unknown effects. Bytes 0x57 and 0x59 are often the same value. Bytes 0x58 and 0x5A are only non-zero on a few rides,
		 * such as go karts and reverser. Stalls and many other rides have default values of FF 00 FF 00. 
		 * It's possible that this may be two 2-byte structures.</summary>*/
		public byte SoundRange;
		/**<summary>Only non-zero on a few rides, such as go karts and reverser.</summary>*/
		public byte Unknown0x5A;

		/**<summary>The acceleration of a powered car. This value is relative to powered max speed.</summary>*/
		public byte PoweredAcceleration;
		/**<summary>The maximum velocity that a powered car will reach on level track. This value is about equal to MPH x 2 when acceleration is 255.
		 * This value must be greater than 0 on powered cars.</summary>*/
		public byte PoweredMaxSpeed;
		/**<summary>Affects how the car is drawn, and is set for specific ride and car types.</summary>*/
		public CarVisuals CarVisual;
		/**<summary>Has an unknown effect but is set for specific ride and car types.</summary>*/
		public CarEffectVisuals EffectVisual;
		/**<summary>Higher values cause the track to be drawn first, while low values cause the car to be drawn first. 0-2 is typical for an inverted ride, while 6-8 is typical for an above-track coaster.</summary>*/
		public byte DrawOrder;
		/**<summary>The number of "special" frames of each car type between rotations. The car flag SpecialFrames must be set in order to use this.
		 * This seems to be set when the default number of frames is not 4. It's also set for rides that don't have an animation but need more than one frame to draw.</summary>*/
		public byte SpecialFrames;
		/**<summary>This value is always zero in dat files.</summary>*/
		public uint Reserved0x61;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default attraction car header.</summary>*/
		public CarHeader() {
			LastRotationFrame = 0;
			NumVerticalFramesUnused = 0;
			NumHorizontalFramesUnused = 0;

			Spacing = 0;
			CarFriction = 0;
			CarTabHeight = 0;
			RiderSettings = 0;
			SpriteFlags = CarSpriteFlags.None;

			SpriteWidth = 0;
			SpriteHeightPositive = 0;
			SpriteHeightNegative = 0;

			Animation = 0;
			Flags = CarFlags.None;
			BaseNumFrames = 0;

			Reserved0x18 = new byte[60];
			for (int i = 0; i < Reserved0x18.Length; i++) Reserved0x18[i] = 0;

			RiderSprites = 0;
			SpinningInertia = 0;
			SpinningFriction = 0;

			FrictionSoundID = 0;
			Unknown0x58 = 0;
			SoundRange = 0;
			Unknown0x5A = 0;

			PoweredAcceleration = 0;
			PoweredMaxSpeed = 0;
			CarVisual = CarVisuals.Default;
			EffectVisual = CarEffectVisuals.Default;
			DrawOrder = 0;
			SpecialFrames = 0;
			Reserved0x61 = 0;
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>True if the car is animated.</summary>*/
		public bool IsAnimated {
			get { return (Animation & 0x1) == 0x1; }
		}
		/**<summary>Gets the number of swinging frames for the car and rider.</summary>*/
		public int SwingingFrames {
			get {
				int count = 1;
				if (Flags.HasFlag(CarFlags.Swinging)) {
					count += 4;
					if (Flags.HasFlag(CarFlags.SwingingMoreFrames))
						count += 2;
					if (Flags.HasFlag(CarFlags.SwingingSlide))
						count += 2;
					if (Flags.HasFlag(CarFlags.SwingingTilting))
						count -= 2;
				}
				return count;
			}
		}
		/**<summary>Gets the number of animation frames for the car.</summary>*/
		public int CarAnimationFrames {
			get {
				int count = 1;
				return count;
			}
		}
		/**<summary>Gets the number of animation frames for the rider.</summary>*/
		public int RiderAnimationFrames {
			get {
				int count = 1;
				if (Flags.HasFlag(CarFlags.SpecialFrames)) {
					count = SpecialFrames;
				}
				else if (IsAnimated && !Flags.HasFlag(CarFlags.RiderSpriteAnimation)) {
					count = 4;
				}
				return count;
			}
		}
		/**<summary>True if there is a single set of rider sprites for every 2 riders.</summary>*/
		public bool RidersRideInPairs {
			get { return (RiderSettings & 0x80) != 0; }
		}
		/**<summary>Gets the total number of riders allowed in the car.</summary>*/
		public int NumberOfRiders {
			get { return (RiderSettings & 0x7F); }
		}
		/**<summary>Gets the number of color remaps.</summary>*/
		public int ColorRemaps {
			get { return (!Flags.HasFlag(CarFlags.NoRemap3) ? 3 : (Flags.HasFlag(CarFlags.Remap2) ? 2 : 1)); }
		}
		/**<summary>Gets the number of frames in the animation.</summary>*/
		public int AnimationFrames {
			get {
				int frameOffset = 1;
				if (Flags.HasFlag(CarFlags.Spinning)) {
					if (Flags.HasFlag(CarFlags.SpinningIndependantWheels))
						frameOffset *= (LastRotationFrame + 1);
				}
				if (Flags.HasFlag(CarFlags.Swinging)) {
					int swingingFrames = 5;
					if (Flags.HasFlag(CarFlags.SwingingMoreFrames))
						swingingFrames += 2;
					if (Flags.HasFlag(CarFlags.SwingingSlide))
						swingingFrames += 2;
					if (Flags.HasFlag(CarFlags.SwingingTilting))
						swingingFrames -= 2;
					frameOffset *= swingingFrames;
				}
				if (SpecialFrames != 0)
					frameOffset *= SpecialFrames;
				if (IsAnimated)
					frameOffset *= 4;
				return frameOffset;
			}
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the object header.</summary>*/
		public void Read(BinaryReader reader) {
			LastRotationFrame = reader.ReadUInt16();

			NumVerticalFramesUnused = reader.ReadByte();
			NumHorizontalFramesUnused = reader.ReadByte();

			Spacing = reader.ReadUInt32();
			CarFriction = reader.ReadUInt16();
			CarTabHeight = reader.ReadSByte();
			RiderSettings = reader.ReadByte();
			SpriteFlags = (CarSpriteFlags)reader.ReadUInt16();

			SpriteWidth = reader.ReadByte();
			SpriteHeightPositive = reader.ReadByte();
			SpriteHeightNegative = reader.ReadByte();

			Animation = reader.ReadByte();
			Flags = (CarFlags)reader.ReadUInt32();
			BaseNumFrames = reader.ReadUInt16();

			reader.Read(Reserved0x18, 0, Reserved0x18.Length);

			RiderSprites = reader.ReadByte();
			SpinningInertia = reader.ReadByte();
			SpinningFriction = reader.ReadByte();

			FrictionSoundID = reader.ReadByte();
			Unknown0x58 = reader.ReadByte();
			SoundRange = reader.ReadByte();
			Unknown0x5A = reader.ReadByte();

			PoweredAcceleration = reader.ReadByte();
			PoweredMaxSpeed = reader.ReadByte();
			CarVisual = (CarVisuals)reader.ReadByte();
			EffectVisual = (CarEffectVisuals)reader.ReadByte();
			DrawOrder = reader.ReadByte();
			SpecialFrames = reader.ReadByte();

			Reserved0x61 = reader.ReadUInt32();
		}
		/**<summary>Writes the object header.</summary>*/
		public void Write(BinaryWriter writer) {
			writer.Write(LastRotationFrame);

			writer.Write(NumVerticalFramesUnused);
			writer.Write(NumHorizontalFramesUnused);

			writer.Write(Spacing);
			writer.Write(CarFriction);
			writer.Write(CarTabHeight);
			writer.Write(RiderSettings);
			writer.Write((ushort)SpriteFlags);

			writer.Write(SpriteWidth);
			writer.Write(SpriteHeightPositive);
			writer.Write(SpriteHeightNegative);

			writer.Write(Animation);
			writer.Write((uint)Flags);
			writer.Write(BaseNumFrames);

			writer.Write(Reserved0x18);

			writer.Write(RiderSprites);
			writer.Write(SpinningInertia);
			writer.Write(SpinningFriction);

			writer.Write(FrictionSoundID);
			writer.Write(Unknown0x58);
			writer.Write(SoundRange);
			writer.Write(Unknown0x5A);

			writer.Write(PoweredAcceleration);
			writer.Write(PoweredMaxSpeed);
			writer.Write((byte)CarVisual);
			writer.Write((byte)EffectVisual);
			writer.Write(DrawOrder);
			writer.Write(SpecialFrames);

			writer.Write(Reserved0x61);
		}

		#endregion
	}
}
