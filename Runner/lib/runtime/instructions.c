//
// Created by maxim on 6/30/19.
//

#include "instructions.h"
#include "environment.h"

void instruction_ret(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_push(struct _nmEnvironment* env, void** data)
{
    pushStack(env->variableStack, data[0]);
}

void instruction_pop(struct _nmEnvironment* env, void** data)
{
    data[0] = popStack(env->variableStack);
}

void instruction_ldi(struct _nmEnvironment* env, void** data)
{
    data[0] = data[1];
}

void instruction_jmp(struct _nmEnvironment* env, void** data)
{
    *env->programCounter = *(uint32_t*)data[0];
}

void instruction_call(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_breq(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Add(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Sub(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Mul(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Div(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_lseq(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_ls(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_gr(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_greq(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_neq(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_eq(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_EDiv(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_LAnd(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_LOr(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_And(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Xor(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Or(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_lsh(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_rlh(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetAdd(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetSub(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetMul(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetDiv(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetEDiv(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetAnd(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetXor(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_SetOr(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Set(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Neg(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_Not(struct _nmEnvironment* env, void** data)
{
    //stub
}

void instruction_A_BNeg(struct _nmEnvironment* env, void** data)
{
    //stub
}

nmInstructionData_t* getInstructionData(int instrIndex)
{
    for(size_t i = 0; i < totalInstructionsCount; i++)
    {
        if (instructionsData[i].index == instrIndex)
            return &instructionsData[i];
    }
    return NULL;
}

size_t getOperandSize(nmInstructionOperandType_t operandType)
{
    switch(operandType)
    {
        case varConstFlag: return 1;
        case varIndex: return 4;
        case varConstIndex: return 4;
        case functionIndex: return 4;
        case jumpIndex: return 4;
        default:
            return 0;
    }
}

size_t getOperandsCount(nmInstructionData_t* data)
{
    size_t count = 0;
    while(count < 5 && data->parameterTypes[count] != 0) count++;
    return count;
}
