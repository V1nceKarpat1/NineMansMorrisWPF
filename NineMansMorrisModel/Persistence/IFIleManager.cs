using NineMansMorrisModel.Model;

namespace NineMansMorrisModel.Persistence
{
    public interface IFIleManager
    {
        Task<MillGameData> LoadAsync(string path);

        Task SaveAsync(MillGameData gameData, string path);
    }
}