using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazingState.WebAssembly
{
    internal static class FieldInfoExtensions
    {
        static public Func<TType, TResult> CreateGetFieldDelegate<TType, TResult>(this FieldInfo fieldInfo)
        {
            var instExp = Expression.Parameter(typeof(TType));
            var fieldExp = Expression.Field(instExp, fieldInfo);
            return Expression.Lambda<Func<TType, TResult>>(fieldExp, instExp).Compile();
        }
    }
}
