//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_INSTRUCTIONS_H
#define NMRUNNER_INSTRUCTIONS_H

#include <stddef.h>
#include <stdint.h>

typedef struct _nmEnvironment {

} nmEnvironment_t;

typedef struct _nmInstruction;

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
    void (*function)(struct _nmInstruction* instr);

} nmInstructionData_t;

typedef struct _nmInstruction {

    nmInstructionData_t* dataPtr;
    nmEnvironment_t* env;

    uint64_t* parameters;

} nmInstruction_t;

void instruction_ret(nmInstruction_t* this);
void instruction_push(nmInstruction_t* this);
void instruction_pop(nmInstruction_t* this);
void instruction_ldi(nmInstruction_t* this);
void instruction_jmp(nmInstruction_t* this);
void instruction_call(nmInstruction_t* this);
void instruction_breq(nmInstruction_t* this);
void instruction_A_Add(nmInstruction_t* this);
void instruction_A_Sub(nmInstruction_t* this);
void instruction_A_Mul(nmInstruction_t* this);
void instruction_A_Div(nmInstruction_t* this);
void instruction_A_lseq(nmInstruction_t* this);
void instruction_A_ls(nmInstruction_t* this);
void instruction_A_gr(nmInstruction_t* this);
void instruction_A_greq(nmInstruction_t* this);
void instruction_A_neq(nmInstruction_t* this);
void instruction_A_eq(nmInstruction_t* this);
void instruction_A_EDiv(nmInstruction_t* this);
void instruction_A_LAnd(nmInstruction_t* this);
void instruction_A_LOr(nmInstruction_t* this);
void instruction_A_And(nmInstruction_t* this);
void instruction_A_Xor(nmInstruction_t* this);
void instruction_A_Or(nmInstruction_t* this);
void instruction_A_lsh(nmInstruction_t* this);
void instruction_A_rlh(nmInstruction_t* this);
void instruction_A_SetAdd(nmInstruction_t* this);
void instruction_A_SetSub(nmInstruction_t* this);
void instruction_A_SetMul(nmInstruction_t* this);
void instruction_A_SetDiv(nmInstruction_t* this);
void instruction_A_SetEDiv(nmInstruction_t* this);
void instruction_A_SetAnd(nmInstruction_t* this);
void instruction_A_SetXor(nmInstruction_t* this);
void instruction_A_SetOr(nmInstruction_t* this);
void instruction_A_Set(nmInstruction_t* this);
void instruction_A_Neg(nmInstruction_t* this);
void instruction_A_Not(nmInstruction_t* this);
void instruction_A_BNeg(nmInstruction_t* this);

#define totalInstructionsCount 37

static nmInstructionData_t instructionsData[totalInstructionsCount] = {
    //   Index     PCnt    Function
        //Generic Instructions
        { "ret",        0x1,  { 0, 0, 0, 0, 0 },                                instruction_ret   }, // ret
        { "push",       0x2,  { varConstFlag, varConstIndex, 0, 0, 0 },         instruction_push  }, // push
        { "pop",        0x3,  { varIndex, 0, 0, 0, 0 },                         instruction_pop   }, // pop
        { "ldi",        0x4,  { varIndex, varConstFlag, varIndex, 0, 0 },       instruction_ldi   }, // ldi
        { "jmp",        0x5,  { jumpIndex, 0, 0, 0, 0 },                        instruction_jmp   }, // jmp
        { "call",       0x6,  { functionIndex, 0, 0, 0, 0 },                    instruction_call  }, // call
        { "breq",       0x7,  { varConstFlag, varConstIndex, jumpIndex, 0, 0 }, instruction_breq  }, // breq

        //"instructions",       "Binary Arithmetic instructions
        { "A_Add",      0x64, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Add     }, // A_Add
        { "A_Sub",      0x65, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Sub     }, // A_Sub
        { "A_Mul",      0x66, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Mul     }, // A_Mul
        { "A_Div",      0x67, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Div     }, // A_Div
        { "A_lseq",     0x68, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_lseq    }, // A_lseq
        { "A_ls",       0x69, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_ls      }, // A_ls
        { "A_gr",       0x6A, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_gr      }, // A_gr
        { "A_greq",     0x6B, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_greq    }, // A_greq
        { "A_neq",      0x6C, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_neq     }, // A_neq
        { "A_eq",       0x6D, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_eq      }, // A_eq
        { "A_EDiv",     0x6E, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_EDiv    }, // A_EDiv
        { "A_LAnd",     0x6F, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_LAnd    }, // A_LAnd
        { "A_LOr",      0x70, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_LOr     }, // A_LOr
        { "A_And",      0x71, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_And     }, // A_And
        { "A_Xor",      0x72, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Xor     }, // A_Xor
        { "A_Or",       0x73, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Or      }, // A_Or
        { "A_lsh",      0x74, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_lsh     }, // A_lsh
        { "A_rlh",      0x75, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_rlh     }, // A_rlh
        { "A_SetAdd",   0x76, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetAdd  }, // A_SetAdd
        { "A_SetSub",   0x77, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetSub  }, // A_SetSub
        { "A_SetMul",   0x78, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetMul  }, // A_SetMul
        { "A_SetDiv",   0x79, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetDiv  }, // A_SetDiv
        { "A_SetEDiv",  0x7A, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetEDiv }, // A_SetEDiv
        { "A_SetAnd",   0x7B, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetAnd  }, // A_SetAnd
        { "A_SetXor",   0x7C, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetXor  }, // A_SetXor
        { "A_SetOr",    0x7D, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_SetOr   }, // A_SetOr
        { "A_Set",      0x7E, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, instruction_A_Set     }, // A_Set

        //"instructions",       "Unary Arithmetic instructions
        { "A_Neg",      0x100, { varIndex, varConstFlag, varConstIndex, 0, 0 }, instruction_A_Neg  }, // A_Neg
        { "A_Not",      0x101, { varIndex, varConstFlag, varConstIndex, 0, 0 }, instruction_A_Not  }, // A_Not
        { "A_BNeg",     0x102, { varIndex, varConstFlag, varConstIndex, 0, 0 }, instruction_A_BNeg }, // A_BNeg
};

nmInstructionData_t* getInstructionData(int instrIndex);
size_t getOperandsCount(nmInstructionData_t* data);
size_t getOperandSize(nmInstructionOperandType_t operandType);

#endif //NMRUNNER_INSTRUCTIONS_H