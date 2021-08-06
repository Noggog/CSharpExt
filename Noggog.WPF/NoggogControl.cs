using System.Windows.Controls;

namespace Noggog.WPF
{
    public class NoggogControl : Control
    {
        protected readonly IDisposableBucket _unloadDisposable = new DisposableBucket();
        protected readonly IDisposableBucket _templateDisposable = new DisposableBucket();

        public NoggogControl()
        {
            this.Loaded += (_, _) => OnLoaded();
            this.Loaded += (_, _) =>
            {
                if (Template != null)
                {
                    OnApplyTemplate();
                }
            };
            this.Unloaded += (_, _) =>
            {
                _templateDisposable.Clear();
                _unloadDisposable.Clear();
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _templateDisposable.Clear();
        }

        protected virtual void OnLoaded()
        {
        }
    }
}
