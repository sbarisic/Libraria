#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#define EXPORT __declspec(dllexport)

EXPORT const char* __cdecl TestFunc(int A, char B, const char* C);
EXPORT const char* __cdecl WatWat();