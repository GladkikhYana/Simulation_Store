using System;

namespace SimulationLib
{
    /// <summary>
    /// Служба доставки
    /// </summary>
    public class DeliveryService : MapObject
    {

        #region Свойства

        /// <summary>
        /// Склад (количество едениц продукции)
        /// </summary>
        public int Stock { private set; get; } = int.MaxValue;

        #endregion Свойства


        #region Методы

        /// <summary>
        /// Служба доставки
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public DeliveryService(float x, float y, float w, float h) : base(x, y, w, h)
        {
        }

        /// <summary>
        /// Доставить
        /// </summary>
        /// <returns></returns>
        public int DeliverRnd()
        {
            var rnd = new Random().Next(1, 100);

            Stock -= rnd;

            if (Stock <= 0) { return 0; }

            return rnd;
        }


        #endregion Методы

    }
}
