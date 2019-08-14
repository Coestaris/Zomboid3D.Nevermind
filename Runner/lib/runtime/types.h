//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_TYPES_H
#define NMRUNNER_TYPES_H

#include <stdint.h>

typedef enum _nmTypeSignature
{
    tInteger = 0x1,
    tUInteger = 0x2,
    tFloat = 0x3,
    tString = 0x4

} nmTypeSignature_t;

typedef struct _nmType
{
    nmTypeSignature_t typeSignature;
    uint8_t typeBase;
    uint32_t typeIndex;

    uint64_t funcIndex;

} nmType_t;

#endif //NMRUNNER_CORETYPES_H