//
// Created by maxim on 6/30/19.
//

#ifndef NMRUNNER_ENVIRONMENT_H
#define NMRUNNER_ENVIRONMENT_H

#include <stdio.h>
#include <math.h>
#include <time.h>
#include <zconf.h>

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

    size_t localsCount;
    nmType_t** localTypes;
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

    nmCallableFunction_t** callableFunctions;

    uint32_t* funcIndex;
    uint32_t* programCounter;

    time_t execStartTime;
    time_t execEndTime;

    void** globals;

} nmEnvironment_t;

extern nmEnvironment_t* currentEnv;

void setDefaultValue(void* var, nmType_t* type);

void nmEnvExecute(nmEnvironment_t* env);
void nmEnvSetStreams(nmEnvironment_t* env, FILE* in, FILE* out);
void nmEnvDump(nmEnvironment_t* env, FILE* f);
nmEnvironment_t* nmEnvCreate(nmProgram_t* program);
void nmEnvFree(nmEnvironment_t* env);
void setCurrentEnv(nmEnvironment_t* env);
double nmGetExecTime(nmEnvironment_t* env);

#endif //NMRUNNER_ENVIRONMENT_H
