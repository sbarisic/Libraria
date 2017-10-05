#include "stdafx.h"

#include <cstdio>
#include <typeinfo>

#include "NativeTest.h"

#line 15 "predefined C++ types (compiler internal)"

NATIVETEST_API class A {
public:
	int SomeInt;
	int SomeOtherInt;

	virtual	void FuncA() { P("Func A"); }
	virtual	void FuncB() { P("Func B"); }
};

NATIVETEST_API class B : public A {
public:
	virtual void PrintTypeInfo(type_info* T) {
		printf("%s\n", T->name());
	}
};

NATIVETEST_API class Test : public  B {
public:
	virtual void SetOtherInt(int I) { P("Setting SomeOtherInt"); this->SomeOtherInt = I; }

	virtual void FuncE(type_info* inf) {
		printf("%s\n", "FuncE\n");
		printf("In FuncE, typeid(this).name() = %s\n", typeid(this).name());

		A* _A = this;
		void** Ptr = (void**)(&_A);
		B* Wat = (B*)(*Ptr);


		printf("%s\n", typeid(*Wat).name());

		//printf("%s\n", inf->name());
		//void* A = dynamic_cast<Test*>(this);
		//throw std::exception("Idiot");
	}
};

NATIVETEST_API Test WAT1;

extern "C" {
	NATIVETEST_API Test* GetPointer() {
		return new Test();
	}
}