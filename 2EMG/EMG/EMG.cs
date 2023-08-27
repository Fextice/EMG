using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EMG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private double[] dataArray;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                label4.Text = "Файл: " + fileName;
                string[] lines = File.ReadAllLines(fileName);

                List<double> data = new List<double>();
                foreach (string line in lines)
                {
                    string[] tokens = line.Split(' ');

                    foreach (string token in tokens)
                    {
                        double value;
                        if (double.TryParse(token, out value))
                        {
                            data.Add(value);
                        }
                    }
                }

                dataArray = data.ToArray();
                panel1.Invalidate();
                // Дальнейшая обработка массива данных
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (dataArray == null || dataArray.Length == 0)
                return;

            Graphics g = e.Graphics;

            // Определяем размеры области рисования
            int margin = 20;
            int width = panel1.ClientSize.Width - 2 * margin;
            int height = panel1.ClientSize.Height - 2 * margin;

            // Определяем максимальное и минимальное значения данных
            double max = dataArray.Max();
            double min = dataArray.Min();

            // Отображаем оси координат
            g.DrawLine(Pens.Black, margin, margin, margin, margin + height);
            g.DrawLine(Pens.Black, margin, margin + height, margin + width, margin + height);

            // Отображаем данные
            Pen pen = new Pen(Color.Black);
            Point[] points = new Point[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                double x = margin + i * (double) width / (dataArray.Length - 1);
                double y = margin + (max - dataArray[i]) * height / (max - min);
                points[i] = new Point((int) x, (int) y);
            }

            g.DrawLines(pen, points);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (dataArray == null || dataArray.Length == 0)
                return;

            Graphics g = e.Graphics;

            // Определяем размеры области рисования
            int margin = 20;
            int width = panel1.ClientSize.Width - 2 * margin;
            int height = panel1.ClientSize.Height - 2 * margin;

            // Определяем максимальное и минимальное значения данных
            double max = dataArray.Max();
            double min = dataArray.Min();

            // Фильтруем данные методом двойных методов
            int samplingRate;
            try
            {
                samplingRate = Int32.Parse(textBox1.Text);

                if (samplingRate < 50)
                {
                    MessageBox.Show("Значение частоты дискретизации должно быть не менее 50 Гц");
                    return;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Некорректное значение частоты дискретизации");
                return;
            }
            double[] filteredData = Program.RemovePowerLineNoise(dataArray, samplingRate);

            // Отображаем оси координат
            g.DrawLine(Pens.Black, margin, margin, margin, margin + height);
            g.DrawLine(Pens.Black, margin, margin + height, margin + width, margin + height);

            // Отображаем данные
            Pen pen = new Pen(Color.Red, 1f);
            Point[] points = new Point[filteredData.Length];
            for (int i = 0; i < filteredData.Length; i++)
            {
                double x = margin + i * (double) width / (filteredData.Length - 1);
                double y = margin + (max - filteredData[i]) * height / (max - min);
                points[i] = new Point((int) x, (int) y);
            }

            g.DrawLines(pen, points);
        }
        
        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                panel2.Invalidate();
                e.Handled = true; // Отменяем обработку клавиши Enter (чтобы избежать звука)
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}