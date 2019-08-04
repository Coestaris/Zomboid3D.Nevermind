//
// Created by maxim on 8/4/19.
//

#ifndef NMRUNNER_DEFAULTSUBROUTINES_H
#define NMRUNNER_DEFAULTSUBROUTINES_H

#include <math.h>

void sr_io_print_i();
void sr_io_print_f();

void sr_io_print_s();
void sr_io_print();
void sr_io_fprint();

void sr_sys_get_time();
void sr_sys_get_gpc();
void sr_sys_get_pc();
void sr_sys_get_fc();

#define subroutineMath(name)                                                          \
    double* local = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0]; \
    *local = name(*local);

#define subroutineMath2(name)                                                         \
    double* local1 = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[0]; \
    double* local2 = currentEnv->callableFunctions[*currentEnv->funcIndex]->locals[1]; \
    *local1 = name(*local1, *local2);

void sr_m_acos();
void sr_m_asin();
void sr_m_atan();
void sr_m_atan2();
void sr_m_ceil();
void sr_m_cos();
void sr_m_cosh();
void sr_m_exp();
void sr_m_fabs();
void sr_m_floor();
void sr_m_fmod();
void sr_m_log();
void sr_m_log10();
void sr_m_log2();
void sr_m_pow();
void sr_m_sin();
void sr_m_sinh();
void sr_m_sqrt();
void sr_m_tan();
void sr_m_tanh();

void sr_m_acosf();
void sr_m_asinf();
void sr_m_atanf();
void sr_m_atan2f();
void sr_m_ceilf();
void sr_m_cosf();
void sr_m_coshf();
void sr_m_expf();
void sr_m_fabsf();
void sr_m_floorf();
void sr_m_fmodf();
void sr_m_logf();
void sr_m_log10f();
void sr_m_log2f();
void sr_m_powf();
void sr_m_sinf();
void sr_m_sinhf();
void sr_m_sqrtf();
void sr_m_tanf();
void sr_m_tanhf();

void sr_m_acosl();
void sr_m_asinl();
void sr_m_atanl();
void sr_m_atan2l();
void sr_m_ceill();
void sr_m_cosl();
void sr_m_coshl();
void sr_m_expl();
void sr_m_fabsl();
void sr_m_floorl();
void sr_m_fmodl();
void sr_m_logl();
void sr_m_log10l();
void sr_m_log2l();
void sr_m_powl();
void sr_m_sinl();
void sr_m_sinhl();
void sr_m_sqrtl();
void sr_m_tanl();
void sr_m_tanhl();

#endif //NMRUNNER_DEFAULTSUBROUTINES_H
