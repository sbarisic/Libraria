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
