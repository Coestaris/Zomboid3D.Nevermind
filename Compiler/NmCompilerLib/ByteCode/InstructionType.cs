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

        _Unary,
        _Binary
    }
}