using Assignment.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace ELTE.Windows.Game.Persistence
{
    /// <summary>
    /// Mező entitás típusa.
    /// </summary>
    class Field
	{
        /// <summary>
        /// Egyedi azonosító.
        /// </summary>
        [Key]
		public Int32 Id { get; set; }

	    /// <summary>
	    /// Vízszintes koordináta.
	    /// </summary>
		public Int32 X { get; set; }
	    /// <summary>
	    /// Függőleges koordináta.
	    /// </summary>
		public Int32 Y { get; set; }
	    /// <summary>
	    /// Tárolt érték.
	    /// </summary>
		public Int32 Value { get; set; }

        public Int32 ShipCount { get; set; }

        public Bomb_Type bType { get; set; }

        /// <summary>
        /// Kapcsolt játék.
        /// </summary>
        public Game Game { get; set; }
	}
}