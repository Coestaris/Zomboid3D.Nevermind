using System;
using System.Collections.Generic;
using System.Linq;
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
                OptimizeRegisters(byteCode, function);
                DeleteDuplicatedRegisters(byteCode, function);
                FixRegistersIndexes(byteCode, function);
            }
        }

        private static void FixRegistersIndexes(ByteCode byteCode, FunctionInstructions function)
        {
            var index = function.Locals.Count + byteCode.Header.UsedGlobals.Count;
            foreach (var register in function.Registers)
            {
                foreach (var instruction in function.Instructions)
                {
                    instruction.FetchUsedVariables(register.Index).ForEach(p => p.Index = index);
                }
                register.Index = index;
                index++;
            }
        }

        private static void DeleteDuplicatedRegisters(ByteCode byteCode, FunctionInstructions function)
        {
            function.Registers = function.Registers.GroupBy(p => p.Index).Select(p => p.ToList()[0]).ToList();
        }

        private static void OptimizeRegisters(ByteCode byteCode, FunctionInstructions function)
        {
            var freeRegisters = GetFreeRegistersList(function);
            for (var i = 0; i < freeRegisters.Length; i++)
            {
                //Some registers are free
                if (freeRegisters[i] != function.Registers.Count)
                {
                    var usedVariables = function.Instructions[i].FetchUsedVariables(-1);
                    //uses at least one variable
                    if(usedVariables.Count == 0)
                        continue;

                    var usedRegisters = usedVariables.FindAll(p => p.Index >= function.Locals.Count);

                    //uses at least one register
                    if(usedRegisters.Count == 0)
                        continue;

                    var unusedRegisters = GetFreeRegisters(i, function);
                    foreach (var unusedRegister in unusedRegisters)
                    {
                        var replaceableRegister = usedRegisters
                            .Find(p => p.Type == unusedRegister.Type && !VariableUsedBefore(p.Index, i - 1, function));

                        if (replaceableRegister != null)
                        {
                            //Can be replaced
                            ReplaceAllUsages(function, i, replaceableRegister.Index, unusedRegister.Index);

                            //update list and start again
                            freeRegisters = GetFreeRegistersList(function);
                            i = 0;

                            function.Registers.RemoveAll(p => p.Index == replaceableRegister.Index);
                        }
                    }
                }
            }
        }

        private static void ReplaceAllUsages(FunctionInstructions function, int startIndex, int oldIndex, int newIndex)
        {
            for (var i = startIndex; i < function.Instructions.Count; i++)
            {
                function.Instructions[i].ReplaceRegisterUsage(oldIndex, newIndex);
                /*var usedVariables = instruction.FetchUsedVariables(oldIndex);
                if (usedVariables.Count != 0)
                {
                    //usedVariables.ForEach(p => p.Index = newIndex);
                }*/
            }
        }

        private static int[] GetFreeRegistersList(FunctionInstructions function)
        {
            var freeRegisters = new int[function.Instructions.Count];
            for (var i = 0; i < freeRegisters.Length; i++)
            {
                foreach (var register in function.Registers)
                    if (VariableUsed(register.Index, i, function))
                        freeRegisters[i] += 1;
            }

            return freeRegisters;
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

        private static List<Variable> GetFreeRegisters(int startIndex, FunctionInstructions function)
        {
            var freeRegisters = new List<Variable>();
            foreach (var register in function.Registers)
                if (!VariableUsed(register.Index, startIndex, function))
                    freeRegisters.Add(register);

            return freeRegisters;
        }

        private static bool VariableUsedBefore(int registerIndex, int startIndex, FunctionInstructions function)
        {
            for (var i = startIndex; i >= 0; i--)
            {
                if (function.Instructions[i].UsesVariable(registerIndex))
                    return true;
            }
            return false;
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