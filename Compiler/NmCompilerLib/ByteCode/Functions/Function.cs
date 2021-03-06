using System.Collections.Generic;
using System.Text;
using Nevermind.ByteCode.Types;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;
using Nevermind.Compiler.Semantics;

namespace Nevermind.ByteCode.Functions
{
    internal class Function
    {
        public readonly BlockLexeme RawLexeme;
        public string Name;
        public FunctionModifier Modifier;
        public Token Token;

        public Type ReturnType;
        public List<FunctionParameter> Parameters;

        public int Scope;
        public NmProgram Program;
        public List<Variable> LocalVariables;
        public List<Attribute> Attributes;

        public int Index;

        public bool IsVariadic;
        public int ModuleIndex = -1;

        public readonly string OriginalName;

        public Function(NmProgram program, string name, FunctionModifier modifier,
            Type returnType, int scope,
            List<FunctionParameter> parameters = null,
            BlockLexeme rawLexeme = null)
        {
            Program = program;
            Name = name;
            Modifier = modifier;
            ReturnType = returnType;
            Parameters = parameters ?? new List<FunctionParameter>();
            RawLexeme = rawLexeme;
            Scope = scope;
            IsVariadic = false;

            OriginalName = name;
        }

        public CompileError ResolveLexemes()
        {
            LocalVariables = new List<Variable>();
            CompileError error = null;

            //int index = Program.ProgramGlobals.Count;
            int index = 0; //hm....

            ResolveLocals(RawLexeme, out error, ref index);
            if (error != null)
                return error;

            ResolveReturns(ReturnType != null, RawLexeme, out error);
            return error;
        }

        private static void ResolveReturns(bool hasReturnType, BlockLexeme parent, out CompileError error)
        {
            error = null;
            if (parent == null)
                return;

            foreach (var lexeme in parent.ChildLexemes)
            {
                if (lexeme.Type == LexemeType.Return && !hasReturnType)
                {
                    error = new CompileError(CompileErrorType.ReturnInVoidFunction, lexeme.Tokens[0]);
                    return;
                }

                if (lexeme.RequireBlock)
                {
                    ResolveReturns(hasReturnType, ((ComplexLexeme)lexeme).Block, out error);
                    if (error != null) return;
                }

                if (lexeme.Type == LexemeType.Block)
                {
                    ResolveReturns(hasReturnType, (BlockLexeme)lexeme, out error);
                    if (error != null) return;
                }
            }
        }

        private static bool HasInStack(BlockLexeme parent, int scope)
        {
            while (parent.Parent != null)
            {
                if (parent.Scope == scope)
                    return true;
                parent = (BlockLexeme)parent.Parent;
            }
            return false;
        }

        private void ResolveLocals(BlockLexeme parent, out CompileError error, ref int index)
        {
            error = null;
            if (parent == null)
                return;

            foreach (var lexeme in parent.ChildLexemes)
            {
                if (lexeme.Type == LexemeType.Var)
                {
                    var varLexeme = (VarLexeme)lexeme;
                    if(!IdentifierFormat.Match(varLexeme.VarName.StringValue))
                    {
                        error = new CompileError(CompileErrorType.WrongIdentifierFormat, varLexeme.VarName);
                        return;
                    }

                    var varWithSameName = LocalVariables.Find(p => p.Name == varLexeme.VarName.StringValue);
                    if (varWithSameName != null)
                    {
                        if(HasInStack(parent, varWithSameName.Scope))
                        {
                            error = new CompileError(CompileErrorType.VariableRedeclaration, varLexeme.VarName);
                            return;
                        }
                    }

                    if(Program.ProgramGlobals.Find(p => p.Name == varLexeme.VarName.StringValue) != null)
                    {
                        error = new CompileError(CompileErrorType.VariableRedeclaration, varLexeme.VarName);
                        return;
                    }

                    Type t;
                    if ((error = Type.GetType(Program, varLexeme.TypeTokens, out t)) != null) { return; }
                    LocalVariables.Add(new Variable(t, varLexeme.VarName.StringValue, parent.Scope, varLexeme.VarName, index++, VariableType.Variable));
                    varLexeme.Index = index - 1;
                }

                if (lexeme.RequireBlock)
                {
                    ResolveLocals(((ComplexLexeme) lexeme).Block, out error, ref index);
                    if (error != null) return;
                }

                if(lexeme.Type == LexemeType.Block)
                {
                    ResolveLocals((BlockLexeme) lexeme, out error, ref index);
                    if (error != null) return;
                }
            }
        }
    }
}