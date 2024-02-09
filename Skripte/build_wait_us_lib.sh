g++ -c -fPIC ../Helfer/wait_us.cpp -o ../Helfer/wait_us.o
g++ -shared -o ../Helfer/lib_wait_us.so ../Helfer/wait_us.o

g++ -c -fPIC ../Helfer/wait_us_test.cpp -o ../Helfer/wait_us_test.o
g++ -o ../Helfer/wait_us_test ../Helfer/wait_us_test.o ../Helfer/wait_us.o