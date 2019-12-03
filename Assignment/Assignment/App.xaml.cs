using Assignment.Data;
using Assignment.Model;
using Assignment.View;
using Assignment.ViewModel;
using ELTE.Windows.Game.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Assignment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Fields

        private GameControlModel _model;
        private GameViewModel _viewModel;
        private MainWindow _view;
        private LoadWindow _loadWindow;
        private SaveWindow _saveWindow;
        private DispatcherTimer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Alkalmazás példányosítása.
        /// </summary>
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            IData data;
            data = new Assignment.Data.Data();
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            // perzisztencia létrehozása




            IDataGame dataAccess;
            dataAccess = new GameDbDataAccess("name=GameModel"); // adatbázis alapú mentés


            // modell létrehozása
            _model = new GameControlModel(data, dataAccess);
            _model.gameOver += new EventHandler<GameOverEvent>(Model_GameOver);
            //_model.NewGame();

            // nézemodell létrehozása
            _viewModel = new GameViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGameOpen += new EventHandler(ViewModel_LoadGameOpen);
            _viewModel.LoadGameClose += new EventHandler<String>(ViewModel_LoadGameClose);
            _viewModel.SaveGameOpen += new EventHandler(ViewModel_SaveGameOpen);
            _viewModel.SaveGameClose += new EventHandler<String>(ViewModel_SaveGameClose);

            // nézet létrehozása
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();

            // időzítő létrehozása
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
        }

        #endregion

        #region View event handlers

        /// <summary>
        /// Nézet bezárásának eseménykezelője.
        /// </summary>
        private void View_Closing(object sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();
            }
        }

        #endregion

        #region ViewModel event handlers

        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private void ViewModel_NewGame(object sender, EventArgs e)
        {
            //_model.NewGame();
            _timer.Start();
        }

        /// <summary>
        /// Játék betöltés választó eseménykezelője.
        /// </summary>
        private void ViewModel_LoadGameOpen(object sender, System.EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            _viewModel.SelectedGame = null; // kezdetben nincsen kiválasztott elem

            _loadWindow = new LoadWindow(); // létrehozzuk a játék állapot betöltő ablakot
            _loadWindow.DataContext = _viewModel;
            _loadWindow.ShowDialog(); // megjelenítjük dialógusként

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
        }

        /// <summary>
        /// Játék betöltésének eseménykezelője.
        /// </summary>
        private async void ViewModel_LoadGameClose(object sender, String name)
        {
            if (name != null)
            {
                try
                {
                    //_model.LoadGame(name);
                    _viewModel.LoadGame(name);
                }
                catch
                {
                    MessageBox.Show("Játék betöltése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            _loadWindow.Close(); // játékállapot betöltőtő ablak bezárása
        }

        /// <summary>
        /// Játék mentés választó eseménykezelője.
        /// </summary>
        private void ViewModel_SaveGameOpen(object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            _viewModel.SelectedGame = null; // kezdetben nincsen kiválasztott elem
            _viewModel.NewName = String.Empty;

            _saveWindow = new SaveWindow(); // létrehozzuk a játék állapot mentő ablakot
            _saveWindow.DataContext = _viewModel;
            _saveWindow.ShowDialog(); // megjelenítjük dialógusként

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
        }

        /// <summary>
        /// Játék mentésének eseménykezelője.
        /// </summary>
        private async void ViewModel_SaveGameClose(object sender, String name)
        {
            if (name != null)
            {
                try
                {                  
                    _model.SaveGame(name);
                    Console.WriteLine("Sikeres Mentés");
                }
                catch
                {
                    MessageBox.Show("Játék mentése sikertelen!", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            _saveWindow.Close(); // játékállapot mentő ablak bezárása
        }

        /// <summary>
        /// Játékból való kilépés eseménykezelője.
        /// </summary>
        private void ViewModel_ExitGame(object sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object sender, GameOverEvent e)
        {
            _timer.Stop();

            MessageBox.Show("Game Over" + Environment.NewLine +
                            "Time: " +_model.gameTime,
                            "Game",
                            MessageBoxButton.OK,
                            MessageBoxImage.Asterisk);
            
        }

        #endregion
    }
}
