using Moq;
using NineMansMorrisModel.Model;
using NineMansMorrisModel.Persistence;

namespace MillTest
{
    [TestClass]
    public class MillTest
    {
        private MillModel model = null!;
        private Mock<IFIleManager> mockedManager = null!;
        private MillGameData mockGameData = null!;

        [TestInitialize]
        public void Init()
        {
            MillBoard mockBoard = new MillBoard();
            mockGameData = new MillGameData();

            mockBoard.GetNodeOn(new Coordinate(0, 0, 0)).SetPiece(PlayerColor.WHITE);
            mockBoard.GetNodeOn(new Coordinate(1, 2, 0)).SetPiece(PlayerColor.BLACK);
            mockBoard.GetNodeOn(new Coordinate(2, 1, 0)).SetPiece(PlayerColor.WHITE);

            mockGameData.Data_GamePhase = GamePhase.PLACE;
            mockGameData.Data_Board = mockBoard;
            mockGameData.Data_CurrentPlayerName = "BLACK PLAYER";
            mockGameData.Data_WhitePlacedPieces = 2;
            mockGameData.Data_WhitePiecesOnBoard = 2;
            mockGameData.Data_BlackPlacedPieces = 1;
            mockGameData.Data_BlackPiecesOnBoard = 1;
            mockGameData.Data_SelectedNode = null;

            mockedManager = new Mock<IFIleManager>();
            mockedManager.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
            .Returns(() => Task.FromResult(mockGameData));
            model = new MillModel(mockedManager.Object);
        }

        private void SkipToMovePhase()
        {
            model.PlayerWhite.InitFromSave(9, model.PlayerWhite.PiecesOnBoard);
            model.PlayerBlack.InitFromSave(9, model.PlayerBlack.PiecesOnBoard);
        }

        private void PlaceTestPieces()
        {
            model.PlayerPlacesPiece(new Coordinate(0, 2, 2));
            model.PlayerPlacesPiece(new Coordinate(1, 2, 0));
            model.PlayerPlacesPiece(new Coordinate(2, 2, 2));
            model.PlayerPlacesPiece(new Coordinate(0, 1, 2));
            model.PlayerPlacesPiece(new Coordinate(1, 2, 1));
            model.PlayerPlacesPiece(new Coordinate(0, 0, 0));
        }

        [TestMethod]
        public async Task NMM_LoadGameTest()
        {
            model.NewGame();

            // majd betöltünk egy játékot
            await model.LoadGameAsync(String.Empty);

            model.Board.IterateOverBoard(node =>
            {
                Assert.AreSame(node, mockGameData.Data_Board.GetNodeOn(node.Coordinate));
            });

            Assert.AreEqual(model.GamePhase, mockGameData.Data_GamePhase);
            Assert.AreEqual(model.CurrentPlayer.Name, mockGameData.Data_CurrentPlayerName);
            Assert.AreEqual(model.PlayerWhite.PlacedPieces, mockGameData.Data_WhitePlacedPieces);
            Assert.AreEqual(model.PlayerWhite.PiecesOnBoard, mockGameData.Data_WhitePiecesOnBoard);
            Assert.AreEqual(model.PlayerBlack.PlacedPieces, mockGameData.Data_BlackPlacedPieces);
            Assert.AreEqual(model.PlayerBlack.PiecesOnBoard, mockGameData.Data_BlackPiecesOnBoard);
        }

        [TestMethod]
        public void NMM_NewGameTest()
        {
            model.NewGame();

            Assert.AreEqual(model.PlayerBlack.PiecesOnBoard, 0);
            Assert.AreEqual(model.PlayerWhite.PiecesOnBoard, 0);

            int emptyNodes = 0;
            model.Board.IterateOverBoard(node =>
            {
                if (node.IsEmpty())
                {
                    emptyNodes++;
                }
            });
            Assert.AreEqual(27, emptyNodes);
        }

        [TestMethod]
        public void NMM_PlaceTest()
        {
            model.NewGame();
            PlaceTestPieces();

            Assert.AreEqual(model.PlayerWhite.PiecesOnBoard, 3);
            Assert.AreEqual(model.PlayerBlack.PiecesOnBoard, 3);
        }

        [TestMethod]
        public void NMM_MoveTest()
        {
            model.NewGame();
            PlaceTestPieces();
            SkipToMovePhase();

            model.PlayerSelectsPieceForMoving(new Coordinate(0, 2, 2));
            model.PlayerSelectsMoveDestination(new Coordinate(1, 2, 2));
            model.PlayerSelectsPieceForMoving(new Coordinate(0, 1, 2));
            model.PlayerSelectsMoveDestination(new Coordinate(0, 2, 2));

            Assert.IsTrue(model.CurrentPlayer == model.PlayerWhite);
            Assert.IsTrue(model.Board.GetNodeOn(new Coordinate(1, 2, 2)).Piece == PlayerColor.WHITE);
            Assert.IsTrue(model.Board.GetNodeOn(new Coordinate(0, 2, 2)).Piece == PlayerColor.BLACK);
            Assert.IsTrue(model.Board.GetNodeOn(new Coordinate(0, 1, 2)).Piece == PlayerColor.NONE);
        }

        [TestMethod]
        public void NMM_MillMadeTest()
        {
            model.NewGame();
            model.PlayerPlacesPiece(new Coordinate(0, 0, 0));//w
            model.PlayerPlacesPiece(new Coordinate(0, 0, 1));//b
            model.PlayerPlacesPiece(new Coordinate(0, 1, 0));//w
            model.PlayerPlacesPiece(new Coordinate(0, 1, 1));//b
            model.PlayerPlacesPiece(new Coordinate(0, 2, 0));//w
            model.PlayerPicksUpOpponentPiece(new Coordinate(0, 0, 1));//w

            Assert.AreEqual(model.PlayerBlack.PiecesOnBoard, 1);
            Assert.AreEqual(model.PlayerWhite.PiecesOnBoard, 3);
            Assert.IsTrue(model.Board.GetNodeOn(new Coordinate(0, 0, 0)).IsInAMill);
            Assert.IsTrue(model.Board.GetNodeOn(new Coordinate(0, 1, 0)).IsInAMill);
            Assert.IsTrue(model.Board.GetNodeOn(new Coordinate(0, 2, 0)).IsInAMill);

            SkipToMovePhase();

            model.PlayerSelectsPieceForMoving(new Coordinate(0, 1, 1)); //b
            model.PlayerSelectsMoveDestination(new Coordinate(0, 0, 1));//b

            model.PlayerSelectsPieceForMoving(new Coordinate(0, 1, 0));//w
            model.PlayerSelectsMoveDestination(new Coordinate(0, 1, 1));//w

            model.PlayerSelectsPieceForMoving(new Coordinate(0, 0, 1));//b
            model.PlayerSelectsMoveDestination(new Coordinate(1, 0, 1));//b

            model.PlayerSelectsPieceForMoving(new Coordinate(0, 1, 1));//w
            model.PlayerSelectsMoveDestination(new Coordinate(0, 1, 0));//w
            model.PlayerPicksUpOpponentPiece(new Coordinate(1, 0, 1));//w

            Assert.AreEqual(model.Board.GetNodeOn(new Coordinate(1, 0, 1)).Piece, PlayerColor.NONE);
            Assert.AreEqual(model.PlayerBlack.PiecesOnBoard, 0);
        }
    }
}