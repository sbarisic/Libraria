using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libraria.Text {
	public static class StringUtils {
		public static int Levenshtein(this string A, string B) {
			int N = A.Length;
			int M = B.Length;
			int[,] D = new int[N + 1, M + 1];

			if (N == 0)
				return M;

			if (M == 0)
				return N;

			for (int i = 0; i <= N; D[i, 0] = i++)
				;

			for (int j = 0; j <= M; D[0, j] = j++)
				;

			for (int i = 1; i <= N; i++) {
				for (int j = 1; j <= M; j++) {
					int Cost = (B[j - 1] == A[i - 1]) ? 0 : 1;
					D[i, j] = Math.Min(Math.Min(D[i - 1, j] + 1, D[i, j - 1] + 1), D[i - 1, j - 1] + Cost);
				}
			}

			return D[N, M];
		}

		public static IEnumerable<string> OrderByLevenshtein(this IEnumerable<string> Strings, string CompareTo) {
			return Strings.OrderBy((K) => K.Levenshtein(CompareTo));
		}

		public static IEnumerable<string> GetClosest(this IEnumerable<string> Strings, string S, int MaxDist) {
			foreach (var Str in Strings)
				if (Str.Levenshtein(S) <= MaxDist)
					yield return Str;
		}
	}
}
