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
    tString = 0x4,
    tArray = 0x5

} nmTypeSignature_t;

typedef struct _nmType
{
    nmTypeSignature_t typeSignature;
    uint32_t typeBase;
    uint8_t dim;

    uint32_t typeIndex;

    uint64_t funcIndex;

} nmType_t;


typedef struct _nmArrayInfo {
    uint32_t index;
    nmType_t* type;
    uint32_t dimenitions;
    uint32_t* size;

    void* data;

} nmArrayInfo_t;

#endif //NMRUNNER_CORETYPES_H