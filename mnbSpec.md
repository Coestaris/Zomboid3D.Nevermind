#.NMB File spec
####File Structure:
  * **3 bytes**: "nmb" signature
  * **rest**   : chunks

####Chunk structure:
  * **4 bytes**: length
  * **4 bytes**: crc
  * **2 bytes**: type
  * **n bytes**: data

####Chunks:
######"HE" - Header Chunk
  * **2 bytes**: NM version
  * **4 bytes**: import count
  * **4 bytes**: function count
  <br>Import format:
     * **1 byte** : module type (0 = sys library, 1 = import)
     * **4 bytes**: module name length (filename)
     * **n bytes**: module name

######"ME" - Metadata Chunk (optional) 
  * **8 bytes**: compile date - time
     * **1 byte** : second
     * **1 byte** : minute
     * **1 byte** : hour
     * **1 byte** : day
     * **1 byte** : month
     * **2 bytes**: year
  * **2 bytes**: Bianry name length
  * **n bytes**: Binary name
  * **2 bytes**: Binary description length
  * **m bytes**: Binary description
  * **2 bytes**: Binary author length
  * **k bytes**: Binary author
  * **2 bytes**: Binary Minor verion
  * **2 bytes**: Binary Major verion

######"TY" - Type Chunk 
  * **4 bytes**: type count
  <br>Type format:
     * **2 bytes**: type signature
     * **1 byte**:  type base

######  "CO" - Constants Chunk 
  * **4 bytes**: constant count
   <br>Constant format:
     * **4 bytes**: type index
   * **len * type base bytes**: value

######  "FU" - Function chunk 
  * **4 bytes**  : function index
  * **4 bytes**  : instructions count
  * **4 bytes**  : locals count
  * **n * 4 bytes**: local types
  * **4 bytes**  : reg count
  * **m * 4 bytes**: reg types
  <br>instruction format:
    * **2 bytes**: instruction index
    * **4 * param count bytes**: parameters


####Instructions format
######ret (0x1)

######push (0x2):
   * **1 byte** : var type ( 0 = var, 1 = const)
   * **4 bytes**: variable index

######pop (0x3):
   * **4 bytes**: result variable index

######ldi (0x4):
   * **4 bytes**: dest index
   * **1 byte**: src type
   * **4 bytes**: src index

######jmp (0x5):
   * **4 bytes**: index

######call (0x6):
   * **4 bytes**: function index

######breq (0x7):
   * **1 byte**: var type
   * **4 bytes**: var index
   * **4 bytes**: jump index

######Any ab instruction (0x64 - 0x7E):
   * **4 bytes**: result index
   * **1 byte**: operand1 type
   * **4 bytes**: operand1
   * **1 byte**: operand2 type
   * **4 bytes**: operand2

######Any au instruction(0x100 - 0x102):
   * **4 bytes**: result index
   * **1 byte**: operand type
   * **4 bytes**: operand
