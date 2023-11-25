using AutoFixture.Dsl;
using System;
using System.Linq.Expressions;

namespace Qcnguyen.AutoFixture.BuildInstance.GenLaw
{
    public class DefaultValue<T, TProp> : BaseDefaultRule<T>
    {
        Expression<Func<T, TProp>> _propertyPicker;
        Func<TProp> _valueFactory;
        TProp _value;

        public DefaultValue(Expression<Func<T, TProp>> propertyPicker, Func<TProp> valFactory)
        {
            _propertyPicker = propertyPicker;
            _valueFactory = valFactory;
        }

        public DefaultValue(Expression<Func<T, TProp>> propertyPicker, TProp val)
        {
            _propertyPicker = propertyPicker;
            _value = val;
        }

        public override IPostprocessComposer<T> ApplyRule(IPostprocessComposer<T> composer)
        {
            if (_valueFactory == null)
                composer = composer.With(_propertyPicker, _value);
            else
                composer = composer.With(_propertyPicker, _valueFactory);
            return composer;
        }
    }
}
