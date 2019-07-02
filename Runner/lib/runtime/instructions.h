//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_INSTRUCTIONS_H
#define NMRUNNER_INSTRUCTIONS_H

#include <stddef.h>
#include <stdint.h>
//#include "environment.h"

struct _nmEnvironment;

typedef enum _nmInstructionOperandType {
    varConstFlag     = 0x1,
    varIndex         = 0x2,
    varConstIndex    = 0x3,
    functionIndex    = 0x4,
    jumpIndex        = 0x5,

} nmInstructionOperandType_t;

typedef struct _nmInstructionData {

    const char* name;
    uint16_t index;
    nmInstructionOperandType_t parameterTypes[5];
    void (*function[6])(struct _nmEnvironment* env, void** data);

} nmInstructionData_t;

typedef struct _nmInstruction {

    nmInstructionData_t* dataPtr;
    uint64_t* parameters;

} nmInstruction_t;

#define totalInstructionsCount 37
#define enumerateFunc(name) { name ## _u8, name ## _u16, name ## _u32, name ## _u64, name ## _f32, name ## _f64 }

#define AInstructionPrototype(name) void name(struct _nmEnvironment* env, void** data);
#define AInstructionPrototypeSet(name1,name2,name3,name4,name5,name6)\
    AInstructionPrototype(name1)  \
    AInstructionPrototype(name2)  \
    AInstructionPrototype(name3)  \
    AInstructionPrototype(name4)  \
    AInstructionPrototype(name5)  \
    AInstructionPrototype(name6)  \

#define instructionPrototype(name) \
    AInstructionPrototypeSet(name ## _u8, name ## _u16, name ## _u32, name ## _u64, name ## _f32, name ## _f64);

instructionPrototype(instruction_ret)
instructionPrototype(instruction_push)
instructionPrototype(instruction_pop)
instructionPrototype(instruction_ldi)
instructionPrototype(instruction_jmp)
instructionPrototype(instruction_call)
instructionPrototype(instruction_breq)

instructionPrototype(instruction_A_Neg)
instructionPrototype(instruction_A_Not)
instructionPrototype(instruction_A_BNeg)

instructionPrototype(instruction_A_Add)
instructionPrototype(instruction_A_Sub)
instructionPrototype(instruction_A_Mul)
instructionPrototype(instruction_A_Div)
instructionPrototype(instruction_A_lseq)
instructionPrototype(instruction_A_ls)
instructionPrototype(instruction_A_gr)
instructionPrototype(instruction_A_greq)
instructionPrototype(instruction_A_neq)
instructionPrototype(instruction_A_eq)
instructionPrototype(instruction_A_EDiv)
instructionPrototype(instruction_A_LAnd)
instructionPrototype(instruction_A_LOr)
instructionPrototype(instruction_A_And)
instructionPrototype(instruction_A_Xor)
instructionPrototype(instruction_A_Or)
instructionPrototype(instruction_A_lsh)
instructionPrototype(instruction_A_rlh)
instructionPrototype(instruction_A_SetAdd)
instructionPrototype(instruction_A_SetSub)
instructionPrototype(instruction_A_SetMul)
instructionPrototype(instruction_A_SetDiv)
instructionPrototype(instruction_A_SetEDiv)
instructionPrototype(instruction_A_SetAnd)
instructionPrototype(instruction_A_SetXor)
instructionPrototype(instruction_A_SetOr)
instructionPrototype(instruction_A_Set)

static nmInstructionData_t instructionsData[totalInstructionsCount] = 
{
        //   Index     PCnt    Function
        //Generic Instructions
        { "ret",        0x1,  { 0, 0, 0, 0, 0 },                                enumerateFunc(instruction_ret)   }, // ret
        { "push",       0x2,  { varConstFlag, varConstIndex, 0, 0, 0 },         enumerateFunc(instruction_push)  }, // push
        { "pop",        0x3,  { varIndex, 0, 0, 0, 0 },                         enumerateFunc(instruction_pop)   }, // pop
        { "ldi",        0x4,  { varIndex, varConstFlag, varConstIndex, 0, 0 },  enumerateFunc(instruction_ldi)   }, // ldi
        { "jmp",        0x5,  { jumpIndex, 0, 0, 0, 0 },                        enumerateFunc(instruction_jmp)   }, // jmp
        { "call",       0x6,  { functionIndex, 0, 0, 0, 0 },                    enumerateFunc(instruction_call)  }, // call
        { "breq",       0x7,  { varConstFlag, varConstIndex, jumpIndex, 0, 0 }, enumerateFunc(instruction_breq)  }, // breq

        //"instructions",       "Binary Arithmetic instructions
        { "A_Add",      0x64, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Add)     }, // A_Add
        { "A_Sub",      0x65, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Sub)     }, // A_Sub
        { "A_Mul",      0x66, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Mul)     }, // A_Mul
        { "A_Div",      0x67, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Div)     }, // A_Div
        { "A_lseq",     0x68, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_lseq)    }, // A_lseq
        { "A_ls",       0x69, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_ls)      }, // A_ls
        { "A_gr",       0x6A, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_gr)      }, // A_gr
        { "A_greq",     0x6B, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_greq)    }, // A_greq
        { "A_neq",      0x6C, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_neq)     }, // A_neq
        { "A_eq",       0x6D, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_eq)      }, // A_eq
        { "A_EDiv",     0x6E, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_EDiv)    }, // A_EDiv
        { "A_LAnd",     0x6F, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_LAnd)    }, // A_LAnd
        { "A_LOr",      0x70, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_LOr)     }, // A_LOr
        { "A_And",      0x71, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_And)     }, // A_And
        { "A_Xor",      0x72, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Xor)     }, // A_Xor
        { "A_Or",       0x73, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Or)      }, // A_Or
        { "A_lsh",      0x74, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_lsh)     }, // A_lsh
        { "A_rlh",      0x75, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_rlh)     }, // A_rlh
        //{ "A_SetAdd",   0x76, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetAdd)  }, // A_SetAdd
        //{ "A_SetSub",   0x77, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetSub)  }, // A_SetSub
        //{ "A_SetMul",   0x78, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetMul)  }, // A_SetMul
        //{ "A_SetDiv",   0x79, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetDiv)  }, // A_SetDiv
        //{ "A_SetEDiv",  0x7A, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetEDiv) }, // A_SetEDiv
        //{ "A_SetAnd",   0x7B, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetAnd)  }, // A_SetAnd
        //{ "A_SetXor",   0x7C, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetXor)  }, // A_SetXor
        //{ "A_SetOr",    0x7D, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_SetOr)   }, // A_SetOr
        //{ "A_Set",      0x7E, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_A_Set)     }, // A_Set

        //"instructions",       "Unary Arithmetic instructions
        { "A_Neg",      0x100, { varIndex, varConstFlag, varConstIndex, 0, 0 }, enumerateFunc(instruction_A_Neg)  }, // A_Neg
        { "A_Not",      0x101, { varIndex, varConstFlag, varConstIndex, 0, 0 }, enumerateFunc(instruction_A_Not)  }, // A_Not
        { "A_BNeg",     0x102, { varIndex, varConstFlag, varConstIndex, 0, 0 }, enumerateFunc(instruction_A_BNeg) }, // A_BNeg
};


