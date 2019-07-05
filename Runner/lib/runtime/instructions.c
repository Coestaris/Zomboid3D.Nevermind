//
// Created by maxim on 6/30/19.
//

#include "instructions.h"
#include "environment.h"

declareRetInstruction(instruction_ret_i8,  int8_t)
declareRetInstruction(instruction_ret_i16, int32_t)
declareRetInstruction(instruction_ret_i32, int32_t)
declareRetInstruction(instruction_ret_i64, uint64_t)
declareRetInstruction(instruction_ret_u8,  uint8_t)
declareRetInstruction(instruction_ret_u16, uint16_t)
declareRetInstruction(instruction_ret_u32, uint32_t)
declareRetInstruction(instruction_ret_u64, uint64_t)
declareRetInstruction(instruction_ret_f32, float)
declareRetInstruction(instruction_ret_f64, double)

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

declareJmpInstruction(instruction_jmp_i8,  int8_t)
declareJmpInstruction(instruction_jmp_i16, int32_t)
declareJmpInstruction(instruction_jmp_i32, int32_t)
declareJmpInstruction(instruction_jmp_i64, uint64_t)
declareJmpInstruction(instruction_jmp_u8,  uint8_t)
declareJmpInstruction(instruction_jmp_u16, uint16_t)
declareJmpInstruction(instruction_jmp_u32, uint32_t)
declareJmpInstruction(instruction_jmp_u64, uint64_t)
declareJmpInstruction(instruction_jmp_f32, float)
declareJmpInstruction(instruction_jmp_f64, double)

declareCallInstruction(instruction_call_i8,  int8_t)
declareCallInstruction(instruction_call_i16, int32_t)
declareCallInstruction(instruction_call_i32, int32_t)
declareCallInstruction(instruction_call_i64, uint64_t)
declareCallInstruction(instruction_call_u8,  uint8_t)
declareCallInstruction(instruction_call_u16, uint16_t)
declareCallInstruction(instruction_call_u32, uint32_t)
declareCallInstruction(instruction_call_u64, uint64_t)
declareCallInstruction(instruction_call_f32, float)
declareCallInstruction(instruction_call_f64, double)

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
