//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_TYPES_H
#define NMRUNNER_TYPES_H

#include <stdint.h>

typedef enum _nmTypeSignature
{
    tInteger = 0x1,
    tFloat = 0x2,
    tString = 0x3

} nmTypeSignature_t;

typedef struct _nmType
{
    nmTypeSignature_t typeSignature;
    uint8_t typeBase;
    uint32_t typeIndex;

} nmType_t;

#endif //NMRUNNER_CORETYPES_H