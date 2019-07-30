//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_INSTRUCTIONS_H
#define NMRUNNER_INSTRUCTIONS_H

#include <stddef.h>
#include <stdint.h>

#include "instructionsMacro.h"

struct _nmEnvironment;

typedef enum _nmInstructionOperandType {
    varConstFlag     = 0x1,
    varIndex         = 0x2,
    varConstIndex    = 0x3,
    functionIndex    = 0x4,
    jumpIndex        = 0x5,

} nmInstructionOperandType_t;

typedef void (*instructionFunction_t)(struct _nmEnvironment* env, void** data);

typedef struct _nmInstructionData {

    const char* name;
    uint16_t index;
    nmInstructionOperandType_t parameterTypes[5];
    instructionFunction_t function[10];

} nmInstructionData_t;

typedef struct _nmInstruction {

    nmInstructionData_t* dataPtr;
    uint64_t* parameters;

} nmInstruction_t;

#define totalInstructionsCount 37

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

castFuncPrototype(i8)
castFuncPrototype(i16)
castFuncPrototype(i32)
castFuncPrototype(i64)
castFuncPrototype(u8)
castFuncPrototype(u16)
castFuncPrototype(u32)
castFuncPrototype(u64)
castFuncPrototype(f32)
castFuncPrototype(f64)

static instructionFunction_t castFunctions[] = {
    enumerateCastFunc(i8)
    enumerateCastFunc(i16)
    enumerateCastFunc(i32)
    enumerateCastFunc(i64)
    enumerateCastFunc(u8)
    enumerateCastFunc(u16)
    enumerateCastFunc(u32)
    enumerateCastFunc(u64)
    enumerateCastFunc(f32)
    enumerateCastFunc(f64)
};
//ret, jmp, call, syscall

void instruction_Ret(struct _nmEnvironment* env, void** data);
void instruction_Jmp(struct _nmEnvironment* env, void** data);
void instruction_Call(struct _nmEnvironment* env, void** data);
void instruction_Syscall(struct _nmEnvironment* env, void** data);

static nmInstructionData_t instructionsData[totalInstructionsCount] = 
{
        //   Index     PCnt    Function
        //Generic Instructions
        { "ret",        0x1,  { 0, 0, 0, 0, 0 },                                { instruction_Ret }              }, // ret
        { "push",       0x2,  { varConstFlag, varConstIndex, 0, 0, 0 },         enumerateFunc(instruction_push)  }, // push
        { "pop",        0x3,  { varIndex, 0, 0, 0, 0 },                         enumerateFunc(instruction_pop)   }, // pop
        { "ldi",        0x4,  { varIndex, varConstFlag, varConstIndex, 0, 0 },  enumerateFunc(instruction_ldi)   }, // ldi
        { "jmp",        0x5,  { jumpIndex, 0, 0, 0, 0 },                        { instruction_Jmp }              }, // jmp
        { "call",       0x6,  { functionIndex, functionIndex, 0, 0, 0 },        { instruction_Call }             }, // call
        { "breq",       0x7,  { varConstFlag, varConstIndex, jumpIndex, 0, 0 }, enumerateFunc(instruction_breq)  }, // breq,
        { "cast",       0x8,  { 0, 0, 0, 0, 0 },                                {}                               }, // cast
        { "vget",       0x9,  { 0, 0, 0, 0, 0 },                                {}                               }, // vget
        { "vset",       0xA,  { 0, 0, 0, 0, 0 },                                {}                               }, // vset
        { "syscall",    0xB,  { functionIndex, 0, 0, 0, 0 },                    { instruction_Syscall }          }, //syscall

        //Binary Arithmetic instructions
        instructionDataBinary(A_Add,      0x64),
        instructionDataBinary(A_Sub,      0x65),
        instructionDataBinary(A_Mul,      0x66),
        instructionDataBinary(A_Div,      0x67),
        instructionDataBinary(A_lseq,     0x68),
        instructionDataBinary(A_ls,       0x69),
        instructionDataBinary(A_gr,       0x6A),
        instructionDataBinary(A_greq,     0x6B),
        instructionDataBinary(A_neq,      0x6C),
        instructionDataBinary(A_eq,       0x6D),
        instructionDataBinary(A_EDiv,     0x6E),
        instructionDataBinary(A_LAnd,     0x6F),
        instructionDataBinary(A_LOr,      0x70),
        instructionDataBinary(A_And,      0x71),
        instructionDataBinary(A_Xor,      0x72),
        instructionDataBinary(A_Or,       0x73),
        instructionDataBinary(A_lsh,      0x74),
        instructionDataBinary(A_rlh,      0x75),

        //instructionDataBinary(A_SetAdd,   0x76),
        //instructionDataBinary(A_SetSub,   0x77),
        //instructionDataBinary(A_SetMul,   0x78),
        //instructionDataBinary(A_SetDiv,   0x79),
        //instructionDataBinary(A_SetEDiv,  0x7A),
        //instructionDataBinary(A_SetAnd,   0x7B),
        //instructionDataBinary(A_SetXor,   0x7C),
        //instructionDataBinary(A_SetOr,    0x7D),
        //instructionDataBinary(A_Set,      0x7E),

        //Unary Arithmetic instructions
        instructionDataUnary(A_Neg,       0x100),
        instructionDataUnary(A_Not,       0x101),
        instructionDataUnary(A_BNeg,      0x102)
};

nmInstructionData_t* getInstructionData(int instrIndex);
size_t getOperandsCount(nmInstructionData_t* data);
size_t getOperandSize(nmInstructionOperandType_t operandType);

#endif //NMRUNNER_INSTRUCTIONS_H