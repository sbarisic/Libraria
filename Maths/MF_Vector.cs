using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Numerics;

namespace Libraria.Maths {
	public static partial class MathFuncs {
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
				Vector2 p = Points[i], p1;

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