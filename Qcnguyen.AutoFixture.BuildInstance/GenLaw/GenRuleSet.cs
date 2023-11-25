using AutoFixture;
using AutoFixture.Dsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Qcnguyen.AutoFixture.BuildInstance.GenLaw
{
    public class GenRuleSet<T>
    {
        IList<BaseGenRule<T>> _rules = new List<BaseGenRule<T>>();
        IList<Action<GenLaw, Fixture>> _deferredRuleCreation = new List<Action<GenLaw, Fixture>>();

        public GenRuleSet<T> EnsureStringMaxLen(Expression<Func<T, string>> propertyPicker, int maxLength)
        {
            _rules.Add(new ConstraintStringMaxLen<T>(propertyPicker, maxLength));
            return this;
        }

        public GenRuleSet<T> DefaultValue<TProp>(Expression<Func<T, TProp>> propertyPicker, TProp value)
        {
            _rules.Add(new DefaultValue<T, TProp>(propertyPicker, value));
            return this;
        }

        public GenRuleSet<T> DefaultValue<TProp>(Expression<Func<T, TProp>> propertyPicker, Func<TProp> valueFactory)
        {
            _rules.Add(new DefaultValue<T, TProp>(propertyPicker, valueFactory));
            return this;
        }

        public GenRuleSet<T> DefaultWithout<TProp>(Expression<Func<T, TProp>> propertyPicker)
        {
            _rules.Add(new DefaultWithout<T, TProp>(propertyPicker));
            return this;
        }

        public GenRuleSet<T> EnsureSameValue<TProp>(Expression<Func<T, TProp>> propertyPicker, Func<T, TProp> expected)
        {
            _rules.Add(new ConstraintSameValue<T, TProp>(propertyPicker, expected));
            return this;
        }

        internal IPostprocessComposer<T> ApplyConstraint(IPostprocessComposer<T> composer, GenLaw genRuleSetDictionary, Fixture fixture)
        {
            if (_deferredRuleCreation != null)
            {
                foreach (var act in _deferredRuleCreation)
                {
                    act(genRuleSetDictionary, fixture);
                }
                _deferredRuleCreation = null;
            }

            foreach (var rule in _rules.Where(x => x.RuleType == GenRuleType.Constraint))
            {
                composer = rule.ApplyRule(composer);
            }
            return composer;
        }

        internal IPostprocessComposer<T> ApplyDefault(IPostprocessComposer<T> composer, GenLaw genRuleSetDictionary, Fixture fixture)
        {
            if (_deferredRuleCreation != null)
            {
                foreach (var act in _deferredRuleCreation)
                {
                    act(genRuleSetDictionary, fixture);
                }
                _deferredRuleCreation = null;
            }

            foreach (var rule in _rules.Where(x => x.RuleType == GenRuleType.Default))
            {
                composer = rule.ApplyRule(composer);
            }
            return composer;
        }
    }
}
