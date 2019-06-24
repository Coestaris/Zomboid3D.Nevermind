//
// Created by maxim on 6/21/19.
//

#include "parser.h"

uint16_t getChunkType(const uint8_t array[2]) 
{
    return array[1] << 8 | array[0];
}

nmProgram_t* parser_load(FILE* file)
{
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

    nmProgram_t* program = malloc(sizeof(nmProgram_t));

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

        if(fread(&crc, sizeof(crc), 1, file) != 1)
        {
            nmPushError("Unable to read chunk crc from file");
            return NULL;
        }

        if(fread(&type, sizeof(type), 1, file) != 1)
        {
            nmPushError("Unable to read chunk type from file");
            return NULL;
        }

        dataBuffer = malloc(sizeof(uint8_t) * len);

        if(fread(dataBuffer, sizeof(uint8_t) * len, 1, file) != 1)
        {
             nmPushError("Unable to read data from file");
             return NULL;
        }

        chunkHanlder_t* handler = NULL;
        for(size_t i = 0; i < chunkHanldersCount; i++)
            if(getChunkType(chunkHanlders[i].chunktype) == type)
            {
                handler = &chunkHanlders[i];
                break;
            }

        if(handler == NULL)
        {
            nmPushErrorf("Unknown chunk type: %c%c", type & 0xFF, (type >> 8) & 0xFF);
            return NULL;   
        }

        printf("Found chunk %c%c\n", type & 0xFF, (type >> 8) & 0xFF);

        FILE* memStream = fmemopen(dataBuffer, len, "r");  
        if(!memStream)
        {
            nmPushError("Unable to open memstream");
            free(dataBuffer);
            return NULL;
        }

        if(!handler->hanlder(program, memStream))
        {
            nmPushError("Unable to parse chunk");
            fclose(memStream);
            free(dataBuffer);
            
            return NULL;
        }

        fclose(memStream);
        free(dataBuffer);
    }

    return program;
}

nmProgram_t* parser_fromFile(const char* filename)
{
    FILE* file = fopen(filename, "rb");
    if(!file)
    {
        nmPushErrorf("Unable to open file %s", filename);
        return NULL;
    }

    nmProgram_t* program = parser_load(file);

    if(fclose(file) == EOF)
    {
        nmPushErrorf("Unable to close file %s", filename);
        return NULL;
    }

    return NULL;
}

uint8_t chunkhandler_header(nmProgram_t* program, FILE* file)
{
    if(fread(&program->nmVersion, sizeof(uint16_t), 1, file) != 1) return 0;
    if(fread(&program->importCount, sizeof(uint32_t), 1, file) != 1) return 0;
    if(fread(&program->funcCount, sizeof(uint32_t), 1, file) != 1) return 0;
    
    program->imports = malloc(sizeof(nmImport_t) * program->importCount);
    program->functions = malloc(sizeof(nmFunction_t) * program->funcCount);

    for(size_t i = 0; i < program->importCount; i++)
    {
        program->imports[i] = malloc(sizeof(nmImport_t));
        if(fread(&program->imports[i]->isLib, sizeof(uint8_t), 1, file) != 1) return 0;
        uint32_t len;
        if(fread(&len, sizeof(uint32_t), 1, file) != 1) return 0;
        program->imports[i]->moduleName = malloc(sizeof(uint8_t) * len + 1);
        if(fread(program->imports[i]->moduleName, sizeof(uint8_t) * len, 1, file) != 1) return 0;
        program->imports[i]->moduleName[len] = '\0';
    }    

    return 1;
}

uint8_t chunkhandler_metadata(nmProgram_t* program, FILE* file)
{
    return 1;
}

uint8_t chunkhandler_types(nmProgram_t* program, FILE* file)
{
    return 1;
}

uint8_t chunkhandler_constants(nmProgram_t* program, FILE* file)
{
    return 1;
}

uint8_t chunkhandler_functions(nmProgram_t* program, FILE* file)
{
    return 1;
}