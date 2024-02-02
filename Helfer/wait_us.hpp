#ifndef WAIT_US_HPP
#define WAIT_US_HPP

#ifdef _WIN32
extern "C" __declspec(dllexport) void SleepForMicroseconds(unsigned int microseconds);
#elif __linux__
extern "C" void SleepForMicroseconds(unsigned int microseconds);
#else
extern "C" void SleepForMicroseconds(unsigned int microseconds);
#endif

#endif // WAIT_US_HPP