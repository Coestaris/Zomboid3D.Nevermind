//
// Created by maxim on 6/21/19.
//

#include "parser.h"

uint16_t getChunkType(const uint8_t array[2]) 
{
    return array[1] << 8 | array[0];
}

nmProgram_t* nmParserLoad(FILE* file)
{
    uint8_t buffer[sizeof(nmbSignature)];
    if(fread(buffer, sizeof(nmbSignature), 1, file) != 1)
    {
        nmPushError("Unable to read file signature");
        return NULL;
    }

    for(size_t i = 0; i < sizeof(nmbSignature); i++)
        if(buffer[i] != nmbSignature[i])
        {
            nmPushError("Wrong file signature");
            return NULL;
        }

    nmProgram_t* program = malloc(sizeof(nmProgram_t));
    program->sourceFilename = NULL;

    uint16_t chunkCount;
    if(fread(&chunkCount, sizeof(chunkCount), 1 ,file) != 1)
    {
        nmPushError("Unable to read chunk count");
        free(program);
        return NULL;
    }

    for(size_t i = 0; i < chunkCount; i++)
    {
        uint32_t len;
        uint32_t crc;
        uint16_t type;
        uint8_t* dataBuffer;

        if(fread(&len, sizeof(len), 1 ,file) != 1)
        {
            nmPushError("Unable to read chunk length from file");
            free(program);
            return NULL;
        }

        if(fread(&crc, sizeof(crc), 1, file) != 1)
        {
            nmPushError("Unable to read chunk crc from file");
            free(program);
            return NULL;
        }

        if(fread(&type, sizeof(type), 1, file) != 1)
        {
            nmPushError("Unable to read chunk type from file");
            free(program);
            return NULL;
        }

        dataBuffer = malloc(sizeof(uint8_t) * len);

        if(fread(dataBuffer, sizeof(uint8_t) * len, 1, file) != 1)
        {
             nmPushError("Unable to read data from file");
             free(program);
             free(dataBuffer);
             return NULL;
        }

        uint32_t dataCrc = crc32(dataBuffer, len);
        if(dataCrc != crc)
        {
            nmPushErrorf("CRCs doesnt match. Calculated %X but expected %X", dataCrc, crc);
            free(program);
            free(dataBuffer);
            return NULL;
        }

        chunkHanlder_t* handler = NULL;
        for(size_t i = 0; i < chunkHanldersCount; i++)
            if(getChunkType(chunkHanlders[i].chunktype) == type)
            {
                handler = (chunkHanlder_t*)(&chunkHanlders[i]);
                break;
            }

        if(handler == NULL)
        {
            nmPushErrorf("Unknown chunk type: %c%c", type & 0xFF, (type >> 8) & 0xFF);
            free(program);
            free(dataBuffer);
            return NULL;
        }

        printf("[%li/%i]: Found chunk %c%c\n", i + 1, chunkCount, type & 0xFF, (type >> 8) & 0xFF);


        FILE* memStream = fmemopen(dataBuffer, len, "r");  
        if(!memStream)
        {
            nmPushError("Unable to open memstream");
            free(program);
            free(dataBuffer);
            return NULL;
        }

        if(!handler->hanlder(program, memStream))
        {
            nmPushError("Unable to parse chunk");
            free(program);
            fclose(memStream);
            free(dataBuffer);
            return NULL;
        }

        fclose(memStream);
        free(dataBuffer);
    }

    return program;
}

nmProgram_t* nmParserFromFile(const char* filename)
{
    FILE* file = fopen(filename, "rb");
    if(!file)
    {
        nmPushErrorf("Unable to open file %s", filename);
        return NULL;
    }

    nmProgram_t* program = nmParserLoad(file);

    if(fclose(file) == EOF)
    {
        nmPushErrorf("Unable to close file %s", filename);
        return NULL;
    }

    return program;
}

