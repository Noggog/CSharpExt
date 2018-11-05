using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;

namespace CSharpExt.Rx
{
    // ToDo
    // Eventually redo to a SourceArray object
    public class SourceBoundedSetList<T> : SourceSetList<T>
    {
        private int _MaxValue;
        public int MaxValue
        {
            get => _MaxValue;
            set => SetMaxValue(value);
        }

        public SourceBoundedSetList(int max = int.MaxValue, IObservable<IChangeSet<T>> source = null)
            : base(source)
        {
            this.MaxValue = max;
        }

        public override void Edit(Action<IExtendedList<T>> updateAction, bool hasBeenSet)
        {
            base.Edit(updateAction, hasBeenSet);
            if (this.Count > this.MaxValue)
            {
                throw new ArgumentException($"Executed an edit on a list that would make it bigger than the allowed value {this.Count} > {_MaxValue}");
            }
        }

        private void SetMaxValue(int max)
        {
            if (max < this.Count)
            {
                throw new ArgumentException($"Max was set on a list that was bigger than the allowed value {this.Count} > {max}");
            }
            this._MaxValue = max;
        }
    }
}
