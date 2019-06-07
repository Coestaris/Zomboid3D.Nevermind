using System.Collections.Generic;
using Nevermind.Compiler;
using Nevermind.Compiler.Formats;
using Nevermind.Compiler.LexemeParsing;
using Nevermind.Compiler.LexemeParsing.Lexemes;

namespace Nevermind.ByteCode.Functions
{
    internal class Variable
    {
        public readonly int Scope;
        public readonly Type Type;
        public readonly string Name;

        public Variable(Type type, string name, int scope)
        {
            Type = type;
            Name = name;
            Scope = scope;
        }
    }

    internal class Function
    {
        public readonly string Name;
        public readonly BlockLexeme RawLexeme;
        public FunctionModifier Modifier;

        public Type ReturnType;
        public List<FunctionParameter> Parameters;

        public int Scope;
        public List<Variable> LocalVariables;

        public Function(string name, FunctionModifier modifier, Type returnType, int scope,
            List<FunctionParameter> parameters = null,
            BlockLexeme rawLexeme = null)
        {
            Name = name;
            Modifier = modifier;
            ReturnType = returnType;
            Parameters = parameters ?? new List<FunctionParameter>();
            RawLexeme = rawLexeme;
            Scope = scope;
        }

        public CompileError ResolveLexemes()
        {
            _vars = new List<Variable>();
            CompileError error = null;
            ResolveLocals(RawLexeme, out error);
            LocalVariables = _vars;
            return error;
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

        private static List<Variable> _vars;

        private static void ResolveLocals(BlockLexeme parent, out CompileError error)
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

                    var varWithSameName = _vars.Find(p => p.Name == varLexeme.VarName.StringValue);
                    if (varWithSameName != null)
                    {
                        if(HasInStack(parent, varWithSameName.Scope))
                        {
                            error = new CompileError(CompileErrorType.VariableRedeclaration, varLexeme.VarName);
                            return;
                        }
                    }
                    _vars.Add(new Variable(new Type(varLexeme.TypeName.StringValue), varLexeme.VarName.StringValue, parent.Scope));
                }

                if (lexeme.RequireBlock)
                {
                    ResolveLocals(((ComplexLexeme) lexeme).Block, out error);
                    if (error != null) return;
                }

                if(lexeme.Type == LexemeType.Block)
                {
                    ResolveLocals((BlockLexeme) lexeme, out error);
                    if (error != null) return;
                }
            }
            return;
        }
    }
}