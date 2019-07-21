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
        Syscall,

        _Unary,
        _Binary,
    }
}