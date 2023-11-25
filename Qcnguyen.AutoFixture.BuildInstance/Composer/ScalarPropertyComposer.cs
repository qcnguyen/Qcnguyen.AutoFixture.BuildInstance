using Qcnguyen.AutoFixture.BuildInstance.GenLaw;
using System;
using System.Linq.Expressions;

namespace AutoFixture
{
    interface IScalarPropertyComposer<TOwnerComposer>
    {
        TOwnerComposer Done();
    }
    public class ScalarPropertyComposer<TOwner, T, TOwnerComposer> : BaseComposer<ScalarPropertyComposer<TOwner, T, TOwnerComposer>, T>, IScalarPropertyComposer<TOwnerComposer>
        where TOwnerComposer : BaseComposer<TOwnerComposer, TOwner>
    {
        private Expression<Func<TOwner, T>> _expression;
        private TOwnerComposer _ownerComposer;
        public ScalarPropertyComposer(TOwnerComposer ownerComposer, Expression<Func<TOwner, T>> expression,
            GenLaw law)
            : base(ownerComposer._fixture, law)
        {
            _expression = expression;
            _ownerComposer = ownerComposer;
        }

        public TOwnerComposer Done()
        {
            ApplyConstraintRuleToComposer();
            _ownerComposer.With(_expression, _composer.Create());
            return _ownerComposer;
        }
    }
}
