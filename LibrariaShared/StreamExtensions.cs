using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using Libraria.Serialization;

namespace Libraria {
	public static class StreamExtensions {
		public static void WriteBytes(this Stream S, byte[] Bytes) {
			S.Write(Bytes, 0, Bytes.Length);
		}

		public static byte[] ReadBytes(this Stream S, int Len) {
			byte[] Bytes = new byte[Len];
			S.Read(Bytes, 0, Bytes.Length);
			return Bytes;
		}
	}

	public unsafe static class BinaryReaderExtensions {
		public static string ReadString(this BinaryReader BR, Encoding Enc, int Length = -1) {
			using (MemoryStream MS = new MemoryStream(Length != -1 ? Length : 16)) {
				if (Length == -1) {
					byte B = 0;
					while ((B = BR.ReadByte()) != 0)
						MS.WriteByte(B);
				} else
					MS.WriteBytes(BR.ReadBytes(Length));

				return Enc.GetString(MS.ToArray());
			}
		}

		public static object Read(this BinaryReader BR, Type T, int Len) {
			FieldInfo[] Fields = T.GetFields();

			if (Fields.Count((I) => I.GetCustomAttribute<StringEncodingAttribute>() != null) > 0) {
				//int Idx = 0;
				//return (T)ValueSerializer.Deserialize(BR.ReadBytes(Len), typeof(string), ref Idx, 10);
				return ValueSerializer.DeserializeStruct(T, BR.ReadBytes(Len), false);
			}

			/*for (int i = 0; i < Fields.Length; i++) 
				if (Fields[i].FieldType == typeof(string) && Fields[i].GetCustomAttribute<StringEncodingAttribute>() != null) {

				}*/

			byte[] Mem = BR.ReadBytes(Len);
			fixed (byte* MemPtr = Mem)
				return Marshal.PtrToStructure(new IntPtr(MemPtr), T);
		}

		public static object Read(this BinaryReader BR, Type T) {
			return BR.Read(T, Marshal.SizeOf(T));
		}

		public static object Read(this BinaryReader BR, Type T, int Offset, int Length) {
			BR.BaseStream.Seek(Offset, SeekOrigin.Begin);
			return BR.Read(T, Length);
		}

		public static T Read<T>(this BinaryReader BR) where T : struct {
			return (T)BR.Read(typeof(T));
		}

		public static T Read<T>(this BinaryReader BR, int Len) {
			return (T)BR.Read(typeof(T), Len);
		}

		public static T Read<T>(this BinaryReader BR, int Offset, int Length) where T : struct {
			return (T)BR.Read(typeof(T), Offset, Length);
		}

		public static object ReadArray(this BinaryReader BR, Type T, int Count) {
			//object[] Values = new object[Count];
			Array Values = Array.CreateInstance(T, Count);

			for (int i = 0; i < Values.Length; i++)
				Values.SetValue(BR.Read(T), i);

			return Values;
		}

		public static T[] ReadArray<T>(this BinaryReader BR, int Count) where T : struct {
			if (typeof(T) == typeof(byte))
				return (T[])(object)BR.ReadBytes(Count);

			T[] Values = new T[Count];
			for (int i = 0; i < Values.Length; i++)
				Values[i] = (T)BR.Read(typeof(T));
			return Values;
		}

		public static object ReadArray(this BinaryReader BR, Type T, int Offset, int Length) {
			BR.BaseStream.Seek(Offset, SeekOrigin.Begin);
			return BR.ReadArray(T, Length / Marshal.SizeOf(T));
		}

		public static T[] ReadArray<T>(this BinaryReader BR, int Offset, int Length) where T : struct {
			BR.BaseStream.Seek(Offset, SeekOrigin.Begin);
			return BR.ReadArray<T>(Length / Marshal.SizeOf(typeof(T)));
		}
	}

	public static class BinaryWriterExtensions {
		public static void Write(this BinaryWriter BW, string Str, Encoding Enc, bool NullTerminated = false) {
			BW.Write(Enc.GetBytes(Str));
			if (NullTerminated)
				BW.Write((byte)0);
		}

		public static void Write(this BinaryWriter BW, IntPtr Mem, int Len) {
			byte[] Bytes = new byte[Len];
			Marshal.Copy(Mem, Bytes, 0, Len);
			BW.Write(Bytes);
		}

		public static void WriteStruct(this BinaryWriter BW, object Structure) {
			Type T = Structure.GetType();
			FieldInfo[] Fields = T.GetFields();

			if (Fields.Count((I) => I.GetCustomAttribute<StringEncodingAttribute>() != null) > 0) {
				//int Idx = 0;
				//return (T)ValueSerializer.Deserialize(BR.ReadBytes(Len), typeof(string), ref Idx, 10);
				//return ValueSerializer.DeserializeStruct(T, BR.ReadBytes(Len), false);

				BW.Write(ValueSerializer.SerializeStruct(Structure, false));
				return;
			}

			/*for (int i = 0; i < Fields.Length; i++) 
				if (Fields[i].FieldType == typeof(string) && Fields[i].GetCustomAttribute<StringEncodingAttribute>() != null) {

				}*/

			int Len = Marshal.SizeOf(Structure);
			IntPtr Mem = Marshal.AllocHGlobal(Len);
			Marshal.StructureToPtr(Structure, Mem, false);
			BW.Write(Mem, Len);
			Marshal.FreeHGlobal(Mem);
		}

		public static void WriteStructArray(this BinaryWriter BW, Array A) {
			for (int i = 0; i < A.Length; i++)
				BW.WriteStruct(A.GetValue(i));
		}
	}
}
