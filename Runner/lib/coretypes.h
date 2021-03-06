//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_CORETYPES_H
#define NMRUNNER_CORETYPES_H

#include <stdint.h>
#include <stddef.h>
#include <malloc.h>
#include <stdlib.h>

#include "runtime/types.h"
#include "runtime/instructions.h"

typedef struct _nmFunction
{
    uint32_t index;
    uint32_t instructionsCount;
    uint32_t localsCount; 
    uint32_t regCount; 

    uint32_t* localTypes;
    uint32_t* regTypes;

    nmInstruction_t** instructions;

    //Debug info
    char** variableNames;
    uint32_t* variableSourceLineIndices;
    uint32_t* variableSourceCharIndices;
    char* name;
    uint32_t sourceLineIndex;
    uint32_t sourceCharIndex;

} nmFunction_t;

typedef struct _nmImport
{
    uint8_t isLib;
    char* moduleName;

} nmImport_t;


typedef struct _nmDate
{
    uint8_t second;
    uint8_t minute;
    uint8_t hour;

    uint8_t day;
    uint8_t month;
    uint16_t year;

} nmDate_t;

typedef struct _nmMetadata
{
    char* binaryName;
    char* binaryDescription;
    char* binaryAuthor;
    
    uint16_t majorVersion;
    uint16_t minorVersion;

    nmDate_t compileDateTime;

} nmMetadata_t;

typedef struct _nmConstant
{
    nmType_t* typePtr;

    uint32_t typeIndex;
    uint32_t len;

    void* value;

} nmConstant_t;

typedef struct _nmProgram
{
    uint16_t nmVersion;
    uint8_t sourceHash[16];

    uint32_t funcCount;
    nmFunction_t** functions;

    uint32_t entryPointFuncIndex;

    uint32_t importCount;
    nmImport_t** imports;

    nmMetadata_t* metadata;

    uint32_t usedTypesCount;
    nmType_t** usedTypes;

    uint32_t constantCount;
    nmConstant_t** constants;

    uint32_t globalsCount;
    uint32_t* globalsTypes;

    //Debug info
    char** globalsNames;
    uint32_t* globalsSourceLineIndices;
    uint32_t* globalsSourceCharIndices;

    //Debug info
    char* sourceFilename;

} nmProgram_t;

char* nmConstantToStr(void* value, nmType_t* type);
const char* nmTypeSignatureToStr(nmTypeSignature_t signature);

void nmProgramFree(nmProgram_t* program);
void nmProgramPrint(nmProgram_t* program, FILE* f);

#endif //NMRUNNER_CORETYPES_H