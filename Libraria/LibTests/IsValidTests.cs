using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libraria;

namespace LibTests {
	class ClassA : IIsValid {
		public int A;

		public ClassA() {
			A = 0;
		}

		public bool IsValid() {
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

	class IsValidTests {
		public IsValidTests() {
			ClassA A = new ClassA();
			ClassB B = null;

			Test.Assert(A.IsValid() == false);
			A.A = 1;
			Test.Assert(A.IsValid() == true);
			Test.Assert(B.IsValid() == false);
			B = new ClassB();
			Test.Assert(B.IsValid());
		}
	}
}