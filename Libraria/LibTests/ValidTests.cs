using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libraria;

namespace LibTests {
	class ClassA : IValid {
		public int A;

		public ClassA() {
			A = 0;
		}

		public bool Valid() {
			if (A == 0)
				return false;
			return true;
		}
	}

	public class ClassB {
		public int A;

		public ClassB() {
			this.A = 0;
		}
	}

	class ValidTests {
		public ValidTests() {
			ClassA A = new ClassA();
			ClassB B = null;

			Test.Assert(A.Valid() == false);
			A.A = 1;
			Test.Assert(A.Valid() == true);
			Test.Assert(B.Valid() == false);
			B = new ClassB();
			Test.Assert(B.Valid());
		}
	}
}