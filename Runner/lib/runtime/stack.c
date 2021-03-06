//
// Created by maxim on 6/30/19.
//

#include "stack.h"

stack_t* createStack()
{
    stack_t* stack = malloc(sizeof(stack_t));
    stack->size = STACK_START_SIZE;
    stack->position = 0;
    stack->stack = malloc(sizeof(void*) * stack->size);
    return stack;
}

void freeStack(stack_t* stack)
{
    free(stack->stack);
    free(stack);
}

void* popStack(stack_t* stack)
{
    assert(stack->position != 0);
    return stack->stack[--stack->position];
}

void* getPrevElement(stack_t* stack)
{
    assert(stack->position != 1);
    return stack->stack[stack->position - 2];
}

void pushStack(stack_t* stack, void* value)
{
    if(stack->position == stack->size - 1)
    {
        size_t newSize = (size_t)(stack->size * STACK_INCREASE);
        stack->stack = realloc(stack->stack, newSize * sizeof(void*));
        stack->size = newSize;
    }

    stack->stack[stack->position++] = value;
}

void printStack(stack_t* stack, FILE* file)
{
    if(stack->position == 0)
        fprintf(file, "empty\n");
    else
    {
        for(size_t i = 0; i < stack->position; i++)
            fprintf(file, "%p%s", stack->stack[i], i == stack->position - 1 ? "\n" : ", ");
    }
}