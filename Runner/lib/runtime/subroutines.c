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

#include "defaultSubroutines.h"
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
        registerSubroutine(createSubroutine(0x20B, sr_m_log));
        registerSubroutine(createSubroutine(0x20C, sr_m_log10));
        registerSubroutine(createSubroutine(0x20D, sr_m_log2));
        registerSubroutine(createSubroutine(0x20E, sr_m_pow));
        registerSubroutine(createSubroutine(0x20F, sr_m_sin));
        registerSubroutine(createSubroutine(0x210, sr_m_sinh));
        registerSubroutine(createSubroutine(0x211, sr_m_sqrt));
        registerSubroutine(createSubroutine(0x212, sr_m_tan));
        registerSubroutine(createSubroutine(0x213, sr_m_tanh));

        registerSubroutine(createSubroutine(0x214, sr_m_acosf));
        registerSubroutine(createSubroutine(0x215, sr_m_asinf));
        registerSubroutine(createSubroutine(0x216, sr_m_atanf));
        registerSubroutine(createSubroutine(0x217, sr_m_atan2f));
        registerSubroutine(createSubroutine(0x218, sr_m_ceilf));
        registerSubroutine(createSubroutine(0x219, sr_m_cosf));
        registerSubroutine(createSubroutine(0x21A, sr_m_coshf));
        registerSubroutine(createSubroutine(0x21B, sr_m_expf));
        registerSubroutine(createSubroutine(0x21C, sr_m_fabsf));
        registerSubroutine(createSubroutine(0x21D, sr_m_floorf));
        registerSubroutine(createSubroutine(0x21E, sr_m_fmodf));
        registerSubroutine(createSubroutine(0x21F, sr_m_logf));
        registerSubroutine(createSubroutine(0x220, sr_m_log10f));
        registerSubroutine(createSubroutine(0x221, sr_m_log2f));
        registerSubroutine(createSubroutine(0x222, sr_m_powf));
        registerSubroutine(createSubroutine(0x223, sr_m_sinf));
        registerSubroutine(createSubroutine(0x224, sr_m_sinhf));
        registerSubroutine(createSubroutine(0x225, sr_m_sqrtf));
        registerSubroutine(createSubroutine(0x226, sr_m_tanf));
        registerSubroutine(createSubroutine(0x227, sr_m_tanhf));

        registerSubroutine(createSubroutine(0x228, sr_m_acosl));
        registerSubroutine(createSubroutine(0x229, sr_m_asinl));
        registerSubroutine(createSubroutine(0x22A, sr_m_atanl));
        registerSubroutine(createSubroutine(0x22B, sr_m_atan2l));
        registerSubroutine(createSubroutine(0x22C, sr_m_ceill));
        registerSubroutine(createSubroutine(0x22D, sr_m_cosl));
        registerSubroutine(createSubroutine(0x22E, sr_m_coshl));
        registerSubroutine(createSubroutine(0x22F, sr_m_expl));
        registerSubroutine(createSubroutine(0x230, sr_m_fabsl));
        registerSubroutine(createSubroutine(0x231, sr_m_floorl));
        registerSubroutine(createSubroutine(0x232, sr_m_fmodl));
        registerSubroutine(createSubroutine(0x233, sr_m_logl));
        registerSubroutine(createSubroutine(0x234, sr_m_log10l));
        registerSubroutine(createSubroutine(0x235, sr_m_log2l));
        registerSubroutine(createSubroutine(0x236, sr_m_powl));
        registerSubroutine(createSubroutine(0x237, sr_m_sinl));
        registerSubroutine(createSubroutine(0x238, sr_m_sinhl));
        registerSubroutine(createSubroutine(0x239, sr_m_sqrtl));
        registerSubroutine(createSubroutine(0x23A, sr_m_tanl));
        registerSubroutine(createSubroutine(0x23B, sr_m_tanhl));
    }
}

uint8_t hasSubroutine(uint32_t index)
{
    for(size_t i = 0; i < subroutinesCount; i++)
        if(subroutines[i] && subroutines[i]->index == index)
            return 1;
    return 0;
}