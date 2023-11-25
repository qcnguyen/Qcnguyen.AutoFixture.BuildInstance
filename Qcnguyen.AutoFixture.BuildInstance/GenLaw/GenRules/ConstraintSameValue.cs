using AutoFixture;
using AutoFixture.Dsl;
using System;
using System.Linq.Expressions;

namespace Qcnguyen.AutoFixture.BuildInstance.GenLaw
{
    public class ConstraintSameValue<T, TProp> : BaseConstraintRule<T>
    {
        Func<T, TProp> _expected;
        Expression<Func<T, TProp>> _propertyPicker;
        public ConstraintSameValue(Expression<Func<T, TProp>> propertyPicker, Func<T, TProp> expected)
        {
            _expected = expected;
            _propertyPicker = propertyPicker;
        }

        public override IPostprocessComposer<T> ApplyRule(IPostprocessComposer<T> composer)
        {
            var obj = composer.Create();
            TProp exp;
            try
            {
                exp = _expected(obj);
            }
            catch (NullReferenceException)
            {
                exp = default;
            }
            composer = composer.With(_propertyPicker, () => exp);
            return composer;
        }
    }
}
