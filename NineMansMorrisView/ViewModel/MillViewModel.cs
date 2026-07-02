using NineMansMorrisModel.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NineMansMorrisView.ViewModel
{
    public class MillViewModel : ViewModelBase
    {
        private MillModel model;
        private int activeNodes;
        private Coordinate[] drawPoints =
        {
                //horizontal middle
                new Coordinate(1,0,2),
                new Coordinate(1,2,2),
                //corner
                new Coordinate(0, 0, 0),
                new Coordinate(0,0,1),
                new Coordinate(0,0,2),
                new Coordinate(2, 2, 0),
                new Coordinate(2,2,1),
                new Coordinate(2,2,2),
                //vertical middle
                new Coordinate(0,1,2),
                new Coordinate(2,1,2)
        };



        public event EventHandler? NewGameEvent;

        public event EventHandler? LoadGameEvent;

        public event EventHandler? SaveGameEvent;

        public event EventHandler? PassTurnEvent;

        public event EventHandler? DeselectEvent;

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand PassTurnCommand { get; private set; }
        public DelegateCommand DeselectCommand { get; private set; }

        public int WhitePiecesOnBoard
        { get { return model.PlayerWhite.PiecesOnBoard; } }
        public int WhitePiecesPlaced
        { get { return model.PlayerWhite.PlacedPieces; } }
        public int BlackPiecesOnBoard
        { get { return model.PlayerBlack.PiecesOnBoard; } }
        public int BlackPiecesPlaced
        { get { return model.PlayerBlack.PlacedPieces; } }
        public string CurrentPlayerName
        { get { return model.CurrentPlayer.Name; } }
        public GamePhase GamePhase
        { get { return model.GamePhase; } }
        public bool CanPassTurn
        { get { return activeNodes == 0; } }
        public bool CanDeselect
        { get { return model.GamePhase == GamePhase.MOVE_END; } }
        public ObservableCollection<VM_Node> VMNodes { get; set; }
        public ObservableCollection<VM_Line> VMLines { get; set; }

        public MillViewModel(MillModel model)
        {
            this.model = model;
            model.FieldChange += Model_FieldChange;

            VMNodes = new ObservableCollection<VM_Node>();
            VMLines = new ObservableCollection<VM_Line>();

            NewGameCommand = new DelegateCommand(p => OnNewGame());
            LoadGameCommand = new DelegateCommand(p => OnLoadGame());
            SaveGameCommand = new DelegateCommand(p => OnSaveGame());
            PassTurnCommand = new DelegateCommand(p => OnPassTurn());
            DeselectCommand = new DelegateCommand(p => OnDeselect());

            for (int r = 0; r < 3; r++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!(i == 1 && j == 1))
                        {
                            Node node = new Node(new Coordinate(i, j, r));
                            node.Init();
                            VMNodes.Add(new VM_Node(node,
                                new DelegateCommand(p =>
                                {
                                    if (p is Coordinate c)
                                    {
                                        NodeClick(c);
                                    }
                                })));
                        }
                    }
                }
            }
            SetupBoardLines();
            OnPropertyChanged(nameof(VMNodes));
            RefreshTable();
        }

        private void SetupBoardLines()
        {
            SetupHorizontalLines();
            SetupVerticalLines();
        }

        private void SetupHorizontalLines()
        {
            for (int p = 0; p < drawPoints.Length - 2; p++)
            {
                Coordinate otherEnd;

                Coordinate drawPoint = drawPoints[p];
                if (drawPoint.I == 0 && drawPoint.J == 0)
                {
                    otherEnd = new Coordinate(0, 2, drawPoint.Ring);
                }
                else if (drawPoint.I == 2 && drawPoint.J == 2)
                {
                    otherEnd = new Coordinate(2, 0, drawPoint.Ring);
                }
                else
                {
                    otherEnd = new Coordinate(drawPoint.I, drawPoint.J, 0);
                }
                Debug.WriteLine(otherEnd.ToString());
                VMLines.Add(new VM_Line(drawPoint, otherEnd));
            }
        }

        private void SetupVerticalLines()
        {
            for (int p = 2; p < drawPoints.Length; p++)
            {
                Coordinate otherEnd;

                Coordinate drawPoint = drawPoints[p];

                if (drawPoint.I == 0 && drawPoint.J == 0)
                {
                    otherEnd = new Coordinate(2, 0, drawPoint.Ring);
                }
                else if (drawPoint.I == 2 && drawPoint.J == 2)
                {
                    otherEnd = new Coordinate(0, 2, drawPoint.Ring);
                }
                else
                {
                    otherEnd = new Coordinate(drawPoint.I, drawPoint.J, 0);
                }
                VMLines.Add(new VM_Line(drawPoint, otherEnd));
            }
        }

        private void RefreshTable()
        {
            activeNodes = 0;
            foreach (VM_Node obNode in VMNodes)
            {
                Node node = model.Board.GetNodeOn(obNode.BoardCoordinate);
                obNode.Piece = node.Piece;
                obNode.IsInAMill = node.IsInAMill;
                obNode.CanInteract = node.CanInteract;
                if (obNode.CanInteract)
                {
                    activeNodes++;
                }
            }
            OnPropertyChanged(nameof(WhitePiecesOnBoard));
            OnPropertyChanged(nameof(WhitePiecesPlaced));
            OnPropertyChanged(nameof(BlackPiecesOnBoard));
            OnPropertyChanged(nameof(BlackPiecesPlaced));
            OnPropertyChanged(nameof(CurrentPlayerName));
            OnPropertyChanged(nameof(GamePhase));
            OnPropertyChanged(nameof(CanPassTurn));
            OnPropertyChanged(nameof(CanDeselect));

        }

        private void NodeClick(Coordinate coord)
        {
            if (model.Board.GetNodeOn(coord).CanInteract)
            {
                model.PlayerExecuteTurn(coord);
            }
        }

        public void Model_FieldChange(object? sender, EventArgs e)
        {
            RefreshTable();
        }

        private void OnNewGame()
        {
            model.NewGame();
        }

        private void OnLoadGame()
        {
            LoadGameEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGameEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnDeselect()
        {
            model.Deselect();
        }

        private void OnPassTurn()
        {
            model.PassTurn();
        }
    }
}