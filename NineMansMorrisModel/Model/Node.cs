namespace NineMansMorrisModel.Model
{
    public class Coordinate
    {
        private readonly int i;
        private readonly int j;
        private readonly int ring;

        public Coordinate(int i, int j, int ring)
        {
            this.i = i;
            this.j = j;
            this.ring = ring;
        }

        public int I
        { get { return i; } }

        public int J
        { get { return j; } }

        public int Ring
        { get { return ring; } }

        public override string ToString()
        {
            return $"{i} {j} {ring}";
        }

        public bool EqualValue(Coordinate other)
        {
            return (i == other.i && j == other.j && ring == other.ring);
        }
    }

    public class Node
    {
        private readonly Coordinate coordinate;

        private PlayerColor piece;
        private bool isInAMill;
        private bool canInteract;

        public Node(Coordinate coordinate)
        {
            this.coordinate = coordinate;

            Init();
        }

        public Coordinate Coordinate
        { get { return coordinate; } }

        public bool IsInAMill
        { get { return isInAMill; } }

        public PlayerColor Piece
        { get { return piece; } }

        public bool CanInteract
        { get { return canInteract; } }

        public bool IsEmpty()
        {
            return piece == PlayerColor.NONE;
        }

        public void SetMillState(bool isInAMill)
        {
            this.isInAMill = isInAMill;
        }

        public void SetPiece(PlayerColor piece)
        {
            this.piece = piece;
        }

        public void SetInteract(bool canInteract)
        {
            this.canInteract = canInteract;
        }

        public void Init()
        {
            piece = PlayerColor.NONE;
            canInteract = true;
            isInAMill = false;
        }

        public void InitFromSave(PlayerColor piece, bool canInteract, bool isInAMill)
        {
            this.piece = piece;
            this.canInteract = canInteract;
            this.isInAMill = isInAMill;
        }

        private Coordinate[] CornerAdjacent()
        {
            Coordinate[] result = new Coordinate[2];
            int I = coordinate.I;
            int J = coordinate.J;
            int Ring = coordinate.Ring;
            result[0] = new Coordinate(I, 1, Ring);
            result[1] = new Coordinate(1, J, Ring);
            return result;
        }

        private Coordinate[] MiddleAdjacent()
        {
            int I = coordinate.I;
            int J = coordinate.J;
            int Ring = coordinate.Ring;
            int neighbours = Ring == 1 ? 4 : 3;
            Coordinate[] result = new Coordinate[neighbours];
            if (I == 1)
            {
                result[0] = new Coordinate(0, J, Ring);
                result[1] = new Coordinate(2, J, Ring);
            }
            else if (J == 1)
            {
                result[0] = new Coordinate(I, 0, Ring);
                result[1] = new Coordinate(I, 2, Ring);
            }
            if (neighbours == 3)
            {
                result[2] = new Coordinate(I, J, 1);
            }
            else if (neighbours == 4)
            {
                result[2] = new Coordinate(I, J, 0);
                result[3] = new Coordinate(I, J, 2);
            }
            return result;
        }

        public Coordinate[] Adjacent()
        {
            if (coordinate.I == 1 || coordinate.J == 1)
            {
                return MiddleAdjacent();
            }
            else return CornerAdjacent();
        }

        public Coordinate[] HorizontalMill()
        {
            Coordinate[] result = new Coordinate[3];
            int I = coordinate.I;
            int J = coordinate.J;
            int Ring = coordinate.Ring;
            if (I != 1)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[j] = new Coordinate(I, j, Ring);
                    // Debug.WriteLine($"Creating HorizontalMill coordinate: I = {I}, J = {j}, Ring = {Ring}");
                }
            }
            else
            {
                for (int r = 0; r < 3; r++)
                {
                    result[r] = new Coordinate(I, J, r);
                    ///Debug.WriteLine($"Creating HorizontalMill coordinate: I = {I}, J = {J}, Ring = {r}");
                }
            }
            return result;
        }

        public Coordinate[] VerticalMill()
        {
            Coordinate[] result = new Coordinate[3];
            int I = coordinate.I;
            int J = coordinate.J;
            int Ring = coordinate.Ring;
            if (J != 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    //  Debug.WriteLine($"Creating VerticalMill coordinate: I = {i}, J = {J}, Ring = {Ring}");
                    result[i] = new Coordinate(i, J, Ring);
                }
            }
            else
            {
                for (int r = 0; r < 3; r++)
                {
                    result[r] = new Coordinate(I, J, r);
                    // Debug.WriteLine($"Creating VerticalMill coordinate: I = {I}, J = {J}, Ring = {r}");
                }
            }

            return result;
        }
    }
}