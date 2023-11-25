using AutoFixture.Dsl;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Qcnguyen.AutoFixture.BuildInstance.GenLaw;
using System.Linq;
using System.Reflection;

namespace AutoFixture
{
    public abstract class BaseComposer<TComposer, TObject>
        where TComposer : BaseComposer<TComposer, TObject>
    {
        protected IPostprocessComposer<TObject> _composer;
        internal readonly Fixture _fixture;
        protected GenLaw _law;
        HashSet<string> _definedProps = new HashSet<string>();
        protected BaseComposer(Fixture fixture, GenLaw law)
        {
            _fixture = fixture;
            
            _law = law;
            _composer = fixture.Build<TObject>();
            _composer = ApplyDefaultLawToComposer(_composer);
        }

        protected void ApplyConstraintRuleToComposer()
        {
            _composer = ApplyConstraintRuleToComposer(_composer);
        }

        protected IPostprocessComposer<T> ApplyConstraintRuleToComposer<T>(IPostprocessComposer<T> composer)
        {
            if (_law != null)
            {
                var rules = _law.GetRuleSet<T>();
                composer = rules.ApplyConstraint(composer, _law, _fixture);
            }
            return composer;
        }

        protected IPostprocessComposer<T> ApplyDefaultLawToComposer<T>(IPostprocessComposer<T> composer)
        {
            if (_law != null)
            {
                var rules = _law.GetRuleSet<T>();
                composer = rules.ApplyDefault(composer, _law, _fixture);
            }
            return composer;
        }

        protected void ApplyDefaultLawToComplexProp()
        {
            if (_law != null)
            {
                var allTypes = _law.GetAllTypeInLaw();
                var propNeedBuild = typeof(TObject).GetProperties().Where(x => !_definedProps.Contains(x.Name) && 
                    (allTypes.Any(a => a == x.PropertyType || 
                        (IsSubclassOfRawGeneric(typeof(IEnumerable<>), x.PropertyType) && a == x.PropertyType.GetGenericArguments().First())) )
                );
                foreach(var p in propNeedBuild)
                {
                    if(IsSubclassOfRawGeneric(typeof(IEnumerable<>), p.PropertyType))
                    {
                        ParameterExpression parameter = Expression.Parameter(typeof(TObject), "x");

                        // Create a member access expression using the PropertyInfo
                        MemberExpression propertyExpression = Expression.Property(parameter, p);
                        var lambda = Expression.Lambda(propertyExpression, parameter);
                        var genericMethod = this.GetType().GetMethod(nameof(BuildCollectionProperty));
                        MethodInfo method = genericMethod.MakeGenericMethod(p.PropertyType.GetGenericArguments().First());
                        var v = method.Invoke(this, new object[] { lambda }) as ICollectionItemPropertyComposer<TComposer>;
                        v.AddManyThenDone();
                    }
                    else
                    {
                        ParameterExpression parameter = Expression.Parameter(typeof(TObject), "x");

                        // Create a member access expression using the PropertyInfo
                        MemberExpression propertyExpression = Expression.Property(parameter, p);
                        var lambda = Expression.Lambda(propertyExpression, parameter);
                        var genericMethod = this.GetType().GetMethod(nameof(BuildScalarProperty));
                        MethodInfo method = genericMethod.MakeGenericMethod(p.PropertyType);
                        var v = method.Invoke(this, new object[] { lambda }) as IScalarPropertyComposer<TComposer>;
                        v.Done();

                    }
                    
                }
                
            }
        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == current)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public TComposer With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, Func<TProperty> valueFactory)
        {
            var propName = (propertyPicker.Body as MemberExpression).Member.Name;
            _definedProps.Add(propName);
            _composer = _composer.With(propertyPicker, valueFactory);
            return this as TComposer;
        }

        public TComposer Without<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker)
        {
            var propName = (propertyPicker.Body as MemberExpression).Member.Name;
            _definedProps.Add(propName);
            _composer = _composer.Without(propertyPicker);
            return this as TComposer;
        }

        public TComposer With<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, TProperty val)
        {
            var propName = (propertyPicker.Body as MemberExpression).Member.Name;
            _definedProps.Add(propName);
            _composer = _composer.With(propertyPicker, val);
            return this as TComposer;
        }

        public TComposer WithAnyExcept<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker, TProperty val) where TProperty : Enum
        {
            var propName = (propertyPicker.Body as MemberExpression).Member.Name;
            _definedProps.Add(propName);
            _composer = _composer.With(propertyPicker, () => _fixture.GenerateExcept(val));
            return this as TComposer;
        }

        public ScalarPropertyComposer<TObject, TProperty, TComposer> BuildScalarProperty<TProperty>(Expression<Func<TObject, TProperty>> propertyPicker) where TProperty : class
        {
            var propName = (propertyPicker.Body as MemberExpression).Member.Name;
            _definedProps.Add(propName);
            return new ScalarPropertyComposer<TObject, TProperty, TComposer>(this as TComposer, propertyPicker, _law);
        }

        public CollectionItemPropertyComposer<TObject, TProperty, TComposer> BuildCollectionProperty<TProperty>(Expression<Func<TObject, IEnumerable<TProperty>>> propertyPicker) where TProperty : class
        {
            var propName = (propertyPicker.Body as MemberExpression).Member.Name;
            _definedProps.Add(propName);
            return new CollectionItemPropertyComposer<TObject, TProperty, TComposer>(this as TComposer, propertyPicker, _law);
        }

    }
}
