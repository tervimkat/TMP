
#ifndef PCH_H
#define PCH_H
#ifdef LIBRARY_EXPORTS
#    define LIBRARY_API __declspec(dllexport)
#else
#    define LIBRARY_API __declspec(dllimport)
#endif
#include <ctime>
#include <cmath>
// при импорте указываем, что будет передана структура и метод по правилам имеинования C
extern "C" struct LIBRARY_API  MyStruct
{
	time_t time;
	double value;
};
extern "C" LIBRARY_API  MyStruct getStruct();
// Добавьте сюда заголовочные файлы для предварительной компиляции
#include "framework.h"

#endif //PCH_H
