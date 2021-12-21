using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace SimulationLib
{
    /// <summary>
    /// Модель данных приложения
    /// </summary>
    public class AppDataModel
    {
        #region Пременные
        /// <summary>
        /// Обслужено в минуту
        /// </summary>
        private int _ServedCustomersOnMin = 0;
        /// <summary>
        /// Всего обслужено клиентов за 10 сек
        /// </summary>
        private int _servedCustomersOnTenSec = 0;
        /// <summary>
        /// Скорость добавления в очередь за 10 сек
        /// </summary>
        private int _addingAueue = 0;
        /// <summary>
        /// Доставка
        /// </summary>
        private DeliveryService _deliveryService;
        /// <summary>
        /// Обьект синхронизации покупателей
        /// </summary>
        private readonly object SyncCustomers = new object();
        /// <summary>
        /// Обьект синхронизации торговых точек
        /// </summary>
        private readonly object SyncStores = new object();
        /// <summary>
        /// Обьект синхронизации доставки
        /// </summary>
        private readonly object SyncВeliveryService = new object();
        // Картинки
        private Image Rescm;
        private Image Rescw;
        private Image Rest;
        #endregion Пременные

        #region Свойства
        /// <summary>
        /// Скорость добавления в очередь за 10 сек
        /// </summary>
        public int AddingAueue { private set; get; } = 0;
        /// <summary>
        /// Всего обслужено клиентов за минуту
        /// </summary>
        public int ServedCustomersOnMin { private set; get; } = 0;
        /// <summary>
        /// Всего обслужено клиентов за 10 сек
        /// </summary>
        public int ServedCustomersOnTenSec { private set; get; } = 0;
        /// <summary>
        /// Всего обслужено клиентов
        /// </summary>
        public int ServedCustomers { private set; get; } = 0;
        /// <summary>
        /// Количество клиентов в очереди
        /// </summary>
        public int CustomersLine { private set; get; } = 0;
        /// <summary>
        /// Секунд с момента старта
        /// </summary>
        public int IntervalOnStart { private set; get; } = 0;
        /// <summary>
        /// Запрет отрисовки
        /// </summary>
        public bool StopDraw { set; get; } = false;
        /// <summary>
        /// Ширина карты
        /// </summary>
        private float _wMap { set; get; }
        /// <summary>
        /// Высота карты
        /// </summary>
        private float _hMap { set; get; }
        /// <summary>
        /// Торговые точки
        /// </summary>
        public List<Store> _stores { private set; get; } = new List<Store>();
        /// <summary>
        /// Покупатели
        /// </summary>
        public List<Customer> _customers { private set; get; } = new List<Customer>();
        #endregion Свойства

        #region Методы
        /// <summary>
        /// Модель проложения
        /// </summary>
        /// <param name="wMap"></param>
        /// <param name="hMap"></param>
        public AppDataModel(float wMap, float hMap)
        {
            Rescm = Bitmap.FromFile("cm.png");
            Rescw = Bitmap.FromFile("cw.png");
            Rest = Bitmap.FromFile("t.png");
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    IntervalOnStart++;
                    // 60 сек
                    if (IntervalOnStart % 60f == 0)
                    {
                        ServedCustomersOnMin = _ServedCustomersOnMin;
                        _ServedCustomersOnMin = 0;
                    }
                    // 10 сек
                    if (IntervalOnStart % 10f == 0)
                    {
                        ServedCustomersOnTenSec = _servedCustomersOnTenSec;
                        _servedCustomersOnTenSec = 0;
                        AddingAueue = _addingAueue;
                        _addingAueue = 0;
                    }
                }
            }
            )
            { IsBackground = true }.Start();
            _wMap = wMap;
            _hMap = hMap;
            _deliveryService = new DeliveryService(_wMap - 80, (Rescm.Size.Height * 2) - 15, 75, _hMap - 100);
            RandomAddCustomer();
            Process();
        }
        /// <summary>
        /// Кол-во клиентов в минуту
        /// </summary>
        /// <returns></returns>
        public float GetServedCustomerOnMinute()
        {
            var kv = ((float)IntervalOnStart / 60f) / ((float)ServedCustomers);
            return kv;
        }
        /// <summary>
        /// Основной метод распределения ресурсов
        /// </summary>
        private void Process()
        {
            var szImg = Rescm.Size;
            var rnd = new Random();
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    lock (SyncCustomers) { if (_customers.Count <= 0) { continue; } }
                    lock (SyncStores)
                    {
                        if (_stores.Count <= 0) { continue; }
                        foreach (var it in _stores)
                        {
                            if (!it.IsBusy)
                            {
                                new Thread(() =>
                                {
                                    if (it.Product > 0)
                                    {
                                        lock (SyncCustomers)
                                        {
                                            if (_customers.Count <= 0) { return; }
                                            _customers.RemoveAt(0);
                                            _ServedCustomersOnMin++;
                                            ServedCustomers++;
                                            _servedCustomersOnTenSec++;
                                        }
                                        for (int i = 0; i < _customers.Count; i++)
                                        {
                                            _customers[i].X -= szImg.Width + 4;
                                        }
                                    }
                                    it.SellOn();
                                })
                                { IsBackground = true }.Start();
                            }
                        }
                    }
                }
            })
            { IsBackground = true }.Start();
        }
        /// <summary>
        /// В отдельном потоке добавляет покупателей через рандомный интервал времени
        /// </summary>
        private void RandomAddCustomer()
        {
            var rnd = new Random(ProjVar.ADD_CUSTOMER_INTERVAL_DEF);
            var szImg = Rescm.Size;
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(rnd.Next(ProjVar.ADD_CUSTOMER_INTERVAL_DEF, ProjVar.ADD_CUSTOMER_INTERVAL_DEF * 4));
                    lock (SyncCustomers)
                    {
                        _addingAueue++;
                        _customers.Add(new Customer((_customers.Count * (szImg.Width + 4)) + 30, 30, szImg.Width, szImg.Height));
                    }
                }
            })
            { IsBackground = true }.Start();
        }
        /// <summary>
        /// Добавить новую точку
        /// </summary>
        public void AddStore()
        {
            var szImg = Rescm.Size;
            lock (SyncStores)
            {
                var t = new Store(11, (_stores.Count * 54) + (szImg.Width * 2) + 40, _wMap - 120, _hMap - (szImg.Width * 3) - 40);
                t.NeedPurchase += T_NeedPurchase;
                _stores.Add(t);
            }
            Resize(_wMap, _hMap);
        }
        /// <summary>
        /// Обрпаботчик для событий необходимости закупок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void T_NeedPurchase(object sender, EventArgs e)
        {
            for (int i = 0; i < _stores.Count; i++)
            {
                if (_stores[i] == sender)
                {
                    _stores[i].Buy(_deliveryService.DeliverRnd());
                }
            }
        }
        /// <summary>
        /// Удалить точку
        /// </summary>
        public void RemStore()
        {
            lock (SyncStores)
            {
                if (_stores.Count <= 0) { return; }
                var indexStore = _stores.Count - 1;
                _stores[indexStore].NeedPurchase += T_NeedPurchase;
                _stores.RemoveAt(indexStore);
            }
            Resize(_wMap, _hMap);
        }
        /// <summary>
        /// Изменение размеров основной формы
        /// </summary>
        /// <param name="wMap"></param>
        /// <param name="hMap"></param>
        public void Resize(float wMap, float hMap)
        {
            var szImg = Rescm.Size;
            lock (SyncStores)
            {
                _wMap = wMap;
                _hMap = hMap;
                foreach (var it in _stores)
                {
                    it.W = _wMap - 120;
                    it.H = 50;
                }
            }
            lock (SyncВeliveryService)
            {
                _deliveryService = new DeliveryService(_wMap - 80, (Rescm.Size.Height * 2) - 15, 75, _hMap - 100);
            }
        }
        /// <summary>
        /// Отрисовка
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            var brp = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
            var brpTxt = new SolidBrush(Color.FromArgb(120, 255, 0, 0));
            var brpI = new SolidBrush(Color.FromArgb(120, 200, 200, 0));
            var brpW = new SolidBrush(Color.FromArgb(120, 200, 200, 0));
            var fnt = new Font("", 32);
            var fntDs = new Font("", 9);
            var fntI = new Font("", 12);
            lock (SyncStores)
            {
                foreach (var it in _stores)
                {
                    g.FillRectangle(brp, it.X, it.Y, it.W, it.H);
                    g.DrawImage(Rest, it.X, it.Y, 50, 50);
                    g.DrawRectangle(Pens.Blue, it.X, it.Y, it.W, it.H);
                    g.DrawString(it.Product.ToString(), fnt, brpTxt, it.X + 50, it.Y);
                    brpTxt.Color = it.IsBusy ? Color.FromArgb(120, 255, 0, 0) : Color.FromArgb(120, 200, 200, 200);
                    g.DrawString(it.IsBusy ? "Обслуживает" : "Свободна", fntI, brpTxt, it.X + 150, it.Y + 4);
                    brpTxt.Color = Color.FromArgb(120, 255, 0, 0);
                    g.DrawString(it.IsPurchase ? "Закупка (ожидание доставки)" : "Товар в наличии", fntI, brpI, it.X + 150, it.Y + 24);
                }
            }
            lock (SyncCustomers)
            {
                foreach (var it in _customers)
                {
                    g.DrawImage(it.Man ? Rescm : Rescw, it.X, it.Y, it.W, it.H);
                }
            }
            lock (SyncВeliveryService)
            {
                g.FillRectangle(brp, _deliveryService.X, _deliveryService.Y, _deliveryService.W, _deliveryService.H);
                g.DrawRectangle(Pens.Yellow, _deliveryService.X, _deliveryService.Y, _deliveryService.W, _deliveryService.H);
                g.DrawString($"Доставка\n{_deliveryService.Stock}", fntDs, Brushes.WhiteSmoke, _deliveryService.X + 2, _deliveryService.Y);
            }
            var minToServ = ((float)_customers.Count / (float)ServedCustomersOnTenSec) / 6;
            var resStr = string.Empty;
            var resK = (float)AddingAueue / (float)ServedCustomersOnTenSec;
            if (AddingAueue < ServedCustomersOnTenSec)
            {
                resStr = "Очередь будет уменьшаться";
                resK = (float)ServedCustomersOnTenSec - (float)AddingAueue;
            }
            else if (AddingAueue > ServedCustomersOnTenSec)
            {
                resStr = "Очередь будет увиличиваться";
                resK = (float)AddingAueue - (float)ServedCustomersOnTenSec;
            }
            else
            {
                resStr = "Очередь не изменится";
                resK = 0;
            }
            var str = string.Format("В очереди (чел): {0}\nВсего обслужено(чел): {1}\nОбслужено в мин (чел/мин): {2}\nЕсли не приходили , то всех обслужат за (мин): {3:0.00}\n{4} (чел/мин): {5:0.00}",
                _customers.Count,
                ServedCustomers,
                ServedCustomersOnMin,
                minToServ,
                resStr,
                resK * 6);
            g.DrawString(str, fntDs, Brushes.WhiteSmoke, 10, _hMap - 90);
            // Чистим обьекты исползующие не управляемый ресурсы
            brp.Dispose();
            brpTxt.Dispose();
            fnt.Dispose();
            fntDs.Dispose();
            fntI.Dispose();
        }

        #endregion Методы
    }
}
