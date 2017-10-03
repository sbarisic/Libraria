#ifdef NATIVETEST_EXPORTS
#define NATIVETEST_API __declspec(dllexport)
#else
#define NATIVETEST_API __declspec(dllimport)
#endif

#define P(Arg) printf("%s\n", Arg)
//#define DEF_FUNC(Name) virtual void Name() override { P(#Name); }

#define DEF_FUNC(N) virtual void N ## _A() { P(#N "_A"); } /*virtual void N ## _B() { P(#N "_B"); }*/

#define DEF_CLASS(C) class C { public: DEF_FUNC(C) };
/*
DEF_CLASS(C)
DEF_CLASS(E)
DEF_CLASS(F)
DEF_CLASS(H)
DEF_CLASS(I)
DEF_CLASS(J)
DEF_CLASS(K)

class A : public I, J {
public:
	DEF_FUNC(A)
};

class G : public K {
public:
	DEF_FUNC(G)
};

class B : public A, E, G {
public:
	DEF_FUNC(B)
};

class D : public F, H {
public:
	DEF_FUNC(D)
};

class Test : public B, C, D
{
public:
	Test() {
	}

	DEF_FUNC(Test)
};*/

class A {
public:
	int SomeInt;
	int SomeOtherInt;

	virtual	void FuncA() { P("Func A"); }
	virtual	void FuncB() { P("Func B"); }
};

class B {
public:
	virtual void FuncC() { P("Func C"); }
};

class Test : public A, B {
public:
	virtual void SetOtherInt(int I) { P("Setting SomeOtherInt"); this->SomeOtherInt = I; }
	virtual void FuncE() { P("Func E"); }
};