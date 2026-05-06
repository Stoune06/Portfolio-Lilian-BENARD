using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

//Author : MERFOUD Kelyan

namespace Tooling.Tweens.Tweenables
{
    internal abstract class Tweenable<T> : BaseTweenable
    {
        public T StartValue { get; internal set; }
        public T EndValue { get; internal set; }

        private Action<object, T> _Setter;
        private Func<object, T> _Getter;

        public Tweenable(object pObject, string pPropertyName, T pEndValue)
        {
            BindingFlags lBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MemberInfo pMemberInfo = pObject.GetType().GetProperty(pPropertyName, lBindingFlags);
            if (pMemberInfo == null) pMemberInfo = pObject.GetType().GetField(pPropertyName, lBindingFlags);
            if (pMemberInfo == null) throw new ArgumentException($"Member {pPropertyName} not found in object {pObject.GetType()}");

            m_Object = pObject;

            if (pMemberInfo is FieldInfo pFieldInfo)
            {
                if (pFieldInfo.FieldType != typeof(T)) throw new ArgumentException($"Field {pPropertyName} is not a \"{typeof(T)}\" but a \"{pFieldInfo.FieldType}\"");

                _Setter = CreateSetter(pFieldInfo);
                _Getter = CreateGetter(pFieldInfo);
            }
            else if (pMemberInfo is PropertyInfo pPropertyInfo)
            {
                if (pPropertyInfo.PropertyType != typeof(T)) throw new ArgumentException($"Property {pPropertyName} is not a \"{typeof(T)}\" but a \"{pPropertyInfo.PropertyType}\"");

                _Setter = CreateSetter(pPropertyInfo);
                _Getter = CreateGetter(pPropertyInfo);
            }
            else throw new ArgumentException($"{pPropertyName} from {pObject.GetType()} is not a field nor a property");

            StartValue = _Getter(pObject);
            EndValue = pEndValue;
        }

        public override void Lerp() => _Setter(m_Object, LerpToT());

        public override void Invert()
        {
            T pHolder = StartValue;
            StartValue = EndValue;
            EndValue = pHolder;
        }

        public override object GetStartValue() => StartValue;
        public override object GetEndValue() => EndValue;
        public override void SetStartValue(object pValue) => StartValue = (T)pValue;
        public override void SetEndValue(object pValue) => EndValue = (T)pValue;

        protected abstract T LerpToT();

        private static Action<object, T> CreateSetter(PropertyInfo propertyInfo)
        {
            ParameterExpression pInstance = Expression.Parameter(typeof(object));
            ParameterExpression pValue = Expression.Parameter(typeof(T));

            UnaryExpression pUnary = Expression.Convert(pInstance, propertyInfo.DeclaringType);
            MemberExpression pMember = Expression.Property(pUnary, propertyInfo);
            BinaryExpression pAssign = Expression.Assign(pMember, pValue);

            return Expression.Lambda<Action<object, T>>(pAssign, pInstance, pValue).Compile();
        }

        private static Func<object, T> CreateGetter(PropertyInfo propertyInfo)
        {
            ParameterExpression pInstance = Expression.Parameter(typeof(object));

            UnaryExpression pUnary = Expression.Convert(pInstance, propertyInfo.DeclaringType);
            MemberExpression pMember = Expression.Property(pUnary, propertyInfo);

            return Expression.Lambda<Func<object, T>>(pMember, pInstance).Compile();
        }

        private static Action<object, T> CreateSetter(FieldInfo pFieldInfo)
        {
            ParameterExpression pInstance = Expression.Parameter(typeof(object));
            ParameterExpression pValue = Expression.Parameter(typeof(T));

            UnaryExpression pUnary = Expression.Convert(pInstance, pFieldInfo.DeclaringType);
            MemberExpression pMember = Expression.Field(pUnary, pFieldInfo);
            BinaryExpression pAssign = Expression.Assign(pMember, pValue);

            return Expression.Lambda<Action<object, T>>(pAssign, pInstance, pValue).Compile();
        }

        private static Func<object, T> CreateGetter(FieldInfo pFieldInfo)
        {
            ParameterExpression pInstance = Expression.Parameter(typeof(object));

            UnaryExpression pUnary = Expression.Convert(pInstance, pFieldInfo.DeclaringType);
            MemberExpression pMember = Expression.Field(pUnary, pFieldInfo);

            return Expression.Lambda<Func<object, T>>(pMember, pInstance).Compile();
        }
    }
}
