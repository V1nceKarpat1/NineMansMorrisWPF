using NineMansMorrisModel.Model;

namespace NineMansMorrisModel.Persistence
{
    public class FileManagerException : IOException
    {
        public FileManagerException(string message) : base(message)
        {
        }
    }

    public class MillFileManager : IFIleManager
    {
        //public class MillGameData
        //{
        //    public GamePhase Data_GamePhase { get; set; }
        //    public string Data_CurrentPlayerName { get; set; } = string.Empty;
        //    public int Data_WhitePiecesPlaced { get; set; }
        //    public int Data_BlackPiecesPlaced { get; set; }
        //    public MillBoard BoardData { get; set; } = null!;
        //    public Node? Data_SelectedNode { get; set; }
        //}
        public async Task<MillGameData> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    MillGameData data = new MillGameData();
                    MillBoard loadedGameBoard = new MillBoard();

                    Coordinate? selectedNodeCoord = null;
                    //reading global gamedata
                    data.Data_GamePhase = (GamePhase)Enum.Parse(typeof(GamePhase), await reader.ReadLineAsync() ?? string.Empty);
                    data.Data_CurrentPlayerName = await reader.ReadLineAsync() ?? string.Empty;
                    data.Data_WhitePlacedPieces = int.Parse(await reader.ReadLineAsync() ?? string.Empty);
                    data.Data_WhitePiecesOnBoard = int.Parse(await reader.ReadLineAsync() ?? string.Empty);
                    data.Data_BlackPlacedPieces = int.Parse(await reader.ReadLineAsync() ?? string.Empty);
                    data.Data_BlackPiecesOnBoard = int.Parse(await reader.ReadLineAsync() ?? string.Empty);

                    //reading selectednode
                    string? fileLine;
                    if ((fileLine = await reader.ReadLineAsync()) != "NOSELECT")
                    {
                        string[] line = fileLine!.Split(' ');
                        selectedNodeCoord = new Coordinate(int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]));
                    }

                    //reading nodes
                    for (int i = 0; i < 27; i++)
                    {
                        fileLine = await reader.ReadLineAsync() ?? string.Empty;
                        string[] line = fileLine.Split(' ');
                        int nodeDataNum = line.Length;
                        Coordinate nodeCoordinate = new Coordinate(int.Parse(line[0]), int.Parse(line[1]), int.Parse(line[2]));
                        Node node = loadedGameBoard.GetNodeOn(nodeCoordinate);
                        PlayerColor pieceColor = (PlayerColor)Enum.Parse(typeof(PlayerColor), line[3]);
                        bool isInAMill = false;
                        bool canInteract = false;
                        if (nodeDataNum > 3 && line[4] == "M")
                        {
                            isInAMill = true;
                        }
                        if (nodeDataNum > 4 && line[5] == "I")
                        {
                            canInteract = true;
                        }
                        node.InitFromSave(pieceColor, canInteract, isInAMill);
                    }
                    data.Data_Board = loadedGameBoard;
                    data.Data_SelectedNode = selectedNodeCoord is null ? null : loadedGameBoard.GetNodeOn(selectedNodeCoord);
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new FileManagerException(ex.Message);
            }
        }

        public async Task SaveAsync(MillGameData gameData, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await writer.WriteLineAsync(gameData.Data_GamePhase.ToString());
                    await writer.WriteLineAsync(gameData.Data_CurrentPlayerName);
                    await writer.WriteLineAsync(gameData.Data_WhitePlacedPieces.ToString());
                    await writer.WriteLineAsync(gameData.Data_WhitePiecesOnBoard.ToString());
                    await writer.WriteLineAsync(gameData.Data_BlackPlacedPieces.ToString());
                    await writer.WriteLineAsync(gameData.Data_BlackPiecesOnBoard.ToString());

                    string selectedNodeCoord_S = gameData.Data_SelectedNode != null ? gameData.Data_SelectedNode.Coordinate.ToString() : "NOSELECT";
                    writer.WriteLine(selectedNodeCoord_S);

                    foreach (Node node in gameData.Data_Board.GetAllNodesInBoard())
                    {
                        string coordinate_S = node.Coordinate.ToString();
                        string nodePiece_S = node.Piece.ToString();
                        string isInAMill_S = node.IsInAMill ? "M" : "";
                        string canInteract_S = node.CanInteract ? "I" : "";
                        await writer.WriteLineAsync(coordinate_S + " " + nodePiece_S + " " + isInAMill_S + " " + canInteract_S);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileManagerException(ex.Message);
            }
        }
    }
}