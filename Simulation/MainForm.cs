using System;
using System.Threading;
using System.Windows.Forms;
using SimulationLib;


namespace Simulation
{
    /// <summary>
    /// Основная форма приложения
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Модель дпанных приложения
        /// </summary>
        private AppDataModel appDataModel;

        /// <summary>
        /// Основная форма приложения
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Форма загружена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // передадим размеры пространства на котором можно производить отрисовку 
            appDataModel = new AppDataModel(this.ClientSize.Width, this.ClientSize.Height);

            // поток для отрисовок
            new Thread(() => { while (true) { Thread.Sleep(ProjVar.UP_DATE_INTERVAL); Invalidate(); } }) { IsBackground = true }.Start();
        }

        /// <summary>
        /// Начало изменения размеров формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            appDataModel.StopDraw = true;
        }

        /// <summary>
        /// Окончание изменений размеров формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            appDataModel.Resize(this.ClientSize.Width, this.ClientSize.Height);
            appDataModel.StopDraw = false;
        }

        /// <summary>
        /// Обработка отрисовок
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            appDataModel.Draw(e.Graphics);
        }

        /// <summary>
        /// Клик по добавит точку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            appDataModel.AddStore();
        }

        /// <summary>
        /// Клик по удалить точку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox2_Click(object sender, EventArgs e)
        {
            appDataModel.RemStore();
        }

        /// <summary>
        /// Клик по выход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
