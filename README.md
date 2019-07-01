# Nevermind
Simple Pascal-Python-C-Like scripting language.
Developing it just for fun...
May be used in a future game.

To run project:
```bash
git clone https://github.com/Coestaris/Zomboid3D.Nevermind
cd Zomboid3D.Nevermind

#To Run compiler
sudo apt install -y mono-complete
cd Compiler
xbuild NmCompiler.sln
cd NmCompilerTests/bin/Debug/
mono ./NmCompilerTests.exe -td <inputSourceFile> -o <outputBinaryFile>
#Use mono ./NmCompilerTests.exe --help for more detailed info

#To Run Runner
sudo apt install -y gcc libc-dev-bin cmake make
cd Runner
cmake CMakeLists.txt
make
./NmRunner <NM Binary FileName>
```

