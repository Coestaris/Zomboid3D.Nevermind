# .NMB File spec
#### File Structure:
  * **3 bytes**: "nmb" signature
  * **2 bytes**: Chunks count
  * **rest**   : Chunks

#### Chunk structure:
  * **4 bytes**: Length
  * **4 bytes**: CRC
  * **2 bytes**: Type
  * **n bytes**: Data

#### Chunks:
###### "HE" - Header Chunk (unique, required)
  * **2 bytes** : NM version
  * **16 bytes**: Source MD5 hash
  * **4 bytes** : Import count
  * **4 bytes** : Function count
  * **4 bytes** : Entry point function index
  <br>Import format:
     * **1 byte** : Module type (0 = sys library, 1 = import)
     * **4 bytes**: Module name length (filename)
     * **n bytes**: Module name

###### "ME" - Metadata Chunk (unique)
  * **8 bytes**: Compile date - time
     * **1 byte** : Second
     * **1 byte** : Minute
     * **1 byte** : Hour
     * **1 byte** : Day
     * **1 byte** : Nonth
     * **2 bytes**: Year
  * **2 bytes**: Bianry name length
  * **n bytes**: Binary name
  * **2 bytes**: Binary description length
  * **m bytes**: Binary description
  * **2 bytes**: Binary author length
  * **k bytes**: Binary author
  * **2 bytes**: Binary Minor verion
  * **2 bytes**: Binary Major verion

###### "GL" - Global variables (unique)
  * **4 bytes**: Globals count
  * **n * 4 bytes**: Globals types
  * **n * 4 bytes**: Globals initial values - constant index (-1 if not set)

###### "TY" - Type Chunk (unique, required)
  * **4 bytes**: Type count
  <br>Type format:
     * **2 bytes**: Type signature
     * **1 byte**:  Type base

###### "CO" - Constants Chunk (unique, required)
  * **4 bytes**: Constant count
   <br>Constant format:
     * **4 bytes**: Type index
     * **4 bytes**: Constant len
     * **len * type base bytes**: Value

###### "FU" - Function chunk (required)
  * **4 bytes**  : Function index
  * **4 bytes**  : Instructions count
  * **4 bytes**  : Locals count
  * **n * 4 bytes**: Local types
  * **n * 4 bytes**: Local initial values - constant index (-1 if not set)
  * **4 bytes**  : Reg count
  * **m * 4 bytes**: Reg types
  <br>Instruction format:
    * **2 bytes**: Instruction index
    * **4 * param count**: Parameters

###### "DE" - Debug information (unique)
  * **4 bytes**: Source filename length (if compiled from text = 0)
  * **n bytes**: Source filename
  <br>Globals info:
     * **4 bytes**: Global name length
     * **n bytes**: Global name
     * **4 bytes**: Global definition line index
     * **4 bytes**: Global definition char index
  <br>For every function in chunk order:
  * **4 bytes**: Function name length
  * **n bytes**: Function name
  * **4 bytes**: Function definition line index
  * **4 bytes**: Function definition char index
  <br>Locals info:
     * **4 bytes**: Local name length
     * **n bytes**: Local name
     * **4 bytes**: Local definition line index
     * **4 bytes**: Local definition char index

#### Syscall specification
todo
##### io

###### io_print_i (0x1)
###### io_print_f (0x2)
###### io_print_s (0x3)
###### io_print   (0x4)
###### io_fprint  (0x5)

##### sys

###### sys_get_time (0x100)
###### sys_get_gpc  (0x101)
###### sys_get_pc   (0x102)
###### sys_get_fc   (0x103)

##### math

###### m_acos  (0x200)
###### m_asin  (0x201)
###### m_atan  (0x202)
###### m_atan2 (0x203)
###### m_ceil  (0x204)
###### m_cos   (0x205)
###### m_cosh  (0x206)
###### m_exp   (0x207)
###### m_fabs  (0x208)
###### m_floor (0x209)
###### m_fmod  (0x20A)
###### m_frexp (0x20B)
###### m_ldexp (0x20C)
###### m_log   (0x20D)
###### m_log10 (0x20E)
###### m_log2  (0x20F)
###### m_modf  (0x210)
###### m_pow   (0x211)
###### m_sin   (0x212)
###### m_sinh  (0x213)
###### m_sqrt  (0x214)
###### m_tan   (0x215)
###### m_tanh  (0x216)

#### Instructions format
###### ret (0x1)

###### push (0x2):
   * **1 byte** : var type ( 0 = var, 1 = const)
   * **4 bytes**: variable index

###### pop (0x3):
   * **4 bytes**: result variable index

###### ldi (0x4):
   * **4 bytes**: dest index
   * **1 byte**: src type
   * **4 bytes**: src index

###### jmp (0x5):
   * **4 bytes**: index

###### call (0x6):
   * **4 bytes**: import index
   * **4 bytes**: function index

###### breq (0x7):
   * **1 byte**: var type
   * **4 bytes**: var index
   * **4 bytes**: jump index

###### cast (0x8):
   * **4 bytes**: dest variable index
   * **1 byte** : var type
   * **4 bytes**: src variable

###### vget (0x9)
   * **4 bytes**: dest variable index
   * **4 bytes**: vector variable index
   * **1 byte** : index var type
   * **4 bytes**: index variable

###### vset (0xA)
   * **4 bytes**: vector variable index
   * **1 byte** : index var type
   * **4 bytes**: index variable
   * **1 byte** : src var type
   * **4 bytes**: src variable

###### syscall (0xB)
   * **4 bytes**: subroutine index

###### Any ab instruction (0x64 - 0x7E):
   * **4 bytes**: result index
   * **1 byte** : operand1 type
   * **4 bytes**: operand1
   * **1 byte** : operand2 type
   * **4 bytes**: operand2

###### Any au instruction(0x100 - 0x102):
   * **4 bytes**: result index
   * **1 byte** : operand type
   * **4 bytes**: operand

