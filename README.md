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
```bash
sudo apt install -y mono-complete
cd Compiler
xbuild NmCompiler.sln
cd NmCompiler/bin/Debug/
mono ./NmCompiler.exe -td <inputSourceFile> -o <outputBinaryFile>
```
You can get list of all possible options by running:
```bash
mono ./NmCompiler.exe --help
```
Put following to ~/.bashrc file to use compiler shortcut:
```bash
alias nmc="mono $(readlink -f ./NmCompiler.exe)"
```

#### To Run Runner
```bash
sudo apt install -y gcc libc-dev-bin cmake make
cd Runner
cmake CMakeLists.txtmake
./NmRunner <NM Binary FileName>
```
