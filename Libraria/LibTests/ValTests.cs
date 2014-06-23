using Libraria;
using System;

namespace LibTests {
	class EmptyClass {
		public int Number = 56;

		public EmptyClass() {
			Number = 128;
		}

		public EmptyClass(int CustomNum) {
			this.Number = CustomNum;
		}
	}

	class ErrorClass {
		public int Number = 128;

		public ErrorClass() {
			Number = 256;
			throw new Exception("This class can not be initialized");
		}
	}

	class ValTests {

		public ValTests() {
			EmptyClass EmptyClass = null;
			EmptyClass NonEmptyClass = new LibTests.EmptyClass(69);
			ErrorClass ErrorClass = null;
			string NullString = null;
			string String = "12345";

			Test.Assert(EmptyClass.Val().Number == 128);
			Test.Assert(EmptyClass.Val(NonEmptyClass).Number == 69);
			Test.Assert(EmptyClass.Val(EmptyClass) == null);
			Test.Assert(ErrorClass.Val(false).Number == 0);
			Test.Assert(NullString.Val().Length == 0);
			Test.Assert(String.Val().Length == 5);
		}
	}
}