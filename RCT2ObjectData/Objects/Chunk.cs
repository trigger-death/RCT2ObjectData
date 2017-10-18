using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects {
	/**<summary>An exception thrown by chunks.</summary>*/
	public class ChunkException : Exception {
		/**<summary>The header for the chunk throwing the exception.</summary>*/
		public ChunkHeader ChunkHeader { get; private set; }

		/**<summary>Constructs the chunk exception.</summary>*/
		internal ChunkException(ChunkHeader header) {
			ChunkHeader = header;
		}
		/**<summary>Constructs the chunk exception.</summary>*/
		internal ChunkException(ChunkHeader header, string message)
			: base(message) {
			ChunkHeader = header;
		}
		/**<summary>Constructs the chunk exception.</summary>*/
		internal ChunkException(ChunkHeader header, string message, Exception innerException)
			: base(message, innerException) {
			ChunkHeader = header;
		}
	}

	/**<summary>The header for chunk data.</summary>*/
	public class ChunkHeader {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The encoding type used by the chunk.</summary>*/
		public ChunkEncoding Encoding;
		/**<summary>The size of the chunk.</summary>*/
		public uint ChunkSize;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the chunk header.</summary>*/
		public ChunkHeader() {
			Encoding = ChunkEncoding.None;
			ChunkSize = 0;
		}
		/**<summary>Constructs the chunk header.</summary>*/
		public ChunkHeader(ChunkEncoding encoding, uint chunkSize) {
			Encoding = encoding;
			ChunkSize = chunkSize;
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the chunk header.</summary>*/
		internal void Read(BinaryReader reader) {
			Encoding = (ChunkEncoding)reader.ReadByte();
			ChunkSize = reader.ReadUInt32();
		}
		/**<summary>Writes the chunk header.</summary>*/
		internal void Write(BinaryWriter writer) {
			writer.Write((byte)Encoding);
			writer.Write(ChunkSize);
		}

		#endregion
	}
	/**<summary>A static class for encoding and decoding chunks.</summary>*/
	public static class Chunk {
		//=========== READING ============
		#region Reading
		
		/**<summary>Reads and decodes the chunk data.</summary>*/
		public static byte[] Read(BinaryReader reader, ChunkHeader header) {
			if (header.Encoding > ChunkEncoding.Rotate) {
				throw new ChunkException(header, "Invalid chunk encoding: " + ((int)header.Encoding).ToString() + "!");
			}

			switch (header.Encoding) {
			case ChunkEncoding.None:
				try {
					return reader.ReadBytes((int)header.ChunkSize);
				} catch (EndOfStreamException ex) {
					throw new ChunkException(header, "Unexpected end of stream while reading chunk!", ex);
				}

			case ChunkEncoding.RLE:
				return DecodeRLE(reader, header);

			case ChunkEncoding.RLECompressed:
				byte[] data = DecodeRLE(reader, header);
				return DecodeRepeat(data, header);

			case ChunkEncoding.Rotate:
				return DecodeRotate(reader, header);
			}

			return null;
		}
		/**<summary>Encodes and returns the chunk data.</summary>*/
		public static byte[] Write(byte[] data, ChunkHeader header) {
			if (header.Encoding > ChunkEncoding.Rotate) {
				throw new ChunkException(header, "Invalid chunk encoding: " + ((int)header.Encoding).ToString() + "!");
			}

			switch (header.Encoding) {
			case ChunkEncoding.None:
				header.ChunkSize = (uint)data.Length;
				byte[] outData = new byte[data.Length];
				Array.Copy(data, outData, data.Length);
				return outData;

			case ChunkEncoding.RLE:
				return EncodeRLE(data, header);

			case ChunkEncoding.RLECompressed:
				data = EncodeRepeat(data, header);
				return EncodeRLE(data, header);

			case ChunkEncoding.Rotate:
				return EncodeRotate(data, header);
			}

			return null;
		}

		#endregion
		//=========== DECODING ===========
		#region Decoding

		/**<summary>Decodes the data written with Run Length Encoding.</summary>*/
		public static byte[] DecodeRLE(BinaryReader reader, ChunkHeader header) {
			try {
				int chunkPosition = 0;

				using (MemoryStream stream = new MemoryStream()) {
					BinaryWriter writer = new BinaryWriter(stream);
					// While the end of the uncompressed chunk has not been reached
					while (chunkPosition < header.ChunkSize) {
						// Read the next byte
						byte b = reader.ReadByte();

						// If the MSB is 0, copy the next (b + 1) bytes
						if ((b & 0x80) == 0) {
							int length = (b + 1);
							chunkPosition += length + 1;
							writer.Write(reader.ReadBytes(length));
						}

						// Else the MSB is 1, repeat the following byte (-b + 1) times
						else {
							byte copyByte = reader.ReadByte();
							int length = ((byte)(-(sbyte)b) + 1);
							chunkPosition += 2;
							for (int i = 0; i < length; i++)
								writer.Write(copyByte);
						}
					}
					
					return stream.ToArray();
				}
			}
			catch (EndOfStreamException ex) {
				throw new ChunkException(header, "Unexpected end of stream while reading chunk!", ex);
			}
		}
		/**<summary>Decodes the data written with Repeat Compression.</summary>*/
		public static byte[] DecodeRepeat(byte[] data, ChunkHeader header) {
			try {
				int position = 0;

				using (MemoryStream stream = new MemoryStream()) {
					BinaryWriter writer = new BinaryWriter(stream);

					for (int i = 0; i < data.Length; i++) {
						byte b = data[i];
						if (b == 0xFF) {
							i++;
							writer.Write(data[i]);
							position++;
						}
						else {
							int count = (b & 0x7) + 1;
							int copyOffset = position + ((b >> 3) - 32);
							writer.Write(stream.GetBuffer(), copyOffset, count);
							position += count;
						}
					}

					return stream.ToArray();
				}
			}
			catch (EndOfStreamException ex) {
				throw new ChunkException(header, "Unexpected end of stream while reading chunk!", ex);
			}
		}
		/**<summary>Decodes the data written with Rotate Encoding.</summary>*/
		public static byte[] DecodeRotate(BinaryReader reader, ChunkHeader header) {
			try {
				byte[] outData = new byte[header.ChunkSize];
			
				int shift = 1;
				for (int i = 0; i < header.ChunkSize; i++) {
					byte b = reader.ReadByte();
					outData[i] = (byte)((b >> shift) | (b << (8 - shift)));
					shift = (shift + 2) % 8;
				}

				return outData;
			}
			catch (EndOfStreamException ex) {
				throw new ChunkException(header, "Unexpected end of stream while reading chunk!", ex);
			}
		}

		#endregion
		//=========== ENCODING ===========
		#region Encoding

		/**<summary>Encodes the data with Run Length Encoding.</summary>*/
		public static byte[] EncodeRLE(byte[] data, ChunkHeader header) {
			int position = 0;
			int chunkPosition = 0;

			using (MemoryStream stream = new MemoryStream()) {
				BinaryWriter writer = new BinaryWriter(stream);

				while (position < data.Length) {

					int startLength = position;
					if (position + 1 == data.Length) {
						writer.Write((byte)0x00);
						writer.Write(data[position]);
						position++;
						chunkPosition += 2;
					}
					else {
						bool duplicate = false;
						byte startByte = data[position];
						byte b = data[position + 1];
						if (b == startByte)
							duplicate = true;
						int count = 2;
						for (; count < 125 && count + position < data.Length; count++) {
							if ((b == data[position + count]) != duplicate) {
								if (!duplicate)
									count--;
								break;
							}
							else {
								b = data[position + count];
							}
						}

						if (!duplicate) {
							writer.Write((byte)(count - 1));
							for (int i = 0; i < count; i++) {
								writer.Write(data[position + i]);
							}
							chunkPosition += (count + 1);
						}
						else {
							writer.Write((byte)(-(sbyte)((byte)count - 1)));
							writer.Write(startByte);
							chunkPosition += 2;
						}

						position += count;
					}
				}

				header.ChunkSize = (uint)chunkPosition;
				return stream.ToArray();
			}
		}
		/**<summary>Encodes the data with Repeat Compression.</summary>*/
		public static byte[] EncodeRepeat(byte[] data, ChunkHeader header) {
			int chunkPosition = 0;

			if (data.Length == 0)
				return new byte[0];

			using (MemoryStream stream = new MemoryStream()) {
				BinaryWriter writer = new BinaryWriter(stream);

				writer.Write((byte)0xFF);
				writer.Write(data[0]);
				chunkPosition += 2;

				for (int i = 1; i < header.ChunkSize; i++) {
					uint searchIndex = (uint)((i < 32) ? 0 : (i - 32));
					uint searchEnd = (uint)i - 1;

					uint bestRepeatIndex = 0;
					uint bestRepeatCount = 0;
					for (uint repeatIndex = searchIndex; repeatIndex <= searchEnd; repeatIndex++) {
						uint repeatCount = 0;
						uint maxRepeatCount = (uint)Math.Min(Math.Min(7, searchEnd - repeatIndex), (int)header.ChunkSize - i - 1);
						for (int j = 0; j <= maxRepeatCount; j++) {
							if (data[repeatIndex + j] == data[i + j])
								repeatCount++;
							else
								break;
						}
						if (repeatCount > bestRepeatCount) {
							bestRepeatIndex = repeatIndex;
							bestRepeatCount = repeatCount;

							// Maximum repeat count is 8
							if (repeatCount == 8)
								break;
						}
					}

					if (bestRepeatCount == 0) {
						writer.Write((byte)0xFF);
						writer.Write(data[i]);
						chunkPosition += 2;
					}
					else {
						writer.Write((byte)((bestRepeatCount - 1) | ((32 - (uint)(i - bestRepeatIndex)) << 3)));
						chunkPosition++;
						i += (int)bestRepeatCount - 1;
					}
				}

				header.ChunkSize = (uint)chunkPosition;
				return stream.ToArray();
			}
		}
		/**<summary>Encodes the data with Rotate Encoding.</summary>*/
		public static byte[] EncodeRotate(byte[] data, ChunkHeader header) {
			header.ChunkSize = (uint)data.Length;
			byte[] outData = new byte[header.ChunkSize];

			int shift = 1;
			for (int i = 0; i < data.Length; i++) {
				byte b = data[i];
				outData[i] = (byte)((b << shift) | (b >> (8 - shift)));
				shift = (shift + 2) % 8;
			}

			return outData;
		}

		#endregion
	}
	/**<summary>The type of encoding used in chunks.</summary>*/
	public enum ChunkEncoding : byte {
		/**<summary>There is no encoding.</summary>*/
		None = 0,
		/**<summary>Uses Run Length Encoding.</summary>*/
		RLE = 1,
		/**<summary>Uses Run Length Encoding then Repeat Compression.</summary>*/
		RLECompressed = 2,
		/**<summary>Uses Rotate Encoding.</summary>*/
		Rotate = 3
	}
}
