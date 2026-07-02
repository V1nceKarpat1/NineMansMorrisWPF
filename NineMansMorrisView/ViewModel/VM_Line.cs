using NineMansMorrisModel.Model;

namespace NineMansMorrisView.ViewModel
{
    public class VM_Line : ViewModelBase
    {
        private static readonly int alignToButtonCenter = 13;

        private int x1;
        private int y1;
        private int x2;
        private int y2;
        public int X1 => x1;
        public int Y1 => y1;
        public int X2 => x2;
        public int Y2 => y2;

        public VM_Line(Coordinate c1, Coordinate c2)
        {
            x1 = borderOffset + c1.J * RingDistance(c1.Ring) + ringSpacing * c1.Ring + alignToButtonCenter;
            y1 = borderOffset + c1.I * RingDistance(c1.Ring) + ringSpacing * c1.Ring + alignToButtonCenter;
            x2 = borderOffset + c2.J * RingDistance(c2.Ring) + ringSpacing * c2.Ring + alignToButtonCenter;
            y2 = borderOffset + c2.I * RingDistance(c2.Ring) + ringSpacing * c2.Ring + alignToButtonCenter;
        }
    }
}