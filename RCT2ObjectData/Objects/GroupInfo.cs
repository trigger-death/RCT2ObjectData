using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCT2ObjectData.Objects {
	/**<summary>Information about an object's group.</summary>*/
	public class GroupInfo {
		//=========== MEMBERS ============
		#region Members

		/**<summary>The flags of the group.</summary>*/
		public GroupInfoFlags Flags;
		/**<summary>The file name of the scenery group.</summary>*/
		public string FileName;
		/**<summary>The checksum of the group info.</summary>*/
		public uint CheckSum;

		#endregion
		//========= CONSTRUCTORS =========
		#region Constructors

		/**<summary>Constructs the default group info.</summary>*/
		public GroupInfo() {
			Flags = GroupInfoFlags.None;
			FileName = "";
			CheckSum = 0;
		}

		#endregion
		//=========== READING ============
		#region Reading

		/**<summary>Reads the group info.</summary>*/
		public void Read(BinaryReader reader) {
			Flags = (GroupInfoFlags)reader.ReadUInt32();
			FileName = "";
			for (int i = 0; i < 8; i++) {
				char c = (char)reader.ReadByte();
				if (c != ' ' && c != '\0')
					FileName += c;
			}
			CheckSum = reader.ReadUInt32();
		}
		/**<summary>Writes the group info.</summary>*/
		public void Write(BinaryWriter writer) {
			writer.Write((uint)Flags);
			for (int i = 0; i < 8; i++) {
				if (i < FileName.Length)
					writer.Write((byte)FileName[i]);
				else
					writer.Write((byte)' ');
			}
			writer.Write(CheckSum);
		}

		#endregion
	}
	/**<summary>Information about an object's group.</summary>*/
	[Flags]
	public enum GroupInfoFlags : uint {
		/**<summary>No flags are selected.</summary>*/
		None = 0x00000000,
		/**<summary>This is a default item in the group.</summary>*/
		DefaultItem = 0xFF000000,
		/**<summary>This is an official scenery group.</summary>*/
		OfficialGroup = 0x00000087,
		/**<summary>This is a custom scenery group.</summary>*/
		CustomGroup = 0x00000007
	}
}
