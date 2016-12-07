using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Libraria.IO {
	public class Tube {
		public event Action<Tube, Tube> ConnectedTo;
		public event Action<Tube, byte> ByteReceived;

		Tube Endpoint;

		public Tube() {

		}

		internal virtual void ConnectTo(Tube Other) {
			if (Endpoint != null)
				throw new InvalidOperationException("Tube already connected");
			Endpoint = Other;
			ConnectedTo?.Invoke(this, Other);
		}

		public void Connect(Tube Other) {
			Other.ConnectTo(this);
			ConnectTo(Other);
		}

		void ReadByte(byte B) {
			ByteReceived?.Invoke(this, B);
		}

		public void WriteByte(byte B) {
			Endpoint?.ReadByte(B);
		}
	}
}