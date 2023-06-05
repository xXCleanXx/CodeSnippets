public class Validator
    {
        private readonly List<ValidationCondition> _conditions = new();

        public bool Validate()
        {
            foreach (ValidationCondition item in this._conditions)
            {
                if (!item.IsValid())
                {
                    return false;
                }
            }

            return true;
        }

        public SqlValidationCondition<T> SqlCondition<T>()
        {
            return new(this);
        }

        public ValidationCondition Condition(bool condition)
        {
            return new(this, condition);
        }
    }

    public abstract class ValidationConditionBase
    {
        protected string _warning;
        protected readonly bool _condition;
        protected Action _if;
        protected Action _else;
        protected readonly Validator _validator;

        protected ValidationConditionBase(Validator validator, bool condition)
        {
            this._validator = validator;
            this._condition = condition;
        }

        protected ValidationConditionBase(Validator validator)
        {
            this._validator = validator;
        }

        public ValidationConditionBase Warning(string warning)
        {
            this._warning = warning;

            return this;
        }

        public ValidationConditionBase If(Action action)
        {
            this._if = action;

            return this;
        }

        public ValidationConditionBase Else(Action action) {
            this._else = action;

            return this;
        }

        public Validator Validator()
        {
            return this._validator;
        }

        public virtual bool IsValid()
        {
            if (this._condition)
            {
                this._if?.Invoke();

                return true;
            }

            this._else?.Invoke();

            WindowManager.Warning(this._warning);

            return false;
        }
    }


    public sealed class SqlValidationCondition<T> : ValidationConditionBase
    {
        private new Func<T, bool> _condition;
        private T _param;

        public SqlValidationCondition(Validator validator) : base(validator) { }

        public SqlValidationCondition<T> TryUpdate(T param)
        {
            this._param = param;
            this._condition = ManagerRegistry.Instance.GetManager<T>().TryUpdate;

            return this;
        }

        public SqlValidationCondition<T> TryDelete(T param)
        {
            this._param = param;
            this._condition = ManagerRegistry.Instance.GetManager<T>().TryDelete;

            return this;
        }

        public override bool IsValid()
        {
            if (this._condition == null) throw new InvalidOperationException("Condition must be set!");

            if (this._condition.Invoke(this._param))
            {
                this._if?.Invoke();

                return true;
            }

            this._else?.Invoke();

            WindowManager.Warning(this._warning);

            return false;
        }
    }

    public sealed class ValidationCondition : ValidationConditionBase
    {
        public ValidationCondition(Validator validator, bool condition) : base(validator, condition) { }
    }
