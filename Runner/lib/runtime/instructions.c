//
// Created by maxim on 6/30/19.
//

#include "instructions.h"

void instruction_ret(nmInstruction_t* this)
{
    //stub!
}

void instruction_push(nmInstruction_t* this)
{
    //stub!
}

void instruction_pop(nmInstruction_t* this)
{
    //stub!
}

void instruction_ldi(nmInstruction_t* this)
{
    //stub!
}

void instruction_jmp(nmInstruction_t* this)
{
    //stub!
}

void instruction_call(nmInstruction_t* this)
{
    //stub!
}

void instruction_breq(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Add(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Sub(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Mul(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Div(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_lseq(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_ls(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_gr(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_greq(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_neq(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_eq(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_EDiv(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_LAnd(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_LOr(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_And(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Xor(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Or(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_lsh(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_rlh(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetAdd(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetSub(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetMul(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetDiv(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetEDiv(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetAnd(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetXor(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_SetOr(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Set(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Neg(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_Not(nmInstruction_t* this)
{
    //stub!
}

void instruction_A_BNeg(nmInstruction_t* this)
{
    //stub!
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
