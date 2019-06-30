//
// Created by maxim on 6/30/19.
//

#include "environment.h"

void nmEnvExecute(nmEnvironment_t* env)
{
    nmCallableFunction_t** functions =  env->callableFunctions;
    nmCallableInstruction_t* current;

    uint32_t pc = 0;
    uint32_t func = env->program->entryPointFuncIndex;

    env->programCounter = &pc;
    env->funcIndex = &func;

    while(pc != functions[func]->instructionsCount)
    {
        current = functions[func]->callableInstructions[pc];
        current->function(env, current->parameters);
        pc++;
    }

    env->programCounter = NULL;
    env->funcIndex = NULL;
}

void nmEnvSetStreams(nmEnvironment_t* env, FILE* in, FILE* out)
{
    env->ins = in;
    env->outs = out;
}

void nmEnvDump(nmEnvironment_t* env, FILE* f)
{
    for(size_t i = 0; i < env->program->funcCount; i++)
    {
        fprintf(f, "Variables of function #%i\n", env->program->functions[i]->index);
        for(size_t j = 0; j < 
            env->program->functions[i]->localsCount + 
            env->program->functions[i]->regCount; 
            j++)
        {
            uint8_t isLocal = j <= env->program->functions[i]->localsCount;
            nmType_t* type = env->program->usedTypes[
                    isLocal ? 
                        env->program->functions[i]->localTypes[j] : 
                        env->program->functions[i]->regTypes[j]
                ];

            char* sValue = nmConstantToStr(
                env->callableFunctions[i]->locals[j], 
                type);
            
            fprintf(f, " - %li: is %s of type %i. Value: %s (%p)\n", 
                j,
                isLocal ? "local" : "register",
                type->typeIndex,
                sValue,
                env->callableFunctions[i]->locals[j]);

            free(sValue);
        }
        
    }
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
        for(size_t localIndex = 0; localIndex < program->functions[i]->localsCount; localIndex++)
        {
            nmType_t* type = program->usedTypes[program->functions[i]->localTypes[localIndex]];
            locals[localIndex] = malloc(sizeof(type->typeBase));
        }
        for(size_t regIndex = 0; regIndex < program->functions[i]->regCount; regIndex++)
        {
            nmType_t* type = program->usedTypes[program->functions[i]->regTypes[regIndex]];
            locals[regIndex] = malloc(sizeof(type->typeBase));
        }

        env->callableFunctions[i] = malloc(sizeof(nmCallableFunction_t));
        env->callableFunctions[i]->locals = locals;
        env->callableFunctions[i]->callableInstructions =
                malloc(sizeof(nmCallableInstruction_t*) * program->functions[i]->instructionsCount);
        env->callableFunctions[i]->instructionsCount = program->functions[i]->instructionsCount;

        for(size_t instrIndex = 0; instrIndex < program->functions[i]->instructionsCount; instrIndex++)
        {
            size_t count = getOperandsCount(program->functions[i]->instructions[instrIndex]->dataPtr);
            env->callableFunctions[i]->callableInstructions[instrIndex] = malloc(sizeof(nmCallableInstruction_t));
            env->callableFunctions[i]->callableInstructions[instrIndex]->function =
                    program->functions[i]->instructions[instrIndex]->dataPtr->function;
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
                        env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                &program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
                        break;
                    }
                    case jumpIndex:
                    {
                        env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                &program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
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

}