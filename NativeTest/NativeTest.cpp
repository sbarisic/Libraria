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

class Mammal : public Animal {
public:
	virtual void Breathe() {
		printf("Mammal breathing\n");
	}
};

class WingedAnimal {
public:
	virtual void Flap() {
		printf("WingedAnimal flapping\n");
	}
};

class Test : public Mammal, public WingedAnimal {
public:
	virtual void DoBatShit() {
		printf("Bat shit\n");
	}
};

extern "C" {
	NATIVETEST_API Test* GetPointer() {
		return new Test();
	}
}