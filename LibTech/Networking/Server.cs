using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Libraria.Serialization;
using Libraria;
using System.Reflection;

namespace LibTech.Networking {
	public class Server : NetworkConnection {
		internal static Server ServerInstance;

		int FreeID;
		NetServer ServerPeer;

		List<NetConnection> Clients;

		public Server() {
			ServerInstance = this;
			Clients = new List<NetConnection>();

			NetPeerConfiguration NetPeerConfig = new NetPeerConfiguration(NetworkConfig.AppName);
			NetPeerConfig.Port = NetworkConfig.Port;
			Peer = ServerPeer = new NetServer(NetPeerConfig);
			Peer.Start();
		}

		public T CreateNetworked<T>(T Obj) {
			Add(Obj, FreeID++);
			foreach (var C in Clients)
				SendCreateObject(C, NetworkedObjects[Obj]);
			return Obj;
		}

		public void RemoveNetworked<T>(T Obj) {
			Remove(Obj);
		}

		protected void SendFullObjectUpdate(NetConnection Client) {
			ServerPeer.SendMessage(CreateMessage(NetMessageType.ObjectRemoveAll), Client, NetDeliveryMethod.ReliableOrdered);

			foreach (var O in NetworkedObjects)
				SendCreateObject(Client, O.Value);
		}

		protected void SendCreateObject(NetConnection Client, NetworkedObject Obj) {
			NetOutgoingMessage Msg = CreateMessage(NetMessageType.ObjectCreate);
			Msg.Write(Obj.ID);
			Msg.Write(Obj.Obj.GetType().AssemblyQualifiedName);
			ServerPeer.SendMessage(Msg, Client, NetDeliveryMethod.ReliableOrdered);
			SendUpdateObject(Client, Obj);
		}

		protected void SendRemoveObject(NetConnection Client, NetworkedObject Obj) {
			NetOutgoingMessage Msg = CreateMessage(NetMessageType.ObjectRemove);
			Msg.Write(Obj.ID);
			ServerPeer.SendMessage(Msg, Client, NetDeliveryMethod.ReliableOrdered);
		}

		protected void SendUpdateObject(NetConnection Client, NetworkedObject Obj) {
			NetOutgoingMessage Msg = CreateMessage(NetMessageType.ObjectUpdate);
			PropertyInfo[] Props = Obj.GetAll();
			for (int i = 0; i < Props.Length; i++) {
				object Val = Props[i].GetValue(Obj.Obj);

				// TODO: Write
			}

			ServerPeer.SendMessage(Msg, Client, NetDeliveryMethod.ReliableOrdered);
		}

		protected override void HandleData(NetMessageType MessageType, NetIncomingMessage Msg) {
			if (MessageType == NetMessageType.RequestFullObjectUpdate)
				SendFullObjectUpdate(Msg.SenderConnection);
			else
				throw new InvalidOperationException("Unknown message type " + MessageType);
		}

		protected override void HandleMessage(NetIncomingMessage Msg) {
			switch (Msg.MessageType) {
				case NetIncomingMessageType.StatusChanged: {
						if (Msg.SenderConnection.Status == NetConnectionStatus.Connected) {
							// Client connected
							Clients.Add(Msg.SenderConnection);
						} else if (Msg.SenderConnection.Status == NetConnectionStatus.Disconnected) {
							// Client disconnected
							Clients.Remove(Msg.SenderConnection);
						}
						break;
					}
			}

			base.HandleMessage(Msg);
		}
	}
}
