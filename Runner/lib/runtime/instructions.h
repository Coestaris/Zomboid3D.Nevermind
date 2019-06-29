//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_INSTRUCTIONS_H
#define NMRUNNER_INSTRUCTIONS_H

#include <stddef.h>
#include <stdint.h>

typedef struct _nmEnvironment {

} nmEnvironment_t;

typedef struct _nmInstruction
{
    nmInstructionData_t* instrPtr;

    uint16_t index;
    uint32_t* parameters;

} nmInstruction_t;

typedef struct _nmInstructionData {

    uint16_t index;
    uint8_t parameterCount;
    void (*function)(nmEnvironment_t* env, nmInstruction_t* instr);

} nmInstructionData_t;

uint8_t getInstructionParameterCount(int instrIndex);

#endif //NMRUNNER_INSTRUCTIONS_H