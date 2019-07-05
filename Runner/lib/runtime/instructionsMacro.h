//
// Created by maxim on 7/5/19.
//

#ifndef NMRUNNER_INSTRUCTIONSMACRO_H
#define NMRUNNER_INSTRUCTIONSMACRO_H

#define enumerateFunc(name) {                              \
    name ## _i8, name ## _i16, name ## _i32, name ## _i64, \
    name ## _u8, name ## _u16, name ## _u32, name ## _u64, \
    name ## _f32, name ## _f64 }

#define AInstructionPrototype(name) void name(struct _nmEnvironment* env, void** data);
#define AInstructionPrototypeSet(name1, name2, name3, name4, name5, name6, name7, name8, name9, name10)\
    AInstructionPrototype(name1)  \
    AInstructionPrototype(name2)  \
    AInstructionPrototype(name3)  \
    AInstructionPrototype(name4)  \
    AInstructionPrototype(name5)  \
    AInstructionPrototype(name6)  \
    AInstructionPrototype(name7)  \
    AInstructionPrototype(name8)  \
    AInstructionPrototype(name9)  \
    AInstructionPrototype(name10) \

#define instructionPrototype(name) AInstructionPrototypeSet(        \
            name ## _i8, name ## _i16, name ## _i32, name ## _i64,  \
            name ## _u8, name ## _u16, name ## _u32, name ## _u64,  \
            name ## _f32, name ## _f64);

#define instructionDataBinary(name, index)  { #name, index, { varIndex, varConstFlag, varConstIndex, varConstFlag, varConstIndex }, enumerateFunc(instruction_ ## name) }
#define instructionDataUnary(name, index)    { #name, index, { varIndex, varConstFlag, varConstIndex, 0, 0 }, enumerateFunc(instruction_ ## name) }

#define declareRetInstruction(name, type)                   \
void name(struct _nmEnvironment* env, void** data) { }

#define declarePushInstruction(name, type)           \
void name(struct _nmEnvironment* env, void** data)   \
{                                                    \
    pushStack(env->variableStack, data[0]);          \
}                                                    \

#define declarePopInstruction(name, type)                   \
void name(struct _nmEnvironment* env, void** data)          \
{                                                           \
    *(type*)data[0] = *(type*)popStack(env->variableStack); \
}                                                           \

#define declareLdiInstruction(name, type)           \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
    *(type*)data[0] = *(type*)data[1];              \
}                                                   \

#define declareJmpInstruction(name, type)           \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
    *env->programCounter = *(type*)data[0];         \
}                                                   \

#define declareCallInstruction(name, type)          \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
                                                    \
}                                                   \

#define declareBreqInstruction(name, type)          \
void name(struct _nmEnvironment* env, void** data)  \
{                                                   \
                                                    \
}                                                   \


#define decalreABInstruction(name, type, sign) void name(struct _nmEnvironment* env, void** data) { *(type*)data[0] = *(type*)data[1] sign *(type*)data[2]; }
#define decalreABInstructionSet(name1, name2, name3, name4, name5, name6, name7, name8, name9, name10, sign)\
    decalreABInstruction(name1, int8_t, sign)    \
    decalreABInstruction(name2, int16_t, sign)   \
    decalreABInstruction(name3, int32_t, sign)   \
    decalreABInstruction(name4, int64_t, sign)   \
    decalreABInstruction(name5, uint8_t, sign)   \
    decalreABInstruction(name6, uint16_t, sign)  \
    decalreABInstruction(name7, uint32_t, sign)  \
    decalreABInstruction(name8, uint64_t, sign)  \
    decalreABInstruction(name9, float, sign)     \
    decalreABInstruction(name10, double, sign)   \

#define declareABInstruction(name, sign) decalreABInstructionSet(   \
        name ## _i8, name ## _i16, name ## _i32, name ## _i64,      \
        name ## _u8, name ## _u16, name ## _u32, name ## _u64,      \
        name ## _f32, name ## _f64, sign);

#define decalreABCInstruction(name, type, sign) void name(struct _nmEnvironment* env, void** data) { *(type*)data[0] = (type)((uint64_t)*(type*)data[1] sign (uint64_t)*(type*)data[2]); }
#define decalreABCInstructionSet(name1, name2, name3, name4, name5, name6, name7, name8, name9, name10, sign)\
    decalreABCInstruction(name1, int8_t, sign)    \
    decalreABCInstruction(name2, int16_t, sign)   \
    decalreABCInstruction(name3, int32_t, sign)   \
    decalreABCInstruction(name4, int64_t, sign)   \
    decalreABCInstruction(name5, uint8_t, sign)   \
    decalreABCInstruction(name6, uint16_t, sign)  \
    decalreABCInstruction(name7, uint32_t, sign)  \
    decalreABCInstruction(name8, uint64_t, sign)  \
    decalreABCInstruction(name9, float, sign)     \
    decalreABCInstruction(name10, double, sign)   \

#define declareABCInstruction(name, sign) decalreABCInstructionSet( \
        name ## _i8, name ## _i16, name ## _i32, name ## _i64,      \
        name ## _u8, name ## _u16, name ## _u32, name ## _u64,      \
        name ## _f32, name ## _f64, sign);


#define decalreAUInstruction(name, type, sign) void name(struct _nmEnvironment* env, void** data) { *(type*)data[0] = sign (uint64_t)(*(type*)data[1]); }
#define decalreAUInstructionSet(name1, name2, name3, name4, name5, name6, name7, name8, name9, name10, sign)\
    decalreAUInstruction(name1, int8_t, sign)    \
    decalreAUInstruction(name2, int16_t, sign)   \
    decalreAUInstruction(name3, int32_t, sign)   \
    decalreAUInstruction(name4, int64_t, sign)   \
    decalreAUInstruction(name5, uint8_t, sign)   \
    decalreAUInstruction(name6, uint16_t, sign)  \
    decalreAUInstruction(name7, uint32_t, sign)  \
    decalreAUInstruction(name8, uint64_t, sign)  \
    decalreAUInstruction(name9, float, sign)     \
    decalreAUInstruction(name10, double, sign)   \

#define declareAUInstruction(name, sign) decalreAUInstructionSet( \
        name ## _i8, name ## _i16, name ## _i32, name ## _i64,      \
        name ## _u8, name ## _u16, name ## _u32, name ## _u64,      \
        name ## _f32, name ## _f64, sign);

#endif //NMRUNNER_INSTRUCTIONSMACRO_H
