//
// Created by maxim on 6/30/19.
//

#include "instructions.h"
#include "environment.h"

declarePushInstruction(instruction_push_i8,  int8_t)
declarePushInstruction(instruction_push_i16, int32_t)
declarePushInstruction(instruction_push_i32, int32_t)
declarePushInstruction(instruction_push_i64, uint64_t)
declarePushInstruction(instruction_push_u8,  uint8_t)
declarePushInstruction(instruction_push_u16, uint16_t)
declarePushInstruction(instruction_push_u32, uint32_t)
declarePushInstruction(instruction_push_u64, uint64_t)
declarePushInstruction(instruction_push_f32, float)
declarePushInstruction(instruction_push_f64, double)

declarePopInstruction(instruction_pop_i8,  int8_t)
declarePopInstruction(instruction_pop_i16, int32_t)
declarePopInstruction(instruction_pop_i32, int32_t)
declarePopInstruction(instruction_pop_i64, uint64_t)
declarePopInstruction(instruction_pop_u8,  uint8_t)
declarePopInstruction(instruction_pop_u16, uint16_t)
declarePopInstruction(instruction_pop_u32, uint32_t)
declarePopInstruction(instruction_pop_u64, uint64_t)
declarePopInstruction(instruction_pop_f32, float)
declarePopInstruction(instruction_pop_f64, double)

declareLdiInstruction(instruction_ldi_i8,  int8_t)
declareLdiInstruction(instruction_ldi_i16, int32_t)
declareLdiInstruction(instruction_ldi_i32, int32_t)
declareLdiInstruction(instruction_ldi_i64, uint64_t)
declareLdiInstruction(instruction_ldi_u8,  uint8_t)
declareLdiInstruction(instruction_ldi_u16, uint16_t)
declareLdiInstruction(instruction_ldi_u32, uint32_t)
declareLdiInstruction(instruction_ldi_u64, uint64_t)
declareLdiInstruction(instruction_ldi_f32, float)
declareLdiInstruction(instruction_ldi_f64, double)

declareBreqInstruction(instruction_breq_i8,  int8_t)
declareBreqInstruction(instruction_breq_i16, int32_t)
declareBreqInstruction(instruction_breq_i32, int32_t)
declareBreqInstruction(instruction_breq_i64, uint64_t)
declareBreqInstruction(instruction_breq_u8,  uint8_t)
declareBreqInstruction(instruction_breq_u16, uint16_t)
declareBreqInstruction(instruction_breq_u32, uint32_t)
declareBreqInstruction(instruction_breq_u64, uint64_t)
declareBreqInstruction(instruction_breq_f32, float)
declareBreqInstruction(instruction_breq_f64, double)

declareABInstruction(instruction_A_Add, +)
declareABInstruction(instruction_A_Sub, -)
declareABInstruction(instruction_A_Mul, *)
declareABInstruction(instruction_A_Div, /)
declareABInstruction(instruction_A_lseq, <=)
declareABInstruction(instruction_A_ls, <)
declareABInstruction(instruction_A_gr, >)
declareABInstruction(instruction_A_greq, >=)
declareABInstruction(instruction_A_neq, !=)
declareABInstruction(instruction_A_eq, ==)

declareABCInstruction(instruction_A_EDiv, %)
declareABCInstruction(instruction_A_LAnd, &&)
declareABCInstruction(instruction_A_LOr, ||)
declareABCInstruction(instruction_A_And, &)
declareABCInstruction(instruction_A_Xor, ^)
declareABCInstruction(instruction_A_Or, |)
declareABCInstruction(instruction_A_lsh, <<)
declareABCInstruction(instruction_A_rlh, >>)
/*
declareABCInstruction(instruction_A_SetEDiv, %=)
declareABCInstruction(instruction_A_SetAnd, &=)
declareABCInstruction(instruction_A_SetXor, ^=)
declareABICnstruction(instruction_A_SetOr, |=)

declareABInstruction(instruction_A_SetAdd, +=)
declareABInstruction(instruction_A_SetSub, -=)
declareABInstruction(instruction_A_SetMul, *=)
declareABInstruction(instruction_A_SetDiv, /=)
declareABInstruction(instruction_A_Set, =) */

declareAUInstruction(instruction_A_Neg, -)
declareAUInstruction(instruction_A_Not, !)
declareAUInstruction(instruction_A_BNeg, ~)

declareCastInstructionSet(int8_t,   i8)
declareCastInstructionSet(int16_t,  i16)
declareCastInstructionSet(int32_t,  i32)
declareCastInstructionSet(int64_t,  i64)
declareCastInstructionSet(uint8_t,  u8)
declareCastInstructionSet(uint16_t, u16)
declareCastInstructionSet(uint32_t, u32)
declareCastInstructionSet(uint64_t, u64)
declareCastInstructionSet(float,    f32)
declareCastInstructionSet(double,   f64)

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

#include "subroutines.h"

void instruction_Syscall(struct _nmEnvironment* env, void** data)
{
    uint64_t index = data[0];
    subroutines[index]->func(env);
}

void instruction_Ret(struct _nmEnvironment* env, void** data)
{
    uint32_t pc = popStack(env->pcStack);
    uint32_t func = popStack(env->callStack);

    *env->funcIndex = func;
    *env->programCounter = pc;
}

void instruction_Jmp(struct _nmEnvironment* env, void** data)
{

}

void instruction_Call(struct _nmEnvironment* env, void** data)
{
    //module index??
    uint64_t funcIndex = data[1];
    pushStack(env->callStack, (void*)*env->funcIndex);
    pushStack(env->pcStack,   (void*)*env->programCounter);

    *env->funcIndex = funcIndex;
    *env->programCounter = -1;
}