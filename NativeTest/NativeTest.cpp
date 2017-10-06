#include "stdafx.h"

#include <cstdio>
#include <typeinfo>

#include "NativeTest.h"

#line 15 "predefined C++ types (compiler internal)"

class Animal {
public:
	virtual void Eat() {
		printf("Animal eating\n");
	}
};

class Mammal : public  Animal {
public:
	virtual void Breathe() {
		printf("Mammal breathing\n");
	}
};

class WingedAnimal : public  Animal {
public:
	virtual void Flap() {
		printf("WingedAnimal flapping\n");
	}
};

class Bat : public Mammal, public WingedAnimal {
};

extern "C" {
	NATIVETEST_API Bat* GetPointer() {
		return new Bat();
	}
}