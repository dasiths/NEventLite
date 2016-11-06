#if !DNXCORE50
    using System;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static Assembly Assembly(this Type type)
        {
            return type.Assembly;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }
    }
#else
    using System.Linq;
    public static class TypeExtensions
    {
        public static Assembly Assembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
    }
#endif
