namespace Nevermind.ByteCode
{
    internal enum InstructionType
    {
        Ret,
        Push,
        Pop,
        Ldi,
        Jmp,
        Call,
        BrEq,
        Cast,
        Vget,
        Vset,

        _Unary,
        _Binary,
    }
}