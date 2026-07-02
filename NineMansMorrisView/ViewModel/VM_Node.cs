using NineMansMorrisModel.Model;

namespace NineMansMorrisView.ViewModel
{
    public class VM_Node : ViewModelBase
    {
        private PlayerColor piece;
        private bool isInAMill;
        private bool canInteract;
        private Coordinate coordinate = null!;

        private int canvasX;
        private int canvasY;

        public VM_Node(Node node, DelegateCommand command)
        {
            coordinate = node.Coordinate;
            piece = node.Piece;
            isInAMill = node.IsInAMill;
            canInteract = node.CanInteract;
            NodeClickCommand = command;

            canvasX = borderOffset + coordinate.J * RingDistance(coordinate.Ring) + ringSpacing * coordinate.Ring;
            canvasY = borderOffset + coordinate.I * RingDistance(coordinate.Ring) + ringSpacing * coordinate.Ring;
        }

        public int CanvasY => canvasY;
        public int CanvasX => canvasX;
        public Coordinate BoardCoordinate => coordinate;

        public PlayerColor Piece
        {
            get { return piece; }
            set
            {
                if (piece != value)
                {
                    piece = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsInAMill
        {
            get
            {
                return isInAMill;
            }
            set
            {
                if (isInAMill != value)
                {
                    isInAMill = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanInteract
        {
            get
            {
                return canInteract;
            }
            set
            {
                if (canInteract != value)
                {
                    canInteract = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand? NodeClickCommand { get; set; }
    }
}