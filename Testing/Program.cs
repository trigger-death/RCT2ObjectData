using RCT2ObjectData.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing {
	class Program {
		static void Main(string[] args) {
			string path = @"C:\Users\Onii-chan\Desktop\1920RACR.DAT";
			string path2 = @"C:\Users\Onii-chan\Desktop\1920RAC2.DAT";
			var obj = ObjectData.FromFile(path);
			obj.Save(path2, true);
			obj = ObjectData.FromFile(path2);
			obj.ChunkHeader.Encoding = ChunkEncoding.None;
			obj.Save(path2, true);
			obj = ObjectData.FromFile(path2);
			obj.ChunkHeader.Encoding = ChunkEncoding.RLE;
			obj.Save(path2, true);
			obj = ObjectData.FromFile(path2);
			obj.ChunkHeader.Encoding = ChunkEncoding.RLECompressed;
			obj.Save(path2, true);
			obj = ObjectData.FromFile(path2);
			obj.ChunkHeader.Encoding = ChunkEncoding.Rotate;
			obj.Save(path2, true);
			obj = ObjectData.FromFile(path2);
		}
	}
}
