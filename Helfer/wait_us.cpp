#include "wait_us.hpp"
#include <chrono>
#include <thread>

#ifdef _WIN32
extern "C" __declspec(dllexport) void WaitForMicroseconds(unsigned int microseconds)
{
    std::this_thread::sleep_for(std::chrono::microseconds(microseconds));
}
#elif __linux__
extern "C" void WaitForMicroseconds(unsigned int microseconds)
{
    std::this_thread::sleep_for(std::chrono::microseconds(microseconds));
}
#else
extern "C" void WaitForMicroseconds(unsigned int microseconds)
{
    std::this_thread::sleep_for(std::chrono::microseconds(microseconds));
} 
#endif