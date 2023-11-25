using AutoFixture.Dsl;

namespace Qcnguyen.AutoFixture.BuildInstance.GenLaw
{
    public enum GenRuleType
    {
        Default,
        Constraint
    }
    public abstract class BaseGenRule<T>
    {
        public abstract IPostprocessComposer<T> ApplyRule(IPostprocessComposer<T> composer);
        public abstract GenRuleType RuleType { get; }
    }

    public abstract class BaseDefaultRule<T> : BaseGenRule<T>
    {
        public override GenRuleType RuleType => GenRuleType.Default;
    }

    public abstract class BaseConstraintRule<T> : BaseGenRule<T>
    {
        public override GenRuleType RuleType => GenRuleType.Constraint;
    }
}
