#ifdef NATIVETEST_EXPORTS
#define NATIVETEST_API __declspec(dllexport)
#else
#define NATIVETEST_API __declspec(dllimport)
#endif

#define P(Arg) printf("%s\n", Arg)