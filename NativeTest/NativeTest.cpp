#include "stdafx.h"

#include <cstdio>

#include "NativeTest.h"

extern "C" {
	NATIVETEST_API Test* GetPointer() {
		return new Test();
	}
}