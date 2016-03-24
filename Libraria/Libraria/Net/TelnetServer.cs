using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Libraria.Net {
	public enum TelnetCommand : byte {
		BRK = 243, IP = 244, AO = 245, AYT = 246, EC = 247, EL = 248, GA = 249, SB = 250,
		WILL = 251, WONT = 252, DO = 253, DONT = 254, IAC = 255,

		SuppressGoAhead = 3,
		Status = 5,
		Echo = 1,
		TimingMark = 6,
		TerminalType = 24,
		WindowSize = 31,
		TerminalSpeed = 32,
		RemoteFlowControl = 33,
		LineMode = 34,
		EnvVars = 36,
	}

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

		public char Read() {
			char C = (char)Receive();
			if (Echo && C != '\r' && C != '\n' && C != '\0') {
				if (C != '\b')
					Write(C);
			}
			return C;
		}

		public string ReadLine(string Prompt, bool SuppressNewLine = false) {
			CurrentInput.Clear();
			char In;

			Reading = true;
			this.Prompt = Prompt;
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
			CurrentInput.Clear();
			Reading = false;
			return Ret;
		}

		bool _DisableWrite;
		public void Write(char C) {
			if (!EscapeSequences && C == (char)0x1B)
				_DisableWrite = true;
			if (!_DisableWrite || EscapeSequences)
				ClientStream.WriteByte((byte)C);
			if (!EscapeSequences && C == 'm')
				_DisableWrite = false;
		}

		public void Write(string Str) {
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
			Write(new string(' ', (Prompt == null ? "" : Prompt).Length + CurrentInput.Length));
			WriteLine("\r" + Str);
			if (Reading) {
				Write("\r" + Prompt);
				Write(CurrentInput.ToString());
			}
		}

		public void InsertLine(string Fmt, params object[] Args) {
			InsertLine(string.Format(Fmt, Args));
		}
	}

	public class TelnetServer {
		public List<TelnetClient> Clients;
		bool Running;
		Socket ServerSocket;
		int Port;

		public event Action<TelnetClient> OnConnected;
		public event Action<TelnetClient, string, string> OnAliasChanged;
		public event Action<string> OnWrite;

		public TelnetServer(int Port = 23) {
			this.Port = Port;
			Clients = new List<TelnetClient>();
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void RaiseOnAliasChanged(TelnetClient C, string Old, string New) {
			if (OnAliasChanged != null)
				OnAliasChanged(C, Old, New);
		}

		public void Run() {
			IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, Port);
			ServerSocket.Bind(EndPoint);
			ServerSocket.Listen(0);

			Running = true;
			while (Running) {
				Socket ClientSocket = ServerSocket.Accept();
				TelnetClient Client = new TelnetClient(this, ClientSocket, DateTime.Now);
				Clients.Add(Client);

				if (OnConnected != null)
					OnConnected(Client);
			}
		}

		public void Close() {
			Running = false;
		}

		public void Write(char C) {
			if (OnWrite != null)
				OnWrite(C.ToString());
			foreach (var Cl in Clients)
				Cl.Write(C);
		}

		public void Write(string Str) {
			if (OnWrite != null)
				OnWrite(Str);
			foreach (var Cl in Clients)
				Cl.Write(Str);
		}

		public void WriteLine(string Str) {
			if (OnWrite != null)
				OnWrite(Str + "\n");
			foreach (var Cl in Clients)
				Cl.WriteLine(Str);
		}

		public void WriteLine(string Fmt, params object[] Args) {
			WriteLine(string.Format(Fmt, Args));
		}

		public void InsertLine(string Str) {
			if (OnWrite != null)
				OnWrite(Str + "\n");
			foreach (var Cl in Clients)
				Cl.InsertLine(Str);
		}

		public void InsertLine(string Fmt, params object[] Args) {
			InsertLine(string.Format(Fmt, Args));
		}

		public void Disconnect(TelnetClient TC) {
			if (Clients.Contains(TC)) {
				Clients.Remove(TC);
				TC.ClientSocket.Disconnect(false);
				TC.ClientSocket.Dispose();
			}
		}
	}
}
