using NineMansMorrisModel.Persistence;

namespace NineMansMorrisModel.Model
{
    public enum GamePhase
    {
        PLACE, MOVE_BEGIN, MOVE_END, PICKUP, GAMEOVER
    }

    public class MillGameData
    {
        public GamePhase Data_GamePhase { get; set; }
        public string Data_CurrentPlayerName { get; set; } = string.Empty;
        public int Data_WhitePlacedPieces { get; set; }
        public int Data_WhitePiecesOnBoard { get; set; }
        public int Data_BlackPlacedPieces { get; set; }
        public int Data_BlackPiecesOnBoard { get; set; }
        public Node? Data_SelectedNode { get; set; }
        public MillBoard Data_Board { get; set; } = null!;
    }

    public class MillModel
    {
        private const int PIECES_PER_PLAYER = 9;

        private readonly Player playerWhite;
        private readonly Player playerBlack;
        private Node? selectedNode;
        private Player currentPlayer;
        private GamePhase gamePhase;
        private MillBoard board;

        private readonly IFIleManager manager;

        public MillModel(IFIleManager manager)
        {
            this.manager = manager;
            board = new MillBoard();
            playerWhite = new Player(PlayerColor.WHITE);
            playerBlack = new Player(PlayerColor.BLACK);
            currentPlayer = playerWhite;
            gamePhase = GamePhase.PLACE;
            selectedNode = null;
        }

        public MillBoard Board
        { get { return board; } }

        public GamePhase GamePhase
        { get { return gamePhase; } }

        public Player CurrentPlayer
        { get { return currentPlayer; } }

        public Player PlayerBlack
        { get { return playerBlack; } }

        public Player PlayerWhite
        { get { return playerWhite; } }

        public event EventHandler? FieldChange;

        public event EventHandler? GameOver;

        public void Deselect()
        {
            if (GamePhase == GamePhase.MOVE_END)
            {
                selectedNode = null;
                UpdateBoardForGamePhase(GamePhase.MOVE_BEGIN);
            }
        }

        public void PassTurn()
        {
            EndTurn();
            ContinueGame();
        }

        private void ContinueGame()
        {
            //meghívódik miután leszedett valaki egy bábut, ha még kell rakni bábukat akkor rakás fázis különben mozgás fázis
            if (playerBlack.PlacedPieces == PIECES_PER_PLAYER && playerWhite.PlacedPieces == PIECES_PER_PLAYER)
            {
                UpdateBoardForGamePhase(GamePhase.MOVE_BEGIN);
            }
            else
            {
                UpdateBoardForGamePhase(GamePhase.PLACE);
            }
        }

        public async Task SaveGameAsync(string path)
        {
            await manager.SaveAsync(GetGameData(), path);
        }

        private MillGameData GetGameData()
        {
            return new MillGameData
            {
                Data_GamePhase = gamePhase,
                Data_WhitePlacedPieces = playerWhite.PlacedPieces,
                Data_WhitePiecesOnBoard = playerWhite.PiecesOnBoard,
                Data_BlackPlacedPieces = playerBlack.PlacedPieces,
                Data_BlackPiecesOnBoard = playerBlack.PiecesOnBoard,
                Data_CurrentPlayerName = currentPlayer.Name,
                Data_SelectedNode = selectedNode,
                Data_Board = board
            };
        }

        public async Task LoadGameAsync(string path)
        {
            MillGameData loadedGame = await manager.LoadAsync(path);
            LoadGameData(loadedGame);
            UpdateBoardForGamePhase(loadedGame.Data_GamePhase);
        }

        private void LoadGameData(MillGameData gameData)
        {
            gamePhase = gameData.Data_GamePhase;
            board = gameData.Data_Board;
            selectedNode = gameData.Data_SelectedNode;
            playerWhite.InitFromSave(gameData.Data_WhitePlacedPieces, gameData.Data_WhitePiecesOnBoard);
            playerBlack.InitFromSave(gameData.Data_BlackPlacedPieces, gameData.Data_BlackPiecesOnBoard);
            currentPlayer = playerWhite.Name == gameData.Data_CurrentPlayerName ? playerWhite : playerBlack;
        }

        private void InitBoard()
        {
            board.IterateOverBoard(node => node.Init());
        }

        private void UpdateBoardInteraction(Func<Node, bool> interactCondition)
        {
            board.IterateOverBoard(node => node.SetInteract(interactCondition(node)));
        }

        private bool Interact_PlayerCanSelect_EmptyNode(Node node)
        {
            return node.IsEmpty();
        }

        private bool Interact_PlayerCanSelect_OwnPiece(Node node)
        {
            return node.Piece == currentPlayer.Color;
        }

