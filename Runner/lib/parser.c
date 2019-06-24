//
// Created by maxim on 6/21/19.
//

#include "parser.h"

nmProgram_t* parser_fromFile(const char* filename)
{
    FILE* file = fopen(filename, "rb");
    if(!file)
    {
        nmPushErrorf("Unable to open file %s", filename);
        return NULL;
    }

    uint8_t buffer[sizeof(nmbSignature)];
    if(fread(buffer, sizeof(nmbSignature), 1, file) != 1)
    {
        nmPushError("Unable to read file signature");
        return NULL;
    }

    for(size_t i = 0; i < sizeof(nmbSignature); i++)
        if(buffer[i] != nmbSignature[i])
        {
            nmPushError("Wrong file signature");
            return NULL;
        }

    while(!feof(file))
    {
        uint32_t len;
        uint32_t crc;
        uint16_t type;
        uint8_t* dataBuffer;

        if(fread(&len, sizeof(len), 1 ,file) != 1)
        {
            nmPushError("Unable to read chunk length from file");
            return NULL;
        }

        if(fread(&crc, 1, sizeof(crc), file) != sizeof(crc))
        {
            nmPushError("Unable to read chunk crc from file");
            return NULL;
        }

        if(fread(&type, 1, sizeof(type), file) != sizeof(type))
        {
            nmPushError("Unable to read chunk type from file");
            return NULL;
        }

        dataBuffer = malloc(sizeof(uint8_t) * len);

        if(fread(&dataBuffer, 1, sizeof(uint8_t) * len, file) != sizeof(uint8_t) * len)
        {
             nmPushError("Unable to read data from file");
             return NULL;
        }

        free(dataBuffer);
    }

    if(fclose(file) == EOF)
    {
        nmPushErrorf("Unable to close file %s", filename);
        return NULL;
    }

    return NULL;
}