using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Assignment.ViewModel
{
    public class BaseButton : Button
    {
        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public Int32 Number { get; set; }

        public BaseButton(Brush baseColor) : base()
        {
            this.Background = baseColor;
            //this.Dock = DockStyle.Fill;
            //this.FlatStyle = FlatStyle.Flat;
            this.Content = "";
            this.IsEnabled = false;
        }
    }
}
