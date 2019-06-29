//
// Created by maxim on 6/24/19.
//

#ifndef NMRUNNER_TYPES_H
#define NMRUNNER_TYPES_H

#include <stdint.h>

typedef struct _nmType
{
    uint16_t typeSignature;
    uint8_t typeBase;
    uint32_t typeIndex;

} nmType_t;

#endif //NMRUNNER_CORETYPES_H