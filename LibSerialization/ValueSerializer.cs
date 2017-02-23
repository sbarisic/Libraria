using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using Libraria.IO.Compression;

namespace Libraria.Serialization {
	public static class ValueSerializer {
		static Encoding TextEncoding = Encoding.UTF8;

		static bool LengthRequired(Type T) {
			if (T == typeof(string))
				return true;
			return false;
		}

		public static byte[] SerializeStruct<T>(T Structure, bool Compress = true) where T : struct {
			FieldInfo[] Fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			List<byte> Bytes = new List<byte>();

			for (int i = 0; i < Fields.Length; i++) {
				byte[] FieldBytes = Serialize(Fields[i].GetValue(Structure));

				if (LengthRequired(Fields[i].FieldType))
					Bytes.AddRange(Serialize(FieldBytes.Length));
				Bytes.AddRange(FieldBytes);
			}

			byte[] BytesArray = Bytes.ToArray();
			if (Compress)
				BytesArray = Lib.Compress(BytesArray);
			return BytesArray;
		}

		public static void DeserializeStruct<T>(ref T Structure, byte[] Bytes, bool Compressed = true) where T : struct {
			FieldInfo[] Fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			int Idx = 0;
			object Obj = Structure;

			if (Compressed)
				Bytes = Lib.Decompress(Bytes);

			for (int i = 0; i < Fields.Length; i++) {
				int Len = -1;
				if (LengthRequired(Fields[i].FieldType))
					Len = Deserialize<int>(Bytes, ref Idx);

				Fields[i].SetValue(Obj, Deserialize(Bytes, Fields[i].FieldType, ref Idx, Len));
			}

			Structure = (T)Obj;
		}

		public static T DeserializeStruct<T>(byte[] Bytes, bool Compressed = true) where T : struct {
			T Ret = new T();
			DeserializeStruct<T>(ref Ret, Bytes, Compressed);
			return Ret;
		}

		public static byte[] Serialize(object Value) {
			if (Value == null)
				return new byte[0];
			else if (Value is bool)
				return BitConverter.GetBytes((bool)Value);
			else if (Value is char)
				return BitConverter.GetBytes((char)Value);
			else if (Value is short)
				return BitConverter.GetBytes((short)Value);
			else if (Value is int)
				return BitConverter.GetBytes((int)Value);
			else if (Value is long)
				return BitConverter.GetBytes((long)Value);
			else if (Value is ushort)
				return BitConverter.GetBytes((ushort)Value);
			else if (Value is uint)
				return BitConverter.GetBytes((uint)Value);
			else if (Value is ulong)
				return BitConverter.GetBytes((ulong)Value);
			else if (Value is float)
				return BitConverter.GetBytes((float)Value);
			else if (Value is double)
				return BitConverter.GetBytes((double)Value);
			else if (Value is string)
				return TextEncoding.GetBytes((string)Value);

			throw new NotImplementedException("Not implemented for type " + Value.GetType().Name);
		}

		public static object Deserialize(byte[] Bytes, Type T, ref int Idx, int Len = -1) {
			object Ret;

			if (T == typeof(bool))
				Ret = BitConverter.ToBoolean(Bytes, Idx);
			else if (T == typeof(char))
				Ret = BitConverter.ToChar(Bytes, Idx);
			else if (T == typeof(short))
				Ret = BitConverter.ToInt16(Bytes, Idx);
			else if (T == typeof(int))
				Ret = BitConverter.ToInt32(Bytes, Idx);
			else if (T == typeof(long))
				Ret = BitConverter.ToInt64(Bytes, Idx);
			else if (T == typeof(ushort))
				Ret = BitConverter.ToUInt16(Bytes, Idx);
			else if (T == typeof(uint))
				Ret = BitConverter.ToUInt32(Bytes, Idx);
			else if (T == typeof(ulong))
				Ret = BitConverter.ToUInt64(Bytes, Idx);
			else if (T == typeof(float))
				Ret = BitConverter.ToSingle(Bytes, Idx);
			else if (T == typeof(double))
				Ret = BitConverter.ToDouble(Bytes, Idx);
			else if (T == typeof(string)) {
				if (Len == -1)
					Len = Bytes.Length - Idx;

				Ret = TextEncoding.GetString(Bytes, Idx, Len);
				Idx += Len;
			} else
				throw new NotImplementedException();

			if (T.IsValueType)
				Idx += Marshal.SizeOf(T);
			return Ret;
		}

		public static T Deserialize<T>(byte[] Bytes, ref int Idx, int Len = -1) {
			return (T)Deserialize(Bytes, typeof(T), ref Idx, Len);
		}
	}
}
