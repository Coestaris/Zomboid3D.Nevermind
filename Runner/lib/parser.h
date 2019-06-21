//
// Created by maxim on 6/21/19.
//

#ifndef NMRUNNER_PARSER_H
#define NMRUNNER_PARSER_H

#include <stdio.h>
#include <stdint.h>

#include "nmError.h"

static const char nmbSignature[] = { 'N', 'M', 'B' };

typedef struct _nmProgram
{

} nmProgram_t;

nmProgram_t* parser_fromFile(const char* filename);
nmProgram_t* parser_load(FILE* file);

#endif //NMRUNNER_PARSER_H
