//
// Created by maxim on 6/29/19.
//

#include "coretypes.h"

void nmProgramFree(nmProgram_t* program)
{
    for(size_t i = 0; i < program->usedTypesCount; i++)
        free(program->usedTypes[i]);
    free(program->usedTypes);

    for(size_t i = 0; i < program->constantCount; i++)
    {
        free(program->constants[i]->value);
        free(program->constants[i]);
    }
    free(program->constants);

    for(size_t i = 0; i < program->importCount; i++)
    {
        free(program->imports[i]->moduleName);
        free(program->imports[i]);
    }
    free(program->imports);

    free(program->metadata->binaryAuthor);
    free(program->metadata->binaryName);
    free(program->metadata->binaryDescription);
    free(program->metadata);

    if(program->sourceFilename)
        free(program->sourceFilename);

    for(size_t i = 0; i < program->funcCount; i++)
    {
        free(program->functions[i]->localTypes);
        free(program->functions[i]->regTypes);
        for(size_t j = 0; j < program->functions[i]->instructionsCount; j++)
        {
            free(program->functions[i]->instructions[j]->parameters);
            free(program->functions[i]->instructions[j]);
        }
        free(program->functions[i]->instructions);

        if(program->functions[i]->name)
        {
            free(program->functions[i]->name);
            for(size_t j = 0; j < program->functions[i]->localsCount; j++)
                free(program->functions[i]->variableNames[j]);
            free(program->functions[i]->variableNames);
            free(program->functions[i]->variableSourceLineIndices);
            free(program->functions[i]->variableSourceCharIndices);
        }

        free(program->functions[i]);
    }    
    free(program->functions);
    free(program);
}

char* nmConstantToStr(void* value, nmType_t* type)
{
    char* result = NULL;
    switch(type->typeSignature)
    {
        case tInteger:
        {
            int64_t i = 0;
            switch(type->typeBase)
            {
                case 1: i = *(int8_t*)value;
                    break;
                case 2: i = *(int16_t*)value;
                    break;
                case 4: i = *(int32_t*)value;
                    break;
                case 8: i = *(int64_t*)value;
                    break;
            }
            result = malloc(10);
            snprintf(result, 10, "%li", i);
            break;
        }
        case tUInteger:
        {
            uint64_t i = 0;
            switch(type->typeBase)
            {
                case 1: i = *(uint8_t*)value;
                    break;
                case 2: i = *(uint16_t*)value;
                    break;
                case 4: i = *(uint32_t*)value;
                    break;
                case 8: i = *(uint64_t*)value;
                    break;
            }
            result = malloc(10);
            snprintf(result, 10, "%lu", i);
            break;
        }
        case tFloat:
        {
            double i = 0;
            switch(type->typeBase)
            {
                case 4:
                    i = *(float*)value;
                    break;
                case 8:
                    i = *(double*)value;
                    break;
            }
            result = malloc(10);
            snprintf(result, 10, "%lf", i);
            break;
        }
        case tString:
        default:
            return NULL;
    }
    return result;
}

const char* nmTypeSignatureToStr(nmTypeSignature_t signature)
{
    switch(signature)
    {
        case tInteger:
            return "integer";
        case tUInteger:
            return "uint";
        case tFloat:
            return "float";
        case tString:
            return "string";
        default:
            return "none";
    }
}

