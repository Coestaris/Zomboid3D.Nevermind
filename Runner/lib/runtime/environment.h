//
// Created by maxim on 6/30/19.
//

#ifndef NMRUNNER_ENVIRONMENT_H
#define NMRUNNER_ENVIRONMENT_H

#include <stdio.h>

#include "stack.h"
#include "../coretypes.h"

typedef struct _nmCallableInstruction
{
    void (*function)(struct _nmEnvironment* env, void** params);
    size_t paramCount;
    void** parameters;

} nmCallableInstruction_t;

typedef struct _nmCallableFunction
{
    size_t instructionsCount;
    nmCallableInstruction_t** callableInstructions;
    void** locals;

} nmCallableFunction_t;

typedef struct _nmEnvironment
{
    nmProgram_t* program;

    stack_t* callStack;
    stack_t* pcStack;
    stack_t* variableStack;

    FILE* outs;
    FILE* ins;

    void* locals;
    void* registers;

    nmCallableFunction_t** callableFunctions;

    uint32_t* funcIndex;
    uint32_t* programCounter;

} nmEnvironment_t;

void nmEnvExecute(nmEnvironment_t* env);
void nmEnvSetStreams(nmEnvironment_t* env, FILE* in, FILE* out);
void nmEnvDump(nmEnvironment_t* env, FILE* f);
nmEnvironment_t* nmEnvCreate(nmProgram_t* program);
void nmEnvFree(nmEnvironment_t* env);

#endif //NMRUNNER_ENVIRONMENT_H
