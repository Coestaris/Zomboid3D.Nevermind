//
// Created by maxim on 6/21/19.
//

#include "parser.h"

nmProgram_t* parser_fromFile(const char* filename)
{
    FILE* f = fopen(filename, "r");
    if(!f)
    {
        nmPushErrorf("Unable to open file %s", filename);
        return NULL;
    }
    nmProgram_t* result = parser_load(f);
    if(fclose(f) == EOF)
    {
        nmPushErrorf("Unable to close file %s", filename);
        return NULL;
    }

    return result;
}

nmProgram_t* parser_load(FILE* file)
{
    uint8_t buffer[sizeof(nmbSignature)];
    if(fread(buffer, 1, sizeof(nmbSignature), file) != sizeof(nmbSignature))
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

    while(1)
    {
        uint32_t len;
        uint32_t crc;
        uint16_t type;
        uint8_t* dataBuffer;

        if(fread(&len, 1, sizeof(len), file) != sizeof(len))
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
    }
}