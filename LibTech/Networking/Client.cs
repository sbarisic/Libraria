using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace LibTech.Networking {
	public class Client : NetworkConnection {
		internal static Client ClientInstance;

		public bool Connected { get; private set; }

		NetClient ClientPeer;

		public Client() {
			ClientInstance = this;
			NetPeerConfiguration NetPeerConfig = new NetPeerConfiguration(NetworkConfig.AppName);
			Peer = ClientPeer = new NetClient(NetPeerConfig);
			Peer.Start();
		}

		public void Connect(string Host, int Port) {
			Peer.Connect(Host, Port);
		}

		public void Connect(string Host) {
			Connect(Host, NetworkConfig.Port);
		}

		protected override void HandleData(NetMessageType MessageType, NetIncomingMessage Msg) {
			if (MessageType == NetMessageType.ObjectRemoveAll)
				NetworkedObjects.Clear();
			else if (MessageType == NetMessageType.ObjectCreate) {
				int ID = Msg.ReadInt32();
				string TypeName = Msg.ReadString();
				Type T = Type.GetType(TypeName);
				object Obj = Activator.CreateInstance(T);
				Add(Obj, ID);
			} else
				throw new InvalidOperationException("Unknown message type " + MessageType);
		}

		protected override void HandleMessage(NetIncomingMessage Msg) {
			switch (Msg.MessageType) {
				case NetIncomingMessageType.Data: {
						break;
					}

				case NetIncomingMessageType.StatusChanged: {
						if (Msg.SenderConnection.Status == NetConnectionStatus.Connected) {
							// Connected to server
							Connected = true;

							NetOutgoingMessage NewMsg = ClientPeer.CreateMessage();
							NewMsg.Write((int)NetMessageType.RequestFullObjectUpdate);
							ClientPeer.SendMessage(NewMsg, NetDeliveryMethod.ReliableOrdered);
						} else if (Msg.SenderConnection.Status == NetConnectionStatus.Disconnected) {
							// Disconnected from server
							Connected = false;
						}
						break;
					}
			}

			base.HandleMessage(Msg);
		}
	}
}
