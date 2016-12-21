using System;
using System.Collections.Generic;
using System.Text;
using Vec = System.Numerics.Vector2;

namespace Libraria.Maths {
	public class RectanglePack {
		public class Spot {
			public Spot Insert(char ID, Vec Rect) {
				if (ChildA != null && ChildB != null) {
					Spot NewNode;
					if ((NewNode = ChildA.Insert(ID, Rect)) != null)
						return NewNode;
					return ChildB.Insert(ID, Rect);
				}

				if (Rect.X > Area.W || Rect.Y > Area.H)
					return null;

				ChildA = new Spot();
				ChildB = new Spot();

				int WidthDelta = (int)Area.W - (int)Rect.X;
				int HeightDelta = (int)Area.H - (int)Rect.Y;

				if (WidthDelta <= HeightDelta) {
					ChildA.Area.SetPos(Area.X + (int)Rect.X, Area.Y);
					ChildA.Area.SetSize(WidthDelta, (int)Rect.Y);
					ChildB.Area.SetPos(Area.X, Area.Y + (int)Rect.Y);
					ChildB.Area.SetSize(Area.W, HeightDelta);
				} else {
					ChildA.Area.SetPos(Area.X, Area.Y + (int)Rect.Y);
					ChildA.Area.SetSize((int)Rect.X, HeightDelta);
					ChildB.Area.SetPos(Area.X + (int)Rect.X, Area.Y);
					ChildB.Area.SetSize(WidthDelta, Area.H);
				}

				Area.SetSize((int)Rect.X, (int)Rect.Y);
				this.ID = ID;

				return this;
			}

			public Spot ChildA;
			public Spot ChildB;
			public Rect Area;
			public char ID;
		}

		Vec _Size;

		public Spot RootSpot;
		public bool AutoResize;
		public Vec Size
		{
			get
			{
				return _Size;
			}
			set
			{
				if (value.X <= 0 || value.Y <= 0)
					throw new Exception("Rectangle pack size must be higher than 0");

				if (RootSpot.ChildA != null && RootSpot.ChildB != null) {
					RectanglePack NewPack = new RectanglePack((int)value.X, (int)value.Y);

					Action<Spot> AddSpots = null;
					AddSpots = (S) => {
						if (S.ChildA == null && S.ChildB == null)
							return;
						NewPack.Add(S.ID, new Vec(S.Area.W, S.Area.H));
						AddSpots(S.ChildA);
						AddSpots(S.ChildB);
					};
					AddSpots(RootSpot);
					RootSpot = NewPack.RootSpot;
				} else
					RootSpot.Area.SetSize((int)value.X, (int)value.Y);

				_Size = value;
			}
		}

		public RectanglePack(int W, int H) {
			AutoResize = true;

			_Size = new Vec(W, H);
			RootSpot = new Spot();
			RootSpot.Area = new Rect(0, 0, W, H);
		}

		public RectanglePack()
			: this(16, 16) {
			AutoResize = true;
		}

		public bool Add(char ID, Vec V) {
			if (V.X == 0 || V.Y == 0)
				return true;
			if (RootSpot.Insert(ID, V) != null)
				return true;

			if (!AutoResize)
				return false;

			int WidthDelta = 0;
			int HeightDelta = 0;
			if (Size.X > Size.Y) {
				HeightDelta += (int)V.Y;
				if (V.X > Size.X)
					WidthDelta += (int)V.X;
			} else {
				WidthDelta += (int)V.X;
				if (V.Y > Size.Y)
					HeightDelta += (int)V.Y;
			}

			bool OldAutoResize = AutoResize;
			AutoResize = false;
			int MaxTries = 10;

			for (int Tries = 1; Tries < MaxTries; Tries++) {
				Size = new Vec(Size.X + WidthDelta * Tries, Size.Y + HeightDelta * Tries);
				if (Add(ID, V))
					break;
				else if (Tries >= MaxTries)
					throw new Exception("Can not fit!");
			}

			AutoResize = OldAutoResize;
			return true;
		}

		delegate void AddSpotsFunc(Spot S);

		public Dictionary<char, Rect> Pack() {
			Dictionary<char, Rect> Packed = new Dictionary<char, Rect>();

			AddSpotsFunc AddSpots = null;
			AddSpots = (S) => {
				if (S.ChildA == null && S.ChildB == null)
					return;
				Packed.Add(S.ID, S.Area);
				AddSpots(S.ChildA);
				AddSpots(S.ChildB);
			};
			AddSpots(RootSpot);

			return Packed;
		}
	}
}