void nmProgramPrint(nmProgram_t* program, FILE* f)
{
    fprintf(f, "NmVersion: %i\n", program->nmVersion);

    if(program->sourceFilename)
        fprintf(f, "Source FileName: %s\n", program->sourceFilename);

    fprintf(f, "Metadata: ");
    if(program->metadata == NULL)
        fprintf(f, "none");
    else
    {
        putc('\n', f);
        fprintf(f, " - Binary Name: %s\n", program->metadata->binaryName);
        fprintf(f, " - Binary Author: %s\n", program->metadata->binaryAuthor);
        fprintf(f, " - Binary Name: %s\n", program->metadata->binaryDescription);
        fprintf(f, " - Binary Compile time: %i:%i:%i %i.%i.%i\n",
                program->metadata->compileDateTime.hour,
                program->metadata->compileDateTime.minute,
                program->metadata->compileDateTime.second,
                program->metadata->compileDateTime.day,
                program->metadata->compileDateTime.month,
                program->metadata->compileDateTime.year);
        fprintf(f, " - Binary Version: %i.%i\n",
                program->metadata->majorVersion,
                program->metadata->minorVersion);
    }

    fprintf(f, "Imports (%i): \n", program->importCount);
    for(size_t i = 0; i < program->importCount; i++)
    {
        fprintf(f, " - \"%s\" (%s)\n",
                program->imports[i]->moduleName,
                program->imports[i]->isLib ? "lib" : "user module");
    }

    fprintf(f, "Constants (%i): \n", program->constantCount);
    for(size_t i = 0; i < program->constantCount; i++)
    {
        char* cValue = nmConstantToStr(
            program->constants[i]->value,
            program->constants[i]->typePtr);

        fprintf(f, " - %i:%i:%s\n",
                program->constants[i]->typeIndex,
                program->constants[i]->len,
                cValue);
        free(cValue);
    }

    fprintf(f, "Types (%i): \n", program->usedTypesCount);
    for(size_t i = 0; i < program->usedTypesCount; i++)
    {
        fprintf(f, " - %i:%s:%i\n",
                program->usedTypes[i]->typeIndex,
                nmTypeSignatureToStr(program->usedTypes[i]->typeSignature),
                program->usedTypes[i]->typeBase * 8);
    }
    fprintf(f, "Functions (%i): \n", program->funcCount);
    for(size_t i = 0; i < program->funcCount; i++)
    {
        if(program->functions[i]->name)
            fprintf(f, "- Function \"%s\" at %i:%i (#%i)\n",
                    program->functions[i]->name,
                    program->functions[i]->sourceLineIndex, program->functions[i]->sourceCharIndex,
                    program->functions[i]->index);
        else
            fprintf(f, "- Function#%i\n", program->functions[i]->index);

        fprintf(f, "  - Registers (%i): ", program->functions[i]->regCount);
        if(program->functions[i]->regCount == 0) fputc('\n', f);
        for(size_t j = 0; j < program->functions[i]->regCount; j++)
        {
            fprintf(f, "%i%s", program->functions[i]->regTypes[j],
                    j == program->functions[i]->regCount - 1 ? "\n" : ", ");
        }

        fprintf(f, "  - Locals (%i): ", program->functions[i]->localsCount);
        if (program->functions[i]->localsCount == 0) fputc('\n', f);
        if(program->functions[i]->name)
        {
            fputc('\n', f);
            for (size_t j = 0; j < program->functions[i]->localsCount; j++)
            {
                fprintf(f, "    - \"%s\" at %i:%i of type %i%s\n",
                        program->functions[i]->variableNames[j],
                        program->functions[i]->variableSourceLineIndices[j],
                        program->functions[i]->variableSourceCharIndices[j],
                        program->functions[i]->localTypes[j],
                        j == program->functions[i]->localsCount - 1 ? "\n" : ", ");
            }
        }
        else
        {
            for (size_t j = 0; j < program->functions[i]->localsCount; j++)
            {
                fprintf(f, "%i%s", program->functions[i]->localTypes[j],
                        j == program->functions[i]->localsCount - 1 ? "\n" : ", ");
            }
        }
        fprintf(f, "  - Instructions (%i): \n", program->functions[i]->instructionsCount);
        for(size_t j = 0; j < program->functions[i]->instructionsCount; j++)
        {
            fprintf(f, "    - %s ", program->functions[i]->instructions[j]->dataPtr->name);
            size_t count = getOperandsCount(program->functions[i]->instructions[j]->dataPtr);

            for(size_t k = 0; k < count; k++)
            {
                const char* separator = k == count - 1 ? "\n" : ", ";
                uint64_t value = program->functions[i]->instructions[j]->parameters[k];

                switch(program->functions[i]->instructions[j]->dataPtr->parameterTypes[k])
                {
                    case varConstFlag:
                        fprintf(f, "%li(varCnstFlag)%s", value, separator);
                        break;
                    case varIndex:
                        fprintf(f, "%li(varIndex)%s", value, separator);
                        break;
                    case varConstIndex:
                        fprintf(f, "%li(varCnstIndex)%s", value, separator);
                        break;
                    case functionIndex:
                        fprintf(f, "%li(fncIndex)%s", value, separator);
                        break;
                    case jumpIndex:
                        fprintf(f, "%li(jmpIndex)%s", value, separator);
                        break;
                }
            }
        }
        putc('\n', f);
    }
}