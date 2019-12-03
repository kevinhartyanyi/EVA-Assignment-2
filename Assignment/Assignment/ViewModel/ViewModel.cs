using Assignment.Model;
using ELTE.Windows.Game.Persistence;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Assignment.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        #region Fields

        private GameControlModel _model; // modell
        private SaveEntry _selectedGame;
        private String _newName = String.Empty;

        //TableLayoutPanel table;
        //MenuStrip menu;
        int _mapSize;
        int shipNumber;

        Brush baseColor;
        Brush shipColor;
        Brush playerColor;
        Brush lightBombColor;
        Brush mediumBombColor;
        Brush heavyBombColor;

        Position player;
        List<Elem> ships;
        List<Elem> bombs;


        #endregion

        #region Properties

        public bool EndOfGame { get; private set; }

        public DelegateCommand LeftMove { get; private set; }
        public DelegateCommand RightMove { get; private set; }
        public DelegateCommand UpMove { get; private set; }
        public DelegateCommand DownMove { get; private set; }

        public DelegateCommand PauseGame { get; private set; }
        public DelegateCommand StartGame { get; private set; }


        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }

        /// <summary>
        /// Játék betöltés választó parancs lekérdezése.
        /// </summary>
        public DelegateCommand LoadGameOpenCommand { get; private set; }

        /// <summary>
        /// Játék betöltése parancs lekérdezése.
        /// </summary>
        public DelegateCommand LoadGameCloseCommand { get; private set; }

        /// <summary>
        /// Játék mentés választó parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameOpenCommand { get; private set; }

        /// <summary>
        /// Játék mentése parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameCloseCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Játékmező gyűjtemény lekérdezése.
        /// </summary>
        public ObservableCollection<Button> Fields { get; set; }

        /// <summary>
        /// Játékidő lekérdezése.
        /// </summary>
        public int GameTime { get { return _model.gameTime; } }

        
        /// <summary>
        /// Perzisztens játékállapot mentések lekérdezése.
        /// </summary>
        public ObservableCollection<SaveEntry> Games { get; set; }

        /// <summary>
        /// Kiválasztott játékállapot mentés lekérdezése.
        /// </summary>
        public SaveEntry SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                if (_selectedGame != null)
                    NewName = String.Copy(_selectedGame.Name);

                OnPropertyChanged();
                LoadGameCloseCommand.RaiseCanExecuteChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }
        

        /// <summary>
        /// Új játék mentés nevének lekérdezése.
        /// </summary>
        public String NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;

                OnPropertyChanged();
                SaveGameCloseCommand.RaiseCanExecuteChanged();
            }
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Új játék eseménye.
        /// </summary>
        public event EventHandler NewGame;

        /// <summary>
        /// Játék betöltés választásának eseménye.
        /// </summary>
        public event EventHandler LoadGameOpen;

        /// <summary>
        /// Játék betöltésének eseménye.
        /// </summary>
        public event EventHandler<String> LoadGameClose;

        /// <summary>
        /// Játék mentés választásának eseménye.
        /// </summary>
        public event EventHandler SaveGameOpen;

        /// <summary>
        /// Játék mentésének eseménye.
        /// </summary>
        public event EventHandler<String> SaveGameClose;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler ExitGame;

        #endregion

        //Helper function, returns index of Elem if exists
        int FindElem(List<Elem> cont, int id)
        {
            int re = -1;
            for (int i = 0; i < cont.Count; i++)
            {
                if (cont[i].ID == id)
                    re = i;
            }
            return re;
        }

        #region Constructors

        /// <summary>
        /// Game nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public GameViewModel(GameControlModel model)
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            // játék csatlakoztatása
            _model = model;
            _model.shipMove += new EventHandler<ShipMoveEvent>(OnShipMoveEvent);
            _model.bombMove += new EventHandler<BombMoveEvent>(OnBombMoveEvent);
            _model.gameOver += new EventHandler<GameOverEvent>(OnGameOverEvent);
            _model.loadGame += new EventHandler<LoadGameEvent>(OnLoadGameEvent);
            _model.bombRemove += new EventHandler<BombRemoveEvent>(OnBombRemoveEvent);
            _model.shipCreate += new EventHandler<ShipCreateEvent>(OnShipCreateEvent);
            _model.bombCreate += new EventHandler<BombCreateEvent>(OnBombCreateEvent);
            _model.playerMove += new EventHandler<PlayerMoveEvent>(OnPlayerMoveEvent);

            EndOfGame = false;

            player = new Position();
            ships = new List<Elem>();
            bombs = new List<Elem>();

            baseColor = Brushes.White;
            shipColor = Brushes.Red;
            playerColor = Brushes.Blue;
            lightBombColor = Brushes.Violet;
            mediumBombColor = Brushes.BlueViolet;
            heavyBombColor = Brushes.DarkViolet;
            _mapSize = 6;
            shipNumber = 2;

            LeftMove = new DelegateCommand((_) => {
                if (_model.isPlaying)
                    _model.PlayerMove(Move.Left);
            });
            RightMove = new DelegateCommand((_) =>
            {
                if (_model.isPlaying)
                    _model.PlayerMove(Move.Right);
            });
            UpMove = new DelegateCommand((_) => {
                if (_model.isPlaying)
                    _model.PlayerMove(Move.Up);
            });
            DownMove = new DelegateCommand((_) =>
            {
                if (_model.isPlaying)
                    _model.PlayerMove(Move.Down);
            });

            PauseGame = new DelegateCommand((_) => OnGameStopEvent());
            StartGame = new DelegateCommand((_) => OnGameStartEvent());





            // parancsok kezelése

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            
            LoadGameOpenCommand = new DelegateCommand(async param =>
            {
                //Games = new ObservableCollection<SaveEntry>(await _model.ListGamesAsync());
                OnLoadGameOpen();
            });
            LoadGameCloseCommand = new DelegateCommand(
                param => SelectedGame != null, // parancs végrehajthatóságának feltétele
                param => { OnLoadGameClose(SelectedGame.Name); });
            SaveGameOpenCommand = new DelegateCommand(async param =>
            {
                OnGameStopEvent();
                Games = new ObservableCollection<SaveEntry>(await _model.ListGamesAsync());
                OnSaveGameOpen();
            });
            SaveGameCloseCommand = new DelegateCommand(
                param => NewName.Length > 0, // parancs végrehajthatóságának feltétele
                param => { OnSaveGameClose(NewName); });
            
            ExitCommand = new DelegateCommand(param => OnExitGame());

            // játéktábla létrehozása
            Fields = new ObservableCollection<Button>();
            /*
            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    var baseElem = new BaseButton(baseColor);
                    Fields[i, j] = baseElem;
                    table.Controls.Add(baseElem, i, j);
                }
            }*/


            for (Int32 i = 0; i < _mapSize; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _mapSize; j++)
                {
                    Fields.Add(new Button());
                }
            }

            RefreshTable();
            _model.NewGame(_mapSize, _mapSize - 1, _mapSize - 1, shipNumber);
            player.SetPosition(_mapSize - 1, _mapSize - 1);
            Fields[player._y * _mapSize + player._x].Background = playerColor;
            //elements[player._x, player._y].BackColor = playerColor;

        }

        #endregion

        

        #region Private methods

        /// <summary>
        /// Tábla frissítése.
        /// </summary>
        private void RefreshTable()
        {
            foreach (Button field in Fields) // inicializálni kell a mezőket is
            {
                field.Background = baseColor;
            }

            OnPropertyChanged("GameTime");
        }

        #endregion

        #region Game event handlers


        

        /// <summary>
        /// Játék létrehozásának eseménykezelője.
        /// </summary>
        


        //Player Controll
        void OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (!_model.isPlaying)
                return;
            if (e.Key == Key.A)
            {
                _model.PlayerMove(Model.Move.Left);
            }
            else if (e.Key == Key.D)
            {
                _model.PlayerMove(Model.Move.Right);
            }
            else if (e.Key == Key.W)
            {
                _model.PlayerMove(Model.Move.Up);
            }
            else if (e.Key == Key.S)
            {
                _model.PlayerMove(Model.Move.Down);
            }
        }

        void OnLoadGameEvent(object sender, LoadGameEvent e)
        {
            RefreshTable();
            player.SetPosition(e.PlayerX, e.PlayerY);
            Fields[e.PlayerY * _mapSize + e.PlayerX].Background = playerColor;
        }

       

        void OnPlayerMoveEvent(object sender, PlayerMoveEvent e)
        {
            Fields[player._y * _mapSize + player._x].Background = baseColor;
            //elements[player._x, player._y].BackColor = baseColor;
            player._x = e.Position._x;
            player._y = e.Position._y;
            Fields[player._y * _mapSize + player._x].Background = playerColor;
            //elements[player._x, player._y].BackColor = playerColor;
        }

        void OnShipMoveEvent(object sender, ShipMoveEvent e)
        {
            int find = FindElem(ships, e.ID);
            var s = ships[find];
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Fields[s.Position._y * _mapSize + s.Position._x].Background = baseColor;
                //elements[s.Position._x, s.Position._y].BackColor = baseColor;
                s.Position = e.Position;
                Fields[s.Position._y * _mapSize + s.Position._x].Background = shipColor;

                //elements[s.Position._x, s.Position._y].BackColor = shipColor;
                
                OnPropertyChanged("GameTime");
            });

        }

        void OnShipCreateEvent(object sender, ShipCreateEvent e)
        {
            Elem elem = new Elem(e.Position, e.ID, Model.Type.Ship);
            ships.Add(elem);
            Fields[elem.Position._y * _mapSize + elem.Position._x].Background = shipColor;
            //elements[elem.Position._x, elem.Position._y].BackColor = shipColor;
        }

        void OnBombCreateEvent(object sender, BombCreateEvent e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Model.Type bombType = Model.Type.Nothing;
                Brush bombColor = Brushes.Black;
                switch (e.BombType)
                {
                    case Bomb_Type.Light:
                        bombType = Model.Type.LightBomb;
                        bombColor = lightBombColor;
                        break;
                    case Bomb_Type.Medium:
                        bombType = Model.Type.MediumBomb;
                        bombColor = mediumBombColor;
                        break;
                    case Bomb_Type.Heavy:
                        bombType = Model.Type.HeavyBomb;
                        bombColor = heavyBombColor;
                        break;
                }
                Elem elem = new Elem(e.Position, e.ID, bombType);
                bombs.Add(elem);
                Fields[elem.Position._y * _mapSize + elem.Position._x].Background = bombColor;
                //elements[elem.Position._x, elem.Position._y].BackColor = bombColor;
            });
        }

        void OnBombMoveEvent(object sender, BombMoveEvent e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (Fields == null)
                    return;
                int find = FindElem(bombs, e.ID);
                var b = bombs[find];
                if (Fields[b.Position._y * _mapSize + b.Position._x] == null)
                    return;
                Fields[b.Position._y * _mapSize + b.Position._x].Background = baseColor;
                //elements[b.Position._x, b.Position._y].BackColor = baseColor;
                b.Position = e.Position;
                Brush bombColor = Brushes.Black;
                switch (b.Type)
                {
                    case Model.Type.LightBomb:
                        bombColor = lightBombColor;
                        break;
                    case Model.Type.MediumBomb:
                        bombColor = mediumBombColor;
                        break;
                    case Model.Type.HeavyBomb:
                        bombColor = heavyBombColor;
                        break;

                }
                Fields[b.Position._y * _mapSize + b.Position._x].Background = bombColor;
                //elements[b.Position._x, b.Position._y].BackColor = bombColor;
            });

        }

        void OnGameOverEvent(object sender, GameOverEvent e)
        {
            Console.WriteLine("GameOver");
            EndOfGame = true;
            OnPropertyChanged("GameTime");
        }

        void OnBombRemoveEvent(object sender, BombRemoveEvent e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Console.WriteLine("Bomb Remove");
                int ind = FindElem(bombs, e.ID);
                Fields[bombs[ind].Position._y * _mapSize + bombs[ind].Position._x].Background = baseColor;
                //elements[bombs[ind].Position._x, bombs[ind].Position._y].BackColor = baseColor;
                bombs.RemoveAt(ind);
            });
        }

        void OnGameStopEvent()
        {
            if (_model.isPlaying && !EndOfGame)
            {
                Console.WriteLine("Game Stop");
                _model.StopGame();
            }
        }

        void OnGameStartEvent()
        {
            if (!_model.isPlaying && !EndOfGame)
            {
                Console.WriteLine("Game Start");
                _model.StartGame();
            }
            else
            {
                Console.WriteLine("Can't Start Game");
            }
        }

        void OnGameSaveEvent(object sender, EventArgs e)
        {
            if (_model.isPlaying)
            {
                Console.WriteLine("Stop Game");
                _model.StopGame();
            }
            Console.WriteLine("Game Save");
            SaveFileDialog sFile = new SaveFileDialog();
            sFile.Filter = "Save files (*.saveGame)|*.saveGame";
            sFile.ShowDialog();
            string fileName = sFile.FileName;
            Console.WriteLine(fileName);
            if (fileName != "")
                _model.SaveGame(fileName);
        }

        #endregion

        #region Event methods

        /// <summary>
        /// Új játék indításának eseménykiváltása.
        /// </summary>
        private void OnNewGame()
        {
            if (NewGame != null)
            {
                EndOfGame = false;
                RefreshTable();
                _model.NewGame(_mapSize, _mapSize - 1, _mapSize - 1, shipNumber);
                player.SetPosition(_mapSize - 1, _mapSize - 1);
                Fields[player._y * _mapSize + player._x].Background = playerColor;
                //elements[player._x, player._y].BackColor = playerColor;


                NewGame(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Játék betöltés választásának eseménykiváltása.
        /// </summary>
        private void OnLoadGameOpen()
        {
            if (LoadGameOpen != null)
            {
                OnGameStopEvent();
                LoadGameOpen(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Játék betöltésének eseménykiváltása.
        /// </summary>
        private void OnLoadGameClose(String name)
        {
            if (LoadGameClose != null)
            {
                Console.WriteLine("Load Game Close");
                LoadGameClose(this, name);
            }
        }

        public void LoadGame(string name)
        {
            _model.LoadGame(name);
            _model.StartGame();
            EndOfGame = false;
            //OnGameStartEvent();
        }

        /// <summary>
        /// Játék mentés választásának eseménykiváltása.
        /// </summary>
        private void OnSaveGameOpen()
        {
            if (SaveGameOpen != null)
            {
                OnGameStopEvent();
                SaveGameOpen(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Játék mentésének eseménykiváltása.
        /// </summary>
        private void OnSaveGameClose(String name)
        {
            if (SaveGameClose != null)
            {
                SaveGameClose(this, name);
            }
        }

        /// <summary>
        /// Játékból való kilépés eseménykiváltása.
        /// </summary>
        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }

        #endregion
    }
}
