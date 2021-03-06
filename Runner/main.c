//
// Created by maxim on 6/21/19.
//

#include <stdio.h>

#include "lib/parser.h"
#include "lib/runtime/environment.h"
#include "lib/runtime/subroutines.h"

int main()
{
    nmProgram_t* program = nmParserFromFile("../../Examples/sample.nmb");
    if(program == NULL)
    {
        nmPrintError();
        return 1;
    }

    registerBuiltinSubroutines(subroutines_default);

    nmProgramPrint(program, stdout);

    nmEnvironment_t* env = nmEnvCreate(program);
    nmEnvSetStreams(env, stdin, stdout);

    //nmEnvDump(env, stdout);
    fputs("\n================\n", stdout);

    nmEnvExecute(env);
    fputs("\n===============\n\n", stdout);

    nmEnvDump(env, stdout);

    printf("Done in %lf ms", nmGetExecTime(env));

    nmEnvFree(env);
    nmProgramFree(program);

    return 0;
}
