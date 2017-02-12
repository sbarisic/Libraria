#pragma once

#define GLEW_STATIC
#pragma comment(lib, "glew32s.lib")
#include <GL\glew.h>

#pragma comment(lib, "opengl32.lib")
#pragma comment(lib, "glfw3.lib")
#include <GLFW\glfw3.h>

#define NANOVG_EXPORT __declspec(dllexport)