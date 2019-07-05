using System;
using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.Instructions;
using Nevermind.ByteCode.InternalClasses;
// ReSharper disable PossibleNullReferenceException

namespace Nevermind.ByteCode
{
    internal static class ReferenceOptimizer
    {
        public static void Optimize(ByteCode byteCode)
        {
            foreach (var function in byteCode.Instructions)
            {
                DeleteUnusedRegisters(byteCode, function);
            }
        }

        private static void DeleteUnusedRegisters(ByteCode byteCode, FunctionInstructions function)
        {
            var toDelete = new List<int>();
            foreach (var register in function.Registers)
                if (!VariableUsed(register.Index, 0, function))
                    toDelete.Add(register.Index);

            foreach (var variable in toDelete)
                function.Registers.RemoveAll(p => p.Index == variable);
            toDelete.Clear();

            foreach (var local in function.Locals)
                if (!VariableUsed(local.Index, 0, function))
                    toDelete.Add(local.Index);

            foreach (var variable in toDelete)
                function.Locals.RemoveAll(p => p.Index == variable);
        }

        private static bool VariableUsed(int registerIndex, int startIndex, FunctionInstructions function)
        {
            for (var i = startIndex; i < function.Instructions.Count; i++)
            {
                if (function.Instructions[i].UsesVariable(registerIndex))
                    return true;
            }
            return false;
        }
    }
}