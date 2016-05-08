#include "stdafx.h"
#include <intrin.h>

const char* TestFunc(int A, char B, const char* C) {
	return C;
}

const char* WatWat() {
	return "WatWat";
}

void write(const char* Str, int Len) {
	/*__asm {
		mov eax, 4;
		mov ebx, 1;
		mov ecx, Str;
		mov edx, Len;
		int 0x80;
	}*/
}

void NativeTest() {
	char Str[] = "Hello linux world!";
	write(Str, sizeof(Str));
}