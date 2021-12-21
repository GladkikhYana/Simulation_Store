using System;
using System.Threading;

namespace SimulationLib
{
    /// <summary>
    /// Торговая точка
    /// </summary>
    public class Store : MapObject
    {

        #region Свойства

        /// <summary>
        /// Закупка
        /// </summary>
        public bool IsPurchase { private set; get; } = false;

        /// <summary>
        /// Занято
        /// </summary>
        public bool IsBusy { private set; get; } = false;

        /// <summary>
        /// Товар
        /// </summary>
        public int Product { private set; get; }

        #endregion Свойства



        #region

        /// <summary>
        /// Необходимо закупить товар
        /// </summary>
        public event EventHandler NeedPurchase = (s, e) => { };

        #endregion



        #region Методы

        /// <summary>
        /// Торговая точка
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Store(float x, float y, float w, float h) : base(x, y, w, h)
        { 
        }

        /// <summary>
        /// Проверяет наличие товара, если он отсутствует - гененрирует событие 
        /// </summary>
        private void ChekProduct()
        {
            if (Product > 0) { return; }

            NeedPurchase(this, EventArgs.Empty);
        }

        /// <summary>
        /// Продать еденицу товара
        /// </summary>
        /// <returns></returns>
        public int SellOn()
        {
            IsBusy = true;
            Thread.Sleep(2000);
            ChekProduct();

            if (Product <= 0) { IsBusy = false; return 0; }

            Product--;

            IsBusy = false;

            return 1;
        }

        /// <summary>
        /// Пробать весь товар
        /// </summary>
        /// <returns></returns>
        public int SellAl()
        {
            IsBusy = true;
            Thread.Sleep(2000);
            ChekProduct();

            if (Product <= 0) { IsBusy = false; return 0; }

            var t = Product;
            Product = 0;
            IsBusy = false;
            return t;
        }

        /// <summary>
        /// Продать указанное число товара
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public int SellOn(int cnt)
        {
            IsBusy = true;
            Thread.Sleep(2000);
            ChekProduct();

            if (Product <= 0) { IsBusy = false; return 0; }
            if (Product < cnt) { IsBusy = false; return 0; }

            Product -=cnt;
            IsBusy = false;
            return Product;
        }

        /// <summary>
        /// Закупить указанное кол-во товара
        /// </summary>
        /// <param name="cnt"></param>
        public int Buy(int cnt)
        {
            IsBusy = true;
            IsPurchase = true;
            Thread.Sleep(5000);

            if (cnt < 0) { IsBusy = false; return Product; }

            Product += cnt;

            IsPurchase = false;
            IsBusy = false;

            return Product;
        }

        #endregion Методы
    }
}