#define declareRetInstruction(name, type)                   \
void name(struct _nmEnvironment* env, void** data) { }

#define declarePushInstruction(name, type)           \
void name(struct _nmEnvironment* env, void** data)   \
{                                                    \
    pushStack(env->variableStack, data[0]);          \
}                                                    \

#define declarePopInstruction(name, type)                   \
void name(struct _nmEnvironment* env, void** data)          \
{                                                           \
    *(type*)data[0] = *(type*)popStack(env->variableStack); \
}                                                           \

#define declareLdiInstruction(name, type)           \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
    *(type*)data[0] = *(type*)data[1];              \
}                                                   \

#define declareJmpInstruction(name, type)           \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
    *env->programCounter = *(type*)data[0];         \
}                                                   \

#define declareCallInstruction(name, type)          \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
                                                    \
}                                                   \

#define declareBreqInstruction(name, type)          \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
                                                    \
}                                                   \


#define decalreABInstruction(name, type, sign) void name(struct _nmEnvironment* env, void** data) { *(type*)data[0] = *(type*)data[1] sign *(type*)data[2]; }
#define decalreABInstructionSet(name1, name2, name3, name4, name5, name6, sign)\
    decalreABInstruction(name1, uint8_t, sign)  \
    decalreABInstruction(name2, uint16_t, sign) \
    decalreABInstruction(name3, uint32_t, sign) \
    decalreABInstruction(name4, uint64_t, sign) \
    decalreABInstruction(name5, float, sign)    \
    decalreABInstruction(name6, double, sign)   \

#define declareABInstruction(name, sign) \
    decalreABInstructionSet(name ## _u8, name ## _u16, name ## _u32, name ## _u64, name ## _f32, name ## _f64, sign);

#define decalreABCInstruction(name, type, sign) void name(struct _nmEnvironment* env, void** data) { *(type*)data[0] = (type)((uint64_t)*(type*)data[1] sign (uint64_t)*(type*)data[2]); }
#define decalreABCInstructionSet(name1, name2, name3, name4, name5, name6, sign)\
    decalreABCInstruction(name1, uint8_t, sign)  \
    decalreABCInstruction(name2, uint16_t, sign) \
    decalreABCInstruction(name3, uint32_t, sign) \
    decalreABCInstruction(name4, uint64_t, sign) \
    decalreABCInstruction(name5, float, sign)    \
    decalreABCInstruction(name6, double, sign)   \

#define declareABCInstruction(name, sign) \
    decalreABCInstructionSet(name ## _u8, name ## _u16, name ## _u32, name ## _u64, name ## _f32, name ## _f64, sign);


#define decalreAUInstruction(name, type, sign) void name(struct _nmEnvironment* env, void** data) { *(type*)data[0] = sign (uint64_t)(*(type*)data[1]); }
#define decalreAUInstructionSet(name1,name2,name3,name4,name5,name6,sign)\
    decalreAUInstruction(name1, uint8_t, sign)  \
    decalreAUInstruction(name2, uint16_t, sign) \
    decalreAUInstruction(name3, uint32_t, sign) \
    decalreAUInstruction(name4, uint64_t, sign) \
    decalreAUInstruction(name5, float, sign)    \
    decalreAUInstruction(name6, double, sign)   \

#define declareAUInstruction(name, sign) \
    decalreAUInstructionSet(name ## _u8, name ## _u16, name ## _u32, name ## _u64, name ## _f32, name ## _f64, sign);




nmInstructionData_t* getInstructionData(int instrIndex);
size_t getOperandsCount(nmInstructionData_t* data);
size_t getOperandSize(nmInstructionOperandType_t operandType);

#endif //NMRUNNER_INSTRUCTIONS_H