        private bool Interact_PlayerCanSelect_MoveDestination(Node node)
        {
            return node.IsEmpty() && board.GetAllNodesOn(selectedNode!.Adjacent()).Contains(node);
        }

        private bool Interact_PlayerCanSelect_OpponentPiece(Node node)
        {
            return !node.IsInAMill && node.Piece != currentPlayer.Color && node.Piece != PlayerColor.NONE;
        }

        private void UpdateBoardForGamePhase(GamePhase gamePhase)
        {
            this.gamePhase = gamePhase;
            switch (gamePhase)
            {
                case GamePhase.PLACE:
                    UpdateBoardInteraction(Interact_PlayerCanSelect_EmptyNode);
                    break;

                case GamePhase.MOVE_BEGIN:
                    UpdateBoardInteraction(Interact_PlayerCanSelect_OwnPiece);
                    break;

                case GamePhase.MOVE_END:
                    UpdateBoardInteraction(Interact_PlayerCanSelect_MoveDestination);
                    break;

                case GamePhase.PICKUP:
                    UpdateBoardInteraction(Interact_PlayerCanSelect_OpponentPiece);
                    break;

                case GamePhase.GAMEOVER:
                    board.IterateOverBoard(node => node.SetInteract(false));
                    break;
            }
            OnFieldsChange();
        }

        public void PlayerExecuteTurn(Coordinate clickPos)
        {
            switch (GamePhase)
            {
                case GamePhase.PLACE:
                    PlayerPlacesPiece(clickPos);
                    break;

                case GamePhase.MOVE_BEGIN:
                    PlayerSelectsPieceForMoving(clickPos);
                    break;

                case GamePhase.MOVE_END:
                    PlayerSelectsMoveDestination(clickPos);
                    break;

                case GamePhase.PICKUP:
                    PlayerPicksUpOpponentPiece(clickPos);
                    break;
            }
        }

        public void NewGame()
        {
            InitBoard();
            playerBlack.Init();
            playerWhite.Init();
            currentPlayer = playerWhite;
            selectedNode = null;
            UpdateBoardForGamePhase(GamePhase.PLACE);
        }

        private void EndTurn()
        {
            currentPlayer = currentPlayer == playerWhite ? playerBlack : playerWhite;
        }

        public void PlayerPlacesPiece(Coordinate coordinate)
        {
            Node node = board.GetNodeOn(coordinate);
            node.SetPiece(currentPlayer.Color);
            currentPlayer.AddPieceToBoard();
            currentPlayer.AddToPlaced();

            if (PlayerMadeAMill(node))
            {
                return;
            }
            PassTurn();
        }

        public void PlayerSelectsPieceForMoving(Coordinate coordinate)
        {
            selectedNode = board.GetNodeOn(coordinate);
            UpdateBoardForGamePhase(GamePhase.MOVE_END);
        }

        public void PlayerSelectsMoveDestination(Coordinate coordinate)
        {
            board.CancelMillStates(selectedNode!);
            selectedNode!.SetPiece(PlayerColor.NONE);
            selectedNode = null;
            Node node = board.GetNodeOn(coordinate);
            node.SetPiece(currentPlayer.Color);
            if (PlayerMadeAMill(node))
            {
                return;
            }
            PassTurn();
        }

        private bool PlayerMadeAMill(Node node)
        {
            board.CheckIfInMills(node);
            if (node.IsInAMill)
            {
                UpdateBoardForGamePhase(GamePhase.PICKUP);
                return true;
            }
            else return false;
        }

        public void PlayerPicksUpOpponentPiece(Coordinate coordinate)
        {
            Node node = board.GetNodeOn(coordinate);
            node.SetPiece(PlayerColor.NONE);
            if (currentPlayer == playerWhite)
            {
                playerBlack.RemovePieceFromBoard();
            }
            else
            {
                playerWhite.RemovePieceFromBoard();
            }
            PassTurn();
            IsGameOver();
        }

        private void IsGameOver()
        {
            if ((playerBlack.PiecesOnBoard < 3 || playerWhite.PiecesOnBoard < 3) && gamePhase != GamePhase.PLACE)
            {
                UpdateBoardForGamePhase(GamePhase.GAMEOVER);
                OnGameOver();
            }
        }

        public string Winner()
        {
            int winner = Math.Max(playerWhite.PiecesOnBoard, playerBlack.PiecesOnBoard);
            return playerWhite.PiecesOnBoard == winner ? playerWhite.Name : playerBlack.Name;
        }

        public void OnFieldsChange()
        {
            FieldChange?.Invoke(this, new EventArgs());
        }

        public void OnGameOver()
        {
            GameOver?.Invoke(this, new EventArgs());
        }
    }
}