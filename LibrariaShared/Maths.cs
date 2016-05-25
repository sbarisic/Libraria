using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Numerics;
using Point = System.Numerics.Vector2;

namespace Libraria {
	public static class Maths {
		public static int Loop(int Min, int Max, int Val) {
			int Range = Max - Min;
			while (Val < Min)
				Val += Range;
			while (Val > Max)
				Val -= Range;
			return Val;
		}

		public static float Angle(this Vector2 A, Vector2 B) {
			return (float)Math.Atan2(B.Y - A.Y, B.X - A.X);
		}

		public static float Cross(this Vector2 A, Vector2 B) {
			return (A.X * B.Y) - (A.Y * B.X);
		}

		public static Vector2 FindLeftBottomMostPoint(IEnumerable<Vector2> Points) {
			Vector2 LBMost = Points.First();

			foreach (var P in Points) {
				if ((P.X < LBMost.X) || (P.X == LBMost.X && P.Y < LBMost.Y))
					LBMost = P;
			}

			return LBMost;
		}

		public static IEnumerable<Vector2> ComputeConvexHull(IEnumerable<Vector2> Vertices) {
			List<Vector2> Points = new List<Vector2>(Vertices);
			Points.Sort((a, b) => a.X == b.X ? a.Y.CompareTo(b.Y) : (a.X > b.X ? 1 : -1));

			LinkedList<Vector2> Hull = new LinkedList<Vector2>();
			int L = 0, U = 0;

			for (int i = Points.Count - 1; i >= 0; i--) {
				Point p = Points[i], p1;

				while (L >= 2 && ((p1 = Hull.Last.Value) - Hull.Last.Previous.Value).Cross(p - p1) >= 0) {
					Hull.RemoveLast();
					L--;
				}

				Hull.AddLast(p);
				L++;

				while (U >= 2 && ((p1 = Hull.First.Value) - Hull.First.Next.Value).Cross(p - p1) <= 0) {
					Hull.RemoveFirst();
					U--;
				}

				if (U != 0)
					Hull.AddFirst(p);

				U++;
				//Debug.Assert(U + L == hull.Count + 1);
			}

			Hull.RemoveLast();
			return Hull;
		}
	}
}