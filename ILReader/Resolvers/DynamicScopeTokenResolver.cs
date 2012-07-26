using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using ReflectionDemos.ILParser.Resolvers;
using ILReader.Base;

namespace ClrTest.Reflection
{
    
    // 
    // The following implementation is based on internal DynamicMethod implementation. 
    // We do not recommend use such approach in any production code; and 
    // it will most likley be broken in the future CLR release.
    // 

    public class DynamicScopeTokenResolver : TokenResolverBase
    {
        static PropertyInfo s_indexer;
        static FieldInfo s_scopeFi;

        static Type s_genMethodInfoType;
        static FieldInfo s_genmethFi1, s_genmethFi2;

        static Type s_varArgMethodType;
        static FieldInfo s_varargFi1, s_varargFi2;

        static Type s_genFieldInfoType;
        static FieldInfo s_genFieldFi1, s_genFieldFi2;

        static DynamicScopeTokenResolver()
        {
            BindingFlags s_bfInternal = BindingFlags.NonPublic | BindingFlags.Instance;
            s_indexer = Type.GetType("System.Reflection.Emit.DynamicScope").GetProperty("Item", s_bfInternal);
            s_scopeFi = Type.GetType("System.Reflection.Emit.DynamicILGenerator").GetField("m_scope", s_bfInternal);

            s_varArgMethodType = Type.GetType("System.Reflection.Emit.VarArgMethod");
            s_varargFi1 = s_varArgMethodType.GetField("m_method", s_bfInternal);
            s_varargFi2 = s_varArgMethodType.GetField("m_signature", s_bfInternal);

            s_genMethodInfoType = Type.GetType("System.Reflection.Emit.GenericMethodInfo");
            s_genmethFi1 = s_genMethodInfoType.GetField("m_method", s_bfInternal);
            s_genmethFi2 = s_genMethodInfoType.GetField("m_context", s_bfInternal);

            s_genFieldInfoType = Type.GetType("System.Reflection.Emit.GenericFieldInfo");
            s_genFieldFi1 = s_genFieldInfoType.GetField("m_field", s_bfInternal);
            s_genFieldFi2 = s_genFieldInfoType.GetField("m_context", s_bfInternal);
        }

        object m_scope = null;

        public DynamicScopeTokenResolver(DynamicMethod dm)
        {
            m_scope = s_scopeFi.GetValue(dm.GetILGenerator());
        }

        internal object this[int token]
        {
            get { return s_indexer.GetValue(m_scope, new object[] { token }); }
        }

        public override String AsString(int token) { return this[token] as string; }

        public override FieldInfo AsField(int token)
        {
            if (this[token].GetType() == s_genFieldInfoType)
                return FieldInfo.GetFieldFromHandle(
                    (RuntimeFieldHandle)s_genFieldFi1.GetValue(this[token]),
                    (RuntimeTypeHandle)s_genFieldFi2.GetValue(this[token]));
            else
                return FieldInfo.GetFieldFromHandle((RuntimeFieldHandle)this[token]);
        }

        public override Type AsType(int token)
        {
            return Type.GetTypeFromHandle((RuntimeTypeHandle)this[token]);
        }

        public override MethodBase AsMethod(int token)
        {
            if (this[token] is DynamicMethod)
                return this[token] as DynamicMethod;

            if (this[token] is RuntimeMethodHandle)
                return MethodBase.GetMethodFromHandle((RuntimeMethodHandle)this[token]);

            if (this[token].GetType() == s_genMethodInfoType)
                return MethodBase.GetMethodFromHandle(
                    (RuntimeMethodHandle)s_genmethFi1.GetValue(this[token]),
                    (RuntimeTypeHandle)s_genmethFi2.GetValue(this[token]));

            if (this[token].GetType() == s_varArgMethodType)
                return (MethodInfo)s_varargFi1.GetValue(this[token]);

            Debug.Assert(false, string.Format("unexpected type: {0}", this[token].GetType()));
            return null;
        }


        public override byte[] AsSignature(int token)
        {
            return this[token] as byte[];
        }
    }
}
