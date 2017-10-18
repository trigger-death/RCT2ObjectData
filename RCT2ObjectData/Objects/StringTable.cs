using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects {
	/**<summary>An RCT string that represents its own codepage based on language.</summary>*/
	public class RCTString {
		//========== CONSTANTS ===========
		#region Constants
		
		/**<summary>The code page for RCT to latin.</summary>*/
		private static readonly Dictionary<byte, string> LatinCodePage = new Dictionary<byte, string>();
		/**<summary>The code page for latin to RCT.</summary>*/
		private static readonly Dictionary<string, byte> RCTCodePage = new Dictionary<string, byte>();

		/**<summary>The code page used to read Japanese.</summary>*/
		private static readonly Encoding JapaneseCodePage = Encoding.GetEncoding(932);
		/**<summary>The code page used to read Korean.</summary>*/
		private static readonly Encoding KoreanCodePage = Encoding.GetEncoding(949);
		/**<summary>The code page used to read Simplified Chinese.</summary>*/
		private static readonly Encoding ChineseSimplifiedCodePage = Encoding.GetEncoding(936);
		/**<summary>The code page used to read Traditional Chinese.</summary>*/
		private static readonly Encoding ChineseTraditionalCodePage = Encoding.GetEncoding(950);

		#endregion
		//=========== MEMBERS ============
		#region Members

		/**<summary>The language of the string.</summary>*/
		public Languages Language;
		/**<summary>The characters of the string.</summary>*/
		public byte[] Data;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Initializes the Latin code page.</summary>*/
		static RCTString() {
			for (byte b = 0; b < 255; b++) {
				LatinCodePage.Add(b, new string((char)b, 1));
			}

			ReplaceLatin( 34, "\u201D"); // '”'
			ReplaceLatin(159, "\u0104"); // 'Ą'
			ReplaceLatin(160, "\u25B2"); // '▲'
			ReplaceLatin(162, "\u0106"); // 'Ć'
			ReplaceLatin(166, "\u0118"); // 'Ę'
			ReplaceLatin(167, "\u0141"); // 'Ł'

			ReplaceLatin(170, "\u25BC"); // '▼'
			ReplaceLatin(172, "\u2713"); // '✓'
			ReplaceLatin(173, "\u2715"); // '✕'
			
			ReplaceLatin(175, "\u25B6"); // '▶'

			ReplaceLatin(180, "\u201C"); // '“'
			ReplaceLatin(181, "\u20AC"); // '€'

			ReplaceLatin(184, "\u2248"); // '≈'
			ReplaceLatin(185, "\u207B\u00B9"); // '⁻¹'
			ReplaceLatin(186, "\u2022"); // '•'
			ReplaceLatin(188, "\u25B4"); // '▴'
			ReplaceLatin(189, "\u25BE"); // '▾'
			ReplaceLatin(190, "\u25C0"); // '◀'
			
			ReplaceLatin(198, "\u0143"); // 'Ń'
			ReplaceLatin(208, "\u015A"); // 'Ś'
			
			ReplaceLatin(215, "\u0179"); // 'Ź'
			ReplaceLatin(216, "\u017B"); // 'Ż' - "\u01B5" 'Ƶ'?
			
			ReplaceLatin(221, "\u0105"); // 'ą'
			ReplaceLatin(222, "\u0107"); // 'ć'
			
			ReplaceLatin(230, "\u0119"); // 'ę'

			ReplaceLatin(240, "\u0144"); // 'ń'

			ReplaceLatin(247, "\u0142"); // 'ł'
			ReplaceLatin(248, "\u015B"); // 'ś'
			
			ReplaceLatin(253, "\u017C"); // 'ż'
			ReplaceLatin(254, "\u017A"); // 'ź'
			
			LatinCodePage.Remove(164); // '¤'
			LatinCodePage.Remove(168); // '¨'
			LatinCodePage.Remove(174); // '®'
			LatinCodePage.Remove(177); // '±' Railway UTF32
			LatinCodePage.Remove(179); // '³'
			LatinCodePage.Remove(182); // '¶' Road UTF32
			LatinCodePage.Remove(183); // '·' Flag UTF32

			// Remove format codes
			LatinCodePage.Remove(1);
			LatinCodePage.Remove(2);
			for (byte b = 5; b <= 15; b++)
				LatinCodePage.Remove(b);
			LatinCodePage.Remove(17);
			LatinCodePage.Remove(23);
			for (byte b = 123; b <= 155; b++)
				LatinCodePage.Remove(b);
			
			foreach (var pair in LatinCodePage) {
				RCTCodePage.Add(pair.Value, pair.Key);
			}
		}
		/**<summary>Replaces a code with a new string.</summary>*/
		private static void ReplaceLatin(byte b, string str) {
			LatinCodePage[b] = str;
		}
		/**<summary>Constructs the default rct string.</summary>*/
		public RCTString(Languages language) {
			Language = language;
			Data = new byte[0];
		}
		/**<summary>Constructs the default rct string.</summary>*/
		public RCTString(Languages language, byte[] rawString) {
			Language = language;
			Data = rawString;
		}
		/**<summary>Constructs the default rct string.</summary>*/
		public RCTString(Languages language, string str) {
			Language = language;
			FromString(str);
		}

		#endregion
		//========== CONVERTING ==========
		#region Converting

		/**<summary>Converts the RCT string to a real string.</summary>*/
		public override string ToString() {
			string str = "";
			switch (Language) {
			case Languages.British:
			case Languages.American:
			case Languages.French:
			case Languages.German:
			case Languages.Spanish:
			case Languages.Italian:
			case Languages.Dutch:
			case Languages.Swedish:
			case Languages.BrazilianPortuguese:
			case Languages.Unused12:
			case Languages.Unused14:
			case Languages.Unused15:
				foreach (byte b in Data) {
					if (LatinCodePage.ContainsKey(b))
						str += LatinCodePage[b];
				}
				break;
			case Languages.Japanese:
				str = JapaneseCodePage.GetString(Data.ToArray());
				break;
			case Languages.Korean:
				str = KoreanCodePage.GetString(Data.ToArray());
				break;
			case Languages.ChineseSimplified:
				str = ChineseSimplifiedCodePage.GetString(Data.ToArray());
				break;
			case Languages.ChineseTraditional:
				str = ChineseTraditionalCodePage.GetString(Data.ToArray());
				break;
			}
			return str;
		}
		/**<summary>Converts the RCT string to a real string.</summary>*/
		public static implicit operator string(RCTString s) {
			return s.ToString();
		}
		/**<summary>Converts the real string to an RCT string.</summary>*/
		public void FromString(string str) {
			List<byte> rawString = new List<byte>();
			switch (Language) {
			case Languages.British:
			case Languages.American:
			case Languages.French:
			case Languages.German:
			case Languages.Spanish:
			case Languages.Italian:
			case Languages.Dutch:
			case Languages.Swedish:
			case Languages.BrazilianPortuguese:
			case Languages.Unused12:
			case Languages.Unused14:
			case Languages.Unused15:
				for (int i = 0; i < str.Length; i++) {
					if (RCTCodePage.ContainsKey(str.Substring(i, 1))) {
						rawString.Add(RCTCodePage[str.Substring(i, 1)]);
					}
					else if (i + 1 < str.Length && RCTCodePage.ContainsKey(str.Substring(i, 2))) {
						rawString.Add(RCTCodePage[str.Substring(i, 2)]);
						i++;
					}
				}
				break;
			case Languages.Japanese:
				rawString.AddRange(JapaneseCodePage.GetBytes(str));
				break;
			case Languages.Korean:
				rawString.AddRange(KoreanCodePage.GetBytes(str));
				break;
			case Languages.ChineseSimplified:
				rawString.AddRange(ChineseSimplifiedCodePage.GetBytes(str));
				break;
			case Languages.ChineseTraditional:
				rawString.AddRange(ChineseTraditionalCodePage.GetBytes(str));
				break;
			}
			Data = rawString.ToArray();
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the length of the raw string.</summary>*/
		public int Length {
			get { return Data.Length; }
		}
		/**<summary>True if the string is empty.</summary>*/
		public bool IsEmpty {
			get { return Data.Length == 0 || string.IsNullOrWhiteSpace(ToString()); }
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the string languages.</summary>*/
		public void Read(BinaryReader reader) {
			// Read the null-terminated string
			byte c = reader.ReadByte();
			List<byte> rawString = new List<byte>();

			while (c != 0x00) {
				rawString.Add(c);
				c = reader.ReadByte();
			}
			Data = rawString.ToArray();
		}
		/**<summary>Writes the string languages.</summary>*/
		public void Write(BinaryWriter writer) {
			// Write the null-termination
			writer.Write(Data);
			writer.Write((byte)0x00);
		}

		#endregion
	}
	/**<summary>A string entry used in string tables.</summary>*/
	public class StringEntry {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The list of different languages for the string.</summary>*/
		public RCTString[] Strings;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default string entry.</summary>*/
		public StringEntry() {
			Strings = new RCTString[16];
			for (int i = 0; i < 16; i++)
				Strings[i] = new RCTString((Languages)i);
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets or sets a language for the string.</summary>*/
		public RCTString this[int index] {
			get { return Strings[index]; }
			set { Strings[index] = value; }
		}
		/**<summary>Gets or sets a language for the string.</summary>*/
		public RCTString this[Languages index] {
			get { return Strings[(int)index]; }
			set { Strings[(int)index] = value; }
		}
		/**<summary>Gets the string with fallback support.</summary>*/
		public RCTString GetWithFallback(Languages language) {
			if (!this[language].IsEmpty)
				return this[language];

			if (language == Languages.ChineseSimplified) {
				if (!this[Languages.ChineseTraditional].IsEmpty)
					return this[Languages.ChineseTraditional];
			}
			else if (language == Languages.ChineseTraditional) {
				if (!this[Languages.ChineseSimplified].IsEmpty)
					return this[Languages.ChineseSimplified];
			}
			return this[Languages.British];
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the string languages.</summary>*/
		public void Read(BinaryReader reader) {
			// Specifies the index of the language, or 0xFF to end the string table
			byte b = reader.ReadByte();

			// If the language index is 0xFF, end the string table
			while (b != 0xFF) {
				
				if (b < Strings.Length) {
					Strings[b].Read(reader);
				}
				else {
					// Read the null-terminated string
					byte c = reader.ReadByte();

					while (c != 0x00) {
						c = reader.ReadByte();
					}
				}

				// Read the byte for the next language
				b = reader.ReadByte();
			}
		}
		/**<summary>Writes the string languages.</summary>*/
		public void Write(BinaryWriter writer) {

			// Write the string in each language
			for (int i = 0; i < Strings.Length; i++) {
				if (Strings[i].Length == 0)
					continue;
				// Write the language id of the string
				writer.Write((byte)i);

				// Write the string
				Strings[i].Write(writer);
			}
			// End the string table with 0xFF
			writer.Write((byte)0xFF);
		}

		#endregion
	}
	/**<summary>A collection of string entries.</summary>*/
	public class StringTable {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The list of string entries.</summary>*/
		public List<StringEntry> Entries;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default string table.</summary>*/
		public StringTable() {
			Entries = new List<StringEntry>();
		}

		#endregion
		//========== PROPERTIES ==========
		#region Properties

		/**<summary>Gets the number of string entries.</summary>*/
		public int Count {
			get { return Entries.Count; }
		}
		/**<summary>Gets the string entry at the specified position.</summary>*/
		public StringEntry this[int index] {
			get { return Entries[index]; }
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the specified number of string entries.</summary>*/
		public void Read(BinaryReader reader, int numEntries = 1) {
			for (int i = 0; i < numEntries; i++) {
				StringEntry entry = new StringEntry();
				entry.Read(reader);
				Entries.Add(entry);
			}
		}
		/**<summary>Writes the string table entries.</summary>*/
		public void Write(BinaryWriter writer) {
			for (int i = 0; i < Entries.Count; i++) {
				Entries[i].Write(writer);
			}
		}
	
		#endregion
	}
	/**<summary>The list of different lanuages.</summary>*/
	public enum Languages : byte {
		British = 0,
		American = 1,
		French = 2,
		German = 3,
		Spanish = 4,
		Italian = 5,
		Dutch = 6,
		Swedish = 7,
		Japanese = 8,
		Korean = 9,
		ChineseSimplified = 10,
		ChineseTraditional = 11,
		BrazilianPortuguese = 13,
		
		Unused12 = 12,
		Unused14 = 14,
		Unused15 = 15
	}
}
