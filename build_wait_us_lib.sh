g++ -c -fPIC Helfer/wait_us.cpp -o Helfer/wait_us.o
g++ -shared -o Helfer/lib_wait_us.so Helfer/wait_us.o