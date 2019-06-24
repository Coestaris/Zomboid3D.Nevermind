//
// Created by maxim on 6/24/19.
//

#include <stdint.h>

typedef struct _nmInstruction
{
    uint16_t index;
    uint32_t* parameters;

} nmInstruction_t;

typedef struct _nmType
{
    uint16_t typeSignature;
    uint8_t typeBase;
    uint32_t typeIndex;

} nmType_t;
