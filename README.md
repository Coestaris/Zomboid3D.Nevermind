# Nevermind
Simple Pascal-Python-C-Like scripting language.
Developing it just for fun...
May be used in a future game.

### Startup:
```bash
git clone https://github.com/Coestaris/Zomboid3D.Nevermind
cd Zomboid3D.Nevermind
```

#### To Run compiler
```
sudo apt install -y mono-complete
cd Compiler
xbuild NmCompiler.sln
cd NmCompilerTests/bin/Debug/
mono ./NmCompilerTests.exe -td <inputSourceFile> -o <outputBinaryFile>
```
You can get list of all possible options by running:
```
mono ./NmCompilerTests.exe --help
```
Put following to ~/.bashrc file to use compiler shortcut:
```
alias nmc="mono $(readlink -f ./NmCompiler.exe)"
```

#### To Run Runner
```
sudo apt install -y gcc libc-dev-bin cmake make
cd Runner
cmake CMakeLists.txtmake
./NmRunner <NM Binary FileName>
```
