//
// Created by maxim on 6/21/19.
//

#include <stdio.h>

#include "lib/parser.h"

int main()
{
    nmProgram_t* program = nmParserFromFile("../../Examples/sample.nmb");
    if(program == NULL)
        nmPrintError();

    nmProgramFree(program);

    return 0;
}