//
// Created by maxim on 6/30/19.
//

#include <math.h>
#include "environment.h"

void setDefaultValue(void* var, nmType_t* type)
{
    switch(type->typeSignature)
    {
        case tInteger:
        {
            switch(type->typeBase)
            {
                case 1:
                    *(int8_t*)var = 0;
                    break;
                case 2:
                    *(int16_t*)var = 0;
                    break;
                case 4:
                    *(int32_t*)var = 0;
                    break;
                case 8:
                    *(int64_t*)var = 0l;
                    break;
            }
            break;
        }
        case tUInteger:
        {
            switch(type->typeBase)
            {
                case 1:
                    *(uint8_t*)var = 0u;
                    break;
                case 2:
                    *(uint16_t*)var = 0u;
                    break;
                case 4:
                    *(uint32_t*)var = 0u;
                    break;
                case 8:
                    *(uint64_t*)var = 0ul;
                    break;
            }
            break;
        }
        case tFloat:
        {
            switch(type->typeBase)
            {
                case 4:
                    *(float*)var = 0.0f;
                    break;
                case 8:
                    *(double*)var = 0.0;
                    break;
            }
            break;
        }
        case tString:
        {
            //hm...
            break;
        }
    }
}

void nmEnvExecute(nmEnvironment_t* env)
{
    nmCallableFunction_t** functions =  env->callableFunctions;
    nmCallableInstruction_t* current;

    uint32_t pc = 0;
    uint32_t func = env->program->entryPointFuncIndex;

    pushStack(env->callStack, &func);

    env->programCounter = &pc;
    env->funcIndex = &func;

    while(pc != functions[func]->instructionsCount)
    {
        current = functions[func]->callableInstructions[pc];
        current->function(env, current->parameters);
        pc++;
    }

    popStack(env->callStack);

    env->programCounter = NULL;
    env->funcIndex = NULL;
}

void nmEnvSetStreams(nmEnvironment_t* env, FILE* in, FILE* out)
{
    env->ins = in;
    env->outs = out;
}
int getFuncIndexByType(nmType_t* type)
{
    switch (type->typeSignature)
    {
        case tInteger:
        {
            switch (type->typeBase)
            {
                case 1: return 0;
                case 2: return 1;
                case 4: return 2;
                case 8: return 3;
            }
            break;
        }
        case tUInteger:
        {
            switch (type->typeBase)
            {
                case 1: return 4;
                case 2: return 5;
                case 4: return 6;
                case 8: return 7;
            }
            break;
        }
        case tFloat:
        {
            switch (type->typeBase)
            {
                case 4: return 8;
                case 8: return 9;
            }
        }
        case tString:
            break;
    }
    return -1;
}

int getFunctionIndex(nmCallableFunction_t* func, nmProgram_t* program, nmInstruction_t* instr)
{
    nmInstructionData_t* data = instr->dataPtr;

    //ret, jmp, call
    if(data->index == 0x1 || data->index == 0x5 || data->index == 0x6) return 0; 
    
    if(data->index == 0x2 || data->index == 0x7)
    {
        uint64_t flag = instr->parameters[0];
        if(flag)
        {
            //var
            uint64_t variableIndex = instr->parameters[1];
            return getFuncIndexByType(func->localTypes[variableIndex]);
        }
        else
        {
            //const
            uint64_t constIndex = instr->parameters[1];
            return getFuncIndexByType(program->constants[constIndex]->typePtr);
        }
    }

    uint64_t variableIndex = instr->parameters[0];
    return getFuncIndexByType(func->localTypes[variableIndex]);
}

void nmEnvDump(nmEnvironment_t* env, FILE* f)
{
    fputs("===Memory Dump===\n\n", f);

    for(size_t i = 0; i < env->program->funcCount; i++)
    {
        fprintf(f, "Variables of function #%i\n", env->program->functions[i]->index);
        for(size_t j = 0; j < env->callableFunctions[i]->localsCount; j++)
        {
            uint8_t isLocal = j < env->program->functions[i]->localsCount;
            nmType_t* type = env->callableFunctions[i]->localTypes[j];

            char* sValue = nmConstantToStr(
                env->callableFunctions[i]->locals[j], 
                type);

            if(env->program->sourceFilename)
            {
                if(isLocal)
                {
                    fprintf(f, " - \"%s\" (%li) at %i:%i: local of type %i. Value: %s (%p)\n",
                            env->program->functions[i]->variableNames[j],
                            j,
                            env->program->functions[i]->variableSourceLineIndices[j],
                            env->program->functions[i]->variableSourceCharIndices[j],
                            type->typeIndex,
                            sValue,
                            env->callableFunctions[i]->locals[j]);
                }
                else
                {
                    fprintf(f, " - (%li): register of type %i. Value: %s (%p)\n",
                            j,
                            type->typeIndex,
                            sValue,
                            env->callableFunctions[i]->locals[j]);
                }
            }
            else
            {
                fprintf(f, " - %li: is %s of type %i. Value: %s (%p)\n",
                        j,
                        isLocal ? "local" : "register",
                        type->typeIndex,
                        sValue,
                        env->callableFunctions[i]->locals[j]);
            }
            free(sValue);
        }
    }
    fputc('\n', f);

    fprintf(f, "Variable Stack: ");
    printStack(env->variableStack, f);
    fprintf(f, "Call Stack: ");
    printStack(env->callStack, f);
    fprintf(f, "PC Stack: ");
    printStack(env->pcStack, f);
    fputc('\n', f);

}

