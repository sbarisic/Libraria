using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

using Libraria.Net;

namespace LibTests {
	public class Program {
		static string ServerCol = "34m";

		static void Main(string[] args) {
			Console.Title = "LibTests";

			TelnetServer TS = new TelnetServer(6666);
			TS.OnConnected += (TC) => new Thread(() => OnClient(TC, TS)).Start();
			TS.OnAliasChanged += (TC, Old, New) => TS.InsertLine(TStamp() + Colored(ServerCol, "{0} changed alias to {1}"), Old, New);
			TS.OnWrite += (Str) => {
				try {
					while (Str.Contains((char)27)) {
						int EscStart = Str.IndexOf((char)27);
						Str = Str.Remove(EscStart, Str.IndexOf('m', EscStart) - EscStart + 1);
					}
					Str = Str.Replace("\a", "");
				} catch (Exception) {
				}
				Console.Write(Str);
			};
			Console.WriteLine("Starting telnet server");
			TS.Run();

			Console.WriteLine("Done!");
			Console.ReadLine();
		}

		static string Esc(string E) {
			return "\x1B[" + E;
		}

		static string Escd(string E, params string[] Escs) {
			foreach (var EE in Escs)
				E = Esc(EE) + E;
			return E + Esc("0m");
		}

		static string Colored(string C, string Str) {
			return Esc(C) + Esc("40m") + Str + Esc("0m");
		}

		static string TStamp() {
			return Escd("[" + DateTime.Now.ToString("HH:mm:ss") + "] ", "37m");
		}

		static void OnClient(TelnetClient Client, TelnetServer Server) {
			try {
				Server.InsertLine(TStamp() + Colored(ServerCol, "Client connected: {0}"), Client);

				string Alias;
			EnterAlias:
				Alias = Client.ReadLine("Username: ");
				if (Alias.Length == 0) {
					Client.InsertLine("Invalid username");
					goto EnterAlias;
				}
				foreach (var Cl in Server.Clients)
					if (Cl.Alias == Alias) {
						Client.InsertLine("Username '{0}' already taken, try again", Alias);
						goto EnterAlias;
					}
				Client.Alias = Alias;

				bool Running = true;
				while (Running) {
					string Input = Client.ReadLine(Escd(Client.ToString(), "1m", "32m") + Escd(" $ ", "1m", "32m"), true).Trim();
					if (Input.Length == 0) {
						Client.Write('\r');
						continue;
					}

					if (Input.StartsWith("/")) {
						string[] Cmd = Input.Substring(1).Split(' ');

						if (Cmd.Length > 0) {
							if (Cmd[0] == "help") {
								Client.InsertLine("List of commands: me, list, quit");
							} else if (Cmd[0] == "quit")
								Running = false;
							else if (Cmd[0] == "me" && Input.Contains(' ')) {
								Server.InsertLine(TStamp() + Escd("\a{0}{1}", "36m"), Client, Input.Substring(Input.IndexOf(' ')));
							} else if (Cmd[0] == "list") {
								Client.InsertLine("Online users:");
								foreach (var Cl in Server.Clients)
									Client.InsertLine("  " + Cl);
							} else
								Client.InsertLine(Escd("Unknown command '{0}'", "31m"), Cmd[0]);
						}
					} else
						Server.InsertLine(TStamp() + Escd("\a{0}: {1}", "36m"), Client, Input);
				}
			} catch (Exception) {
			}

			Client.Disconnect();
			Server.InsertLine(TStamp() + Colored(ServerCol, "Client disconnected: {0}"), Client);
		}
	}
}