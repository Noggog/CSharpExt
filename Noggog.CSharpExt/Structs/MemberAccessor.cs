using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
#nullable disable

namespace Noggog
{
    public struct MemberAccessor<I, T>
    {
        public readonly Action<I, T> Setter;
        public readonly Func<I, T> Getter;

        public MemberAccessor(
            Action<I, T> setter,
            Func<I, T> getter)
        {
            this.Setter = setter;
            this.Getter = getter;
        }

        public MemberAccessor(
            Expression<Func<I, T>> getterExpr,
            Expression<Action<I, T>> setterExpr)
        {
            Func<I, T> get = getterExpr.Compile();
            this.Getter = (obj) =>
            {
                return get((I)obj);
            };
            Action<I, T> set = setterExpr.Compile();
            this.Setter = (obj, val) =>
            {
                set((I)obj, val);
            };
        }

        public MemberAccessor(Expression<Func<I, T>> propertyExpression)
        {
            if (propertyExpression.Body.NodeType != ExpressionType.Parameter)
            {
                Process<T>(propertyExpression.Body, out Func<object, T> tmpGetter, out Action<object, T> tmpSetter, out bool pass);
                if (!pass)
                {
                    throw new NotImplementedException("Node type of " + propertyExpression.Body.NodeType + " is not yet implemented for MemberAccessor");
                }
                this.Getter = (i) => tmpGetter(i);
                this.Setter = (i, val) => tmpSetter(i, val);
            }
            else
            {
                this.Setter = null;
            }
            Func<I, T> f = propertyExpression.Compile();
            this.Getter = (obj) =>
            {
                return f((I)obj);
            };
        }

        private static void Process<V>(Expression expr, out Func<object, V> getter, out Action<object, V> setter, out bool pass)
        {
            Expression parentExpr;
            switch (expr.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpr = expr as MemberExpression;
                    var propertyInfo = memberExpr.Member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        List<PropertyInfo> nestedPropertyInfos = new List<PropertyInfo>();
                        setter = (i, t) =>
                        {
                            propertyInfo.SetValue(i, t, null);
                        };
                        getter = (i) =>
                        {
                            return (V)propertyInfo.GetValue(i, null);
                        };
                    }
                    else
                    {
                        var fieldInfo = memberExpr.Member as FieldInfo;
                        setter = (i, t) =>
                        {
                            fieldInfo.SetValue(i, t);
                        };
                        getter = (i) =>
                        {
                            return (V)fieldInfo.GetValue(i);
                        };
                    }

                    pass = true;
                    parentExpr = memberExpr.Expression;
                    break;
                default:
                    setter = null;
                    getter = null;
                    pass = false;
                    return;
            }

            if (parentExpr != null)
            {
                Process(parentExpr, out Func<object, object> parentGetter, out Action<object, object> parentSetter, out bool passParent);
                var tmpSetter = setter;
                var tmpGetter = getter;
                if (passParent)
                {
                    setter = (obj, item) =>
                    {
                        tmpSetter(parentGetter(obj), item);
                    };
                    getter = (obj) =>
                    {
                        return tmpGetter(parentGetter(obj));
                    };
                }
            }
            pass = true;
        }

        public static implicit operator MemberAccessor<I, T>(Expression<Func<I, T>> expr)
        {
            return new MemberAccessor<I, T>(expr);
        }
    }
}
