//
// Created by maxim on 6/21/19.
//

#ifndef NMRUNNER_PARSER_H
#define NMRUNNER_PARSER_H

#include <stdio.h>
#include <stdint.h>

#include "nmError.h"

#include "coretypes.h"

static const uint8_t nmbSignature[] = { 'N', 'M', 'B' };

typedef struct _chunkHanlder
{
    uint8_t (*hanlder)(nmProgram_t*, FILE*);
    const uint8_t chunktype[2];

    uint8_t required;
    uint8_t unique;

} chunkHanlder_t;

uint8_t chunkhandler_header(nmProgram_t*, FILE*);
uint8_t chunkhandler_metadata(nmProgram_t*, FILE*);
uint8_t chunkhandler_types(nmProgram_t*, FILE*);
uint8_t chunkhandler_constants(nmProgram_t*, FILE*);
uint8_t chunkhandler_functions(nmProgram_t*, FILE*);
uint8_t chunkhandler_debug(nmProgram_t*, FILE*);

#define chunkHanldersCount 6
static const chunkHanlder_t chunkHanlders[chunkHanldersCount] = 
{
    { chunkhandler_header,    { 'H', 'E' }, 1, 1 },
    { chunkhandler_metadata,  { 'M', 'E' }, 0, 1 },
    { chunkhandler_types,     { 'T', 'Y' }, 1, 1 },
    { chunkhandler_constants, { 'C', 'O' }, 1, 1 },
    { chunkhandler_functions, { 'F', 'U' }, 1, 0 },
    { chunkhandler_debug,     { 'D', 'E' }, 1, 0 },
};

uint16_t getChunkType(const  uint8_t array[2]);

nmProgram_t* nmParserFromFile(const char* filename);
nmProgram_t* nmParserLoad(FILE* file);

#endif //NMRUNNER_PARSER_H
