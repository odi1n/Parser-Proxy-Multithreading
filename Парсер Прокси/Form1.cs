using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using xNet;

namespace Парсер_Прокси
{
    public partial class Form1 : Form
    {
        string[] Mass = { };
        Thread[] threads;

        public Form1()
        {
            InitializeComponent();

            pLock = new object();
            Masss = new List<string>();
        }

        private readonly object pLock;
        private int pIndexPhotoList;
        private readonly List<string> Masss;



        private void bunifuTrackbar1_ValueChanged(object sender, EventArgs e)
        {
            bunifuCustomLabel6.Text = bunifuTrackbar1.Value.ToString();
        }//сколько потоков

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            Masss.Clear();

            OpenFileDialog OpFile = new OpenFileDialog();
            if (OpFile.ShowDialog() == DialogResult.OK)
            {
                Mass = System.IO.File.ReadAllLines(OpFile.FileName);
            }
            bunifuCustomLabel2.Text = Mass.Count().ToString();
            bunifuCircleProgressbar1.MaxValue = Mass.Count();
            for (int i = 0; i < Mass.Count(); i++)
                Masss.Add(Mass[i]);
        }//открыть файл




        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            bunifuCircleProgressbar1.Value = 0;


            if (Masss.Count <= 0)
            {
                MessageBox.Show("Не инициализирована коллекция.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Обнуляем глобальный индекс
            pIndexPhotoList = 0;
            // Получаем кол-во запускаемых потоков
            int countThread = Convert.ToInt32(bunifuTrackbar1.Value);
            // Создаём массив потоков
            threads = new Thread[countThread];
            // Циклом создаём потоки
            for (int i = 0; i < countThread; i++)
            {
                threads[i] = new Thread(new ThreadStart(DoThread));
                threads[i].Name = "Поток № " + i;
                threads[i].Start();
            }
        }

        private void DoThread()
        {
            Random rnd = new Random();

            // Получаем ID и Имя потока
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string threadName = Thread.CurrentThread.Name;

            string photo = string.Empty;

            // Создаём бесконечный цикл
            while (true)
            {
                photo = string.Empty;
                // Открываем объект синхронизации
                lock (pLock)
                { // Код в этих скобках будет обрабатывать только один поток, остальные потоки будут ждать, пока текущий поток обработает этот код
                    // Проверяем возможность взять фотографию из коллекции (Если индекс фото >= общего кол-ва фото, значит завершаем поток)
                    if (pIndexPhotoList >= Masss.Count)
                    {
                        break; // Завершаем цикл потока
                    }
                    // Берём фотографию из списка
                    photo = Masss[pIndexPhotoList];
                    // Увеличиваем индекс
                    pIndexPhotoList++;
                }

                try
                {
                    var danni = new HttpRequest();
                    string response = danni.Get(photo).ToString();
                    Regular_Virag(response);
                }
                catch { }
                this.Invoke((MethodInvoker)delegate() { bunifuCircleProgressbar1.Value++; });

                // Псевдо задержка
                Thread.Sleep(rnd.Next(100, 500));
            }
        }





        void Poluchit_proxy()
        {
            for (int i = 0; i < Mass.Count(); i++)
            {
                var danni = new HttpRequest();
                string response = danni.Get(Mass[i]).ToString();
                Regular_Virag(response);

                this.Invoke((MethodInvoker)delegate() { bunifuCircleProgressbar1.Value++; });
            }
        }/*Получить прокси*/

        void Regular_Virag(string response)
        {
            Regex regex = new Regex(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}");
            MatchCollection matches = regex.Matches(response);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                    this.Invoke((MethodInvoker)delegate() { System.IO.File.AppendAllText("Proxy.txt", match.Value + "\n");  });
            }
        }




        //Дизайн
        private void bunifuCustomLabel7_MouseDown(object sender, MouseEventArgs e)
        {
            bunifuCustomLabel7.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

        private void bunifuCircleProgressbar1_MouseDown(object sender, MouseEventArgs e)
        {
            bunifuCircleProgressbar1.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
        }

       
    }
}
