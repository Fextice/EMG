using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMG
{
  static class Program
  {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static double[] RemovePowerLineNoise(double[] data, int samplingRate)
        {
            int n = data.Length;

            // Размер окна фильтрации (в отсчетах)
            int windowSize1 = (int)(0.02 * samplingRate); // 20 мс
            int windowSize2 = (int)(0.2 * samplingRate); // 200 мс

            // Фильтрация с помощью двух скользящих средних фильтров
            double[] filteredData1 = MovingAverageFilter(data, windowSize1);
            double[] filteredData2 = MovingAverageFilter(filteredData1, windowSize2);

            // Вычитание отфильтрованных данных из исходных
            double[] result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = data[i] - filteredData2[i];
            }

            return result;
        }

// Функция для реализации скользящего среднего фильтра
        private static double[] MovingAverageFilter(double[] data, int windowSize)
        {
            int n = data.Length;
            double[] result = new double[n];

            // Применение фильтра к краям сигнала
            for (int i = 0; i < windowSize / 2; i++)
            {
                result[i] = data[i];
                result[n - i - 1] = data[n - i - 1];
            }

            // Применение фильтра к остальной части сигнала
            for (int i = windowSize / 2; i < n - windowSize / 2; i++)
            {
                double sum = 0;
                for (int j = i - windowSize / 2; j <= i + windowSize / 2; j++)
                {
                    sum += data[j];
                }
                result[i] = sum / windowSize;
            }

            return result;
        }

        
      

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}