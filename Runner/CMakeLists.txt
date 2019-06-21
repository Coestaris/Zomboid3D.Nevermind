cmake_minimum_required(VERSION 3.10)
project(NmRunner C)

set(CMAKE_C_STANDARD 99)

add_executable(NmRunner main.c lib/parser.c lib/parser.h lib/nmError.c lib/nmError.h)