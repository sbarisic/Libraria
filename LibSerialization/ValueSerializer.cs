using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using Libraria.IO.Compression;

namespace Libraria.Serialization {
	public enum EncodingType {
		ASCII,
		UTF8
	}

	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class StringEncodingAttribute : Attribute {
		public Encoding Encoding;
		public int Length;

		public StringEncodingAttribute(EncodingType E, int Length = -1) {
			switch (E) {
				case EncodingType.ASCII:
					this.Encoding = Encoding.ASCII;
					break;

				case EncodingType.UTF8:
					this.Encoding = Encoding.UTF8;
					break;

				default:
					throw new NotImplementedException("Unknown encoding type " + E);
			}

			this.Length = Length;
		}
	}

	public static class ValueSerializer {
		public static Encoding TextEncoding = Encoding.UTF8;

		static bool LengthRequired(FieldInfo FInfo) {
			if (FInfo.FieldType == typeof(string)) {
				if (FInfo.GetCustomAttribute<StringEncodingAttribute>() != null)
					return false;
				return true;
			}

			return false;
		}

		public static byte[] SerializeStruct(object Structure, bool Compress = true)  {
			Type T = Structure.GetType();
			FieldInfo[] Fields = T.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			List<byte> Bytes = new List<byte>();

			for (int i = 0; i < Fields.Length; i++) {
				byte[] FieldBytes = Serialize(Fields[i].GetValue(Structure));

				if (LengthRequired(Fields[i]))
					Bytes.AddRange(Serialize(FieldBytes.Length));
				Bytes.AddRange(FieldBytes);
			}

			byte[] BytesArray = Bytes.ToArray();
			if (Compress)
				BytesArray = Lib.Compress(BytesArray);
			return BytesArray;
		}

		public static void DeserializeStruct(Type T, ref object Structure, byte[] Bytes, bool Compressed = true)  {
			FieldInfo[] Fields = T.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			int Idx = 0;

			if (Compressed)
				Bytes = Lib.Decompress(Bytes);

			for (int i = 0; i < Fields.Length; i++) {
				int Len = -1;
				if (LengthRequired(Fields[i]))
					Len = Deserialize<int>(Bytes, ref Idx);

				Fields[i].SetValue(Structure, Deserialize(Bytes, Fields[i], ref Idx, Len));
			}
		}

		public static object DeserializeStruct(Type T, byte[] Bytes, bool Compressed = true) {
			object Instance = Activator.CreateInstance(T);
			DeserializeStruct(T, ref Instance, Bytes, Compressed);
			return Instance;
		}

		public static T DeserializeStruct<T>(byte[] Bytes, bool Compressed = true) where T : struct {
			return (T)DeserializeStruct(typeof(T), Bytes, Compressed);
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

		public static object Deserialize(byte[] Bytes, FieldInfo Field, ref int Idx, int Len = -1) {
			StringEncodingAttribute StringEncoding = Field.GetCustomAttribute<StringEncodingAttribute>();
			if (StringEncoding != null)
				Len = StringEncoding.Length;

			return Deserialize(Bytes, Field.FieldType, ref Idx, Len);
		}

		public static T Deserialize<T>(byte[] Bytes, ref int Idx, int Len = -1) {
			return (T)Deserialize(Bytes, typeof(T), ref Idx, Len);
		}
	}
}
