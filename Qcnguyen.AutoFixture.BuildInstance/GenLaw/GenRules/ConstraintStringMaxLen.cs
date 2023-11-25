using AutoFixture;
using AutoFixture.Dsl;
using System;
using System.Linq.Expressions;

namespace Qcnguyen.AutoFixture.BuildInstance.GenLaw
{
    public class ConstraintStringMaxLen<T> : BaseConstraintRule<T>
    {
        Expression<Func<T, string>> _propertyPicker;
        int _maxLength;
        public ConstraintStringMaxLen(Expression<Func<T, string>> propertyPicker, int maxLength)
        {
            _propertyPicker = propertyPicker;
            _maxLength = maxLength;
        }

        public override IPostprocessComposer<T> ApplyRule(IPostprocessComposer<T> composer)
        {
            var obj = composer.Create();
            var val = _propertyPicker.Compile()(obj);
            if (val.Length > _maxLength)
            {
                composer = composer.With(_propertyPicker, () => val.Substring(0, _maxLength));
            }
            return composer;
        }
    }
}
