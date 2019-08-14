//
// Created by maxim on 6/30/19.
//

#include "environment.h"

nmEnvironment_t* currentEnv;

double nmGetExecTime(nmEnvironment_t* env)
{
    return 1000.0 * (double)(env->execEndTime - env->execStartTime) / (double)CLOCKS_PER_SEC;
}

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

    setCurrentEnv(env);

    pushStack(env->callStack, (void*)func);
    pushStack(env->pcStack, (void*)functions[func]->instructionsCount);

    env->programCounter = &pc;
    env->funcIndex = &func;

    env->execStartTime = clock();

    while(pc < functions[func]->instructionsCount)
    {
        current = functions[func]->callableInstructions[pc];
        current->function(env, current->parameters);
        pc++;
    }

    env->execEndTime = clock();

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

nmType_t* getTypeByIndex(nmCallableFunction_t* func, nmProgram_t* program, uint64_t index)
{
    if(index < program->globalsCount)
        return program->usedTypes[program->globalsTypes[index]];
    else
        return func->localTypes[index - program->globalsCount];
}

instructionFunction_t getInstructionFunction(nmCallableFunction_t* func, nmProgram_t* program, nmInstruction_t* instr)
{
    nmInstructionData_t* data = instr->dataPtr;

    //ret, jmp, call, syscall
    if(data->index == 0x1 || data->index == 0x5 || data->index == 0x6 || data->index == 0xB)
        return data->function[0]; //doesn't really care

    //push breq
    if(data->index == 0x2 || data->index == 0x7)
    {
        uint64_t flag = instr->parameters[0];
        if(!flag)
        {
            //var
            uint64_t variableIndex = instr->parameters[1];
            if((int32_t)variableIndex < 0)
            {
                return data->function[2];
            }
            else
            {
                return data->function[getFuncIndexByType(getTypeByIndex(func, program, variableIndex))];
            }
        }
        else
        {
            //const
            uint64_t constIndex = instr->parameters[1];
            return data->function[getFuncIndexByType(program->constants[constIndex]->typePtr)];
        }
    }

    //cast
    if(data->index == 0x8)
    {
        uint64_t index1 = getFuncIndexByType(getTypeByIndex(func, program, instr->parameters[0]));
        uint64_t index2;
        uint64_t flag = instr->parameters[1];
        if(!flag)
        {
            //var
            index2 = getFuncIndexByType(getTypeByIndex(func, program, instr->parameters[2]));
        }
        else //const
            index2 = getFuncIndexByType(program->constants[instr->parameters[2]]->typePtr);

        return castFunctions[index1 * 10 + index2]; // =3
    }

    uint64_t variableIndex = instr->parameters[0];
    return data->function[getFuncIndexByType(getTypeByIndex(func, program, variableIndex))];
}

void nmEnvDump(nmEnvironment_t* env, FILE* f)
{
    fputs("===Memory Dump===\n\n", f);

    fprintf(f, "Globals: \n");
    for(size_t i = 0; i < env->program->globalsCount; i++)
    {
        nmType_t* type = getTypeByIndex(NULL, env->program, env->program->globalsTypes[i]);
        char* sValue = nmConstantToStr(env->globals[i], type);

        if(env->program->sourceFilename)
        {
            fprintf(f, " - \"%s\" (%li) at %i:%i: global of type %i. Value: %s (%p)\n",
                    env->program->globalsNames[i],
                    i,
                    env->program->globalsSourceLineIndices[i],
                    env->program->globalsSourceCharIndices[i],
                    type->typeIndex,
                    sValue,
                    env->globals[i]);
        }
        else
        {
            fprintf(f, " - %li: global of type %i. Value: %s (%p)\n",
                    i,
                    type->typeIndex,
                    sValue,
                    env->globals[i]);
        }
    }

    for(size_t i = 0; i < env->program->funcCount; i++)
    {
        if(env->program->sourceFilename)
            fprintf(f, "Variables of function \"%s\" at %i:%i (#%i)\n",
                    env->program->functions[i]->name, env->program->functions[i]->sourceLineIndex,
                    env->program->functions[i]->sourceCharIndex, env->program->functions[i]->index);
        else
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

#include "subroutines.h"
nmEnvironment_t* nmEnvCreate(nmProgram_t* program)
{
    if(!subroutines)
    {
        puts("You should initialize subroutines list first");
        abort();
    }

    nmEnvironment_t* env = malloc(sizeof(nmEnvironment_t));
    env->program = program;
    env->funcIndex = NULL;
    env->programCounter = NULL;
    
    env->pcStack = createStack();
    env->callStack = createStack();
    env->variableStack = createStack();
    env->callableFunctions = malloc(sizeof(nmCallableFunction_t*) * program->funcCount);

    for(size_t i = 0; i < env->program->usedTypesCount; i++)
    {
        env->program->usedTypes[i]->funcIndex =
                getFuncIndexByType(env->program->usedTypes[i]);
    }

    //allocating globals
    env->globals = malloc(sizeof(void*) * program->globalsCount);
    for(size_t i = 0; i < program->globalsCount; i++)
        env->globals[i] = malloc(sizeof(program->globalsTypes[i]));

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
            if(program->functions[i]->instructions[instrIndex]->dataPtr->index == 0xB) //syscall
            {
                uint32_t index = program->functions[i]->instructions[instrIndex]->parameters[0];
                if(!hasSubroutine(index))
                {
                    printf("Unknown subroutine with index 0x%X\n", index);
                    abort();
                }
            }

            size_t count = getOperandsCount(program->functions[i]->instructions[instrIndex]->dataPtr);
            env->callableFunctions[i]->callableInstructions[instrIndex] = malloc(sizeof(nmCallableInstruction_t));
            env->callableFunctions[i]->callableInstructions[instrIndex]->function =
                        getInstructionFunction(
                                env->callableFunctions[i],
                                program,
                                program->functions[i]->instructions[instrIndex]);

            assert(env->callableFunctions[i]->callableInstructions[instrIndex]->function);

            env->callableFunctions[i]->callableInstructions[instrIndex]->paramCount = count;
            env->callableFunctions[i]->callableInstructions[instrIndex]->parameters = malloc(sizeof(uint64_t) * count);

            uint8_t varConstFlagValue = 0;
            size_t counter = 0;

            //push
            if(program->functions[i]->instructions[instrIndex]->dataPtr->index == 0x2)
            {
                if((int32_t)program->functions[i]->instructions[instrIndex]->parameters[1] < 0)
                {
                    env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[0] =
                            (void*)(-(int32_t)program->functions[i]->instructions[instrIndex]->parameters[1] - 1);
                    continue;
                }
            }

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
                        //could be local or global
                        uint64_t index = program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
                        env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                index < program->globalsCount ? env->globals[index] : locals[index - program->globalsCount];
                        break;
                    }
                    case varConstIndex:
                    {
                        if(varConstFlagValue == 0)
                        {
                            //could be local or global
                            uint64_t index = program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
                            env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++] =
                                    index < program->globalsCount ? env->globals[index] : locals[index - program->globalsCount];
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
                    case jumpIndex:
                    {
                        env->callableFunctions[i]->callableInstructions[instrIndex]->parameters[counter++]
                                = (void*)program->functions[i]->instructions[instrIndex]->parameters[parameterIndex];
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

void setCurrentEnv(nmEnvironment_t* env)
{
    currentEnv = env;
}