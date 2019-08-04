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
    if(!subroutines)
    {
        subroutinesCount = 10;
        subroutines = malloc(sizeof(nmSubroutine_t*) * subroutinesCount);
        memset(subroutines, 0, sizeof(nmSubroutine_t*) * subroutinesCount);
    }

    if(subroutine->index + 1 >= subroutinesCount)
    {
        size_t newSize = (size_t)((double)subroutine->index * 1.5);

        subroutines = realloc(subroutines, sizeof(nmSubroutine_t*) * newSize);
        for(size_t i = subroutinesCount + 1; i < newSize; i++)
            subroutines[i] = NULL;

        subroutinesCount = newSize;
    };

    subroutines[subroutine->index] = subroutine;
}

void sr_io_print_i()
{
    void* local = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0];
    fprintf(currentEnv->outs,"[PROGRAM]: %i\n", *((int32_t*)local));
}

void sr_io_print_f()
{
    void* local = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0];
    fprintf(currentEnv->outs,"[PROGRAM]: %lf\n", *((double*)local));
}

void sr_io_print_s() { }
void sr_io_print() { }
void sr_io_fprint() { }

void sr_sys_get_time() {}
void sr_sys_get_gpc() {}
void sr_sys_get_pc() {}
void sr_sys_get_fc() {}

#define subroutineMath(name)                                                          \
    double* local = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0]; \
    *local = name(*local);

void sr_m_acos() { }
void sr_m_asin() {}
void sr_m_atan() {}
void sr_m_atan2() {}
void sr_m_ceil() {}
void sr_m_cos() { subroutineMath(cos) }
void sr_m_cosh() {}
void sr_m_exp() {}
void sr_m_fabs() {}
void sr_m_floor() {}
void sr_m_fmod() {}
void sr_m_frexp() {}
void sr_m_ldexp() {}
void sr_m_log() {}
void sr_m_log10() {}
void sr_m_log2() {}
void sr_m_modf() {}
void sr_m_pow() {}
void sr_m_sin() { subroutineMath(sin) }
void sr_m_sinh() {}
void sr_m_sqrt()
{
    double* local = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0];
    *local = sqrt(*local);
}

void sr_m_tan() {}
void sr_m_tanh() {}

void registerBuiltinSubroutines(nmSubroutineScope_t scope)
{
    if(scope & subroutines_io)
    {
        registerSubroutine(createSubroutine(0x1, sr_io_print_i));
        registerSubroutine(createSubroutine(0x2, sr_io_print_f));
        registerSubroutine(createSubroutine(0x3, sr_io_print_s));
        registerSubroutine(createSubroutine(0x4, sr_io_print));
        registerSubroutine(createSubroutine(0x5, sr_io_fprint));
    }

    if(scope & subroutines_sys)
    {
        registerSubroutine(createSubroutine(0x100, sr_sys_get_time));
        registerSubroutine(createSubroutine(0x101, sr_sys_get_gpc));
        registerSubroutine(createSubroutine(0x102, sr_sys_get_pc));
        registerSubroutine(createSubroutine(0x103, sr_sys_get_fc));
    }

    if(scope & subroutines_math)
    {
        registerSubroutine(createSubroutine(0x200, sr_m_acos));
        registerSubroutine(createSubroutine(0x201, sr_m_asin));
        registerSubroutine(createSubroutine(0x202, sr_m_atan));
        registerSubroutine(createSubroutine(0x203, sr_m_atan2));
        registerSubroutine(createSubroutine(0x204, sr_m_ceil));
        registerSubroutine(createSubroutine(0x205, sr_m_cos));
        registerSubroutine(createSubroutine(0x206, sr_m_cosh));
        registerSubroutine(createSubroutine(0x207, sr_m_exp));
        registerSubroutine(createSubroutine(0x208, sr_m_fabs));
        registerSubroutine(createSubroutine(0x209, sr_m_floor));
        registerSubroutine(createSubroutine(0x20A, sr_m_fmod));
        registerSubroutine(createSubroutine(0x20B, sr_m_frexp));
        registerSubroutine(createSubroutine(0x20C, sr_m_ldexp));
        registerSubroutine(createSubroutine(0x20D, sr_m_log));
        registerSubroutine(createSubroutine(0x20E, sr_m_log10));
        registerSubroutine(createSubroutine(0x20F, sr_m_log2));
        registerSubroutine(createSubroutine(0x210, sr_m_modf));
        registerSubroutine(createSubroutine(0x211, sr_m_pow));
        registerSubroutine(createSubroutine(0x212, sr_m_sin));
        registerSubroutine(createSubroutine(0x213, sr_m_sinh));
        registerSubroutine(createSubroutine(0x214, sr_m_sqrt));
        registerSubroutine(createSubroutine(0x215, sr_m_tan));
        registerSubroutine(createSubroutine(0x216, sr_m_tanh));
    }
}

uint8_t hasSubroutine(uint32_t index)
{
    for(size_t i = 0; i < subroutinesCount; i++)
        if(subroutines[i] && subroutines[i]->index == index)
            return 1;
    return 0;
}