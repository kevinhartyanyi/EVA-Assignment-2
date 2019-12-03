using System;
using System.Data.Entity;

namespace ELTE.Windows.Game.Persistence
{
    /// <summary>
    /// Adatbázis kontextus típusa.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
	class GameContext : DbContext
	{
		public GameContext(String connection): base(connection)
		{

		}

        public GameContext(): base()
        { }

		public DbSet<Game> Games { get; set; }
		//public DbSet<Field> Fields { get; set; }
	}
}