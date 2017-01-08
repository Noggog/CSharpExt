using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System
{
    public struct MemberAccessor<I, T>
    {
        Action<I, T> setter;
        public Action<I, T> Setter { get { return setter; } }
        Func<I, T> getter;
        public Func<I, T> Getter { get { return getter; } }

        public MemberAccessor(
            Action<I, T> setter,
            Func<I, T> getter)
        {
            this.setter = setter;
            this.getter = getter;
        }

        public MemberAccessor(
            Expression<Func<I, T>> getterExpr,
            Expression<Action<I, T>> setterExpr)
        {
            Func<I, T> get = getterExpr.Compile();
            getter = (obj) =>
            {
                return get((I)obj);
            };
            Action<I, T> set = setterExpr.Compile();
            setter = (obj, val) =>
            {
                set((I)obj, val);
            };
        }

        public MemberAccessor(Expression<Func<I, T>> propertyExpression)
        {
            if (propertyExpression.Body.NodeType != ExpressionType.Parameter)
            {
                bool pass;
                Func<object, T> tmpGetter;
                Action<object, T> tmpSetter;
                Process<T>(propertyExpression.Body, out tmpGetter, out tmpSetter, out pass);
                if (!pass)
                {
                    throw new NotImplementedException("Node type of " + propertyExpression.Body.NodeType + " is not yet implemented for MemberAccessor");
                }
                getter = (i) => tmpGetter(i);
                setter = (i, val) => tmpSetter(i, val);
            }
            else
            {
                setter = null;
            }
            Func<I, T> f = propertyExpression.Compile();
            getter = (obj) =>
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
                Func<object, object> parentGetter;
                Action<object, object> parentSetter;
                bool passParent;
                Process(parentExpr, out parentGetter, out parentSetter, out passParent);
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
