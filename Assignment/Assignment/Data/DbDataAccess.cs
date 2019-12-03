using Assignment.Data;
using Assignment.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ELTE.Windows.Game.Persistence
{
    /// <summary>
    /// Game perzisztencia adatbáziskezelő típusa.
    /// </summary>
	public class GameDbDataAccess : IDataGame
	{
		private GameContext _context;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="connection">Adatbázis connection string.</param>
        public GameDbDataAccess(String connection)
		{
			_context = new GameContext(connection);
			_context.Database.CreateIfNotExists(); // adatbázis séma létrehozása, ha nem létezik
		}

        /// <summary>
        /// Játékállapot betöltése.
        /// </summary>
        /// <param name="name">Név vagy elérési útvonal.</param>
        /// <returns>A beolvasott játéktábla.</returns>
        public async Task<ModelValues> LoadAsync(String name)
		{
			Game game = await _context.Games
				.SingleAsync(g => g.Name == name); // játék állapot lekérdezése
            ModelValues table = new ModelValues(); // játéktábla modell létrehozása

            table.mapSize = game.TableSize;
				
            table.difficultyTime = game.DifficultyTime;
            table.gameTime = game.GameTime;

			return table;
			
		}


        /// <summary>
        /// Játékállapot mentése.
        /// </summary>
        /// <param name="name">Név vagy elérési útvonal.</param>
        /// <param name="table">A kiírandó játéktábla.</param>
        public async Task SaveAsync(String name, GameControlModel table)
		{
            Console.WriteLine("SAVE");
            // játékmentés keresése azonos névvel
            Game overwriteGame = await _context.Games
			    .SingleOrDefaultAsync(g => g.Name == name);
			if (overwriteGame != null)
			    _context.Games.Remove(overwriteGame); // törlés

			Game dbGame = new Game
			{
				TableSize = table.mapSize,
				Name = name
			}; // új mentés létrehozása
            dbGame.DifficultyTime = table.difficultyTime;
            dbGame.GameTime = table.gameTime;

            //dbGame.ShipCount = table.shipCount

            _context.Games.Add(dbGame); // mentés hozzáadása a perzisztálandó objektumokhoz
			await _context.SaveChangesAsync(); // mentés az adatbázisba
			
		}

	    /// <summary>
	    /// Játékállapot mentések lekérdezése.
	    /// </summary>
	    public async Task<ICollection<SaveEntry>> ListAsync()
	    {
            Console.WriteLine("SaveList");
	        return await _context.Games
	            .OrderByDescending(g => g.Time) // rendezés mentési idő szerint csökkenő sorrendben
	            .Select(g => new SaveEntry {Name = g.Name, Time = g.Time}) // leképezés: Game => SaveEntry
	            .ToListAsync();
	       
	    }
    }
}
