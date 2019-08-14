//
// Created by maxim on 7/30/19.
//

#ifndef NMRUNNER_SUBROUTINES_H
#define NMRUNNER_SUBROUTINES_H

#include <stdint.h>
#include <malloc.h>
#include <math.h>
#include <memory.h>

#include "environment.h"

typedef struct _nmSubroutine {

    uint32_t index;
    void (*func)();
} nmSubroutine_t;

typedef enum _nmSubroutineScope {

    subroutines_io     = 0x1U,
    subroutines_sys    = 0x2U,
    subroutines_math   = 0x4U,
    subroutines_arrays = 0x8U,


    subroutines_default = 0x1U | 0x2U | 0x4U | 0x8U,

} nmSubroutineScope_t;

void registerSubroutine(nmSubroutine_t*);
nmSubroutine_t* createSubroutine(uint32_t index, void (*func)());
void registerBuiltinSubroutines(nmSubroutineScope_t);
uint8_t hasSubroutine(uint32_t index);

extern size_t subroutinesCount;
extern nmSubroutine_t** subroutines;

#endif //NMRUNNER_SUBROUTINES_H
