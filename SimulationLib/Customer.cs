using System;


namespace SimulationLib
{
    /// <summary>
    /// Покупатель
    /// </summary>
    public class Customer : MapObject
    {
        #region Свойства

        /// <summary>
        /// Мужчина (пол)
        /// </summary>
        public bool Man { set; get; } = false;

        #endregion Свойства



        #region Методы

        /// <summary>
        /// Покупатель
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Customer(float x, float y, float w, float h) : base(x, y, w, h)
        {
            Man = new Random().Next(0, 100) > 50;
        }

        /// <summary>
        /// Покупатель
        /// </summary>
        /// <param name="sex">Пол</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Customer(bool sex, float x, float y, float w, float h) : this(x, y, w, h)
        {
            Man = sex;
        }

        #endregion Методы
    }
}
