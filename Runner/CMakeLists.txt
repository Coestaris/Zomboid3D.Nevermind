cmake_minimum_required(VERSION 3.10)
project(NmRunner C)

set(CMAKE_C_STANDARD 99)

add_executable(NmRunner main.c lib/parser.c lib/parser.h lib/nmError.c lib/nmError.h lib/runtime/instructions.c lib/program.c lib/coretypes.h lib/runtime/environment.c lib/runtime/environment.h lib/runtime/stack.c lib/runtime/stack.h lib/crc32.c lib/crc32.h lib/runtime/instructionsMacro.h lib/runtime/subroutines.c lib/runtime/subroutines.h lib/runtime/defaultSubroutines.c lib/runtime/defaultSubroutines.h)

target_link_libraries(NmRunner m)