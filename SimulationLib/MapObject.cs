using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationLib
{
    /// <summary>
    /// Обьект карты
    /// </summary>
    public class MapObject
    {
        #region Свойства

        /// <summary>
        /// Ширина
        /// </summary>
        public float W { set; get; }

        /// <summary>
        /// Высота
        /// </summary>
        public float H { set; get; }

        /// <summary>
        /// Координата X
        /// </summary>
        public float X { set; get; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public float Y { set; get; }

        #endregion Свойства



        #region Методы

        /// <summary>
        /// Обьект карты
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="w">Ширина</param>
        /// <param name="h">Высота</param>
        public MapObject(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        #endregion Методы
    }
}
