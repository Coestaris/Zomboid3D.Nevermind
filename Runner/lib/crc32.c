//
// Created by maxim on 7/1/19.
//

#include "crc32.h"

static inline uint32_t updateCRC32(uint8_t ch, uint32_t crc)
{
    uint32_t idx = ((crc) ^ (ch)) & 0xff;
    uint32_t tab_value = *(crc_32_tab + idx);
    return tab_value ^ ((crc) >> 8);
}

uint32_t crc32(uint8_t* bytes, uint32_t len)
{
    uint32_t oldcrc32 = 0xFFFFFFFF;

    for(int i = 0; i < len; i++)
        oldcrc32 = updateCRC32(bytes[i], oldcrc32);

    return ~oldcrc32;
}