#define readStr(ptr, lenType) {                                              \
    lenType len;                                                             \
    if(fread(&len, sizeof(lenType), 1, file) != 1) return 0;                 \
    ptr = malloc(sizeof(uint8_t) * len + 1);                                 \
    if(fread(ptr, sizeof(uint8_t) * len, 1, file) != 1) return 0;            \
    ptr[len] = '\0';                                                         \
}

#define readValue(ptr) if(fread(&ptr, sizeof(ptr), 1, file) != 1) return 0;

uint8_t chunkhandler_header(nmProgram_t* program, FILE* file)
{
    readValue(program->nmVersion);

    if(fread(program->sourceHash, sizeof(uint8_t) * 16, 1, file) != 1) return 0;

    readValue(program->importCount);
    readValue(program->funcCount);
    readValue(program->entryPointFuncIndex);

    program->imports = malloc(sizeof(nmImport_t*) * program->importCount);
    program->functions = malloc(sizeof(nmFunction_t*) * program->funcCount);

    for(size_t i = 0; i < program->importCount; i++)
    {
        program->imports[i] = malloc(sizeof(nmImport_t));
        readValue(program->imports[i]->isLib);
        readStr(program->imports[i]->moduleName, uint32_t);
    }    

    return 1;
}

uint8_t chunkhandler_metadata(nmProgram_t* program, FILE* file)
{
    program->metadata = malloc(sizeof(nmMetadata_t));
    //Date
    readValue(program->metadata->compileDateTime.second);
    readValue(program->metadata->compileDateTime.minute);
    readValue(program->metadata->compileDateTime.hour);
    readValue(program->metadata->compileDateTime.day);
    readValue(program->metadata->compileDateTime.month);
    readValue(program->metadata->compileDateTime.year);

    readStr(program->metadata->binaryName, uint16_t);
    readStr(program->metadata->binaryDescription, uint16_t);
    readStr(program->metadata->binaryAuthor, uint16_t);
    
    readValue(program->metadata->minorVersion);
    readValue(program->metadata->majorVersion);

    return 1;
}

uint8_t chunkhandler_types(nmProgram_t* program, FILE* file)
{
    uint32_t count;
    readValue(count);
    program->usedTypesCount = count;
    program->usedTypes = malloc(sizeof(nmType_t*) * count);
    for(size_t i = 0; i < count; i++)
    {
        program->usedTypes[i] = malloc(sizeof(nmType_t));
        uint16_t signature;
        readValue(signature);
        program->usedTypes[i]->typeSignature = signature;

        readValue(program->usedTypes[i]->typeBase);
        program->usedTypes[i]->typeIndex = i;
        program->usedTypes[i]->typeBase /= 8;
    }
    return 1;
}

uint8_t chunkhandler_constants(nmProgram_t* program, FILE* file)
{
    uint32_t count;
    readValue(count);
    program->constantCount = count;
    program->constants = malloc(sizeof(nmConstant_t*) * count);
    for(size_t i = 0; i < count; i++)
    {
        program->constants[i] = malloc(sizeof(nmConstant_t));
        readValue(program->constants[i]->typeIndex);
        readValue(program->constants[i]->len);
        
        program->constants[i]->typePtr = program->usedTypes[program->constants[i]->typeIndex];
        size_t valueLen = program->constants[i]->typePtr->typeBase * program->constants[i]->len;
        program->constants[i]->value = malloc(valueLen);
        
        if(fread(program->constants[i]->value, valueLen, 1, file) != 1) return 0;
    }
    return 1;
}

