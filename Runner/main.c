//
// Created by maxim on 6/21/19.
//

#include <stdio.h>

#include "lib/parser.h"
#include "lib/runtime/environment.h"

int main()
{
    nmProgram_t* program = nmParserFromFile("../../Examples/sample.nmb");
    if(program == NULL)
    {
        nmPrintError();
        return 1;
    }

    nmProgramPrint(program, stdout);

    nmEnvironment_t* env = nmEnvCreate(program);
    nmEnvSetStreams(env, stdout, stdin);

    nmEnvDump(env, stdout);
    nmEnvExecute(env);
    nmEnvDump(env, stdout);

    nmEnvFree(env);
    nmProgramFree(program);

    return 0;
}
