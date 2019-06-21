//
// Created by maxim on 6/21/19.
//

#ifndef NMRUNNER_NMERROR_H
#define NMRUNNER_NMERROR_H

#include <stdlib.h>
#include <string.h>
#include <stdarg.h>
#include <stdio.h>

#define MAX_ERRORF_LEN 1000

void nmPrintError(void);
char* nmGetError(void);
void nmClearError(void);
void nmPushError(char* error);
void nmPushErrorf(const char* fmt, ...);

#endif //NMRUNNER_NMERROR_H
