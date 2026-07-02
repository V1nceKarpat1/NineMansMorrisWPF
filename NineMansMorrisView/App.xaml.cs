using Microsoft.Win32;
using NineMansMorrisModel.Model;
using NineMansMorrisModel.Persistence;
using NineMansMorrisView.View;
using NineMansMorrisView.ViewModel;

using System.Windows;

namespace NineMansMorrisView
{
    public partial class App : Application
    {
        private MillModel model = null!;
        private MillViewModel viewModel = null!;
        private MainWindow view = null!;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            model = new MillModel(new MillFileManager());
            model.GameOver += Model_GameOver;
            model.NewGame();

            viewModel = new MillViewModel(model);
            viewModel.NewGameEvent += ViewModel_NewGame;
            viewModel.LoadGameEvent += ViewModel_LoadGame;
            viewModel.SaveGameEvent += ViewModel_SaveGame;
            viewModel.PassTurnEvent += ViewModel_PassTurn;
            viewModel.DeselectEvent += ViewModel_Deselect;

            view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();
        }

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            model.NewGame();
        }

        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                await model.LoadGameAsync(openFileDialog.FileName);
            }
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                await model.SaveGameAsync(saveFileDialog.FileName);
            }
        }

        private void ViewModel_PassTurn(object? sender, EventArgs e)
        {
            model.PassTurn();
        }

        private void ViewModel_Deselect(object? sender, EventArgs e)
        {
            model.Deselect();
        }

        private void Model_GameOver(object? sender, EventArgs e)
        {
            MessageBox.Show($"{model.Winner()} is the winner");
        }
    }
}