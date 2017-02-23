using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Reflection;

namespace LibTech.Networking {
	public static class NetworkConfig {
		public const string AppName = "LibTech";
		public static int Port = 42690;
	}

	public class NetworkedObject {
		public int ID;
		public object Obj;
		public ObjectNetworkedProperties Properties;

		HashSet<PropertyInfo> DirtyProps;

		public NetworkedObject(object Obj, int ID, ObjectNetworkedProperties Properties) {
			DirtyProps = new HashSet<PropertyInfo>();
			this.Properties = Properties;
			this.Obj = Obj;
			this.ID = ID;
		}

		public void MarkDirty(PropertyInfo Prop) {
			if (!DirtyProps.Contains(Prop))
				DirtyProps.Add(Prop);
		}

		public PropertyInfo[] GetDirty() {
			PropertyInfo[] Dirty = DirtyProps.ToArray();
			DirtyProps.Clear();
			return Dirty;
		}

		public PropertyInfo[] GetAll() {
			return Properties.Properties.ToArray();
		}
	}

	public enum NetMessageType : int {
		Unknown = 0,
		ObjectCreate,
		ObjectUpdate,
		ObjectRemove,
		ObjectRemoveAll,

		RequestFullObjectUpdate = 8192,
	}

	public class NetworkConnection {
		public static void SetOverrideServer(object Origin, object Value, PropertyOverride Override) {
			Server.ServerInstance.SetOverride(Origin, Value, Override);
		}

		public static void SetOverrideClient(object Origin, object Value, PropertyOverride Override) {
			Client.ClientInstance.SetOverride(Origin, Value, Override);
		}

		protected NetPeer Peer;
		protected Dictionary<object, NetworkedObject> NetworkedObjects;

		public NetworkConnection() {
			NetworkedObjects = new Dictionary<object, NetworkedObject>();
		}

		public virtual void Update() {
			NetIncomingMessage Msg;

			while ((Msg = Peer.ReadMessage()) != null)
				HandleMessage(Msg);
		}

		public object GetObject(int ID) {
			foreach (var O in NetworkedObjects)
				if (O.Value.ID == ID)
					return O.Value.Obj;
			return null;
		}

		protected void Add(object Obj, int ID) {
			if (NetworkedObjects.ContainsKey(Obj))
				throw new InvalidOperationException("Could not add an object which already exists");

			ObjectNetworkedProperties Props;
			if (this is Server)
				Props = PropertyHooks.Hook(Obj, SetOverrideServer);
			else
				Props = PropertyHooks.Hook(Obj, SetOverrideClient);

			NetworkedObjects.Add(Obj, new NetworkedObject(Obj, ID, Props));
		}

		protected void Remove(object Obj) {
			if (!NetworkedObjects.ContainsKey(Obj))
				throw new InvalidOperationException("Could not remove an object which does not exist");

			NetworkedObjects.Remove(Obj);
			PropertyHooks.Unhook(Obj);
		}

		protected void SetOverride(object Origin, object Value, PropertyOverride Override) {
			NetworkedObjects[Origin].MarkDirty(Override.Property);
			Override.Set(Value);
		}

		protected NetOutgoingMessage CreateMessage(NetMessageType Type) {
			NetOutgoingMessage Msg = Peer.CreateMessage();
			Msg.Write((int)Type);
			return Msg;
		}

		protected virtual void HandleData(NetMessageType MessageType, NetIncomingMessage Msg) {
		}

		protected virtual void HandleMessage(NetIncomingMessage Msg) {
			if (Msg.MessageType == NetIncomingMessageType.Data)
				HandleData((NetMessageType)Msg.ReadInt32(), Msg);
		}
	}
}
