//
// Created by maxim on 7/30/19.
//

#ifndef NMRUNNER_SUBROUTINES_H
#define NMRUNNER_SUBROUTINES_H

#include <stdint.h>
#include <malloc.h>
#include "environment.h"

typedef struct _nmSubroutine {

    uint32_t index;
    void (*func)();
} nmSubroutine_t;

typedef enum _nmSubroutineScope {

    subroutines_io   = 0x1U,
    subroutines_sys  = 0x2U,
    subroutines_math = 0x4U,

} nmSubroutineScope_t;

void registerSubroutine(nmSubroutine_t*);
nmSubroutine_t* createSubroutine(uint32_t index, void (*func)());
void registerBuiltinSubroutines(nmSubroutineScope_t);

extern nmSubroutine_t** subroutines;

#endif //NMRUNNER_SUBROUTINES_H
