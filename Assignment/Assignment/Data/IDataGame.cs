using Assignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Data
{
    public interface IDataGame
    {

        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        Task<ModelValues> LoadAsync(String name);
        //ModelValues Load(String path);


        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="model">A fájlba kiírandó model.</param>
        Task SaveAsync(String name, GameControlModel table);
        //void Save(String path, GameControlModel model);
    }
}
