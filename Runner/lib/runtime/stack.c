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
    return stack->stack[stack->position--];
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