using AutoFixture.Dsl;
using System;
using System.Linq.Expressions;

namespace Qcnguyen.AutoFixture.BuildInstance.GenLaw
{
    public class DefaultWithout<T, TProp> : BaseDefaultRule<T>
    {
        Expression<Func<T, TProp>> _propertyPicker;
        public DefaultWithout(Expression<Func<T, TProp>> propertyPicker)
        {
            _propertyPicker = propertyPicker;
        }

        public override IPostprocessComposer<T> ApplyRule(IPostprocessComposer<T> composer)
        {
            composer = composer.Without(_propertyPicker);
            return composer;
        }
    }
}
