 #include "pch.h"

// метод передачи структуры
__declspec(dllexport)  MyStruct getStruct()
{
	MyStruct st;
	st.time = time(NULL);
	st.value = sin(st.time);
	return st;
}