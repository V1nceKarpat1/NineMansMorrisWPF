namespace NineMansMorrisModel.Model
{
    public enum PlayerColor
    {
        WHITE, BLACK, NONE
    }

    public class Player
    {
        private readonly PlayerColor color;
        private readonly string name;

        private int placedPieces;
        private int piecesOnBoard;

        public Player(PlayerColor color)
        {
            this.color = color;
            name = color == PlayerColor.WHITE ? "WHITE PLAYER" : "BLACK PLAYER";
            Init();
        }

        public PlayerColor Color
        { get { return color; } }

        public int PlacedPieces
        { get { return placedPieces; } }

        public int PiecesOnBoard
        { get { return piecesOnBoard; } }

        public string Name
        { get { return name; } }

        public void Init()
        {
            placedPieces = 0;
            piecesOnBoard = 0;
        }

        public void InitFromSave(int placedPieces, int piecesOnBoard)
        {
            this.placedPieces = placedPieces;
            this.piecesOnBoard = piecesOnBoard;
        }

        public void AddToPlaced()
        {
            placedPieces++;
        }

        public void AddPieceToBoard()
        {
            piecesOnBoard++;
        }

        public void RemovePieceFromBoard()
        {
            piecesOnBoard--;
        }
    }
}