nmEnvironment_t* nmEnvCreate(nmProgram_t* program)
{
    nmEnvironment_t* env = malloc(sizeof(nmEnvironment_t));
    env->program = program;
    env->funcIndex = NULL;
    env->programCounter = NULL;
    
    env->pcStack = createStack();
    env->callStack = createStack();
    env->variableStack = createStack();
    env->callableFunctions = malloc(sizeof(nmCallableFunction_t*) * program->funcCount);

    for(size_t i = 0; i < program->funcCount; i++)
    {
        void** locals = malloc(sizeof(void*) * (program->functions[i]->localsCount + program->functions[i]->regCount));
        nmType_t** localTypes = malloc(sizeof(void*) * (program->functions[i]->localsCount + program->functions[i]->regCount));
        for(size_t localIndex = 0; localIndex < program->functions[i]->localsCount; localIndex++)
        {
            nmType_t* type = program->usedTypes[program->functions[i]->localTypes[localIndex]];
            locals[localIndex] = malloc(sizeof(type->typeBase));
            localTypes[localIndex] = type;
            setDefaultValue(locals[localIndex], type);
        }
        for(size_t regIndex = 0; regIndex < program->functions[i]->regCount; regIndex++)
        {
            nmType_t* type = program->usedTypes[program->functions[i]->regTypes[regIndex]];
            locals[regIndex + program->functions[i]->localsCount] = malloc(sizeof(type->typeBase));
            localTypes[regIndex + program->functions[i]->localsCount] = type;
            setDefaultValue(locals[regIndex + program->functions[i]->localsCount], type);
        }

        env->callableFunctions[i] = malloc(sizeof(nmCallableFunction_t));
        env->callableFunctions[i]->localsCount = program->functions[i]->localsCount + program->functions[i]->regCount;
        env->callableFunctions[i]->localTypes = localTypes;
        env->callableFunctions[i]->locals = locals;

        env->callableFunctions[i]->callableInstructions =
                malloc(sizeof(nmCallableInstruction_t*) * program->functions[i]->instructionsCount);
        env->callableFunctions[i]->instructionsCount = program->functions[i]->instructionsCount;

        for(size_t instrIndex = 0; instrIndex < program->functions[i]->instructionsCount; instrIndex++)
        {
            size_t count = getOperandsCount(program->functions[i]->instructions[instrIndex]->dataPtr);
            env->callableFunctions[i]->callableInstructions[instrIndex] = malloc(sizeof(nmCallableInstruction_t));
            env->callableFunctions[i]->callableInstructions[instrIndex]->function =
                    program->functions[i]->instructions[instrIndex]->dataPtr->function[
                        getFunctionIndex(
                            env->callableFunctions[i],
                            program,
                            program->functions[i]->instructions[instrIndex])];

            env->callableFunctions[i]->callableInstructions[instrIndex]->paramCount = count;
            env->callableFunctions[i]->callableInstructions[instrIndex]->parameters = malloc(sizeof(uint64_t) * count);

            uint8_t varConstFlagValue = 0;
            size_t counter = 0;

            for(size_t parameterIndex = 0; parameterIndex < count; parameterIndex++)
            {
                switch (program->functions[i]->instructions[instrIndex]->dataPtr->parameterTypes[parameterIndex])
                {
                    case varConstFlag:
                    {
                        env->callableFunctions[i]->callableInstructions[instrIndex]->paramCount--;
                        varConstFlagValue = program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
                        break;
                    }
                    case varIndex:
                    {
                        env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                locals[program->functions[i]->instructions[instrIndex]->parameters[parameterIndex]];
                        break;
                    }
                    case varConstIndex:
                    {
                        if(varConstFlagValue == 0)
                        {
                            env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                    locals[program->functions[i]->instructions[instrIndex]->parameters[parameterIndex]];
                        }
                        else
                        {
                            env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                    program->constants[program->functions[i]->
                                        instructions[instrIndex]->parameters[parameterIndex]]->value;
                        }
                        break;
                    }
                    case functionIndex:
                    {
                        *(uint64_t*)env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++]
                            = program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
                        break;
                    }
                    case jumpIndex:
                    {
                        *(uint64_t*)env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++]
                                = program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
                        break;
                    }
                }
            }
        }
    }

    return env;
}

void nmEnvFree(nmEnvironment_t* env)
{
    freeStack(env->variableStack);
    freeStack(env->pcStack);
    freeStack(env->callStack);
    for(size_t i = 0; i < env->program->funcCount; i++)
    {
        for(size_t j = 0; j < env->callableFunctions[i]->instructionsCount; j++)
        {
            free(env->callableFunctions[i]->callableInstructions[j]->parameters);
            free(env->callableFunctions[i]->callableInstructions[j]);
        }
        free(env->callableFunctions[i]->callableInstructions);
        for(size_t j = 0; j < env->callableFunctions[i]->localsCount; j++)
        {
            free(env->callableFunctions[i]->locals[j]);
        }

        free(env->callableFunctions[i]->locals);
        free(env->callableFunctions[i]->localTypes);
        free(env->callableFunctions[i]);
    }
    free(env->callableFunctions);
    free(env);
}