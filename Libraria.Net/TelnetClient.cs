using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Libraria.Net {
	public class TelnetClient {
		public static bool IsCtrlChar(byte B) {
			return (B & 128) != 0;
		}

		/*public static byte ToCtrlChar(byte B) {
			return (byte)(B & 127);
		}*/

		public string RemoteEndPoint { get; private set; }
		public string Alias
		{
			get
			{
				return AliasInternal;
			}
			set
			{
				if (AliasInternal != null)
					Server.RaiseOnAliasChanged(this, AliasInternal, value);
				AliasInternal = value;
			}
		}
		public Socket ClientSocket;
		public DateTime ConnectedAt;
		public string Prompt;
		public bool EscapeSequences;
		public object Userdata;

		public event Action<char> OnCharReceived;

		string AliasInternal;
		TelnetServer Server;
		NetworkStream ClientStream;
		StreamWriter Out;
		StreamReader In;
		StringBuilder CurrentInput;
		bool Reading;

		bool Echo;

		public TelnetClient(TelnetServer Server, Socket ClientSocket, DateTime ConnectedAt) {
			this.Server = Server;
			this.ConnectedAt = ConnectedAt;
			this.ClientSocket = ClientSocket;
			ClientStream = new NetworkStream(ClientSocket);
			Out = new StreamWriter(ClientStream);
			In = new StreamReader(ClientStream);
			CurrentInput = new StringBuilder();
			Alias = RemoteEndPoint = ((IPEndPoint)ClientSocket.RemoteEndPoint).ToString();

			Echo = true;
			EscapeSequences = true;
			Send(TelnetCommand.IAC, Echo ? TelnetCommand.WILL : TelnetCommand.WONT, TelnetCommand.Echo);
			Send(TelnetCommand.IAC, TelnetCommand.WILL, TelnetCommand.SuppressGoAhead);
		}

		public override string ToString() {
			return Alias;
		}

		public void Disconnect() {
			Server.Disconnect(this);
		}

		public byte ReceiveRaw() {
			return (byte)ClientStream.ReadByte();
		}

		public byte Receive() {
			byte In = ReceiveRaw();

			if (IsCtrlChar(In) && In == 255) {
				TelnetCommand Cmd = (TelnetCommand)ReceiveRaw();
				TelnetCommand Option = (TelnetCommand)ReceiveRaw();
				bool Unimplemented = false;

				if (Cmd == TelnetCommand.IAC && Option == TelnetCommand.IAC)
					throw new Exception("Invalid control code");

				if (Option == TelnetCommand.SuppressGoAhead) {
					Send(TelnetCommand.IAC, TelnetCommand.WILL, TelnetCommand.SuppressGoAhead);
				} else if (Option == TelnetCommand.Echo) {
					if (Cmd == TelnetCommand.DO)
						Echo = true;
					else if (Cmd == TelnetCommand.DONT)
						Echo = false;
					else
						Unimplemented = true;

					Send(TelnetCommand.IAC, Echo ? TelnetCommand.WILL : TelnetCommand.WONT, TelnetCommand.Echo);
				} else
					Unimplemented = true;

				if (Unimplemented) {
					Console.WriteLine("Unsupported: IAC {0} {1}", Cmd, Option);
				}
				return Receive();
			}

			if (In == 127)
				In = (byte)'\b';
			return In;
		}

		public void Send(params object[] Args) {
			for (int i = 0; i < Args.Length; i++)
				if (Args[i] is byte) {
					ClientStream.WriteByte((byte)Args[i]);
				} else if (Args[i] is TelnetCommand) {
					ClientStream.WriteByte((byte)(TelnetCommand)Args[i]);
				} else
					throw new Exception("Cannot send " + Args[i].GetType());
		}

		public Thread StartDispatchingReceivedInput() {
			Thread T = new Thread(() => {
				try {
					while (true) {
						char C = Read();
						OnCharReceived?.Invoke(C);
					}
				} catch (Exception) {
				}
			});
			T.IsBackground = true;
			T.Start();
			return T;
		}

		public char Read() {
			char C = (char)Receive();
			if (Echo && C != '\r' && C != '\n' && C != '\0') {
				if (C != '\b')
					Write(C);
			}
			return C;
		}

		public string ReadLine(string Prompt = "", bool SuppressNewLine = false) {
			CurrentInput.Length = 0;
			char In;

			Reading = true;
			this.Prompt = Prompt;
			if (Prompt.Length > 0)
				Write(Prompt);

			while ((In = Read()) != '\0' && In != '\n') {
				if (In == '\b') {
					if (CurrentInput.Length > 0) {
						CurrentInput.Length--;
						Write("\b \b");
					}
				} else
					CurrentInput.Append(In);
			}

			if (!SuppressNewLine)
				Write("\r\n");

			string Ret = CurrentInput.ToString().TrimEnd('\r', '\n');
			CurrentInput.Length = 0;
			Reading = false;
			return Ret;
		}

		public void Delete(string Str) {
			for (int i = 0; i < Str.Length; i++)
				Write("\b \b");
		}

		bool _DisableWrite;
		public void Write(char C) {
			lock (this) {
				if (!EscapeSequences && C == (char)0x1B)
					_DisableWrite = true;
				if (!_DisableWrite || EscapeSequences)
					ClientStream.WriteByte((byte)C);
				if (!EscapeSequences && C == 'm')
					_DisableWrite = false;
			}
		}

		public void Write(string Str) {
			lock (this)
				for (int i = 0; i < Str.Length; i++)
					Write(Str[i]);
		}

		public void Write(string Fmt, params object[] Args) {
			Write(string.Format(Fmt, Args));
		}

		public void WriteLine(string Str) {
			Write(Str + "\r\n");
		}

		public void WriteLine(string Fmt, params object[] Args) {
			WriteLine(string.Format(Fmt, Args));
		}

		public void InsertLine(string Str) {
			lock (this) {
				Write(new string(' ', (Prompt == null ? "" : Prompt).Length + CurrentInput.Length));
				WriteLine("\r" + Str);
				if (Reading) {
					Write("\r" + Prompt);
					Write(CurrentInput.ToString());
				}
			}
		}

		public void InsertLine(string Fmt, params object[] Args) {
			InsertLine(string.Format(Fmt, Args));
		}
	}
}