#!/bin/sh
nano -n 10 ./tracker 100 &
nano -n 10 ./example2 2 3 100 &
nano -n 10 ./example 1 2 100 &
nano -n 10 ./integration 0 1 3 100 192.168.0.10 &
