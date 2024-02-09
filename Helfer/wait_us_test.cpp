#include <cstdlib>
#include <iomanip>
#include <iostream>
#include <chrono>
#include "wait_us.hpp"

int main(int argc, char *argv[])
{
    std::chrono::steady_clock::time_point begin = std::chrono::steady_clock::now();

    SetStandardSleepTime(10000);
    StandardSleep();
    std::chrono::steady_clock::time_point end = std::chrono::steady_clock::now();

    std::cout << "Time difference = " << std::chrono::duration_cast<std::chrono::microseconds>(end - begin).count() << "[µs]" << std::endl;
    std::cout << "Time difference = " << std::chrono::duration_cast<std::chrono::nanoseconds>(end - begin).count() << "[ns]" << std::endl;
    return 0;
}

//***C++11 Style:***
#include <chrono>



