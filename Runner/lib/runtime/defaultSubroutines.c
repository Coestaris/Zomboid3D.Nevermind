//
// Created by maxim on 8/4/19.
//

#include "defaultSubroutines.h"
#include "environment.h"

void sr_array_resize()
{

}

void sr_array_free()
{

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

void sr_io_print()
{
    size_t count = (size_t)popStack(currentEnv->variableStack);
    for(size_t i = 0; i < count; i++)
    {
        void* variable = popStack(currentEnv->variableStack);
        uint64_t typeIndex = (uint64_t)popStack(currentEnv->variableStack);
        nmType_t* type = currentEnv->program->usedTypes[typeIndex];

        switch(type->funcIndex)
        {
            case 0:
                fprintf(currentEnv->outs,"%i", *(int8_t*)variable);
                break;
            case 1:
                fprintf(currentEnv->outs,"%i", *(int16_t*)variable);
                break;
            case 2:
                fprintf(currentEnv->outs,"%i", *(int32_t*)variable);
                break;
            case 3:
                fprintf(currentEnv->outs,"%li", *(int64_t*)variable);
                break;
            case 4:
                fprintf(currentEnv->outs,"%u", *(uint8_t*)variable);
                break;
            case 5:
                fprintf(currentEnv->outs,"%u", *(uint16_t*)variable);
                break;
            case 6:
                fprintf(currentEnv->outs,"%u", *(uint32_t*)variable);
                break;
            case 7:
                fprintf(currentEnv->outs,"%lu", *(uint64_t*)variable);
                break;
            case 8:
                fprintf(currentEnv->outs,"%f", *(float*)variable);
                break;
            case 9:
                fprintf(currentEnv->outs,"%lf", *(double*)variable);
                break;
            default:
                abort();
        }

        printf(", ");
    }

    putc('\n', currentEnv->outs);
}

void sr_io_fprint() { }

void sr_sys_get_time() {}
void sr_sys_get_gpc() {}
void sr_sys_get_pc() {}
void sr_sys_get_fc() {}

void sr_m_acos()   { subroutineMath (acos)  }
void sr_m_asin()   { subroutineMath (asin)  }
void sr_m_atan()   { subroutineMath (atan)  }
void sr_m_atan2()  { subroutineMath2(atan2) }
void sr_m_ceil()   { subroutineMath (ceil)  }
void sr_m_cos()    { subroutineMath (cos)   }
void sr_m_cosh()   { subroutineMath (cosh)  }
void sr_m_exp()    { subroutineMath (exp)   }
void sr_m_fabs()   { subroutineMath (fabs)  }
void sr_m_floor()  { subroutineMath (floor) }
void sr_m_fmod()   { subroutineMath2(fmod)  }
void sr_m_log()    { subroutineMath (log)   }
void sr_m_log10()  { subroutineMath (log10) }
void sr_m_log2()   { subroutineMath (log2)  }
void sr_m_pow()    { subroutineMath2(pow)   }
void sr_m_sin()    { subroutineMath (sin)   }
void sr_m_sinh()   { subroutineMath (sinh)  }
void sr_m_sqrt()   { subroutineMath (sqrt)  }
void sr_m_tan()    { subroutineMath (tan)   }
void sr_m_tanh()   { subroutineMath (tanh)  }

void sr_m_acosf()   { subroutineMath (acosf)  }
void sr_m_asinf()   { subroutineMath (asinf)  }
void sr_m_atanf()   { subroutineMath (atanf)  }
void sr_m_atan2f()  { subroutineMath2(atan2f) }
void sr_m_ceilf()   { subroutineMath (ceilf)  }
void sr_m_cosf()    { subroutineMath (cosf)   }
void sr_m_coshf()   { subroutineMath (coshf)  }
void sr_m_expf()    { subroutineMath (expf)   }
void sr_m_fabsf()   { subroutineMath (fabsf)  }
void sr_m_floorf()  { subroutineMath (floorf) }
void sr_m_fmodf()   { subroutineMath2(fmodf)  }
void sr_m_logf()    { subroutineMath (logf)   }
void sr_m_log10f()  { subroutineMath (log10f) }
void sr_m_log2f()   { subroutineMath (log2f)  }
void sr_m_powf()    { subroutineMath2(powf)   }
void sr_m_sinf()    { subroutineMath (sinf)   }
void sr_m_sinhf()   { subroutineMath (sinhf)  }
void sr_m_sqrtf()   { subroutineMath (sqrtf)  }
void sr_m_tanf()    { subroutineMath (tanf)   }
void sr_m_tanhf()   { subroutineMath (tanhf)  }

void sr_m_acosl()   { subroutineMath (acosl)  }
void sr_m_asinl()   { subroutineMath (asinl)  }
void sr_m_atanl()   { subroutineMath (atanl)  }
void sr_m_atan2l()  { subroutineMath2(atan2l) }
void sr_m_ceill()   { subroutineMath (ceill)  }
void sr_m_cosl()    { subroutineMath (cosl)   }
void sr_m_coshl()   { subroutineMath (coshl)  }
void sr_m_expl()    { subroutineMath (expl)   }
void sr_m_fabsl()   { subroutineMath (fabsl)  }
void sr_m_floorl()  { subroutineMath (floorl) }
void sr_m_fmodl()   { subroutineMath2(fmodl)  }
void sr_m_logl()    { subroutineMath (logl)   }
void sr_m_log10l()  { subroutineMath (log10l) }
void sr_m_log2l()   { subroutineMath (log2l)  }
void sr_m_powl()    { subroutineMath2(powl)   }
void sr_m_sinl()    { subroutineMath (sinl)   }
void sr_m_sinhl()   { subroutineMath (sinhl)  }
void sr_m_sqrtl()   { subroutineMath (sqrtl)  }
void sr_m_tanl()    { subroutineMath (tanl)   }
void sr_m_tanhl()   { subroutineMath (tanhl)  }