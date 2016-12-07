using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

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

		public Action<TelnetClient> OnConnected;
		public event Action<TelnetClient, string, string> OnAliasChanged;
		public event Action<string> OnWrite;

		public TelnetServer(int Port = 23) {
			this.Port = Port;
			Clients = new List<TelnetClient>();
			ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void RaiseOnAliasChanged(TelnetClient C, string Old, string New) {
			OnAliasChanged?.Invoke(C, Old, New);
		}

		public void Run() {
			IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, Port);
			ServerSocket.Bind(EndPoint);
			ServerSocket.Listen(0);

			Running = true;
			while (Running) {
				Socket ClientSocket = ServerSocket.Accept();
				TelnetClient Client = new TelnetClient(this, ClientSocket, DateTime.Now);
				lock (Clients)
					Clients.Add(Client);

				// TODO: Better way
				Thread ClientThread = new Thread((C) => {
					OnConnected?.Invoke((TelnetClient)C);
					lock (Clients)
						Clients.Remove((TelnetClient)C);
				});
				ClientThread.IsBackground = true;
				ClientThread.Start(Client);
			}
		}

		public Thread RunAsync() {
			Thread T = new Thread(Run);
			T.IsBackground = true;
			T.Start();
			return T;
		}

		public void Close() {
			Running = false;
		}

		public void Write(char C) {
			OnWrite?.Invoke(C.ToString());
			lock (Clients)
				foreach (var Cl in Clients)
					Cl.Write(C);
		}

		public void Write(string Str) {
			OnWrite?.Invoke(Str);
			lock (Clients)
				foreach (var Cl in Clients)
					Cl.Write(Str);
		}

		public void WriteLine(string Str) {
			OnWrite?.Invoke(Str + "\n");
			lock (Clients)
				foreach (var Cl in Clients)
					Cl.WriteLine(Str);
		}

		public void WriteLine(string Fmt, params object[] Args) {
			WriteLine(string.Format(Fmt, Args));
		}

		public void InsertLine(string Str) {
			OnWrite?.Invoke(Str + "\n");
			lock (Clients)
				foreach (var Cl in Clients)
					Cl.InsertLine(Str);
		}

		public void InsertLine(string Fmt, params object[] Args) {
			InsertLine(string.Format(Fmt, Args));
		}

		public void Disconnect(TelnetClient TC) {
			lock (Clients)
				if (Clients.Contains(TC)) {
					Clients.Remove(TC);
					TC.ClientSocket.Disconnect(false);
					TC.ClientSocket.Close();
				}
		}

		public Action<TelnetClient> CreateCommandLine() {
			return new Action<TelnetClient>((C) => {
				ProcessStartInfo PSI = new ProcessStartInfo("cmd");
				PSI.RedirectStandardError = true;
				PSI.RedirectStandardInput = true;
				PSI.RedirectStandardOutput = true;
				PSI.UseShellExecute = false;
				PSI.CreateNoWindow = true;

				Process P = new Process();
				P.StartInfo = PSI;
				P.EnableRaisingEvents = true;
				P.Start();

				Thread OutputThread = new Thread(() => {
					while (!P.HasExited && Running)
						C.Write((char)P.StandardOutput.Read());
				});

				Thread ErrorThread = new Thread(() => {
					while (!P.HasExited && Running)
						C.Write((char)P.StandardError.Read());
				});

				OutputThread.IsBackground = true;
				OutputThread.Start();
				ErrorThread.IsBackground = true;
				ErrorThread.Start();

				while (!P.HasExited && Running) {
					string InLine = C.ReadLine("", true);
					C.Delete(InLine);

					P.StandardInput.WriteLine(InLine);
					P.StandardInput.Flush();
				}

				if (!P.HasExited)
					P.Kill();
				C.Disconnect();
			});
		}
	}
}
