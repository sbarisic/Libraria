#ifdef __cplusplus
extern "C" {
#endif

	NANOVG_EXPORT int InitOpenGL() {
		if (!glfwInit())
			return 0;
		if (glewInit() != GLEW_OK)
			return 0;

		return 1;
	}

#ifdef __cplusplus
}
#endif