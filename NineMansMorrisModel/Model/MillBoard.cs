namespace NineMansMorrisModel.Model
{
    public class NodeRing
    {
        private const int RINGSIZE = 3;
        private Node[,] nodes;

        public NodeRing(int ring)
        {
            nodes = new Node[RINGSIZE, RINGSIZE];
            for (int i = 0; i < RINGSIZE; i++)
            {
                for (int j = 0; j < RINGSIZE; j++)
                {
                    nodes[i, j] = new Node(new Coordinate(i, j, ring));
                }
            }
        }

        public Node[,] Nodes
        { get { return nodes; } }
    }

    public class MillBoard
    {
        private NodeRing[] board;
        private const int RINGCOUNT = 3;

        public MillBoard()
        {
            board = new NodeRing[RINGCOUNT];
            for (int i = 0; i < RINGCOUNT; i++)
            {
                board[i] = new NodeRing(i);
            }
        }

        public void IterateOverBoard(Action<Node> action)
        {
            foreach (NodeRing nodeRing in board)
            {
                foreach (Node node in nodeRing.Nodes)
                {
                    action(node);
                }
            }
        }

        public bool CanInteract(Coordinate c, Predicate<Node> pr)
        {
            return pr(GetNodeOn(c));
        }

        public Node GetNodeOn(Coordinate coordinate)
        {
            return board[coordinate.Ring].Nodes[coordinate.I, coordinate.J];
        }

        public Node[] GetAllNodesOn(Coordinate[] mill)
        {
            List<Node> nodes = new List<Node>();
            foreach (Coordinate coord in mill)
            {
                nodes.Add(GetNodeOn(coord));
            }
            return nodes.ToArray();
        }

        public Node[] GetAllNodesInBoard()
        {
            List<Node> nodes = new List<Node>();
            IterateOverBoard(node => nodes.Add(node));
            return nodes.ToArray();
        }

        public void CheckIfInMills(Node node)
        //megnézi hogy node benne van e egy malomban (végrehajtva mozgatás után és rakás után)
        {
            TrySetMill(node.Piece, node.HorizontalMill());
            TrySetMill(node.Piece, node.VerticalMill());
        }

        public void CancelMillStates(Node node)
        //az eddig malomnak lévő csúcsokat oldja fel (végrehajtva mozgatás előtt)
        {
            node.SetMillState(false);
            CancelMill(node.HorizontalMill(), node => node.VerticalMill());
            CancelMill(node.VerticalMill(), node => node.HorizontalMill());
        }

        private void CancelMill(Coordinate[] mill, Func<Node, Coordinate[]> othermill)
        {
            Node[] millNodes = GetAllNodesOn(mill);
            foreach (Node node in millNodes)
            {
                if (!isInOtherMill(othermill(node)))
                {
                    node.SetMillState(false);
                }
            }
        }

        private bool isInOtherMill(Coordinate[] otherMill)
        {
            return otherMill.Select(c => GetNodeOn(c).Piece).Distinct().Count() == 1;
        }

        private void TrySetMill(PlayerColor pieceColor, Coordinate[] mill)
        {
            if (Array.TrueForAll(mill, c => GetNodeOn(c).Piece == pieceColor))
            {
                Array.ForEach(mill, c => GetNodeOn(c).SetMillState(true));
            }
        }
    }
}