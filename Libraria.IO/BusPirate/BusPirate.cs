using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace Libraria.IO {
	public class BusPirate {
		public enum Commands : byte {
			ResetBitbang = 0b00000000,
			EnterBinarySPI = 0b00000001,
			Reset = 0b00001111,
			ProbeVoltage = 0b00010100,
		}

		Encoding StringEncoding;
		SerialPort Port;
		//BinaryWriter Writer;
		//BinaryReader Reader;

		private BusPirate(string PortName) {
			StringEncoding = Encoding.UTF8;
			Port = new SerialPort(PortName, 115200, Parity.None, 8, StopBits.One);
			Port.ReadBufferSize = 4096;
		}

		void Init() {
		}

		public void Write(params byte[] Bytes) {
			Port.Write(Bytes, 0, Bytes.Length);
			Port.BaseStream.Flush();
		}

		public void Write(Commands Cmd) {
			Write((byte)Cmd);
		}

		public void Write(string S) {
			Write(StringEncoding.GetBytes(S));
		}

		public void WriteLine(string S = "") {
			if (S.Length > 0)
				Write(S);
			Write("\n");
		}

		public string ReadExisting() {
			return Port.ReadExisting();
		}

		public byte[] Read(int Count = 1, bool Wait = true) {
			if (Port.BytesToRead < Count && !Wait)
				Count = Port.BytesToRead;

			byte[] Buffer = new byte[Count];
			for (int i = 0; i < Buffer.Length; i++)
				Buffer[i] = (byte)Port.ReadByte();

			return Buffer;
		}

		public string ReadString(int Count = 1, bool Wait = true) {
			return StringEncoding.GetString(Read(Count, Wait));
		}

		public float ProbeVoltage() {
			Write(Commands.ProbeVoltage);
			byte[] ADCBytes = Read(2);
			return ((ushort)((ushort)ADCBytes[0] << 8 | (ushort)ADCBytes[1])) * 0.0066f;
		}

		public void ClearInputBuffer() {
			int PrevCnt = -1;
			do {
				Thread.Sleep(1);
				int CurCnt = Port.BytesToRead;

				if (PrevCnt == CurCnt)
					break;

				PrevCnt = CurCnt;
			} while (true);

			Port.DiscardInBuffer();
		}

		public static BusPirate OpenBBIO1(string PortName) {
			BusPirate Pirate = new BusPirate(PortName);

#if !DEBUG
			try {
#else
#endif
			Pirate.Port.Open();
			Pirate.Init();
#if !DEBUG
			} catch (Exception) {
				return null;
			}
#else
#endif

			Pirate.Write(Commands.Reset);
			for (int i = 0; i < 10; i++)
				Pirate.WriteLine();
			Pirate.WriteLine("#");
			Pirate.ClearInputBuffer();

			for (int i = 0; i < 30; i++) {
				Pirate.Write(Commands.ResetBitbang);
				Thread.Sleep(1);
				string Str = Pirate.ReadString(5, false);
				if (Str == "BBIO1")
					return Pirate;
			}

			throw new Exception("Could not enter BBIO1 mode");
		}

		public static BusPirate OpenSPI1(string PortName) {
			BusPirate Pirate = OpenBBIO1(PortName);
			Pirate.Write(Commands.EnterBinarySPI);
			if (Pirate.ReadString(4) != "SPI1")
				throw new Exception("Could not enter SPI");
			return Pirate;
		}
	}

	public class BusPirateSPI1 {
		BusPirate Pirate;

		public BusPirateSPI1(string PortName) {
			Pirate = BusPirate.OpenSPI1(PortName);
		}
	}
}
