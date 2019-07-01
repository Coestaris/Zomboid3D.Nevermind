//
// Created by maxim on 6/30/19.
//

#ifndef NMRUNNER_STACK_H
#define NMRUNNER_STACK_H

#include <stdint.h>
#include <stddef.h>
#include <malloc.h>
#include <assert.h>

#define STACK_START_SIZE 10
#define STACK_INCREASE 1.5

typedef struct _stack {
    size_t size;
    size_t position;
    void** stack;

} stack_t;

stack_t* createStack();
void freeStack(stack_t* stack);
void* popStack(stack_t* stack);
void pushStack(stack_t* stack, void* value);
void printStack(stack_t* stack, FILE* file);

#endif //NMRUNNER_STACK_H
