//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_INSTRUCTIONS_H
#define NMRUNNER_INSTRUCTIONS_H

#include <stddef.h>
#include <stdint.h>

#include "instructionsMacro.h"

struct _nmEnvironment;

typedef enum _instructionIndex {
    ii_ret = 0x1,
    ii_push = 0x2,
    ii_pop = 0x3,
    ii_ldi = 0x4,
    ii_jmp = 0x5,
    ii_call = 0x6,
    ii_breq = 0x7,
    ii_cast = 0x8,
    ii_vget = 0x9,
    ii_vset = 0xA,
    ii_syscall = 0xB,
    ii_vect = 0xC,
    ii_vind = 0xD,

    ii_A_Add  = 0x64,
    ii_A_Sub  = 0x65,
    ii_A_Mul  = 0x66,
    ii_A_Div  = 0x67,
    ii_A_lseq = 0x68,
    ii_A_ls   = 0x69,
    ii_A_gr   = 0x6A,
    ii_A_greq = 0x6B,
    ii_A_neq  = 0x6C,
    ii_A_eq   = 0x6D,
    ii_A_EDiv = 0x6E,
    ii_A_LAnd = 0x6F,
    ii_A_LOr  = 0x70,
    ii_A_And  = 0x71,
    ii_A_Xor  = 0x72,
    ii_A_Or   = 0x73,
    ii_A_lsh  = 0x74,
    ii_A_rlh  = 0x75,

    ii_A_SetAdd  = 0x76,
    ii_A_SetSub  = 0x77,
    ii_A_SetMul  = 0x78,
    ii_A_SetDiv  = 0x79,
    ii_A_SetEDiv = 0x7A,
    ii_A_SetAnd  = 0x7B,
    ii_A_SetXor  = 0x7C,
    ii_A_SetOr   = 0x7D,
    ii_A_Set     = 0x7E,

    ii_A_Neg  = 0x100,
    ii_A_Not  = 0x101,
    ii_A_BNeg = 0x102
} instructionIndex_t;


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
    instructionIndex_t index;
    nmInstructionOperandType_t parameterTypes[5];
    instructionFunction_t function[11];

} nmInstructionData_t;

typedef struct _nmInstruction {

    nmInstructionData_t* dataPtr;
    uint64_t* parameters;

} nmInstruction_t;

#define totalInstructionsCount 37

instructionPrototype(instruction_ret)
instructionPrototypeA(instruction_push)
instructionPrototypeA(instruction_pop)
instructionPrototype(instruction_ldi)
instructionPrototype(instruction_jmp)
instructionPrototype(instruction_call)
instructionPrototype(instruction_breq)
instructionPrototype(instruction_vind)

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
void instruction_VGet(struct _nmEnvironment* env, void** data);
void instruction_VSet(struct _nmEnvironment* env, void** data);
void instruction_Vect(struct _nmEnvironment* env, void** data);

static nmInstructionData_t instructionsData[totalInstructionsCount] = 
{
        //   Index     PCnt    Function
        //Generic Instructions
        { "ret",        ii_ret,       { 0, 0, 0, 0, 0 },                                { instruction_Ret }              }, // ret
        { "push",       ii_push,     { varConstFlag, varConstIndex, 0, 0, 0 },         enumerateAFunc(instruction_push) }, // push
        { "pop",        ii_pop,      { varIndex, 0, 0, 0, 0 },                         enumerateAFunc(instruction_pop)  }, // pop
        { "ldi",        ii_ldi,      { varIndex, varConstFlag, varConstIndex, 0, 0 },  enumerateFunc(instruction_ldi)   }, // ldi
        { "jmp",        ii_jmp,      { jumpIndex, 0, 0, 0, 0 },                        { instruction_Jmp }              }, // jmp
        { "call",       ii_call,     { functionIndex, functionIndex, 0, 0, 0 },        { instruction_Call }             }, // call
        { "breq",       ii_breq,     { varConstFlag, varConstIndex, jumpIndex, 0, 0 }, enumerateFunc(instruction_breq)  }, // breq,
        { "cast",       ii_cast,     { varIndex, varConstFlag, varConstIndex, 0, 0 },  {}                               }, // cast
        { "vget",       ii_vget,     { varIndex, 0, 0, 0, 0 },                         { instruction_VGet }             }, // vget
        { "vset",       ii_vset,     { varConstFlag, varConstIndex, 0, 0, 0 },         { instruction_VSet}              }, // vset
        { "syscall",    ii_syscall,  { functionIndex, 0, 0, 0, 0 },                    { instruction_Syscall }          }, // syscall
        { "vect",       ii_vect,     { varIndex, 0, 0, 0, 0 },                         { instruction_Vect }             }, // vect
        { "vind",       ii_vind,     { varConstFlag, varConstIndex, 0, 0, 0 },         enumerateFunc(instruction_vind)  }, // vind

        //Binary Arithmetic instructions
        instructionDataBinary(A_Add,      ii_A_Add ),
        instructionDataBinary(A_Sub,      ii_A_Sub ),
        instructionDataBinary(A_Mul,      ii_A_Mul ),
        instructionDataBinary(A_Div,      ii_A_Div ),
        instructionDataBinary(A_lseq,     ii_A_lseq),
        instructionDataBinary(A_ls,       ii_A_ls  ),
        instructionDataBinary(A_gr,       ii_A_gr  ),
        instructionDataBinary(A_greq,     ii_A_greq),
        instructionDataBinary(A_neq,      ii_A_neq ),
        instructionDataBinary(A_eq,       ii_A_eq  ),
        instructionDataBinary(A_EDiv,     ii_A_EDiv),
        instructionDataBinary(A_LAnd,     ii_A_LAnd),
        instructionDataBinary(A_LOr,      ii_A_LOr ),
        instructionDataBinary(A_And,      ii_A_And ),
        instructionDataBinary(A_Xor,      ii_A_Xor ),
        instructionDataBinary(A_Or,       ii_A_Or  ),
        instructionDataBinary(A_lsh,      ii_A_lsh ),
        instructionDataBinary(A_rlh,      ii_A_rlh ),

        //instructionDataBinary(A_SetAdd,  ii_A_SetAdd ),
        //instructionDataBinary(A_SetSub,  ii_A_SetSub ),
        //instructionDataBinary(A_SetMul,  ii_A_SetMul ),
        //instructionDataBinary(A_SetDiv,  ii_A_SetDiv ),
        //instructionDataBinary(A_SetEDiv, ii_A_SetEDiv),
        //instructionDataBinary(A_SetAnd,  ii_A_SetAnd ),
        //instructionDataBinary(A_SetXor,  ii_A_SetXor ),
        //instructionDataBinary(A_SetOr,   ii_A_SetOr  ),
        //instructionDataBinary(A_Set,     ii_A_Set    ),

        //Unary Arithmetic instructions
        instructionDataUnary(A_Neg,       ii_A_Neg ),
        instructionDataUnary(A_Not,       ii_A_Not ),
        instructionDataUnary(A_BNeg,      ii_A_BNeg)
};

nmInstructionData_t* getInstructionData(int instrIndex);
size_t getOperandsCount(nmInstructionData_t* data);
size_t getOperandSize(nmInstructionOperandType_t operandType);

#endif //NMRUNNER_INSTRUCTIONS_H