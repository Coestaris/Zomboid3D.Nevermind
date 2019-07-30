//
// Created by maxim on 7/30/19.
//

#include "subroutines.h"
#include "../coretypes.h"

nmSubroutine_t* createSubroutine(uint32_t index, void (*func)())
{
    nmSubroutine_t* subroutine = malloc(sizeof(nmSubroutine_t));
    subroutine->func = func;
    subroutine->index = index;
    return subroutine;
}

size_t subroutinesCount = 0;
nmSubroutine_t** subroutines = NULL;

void registerSubroutine(nmSubroutine_t* subroutine)
{
    if(subroutine->index + 1>= subroutinesCount)
    {
        subroutines = realloc(subroutines, sizeof(nmSubroutine_t*) * (subroutine->index + 1));
        subroutinesCount = subroutine->index;
    };

    subroutines[subroutine->index] = subroutine;
}

void printi()
{
    void* local = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0];
    fprintf(currentEnv->outs,"[PROGRAM]: %i", *((int32_t*)local));
}

void registerBuiltinSubroutines(nmSubroutineScope_t scope)
{
    if(scope & subroutines_io)
    {
        registerSubroutine(createSubroutine(0x1, printi));
    }
}

