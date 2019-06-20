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
        _Unary,
        _Binary
    }
}