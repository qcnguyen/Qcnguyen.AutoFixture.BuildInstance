using AutoFixture.Kernel;
using Qcnguyen.AutoFixture.BuildInstance.GenLaw;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoFixture
{
    public class InstanceComposer<T> : BaseComposer<InstanceComposer<T>, T> where T : class
    {
        public InstanceComposer(Fixture fixture, GenLaw law = null) : base(fixture, law)
        {
        }

        public T Create()
        {
            ApplyConstraintRuleToComposer();
            ApplyDefaultLawToComplexProp();
            var rs = _composer.Create();
            return rs;
        }

        public IEnumerable<T> CreateMany(int count = 3)
        {
            ApplyConstraintRuleToComposer();
            ApplyDefaultLawToComplexProp();
            var rs =_composer.CreateMany(count);
            return rs;
        }
    }

}
