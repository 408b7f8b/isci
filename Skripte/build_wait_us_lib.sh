arch=$(uname -m)

echo "$arch"

g++ -c -fPIC ../Helfer/wait_us.cpp -o ../wait_us.o
ar rcs ../lib_wait_us.a ../wait_us.o
mv ../lib_wait_us.a "../lib_wait_us_$arch.a"
rm ../wait_us.o