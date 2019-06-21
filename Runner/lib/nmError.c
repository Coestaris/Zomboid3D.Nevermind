//
// Created by maxim on 6/21/19.
//

#include "nmError.h"

char* error = NULL;

void nmPrintError()
{
    char* err = nmGetError();
    if(err == NULL) puts("no errors");
    else puts(err);
    nmClearError();
}

void nmClearError(void)
{
    free(error);
    error = NULL;
}

char* nmGetError()
{
    return error;
}

void nmPushError(char* new_error)
{
    if(error == NULL) {
        error = malloc(strlen(new_error) + 2);
        memcpy(error, new_error, strlen(new_error));
        error[strlen(new_error)] = '\n';
        error[strlen(new_error) + 1] = '\0';

    } else {

        error = realloc(error, strlen(error) + strlen(new_error) + 3);
        memcpy(error + strlen(error), new_error, strlen(new_error));
        error[strlen(error)] = '\n';
        error[strlen(error) + 1] = '\0';
    }
}

void nmPushErrorf(const char* fmt, ...)
{
    va_list list;
    va_start(list, fmt);

    char newError[MAX_ERRORF_LEN];
    vsnprintf(newError, MAX_ERRORF_LEN, fmt, list);

    nmPushError(newError);
}