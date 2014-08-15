using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libraria;

namespace LibTests {
	class DoubleFloatTests {

		double GetDouble() {
			return 1;
		}

		float GetFloat() {
			return 1;
		}

		void TakeFloat(float F) {
			F = F + 1;
		}

		void TakeDouble(double D) {
			D = D + 1;
		}

		public DoubleFloatTests() {
			TakeFloat(GetFloat());
			TakeDouble(GetFloat());
			TakeFloat(GetDouble().f());
			TakeDouble(GetDouble());
		}
	}
}
