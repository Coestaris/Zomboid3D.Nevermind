# CMAKE generated file: DO NOT EDIT!
# Generated by "Unix Makefiles" Generator, CMake Version 3.10

# Delete rule output on recipe failure.
.DELETE_ON_ERROR:


#=============================================================================
# Special targets provided by cmake.

# Disable implicit rules so canonical targets will work.
.SUFFIXES:


# Remove some rules from gmake that .SUFFIXES does not remove.
SUFFIXES =

.SUFFIXES: .hpux_make_needs_suffix_list


# Suppress display of executed commands.
$(VERBOSE).SILENT:


# A target that is always out of date.
cmake_force:

.PHONY : cmake_force

#=============================================================================
# Set environment variables for the build.

# The shell in which to execute make rules.
SHELL = /bin/sh

# The CMake executable.
CMAKE_COMMAND = /usr/bin/cmake

# The command to remove a file.
RM = /usr/bin/cmake -E remove -f

# Escaping for special characters.
EQUALS = =

# The top-level source directory on which CMake was run.
CMAKE_SOURCE_DIR = /home/maxim/Coding/Zomboid3D.Nevermind/Runner

# The top-level build directory on which CMake was run.
CMAKE_BINARY_DIR = /home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug

# Include any dependencies generated for this target.
include CMakeFiles/Runner.dir/depend.make

# Include the progress variables for this target.
include CMakeFiles/Runner.dir/progress.make

# Include the compile flags for this target's objects.
include CMakeFiles/Runner.dir/flags.make

CMakeFiles/Runner.dir/main.c.o: CMakeFiles/Runner.dir/flags.make
CMakeFiles/Runner.dir/main.c.o: ../main.c
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --progress-dir=/home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_1) "Building C object CMakeFiles/Runner.dir/main.c.o"
	/usr/bin/cc $(C_DEFINES) $(C_INCLUDES) $(C_FLAGS) -o CMakeFiles/Runner.dir/main.c.o   -c /home/maxim/Coding/Zomboid3D.Nevermind/Runner/main.c

CMakeFiles/Runner.dir/main.c.i: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Preprocessing C source to CMakeFiles/Runner.dir/main.c.i"
	/usr/bin/cc $(C_DEFINES) $(C_INCLUDES) $(C_FLAGS) -E /home/maxim/Coding/Zomboid3D.Nevermind/Runner/main.c > CMakeFiles/Runner.dir/main.c.i

CMakeFiles/Runner.dir/main.c.s: cmake_force
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green "Compiling C source to assembly CMakeFiles/Runner.dir/main.c.s"
	/usr/bin/cc $(C_DEFINES) $(C_INCLUDES) $(C_FLAGS) -S /home/maxim/Coding/Zomboid3D.Nevermind/Runner/main.c -o CMakeFiles/Runner.dir/main.c.s

CMakeFiles/Runner.dir/main.c.o.requires:

.PHONY : CMakeFiles/Runner.dir/main.c.o.requires

CMakeFiles/Runner.dir/main.c.o.provides: CMakeFiles/Runner.dir/main.c.o.requires
	$(MAKE) -f CMakeFiles/Runner.dir/build.make CMakeFiles/Runner.dir/main.c.o.provides.build
.PHONY : CMakeFiles/Runner.dir/main.c.o.provides

CMakeFiles/Runner.dir/main.c.o.provides.build: CMakeFiles/Runner.dir/main.c.o


# Object files for target Runner
Runner_OBJECTS = \
"CMakeFiles/Runner.dir/main.c.o"

# External object files for target Runner
Runner_EXTERNAL_OBJECTS =

Runner: CMakeFiles/Runner.dir/main.c.o
Runner: CMakeFiles/Runner.dir/build.make
Runner: CMakeFiles/Runner.dir/link.txt
	@$(CMAKE_COMMAND) -E cmake_echo_color --switch=$(COLOR) --green --bold --progress-dir=/home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug/CMakeFiles --progress-num=$(CMAKE_PROGRESS_2) "Linking C executable Runner"
	$(CMAKE_COMMAND) -E cmake_link_script CMakeFiles/Runner.dir/link.txt --verbose=$(VERBOSE)

# Rule to build all files generated by this target.
CMakeFiles/Runner.dir/build: Runner

.PHONY : CMakeFiles/Runner.dir/build

CMakeFiles/Runner.dir/requires: CMakeFiles/Runner.dir/main.c.o.requires

.PHONY : CMakeFiles/Runner.dir/requires

CMakeFiles/Runner.dir/clean:
	$(CMAKE_COMMAND) -P CMakeFiles/Runner.dir/cmake_clean.cmake
.PHONY : CMakeFiles/Runner.dir/clean

CMakeFiles/Runner.dir/depend:
	cd /home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug && $(CMAKE_COMMAND) -E cmake_depends "Unix Makefiles" /home/maxim/Coding/Zomboid3D.Nevermind/Runner /home/maxim/Coding/Zomboid3D.Nevermind/Runner /home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug /home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug /home/maxim/Coding/Zomboid3D.Nevermind/Runner/cmake-build-debug/CMakeFiles/Runner.dir/DependInfo.cmake --color=$(COLOR)
.PHONY : CMakeFiles/Runner.dir/depend

