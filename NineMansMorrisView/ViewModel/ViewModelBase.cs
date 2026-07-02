using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NineMansMorrisView.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected ViewModelBase()
        { }

        protected static readonly int ringSpacing = 100;
        protected static readonly int innerRingDistance = ringSpacing;
        protected static readonly int middleRingDistance = 2 * ringSpacing;
        protected static readonly int outerRingDistance = 3 * ringSpacing;
        protected static readonly int borderOffset = 10;

        protected int RingDistance(int ring)
        {
            switch (ring)
            {
                case 0:
                    return outerRingDistance;

                case 1:
                    return middleRingDistance;

                case 2:
                    return innerRingDistance;

                default:
                    return 0;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}