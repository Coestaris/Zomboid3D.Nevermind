//
// Created by maxim on 6/29/19.
//

#include "coretypes.h"

void nmProgramFree(nmProgram_t* program)
{
    for(size_t i = 0; i < program->usedTypesCount; i++)
        free(program->usedTypes[i]);
    free(program->usedTypes);

    for(size_t i = 0; i < program->constantCount; i++)
    {
        free(program->constants[i]->value);
        free(program->constants[i]);
    }
    free(program->constants);

    for(size_t i = 0; i < program->importCount; i++)
    {
        free(program->imports[i]->moduleName);
        free(program->imports[i]);
    }
    free(program->imports);

    free(program->metadata->binaryAuthor);
    free(program->metadata->binaryName);
    free(program->metadata->binaryDescription);
    free(program->metadata);

    for(size_t i = 0; i < program->funcCount; i++)
    {
        free(program->functions[i]->localTypes);
        free(program->functions[i]->regTypes);
        for(size_t j = 0; j < program->functions[i]->instructionsCount; j++)
        {
            free(program->functions[i]->instructions[j]->parameters);
            free(program->functions[i]->instructions[j]);
        }
        free(program->functions[i]->instructions);
        free(program->functions[i]);
    }    
    free(program->functions);
    free(program);
}

void nmProgramPrint(nmProgram_t* program, FILE* f)
{
    fprintf(f, "Metadata: ");
    if(program.metadata == NULL)
        fprintf(f, "none");
    else
    {
        putc(f, '\n');
        fprintf(f, "Binary Name: %s", program->metadata->binaryName);
        fprintf(f, "Binary Author: %s", program->metadata->binaryAuthor);
        fprintf(f, "Binary Name: %s", program->metadata->binaryDescription);
    }
}