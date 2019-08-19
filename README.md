# Nevermind
Simple Pascal-Python-C-Like scripting language.
Developing it just for fun...
May be used in a future game.

#### Startup:
```bash
git clone https://github.com/Coestaris/Zomboid3D.Nevermind
cd Zomboid3D.Nevermind
```

##### To Run compiler
```bash
sudo apt install -y mono-complete
cd Compiler
xbuild NmCompiler.sln
cd NmCompiler/bin/Debug/
mono ./NmCompiler.exe <inputSourceFile> -o <outputBinaryFile> -td
```
You can get list of all possible options by running:
```bash
mono ./NmCompiler.exe --help
```
Use the following command to put alias at bash config file to use compiler shortcut:
```bash
printf "alias nmc=\"mono $(readlink -f ./NmCompiler.exe)"\" > ~/bashrc
```

##### To Run Runner
```bash
sudo apt install -y gcc libc-dev-bin cmake make
cd Runner
cmake CMakeLists.txt
make
./NmRunner <NM Binary FileName>
```

### Language Examples
(some features still in development)

##### "Hello world"
```
import io;

entrypoint function main()
{
    print("Hello world!");
}
```

##### Fibonacci numbers
```
import io; 

//Declaring necesary global variables
var a : integer;
var b : integer;
var c : integer;

//Reseting all global variables
function resetValues()
{
   a = 0;
   b = 1;
   c = 0;
}

entrypoint function main()
{
    resetValues();    
    while(a <= 1000)
    {
       println(a);
       
       a = b + c;
       c = b;
       b = a;
    }
}
```

##### Complex expressions
```
import io;
import math;

function real sqr(a : real)
{
    return a * a;
}

function real inc(a : real)
{
    return a + 1;
}

entrypoint function main()
{
    const a : real = 5.2;
    const b : real = 6.1;
    
    printlf("Result is: %f", 0.5);
    printlf("Result is: %f", 41);   //impicit casting to float
    printlf("Result is: %f", -0.1); //unary operations
    printlf("Result is: %f", 9 * 1 - !a * b); //complex expressions
    printlf("Result is: %f", inc(a) + sqr(b)); //calling functions
    printlf("Result is: %f", pow(a, b)); //calling library functions
}
```

##### Simple fractal painting
```
import graph;
import math;

const PI     : real = 3.141;

const width  : integer = 400;
const height : integer = 300;

const startDepth   : integer =  10;
const startAngle   : real = PI / 2.0;
const branchLength : real = 10;

function draw(x : real, y : real, angle : real, depth : integer)
{
   if(depth == 0)
      return;
   
   //this value can be precalculated  
   var k : real = branchLength / (startDepth - depth);
   
   var x1 : real = x + cos(angle + PI / 8.0) * k; 
   var y1 : real = y + sin(angle + PI / 8.0) * k; 
   var x2 : real = x + cos(angle - PI / 8.0) * k; 
   var y2 : real = x + sin(angle - PI / 8.0) * k;
   
   line(x, y, x1, y1);
   line(x, y, x2, y2);
   
   draw(x1, y1, angle + PI_8, depth - 1); 
   draw(x2, y2, angle - PI_8, depth - 1); 
}

function setup()
{ 
  initCanvas(width, height);
  lockDrawing();
}

entrypoint function main()
{
    setup();
    
    draw(width * 2.0, height * 2.0 / 3.0, startAngle, startDepth); 
    
    loop();
}
```

You can find more examples in "Examples" folder.
