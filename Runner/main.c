//
// Created by maxim on 6/21/19.
//

#include <stdio.h>

#include "lib/parser.h"

int main()
{
    nmProgram_t* program = parser_fromFile("../../Examples/sample.nmb");
    if(program == NULL)
        nmPrintError();

    return 0;
}