uint8_t chunkhandler_functions(nmProgram_t* program, FILE* file)
{
    uint32_t funcIndex;
    readValue(funcIndex);
    program->functions[funcIndex] = malloc(sizeof(nmFunction_t));
    program->functions[funcIndex]->index = funcIndex;
    readValue(program->functions[funcIndex]->instructionsCount);
    readValue(program->functions[funcIndex]->localsCount);

    program->functions[funcIndex]->variableNames = NULL;
    program->functions[funcIndex]->variableSourceCharIndices = NULL;
    program->functions[funcIndex]->variableSourceLineIndices = NULL;
    program->functions[funcIndex]->name = NULL;

    program->functions[funcIndex]->localTypes = malloc(
        sizeof(uint32_t) * program->functions[funcIndex]->localsCount);
    for(size_t i = 0; i < program->functions[funcIndex]->localsCount; i++)
        readValue(program->functions[funcIndex]->localTypes[i]);

    readValue(program->functions[funcIndex]->regCount);
    program->functions[funcIndex]->regTypes = malloc(
        sizeof(uint32_t) * program->functions[funcIndex]->regCount);
    for(size_t i = 0; i < program->functions[funcIndex]->regCount; i++)
        readValue(program->functions[funcIndex]->regTypes[i]);

    program->functions[funcIndex]->instructions = malloc(
        sizeof(nmInstruction_t*) * program->functions[funcIndex]->instructionsCount);
    for(size_t i = 0; i < program->functions[funcIndex]->instructionsCount; i++)
    {
        program->functions[funcIndex]->instructions[i] = malloc(sizeof(nmInstruction_t));
        uint16_t index;
        readValue(index);

        program->functions[funcIndex]->instructions[i]->dataPtr = getInstructionData(index);

        if(!program->functions[funcIndex]->instructions[i]->dataPtr)
        {
            return 0;
        }

        size_t count = getOperandsCount(program->functions[funcIndex]->instructions[i]->dataPtr);
        program->functions[funcIndex]->instructions[i]->parameters = malloc(sizeof(uint64_t) * count);
        for(size_t j = 0; j < count; j++)
        {
            program->functions[funcIndex]->instructions[i]->parameters[j] = 0;
            if(fread(&program->functions[funcIndex]->instructions[i]->parameters[j],
                    getOperandSize(program->functions[funcIndex]->instructions[i]->dataPtr->parameterTypes[j]), 1, file) != 1) return 0;
        }
    }

    return 1;
}

uint8_t chunkhandler_debug(nmProgram_t* program, FILE* file)
{
    readStr(program->sourceFilename, uint32_t);

    program->globalsNames = malloc(sizeof(char*) * program->globalsCount);
    program->globalsSourceLineIndices = malloc(sizeof(uint32_t) * program->globalsCount);
    program->globalsSourceCharIndices = malloc(sizeof(uint32_t) * program->globalsCount);
    for(size_t i = 0; i < program->globalsCount; i++)
    {
        readStr  (program->globalsNames[i], uint32_t);
        readValue(program->globalsSourceLineIndices[i]);
        readValue(program->globalsSourceCharIndices[i]);
    }

    for(size_t i = 0; i < program->funcCount; i++)
    {
        readStr(program->functions[i]->name, uint32_t);
        readValue(program->functions[i]->sourceLineIndex);
        readValue(program->functions[i]->sourceCharIndex);

        program->functions[i]->variableNames = malloc(sizeof(char*) * program->functions[i]->localsCount);
        program->functions[i]->variableSourceLineIndices = malloc(sizeof(uint32_t) * program->functions[i]->localsCount);
        program->functions[i]->variableSourceCharIndices = malloc(sizeof(uint32_t) * program->functions[i]->localsCount);
        for(size_t j = 0; j < program->functions[i]->localsCount; j++)
        {
            readStr(program->functions[i]->variableNames[j], uint32_t);
            readValue(program->functions[i]->variableSourceLineIndices[j]);
            readValue(program->functions[i]->variableSourceCharIndices[j]);
        }
    }
    return 1;
}

uint8_t chunkhandler_globals(nmProgram_t* program, FILE* file)
{
    readValue(program->globalsCount);
    program->globalsTypes = malloc(sizeof(uint32_t) * program->globalsCount);
    for(size_t i = 0; i < program->globalsCount; i++)
    {
        readValue(program->globalsTypes[i]);
    }
}