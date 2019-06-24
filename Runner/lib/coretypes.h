//
// Created by maxim on 6/24/19.
//

#include <stdint.h>

#include "runtime/types.h"

typedef struct _nmFunction
{
    uint32_t index;
    uint32_t instructionsCount;
    uint32_t localsCount; 
    uint32_t regCount; 

    uint32_t* localTypes;
    uint32_t* regTypes;

    nmInstruction_t** instructions;

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
    char* binaryAutor;
    
    uint16_t majorVersion;
    uint16_t minorVersion;

    nmDate_t compileDateTime;

} nmMetadata_t;

typedef struct _nmConstant
{
    uint32_t typeIndex;
    void* value;

} nmConstant_t;

typedef struct _nmProgram
{
    uint16_t nmVersion;
    
    uint32_t funcCount;
    nmFunction_t** functions;

    uint32_t importCount;
    nmImport_t** imports;

    nmMetadata_t* metadata;

    uint32_t usedTypesCount;
    nmType_t** usedTypes;

    uint32_t constantCount;
    nmConstant_t* constants;

} nmProgram_t;

