using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Libraria;

using Libraria.IO.Compression.SevenZip;
using SZEncoder = Libraria.IO.Compression.SevenZip.LZMA.Encoder;
using SZDecoder = Libraria.IO.Compression.SevenZip.LZMA.Decoder;

namespace Libraria {
	public static partial class Lib {
		public static byte[] Compress(byte[] Bytes) {
			MemoryStream In = new MemoryStream(Bytes);
			MemoryStream Out = new MemoryStream();
			SZEncoder E = new SZEncoder();

			E.WriteCoderProperties(Out);
			Out.WriteBytes(BitConverter.GetBytes(In.Length));

			E.Code(In, Out, -1, -1, null);
			return Out.ToArray();
		}

		public static byte[] Decompress(byte[] Bytes) {
			MemoryStream In = new MemoryStream(Bytes);
			MemoryStream Out = new MemoryStream();
			SZDecoder D = new SZDecoder();

			byte[] Props = new byte[5];
			if (In.Read(Props, 0, Props.Length) != Props.Length)
				throw new InvalidOperationException("Compressed memory too short");
			D.SetDecoderProperties(Props);
			long Len = BitConverter.ToInt64(In.ReadBytes(sizeof(long)), 0);

			D.Code(In, Out, In.Length - In.Position, Len, null);
			return Out.ToArray();
		}
	}
}
