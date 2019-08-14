using System;
using System.Collections.Generic;
using Nevermind.ByteCode.Functions;
using Nevermind.ByteCode.NMB;
using Nevermind.ByteCode.Types;

namespace Nevermind.Compiler.Semantics.Attributes
{
    internal enum VarRestrictVarType
    {
        Array,
        Vector,
        Variable
    }

    internal enum VarRestrictVarBase
    {
        Integer,
        Float,
        Char,
        Any
    }


    internal class VarRestrictAttribute : Attribute
    {
        public int ParameterIndex;
        public VarRestrictVarType VarType;
        public VarRestrictVarBase VarBase;

        private Dictionary<string, VarRestrictVarType> varTypes = new Dictionary<string, VarRestrictVarType>
        {
            { "array",    VarRestrictVarType.Array },
            { "vector",   VarRestrictVarType.Vector },
            { "variable", VarRestrictVarType.Variable },
        };

        private Dictionary<string, VarRestrictVarBase> varBases = new Dictionary<string, VarRestrictVarBase>
        {
            { "integer", VarRestrictVarBase.Integer },
            { "float",   VarRestrictVarBase.Float },
            { "char",    VarRestrictVarBase.Char },
            { "any",     VarRestrictVarBase.Any },
        };

        protected override CompileError VerifyParameters()
        {
            if(!int.TryParse(Parameters[0].StringValue, out ParameterIndex))
                return new CompileError(CompileErrorType.WrongParameterIndexFormat, Parameters[0]);

            if(!varTypes.TryGetValue(Parameters[1].StringValue, out VarType))
                return new CompileError(CompileErrorType.UnknownVariableTypeFormat, Parameters[1]);

            if(!varBases.TryGetValue(Parameters[2].StringValue, out VarBase))
                return new CompileError(CompileErrorType.UnknownVariableBaseFormat, Parameters[2]);

            if(VarType == VarRestrictVarType.Array && VarBase != VarRestrictVarBase.Any)
                return new CompileError(CompileErrorType.WrongResrtictOptions, Parameters[2]);

            return null;
        }

        public VarRestrictAttribute(List<Token> parameters) : base(AttributeType.VarRestrict, parameters) { }

        private bool CompareBase(VarRestrictVarBase varBase, TypeID typeId)
        {
            if (varBase == VarRestrictVarBase.Integer && typeId != TypeID.Integer)
                return false;
            if (varBase == VarRestrictVarBase.Float && typeId != TypeID.Float)
                return false;
            if (varBase == VarRestrictVarBase.Char && typeId != TypeID.Integer)
                return false;

            return true;
        }

        public bool CheckValues(List<Variable> variables)
        {
            if (ParameterIndex > variables.Count - 1)
                return false;

            var var = variables[ParameterIndex];

            //expected array but found variable
            if ((VarType == VarRestrictVarType.Array || VarType == VarRestrictVarType.Vector) && var.Type.ID != TypeID.Array)
                return false;

            //expected vector but found array
            if (VarType == VarRestrictVarType.Vector && (var.Type as ArrayType).ElementType.ID == TypeID.Array)
                return false;

            //expected variable but found array
            if (VarType == VarRestrictVarType.Variable && var.Type.ID == TypeID.Array)
                return false;

            //check vector type
            if (VarType == VarRestrictVarType.Vector &&
                VarBase != VarRestrictVarBase.Any &&
                !CompareBase(VarBase, (var.Type as ArrayType).ElementType.ID))
                return false;

            //check variable type
            if (VarType == VarRestrictVarType.Variable &&
                VarBase != VarRestrictVarBase.Any &&
                !CompareBase(VarBase, var.Type.ID))
                return false;

            return true;
        }
    }
}