    public abstract class ChangesViewModel<T> : ViewModelBase where T : ICloneable
    {
        private T _clone;

        private T _SelectedItem;
        public T SelectedItem {
            get => this._SelectedItem;
            set
            {
                if (base.SetProperty(ref this._SelectedItem, value))
                {
                    this.OnSelectedItemChanged(value);
                }
            }
        }

        protected void Clone()
        {
            if (this.SelectedItem == null) return;

            this._clone = (T)this.SelectedItem.Clone();
        }

        protected void Rollback()
        {
            if (this._clone == null) return;

            PropertyInfo[] properties = this.SelectedItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] cloneProperties = this.SelectedItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < properties.Length; i++) {
                if (properties[i].CanWrite) {
                    properties[i].SetValue(this.SelectedItem, cloneProperties[i].GetValue(this._clone));
                }
            }

            this._clone = default;
        }

        protected abstract void OnSelectedItemChanged(T value);

        protected virtual void Commit() => this._clone = default;
    }
