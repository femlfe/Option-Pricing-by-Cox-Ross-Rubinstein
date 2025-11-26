using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;


namespace CRR_Model.Classes
{
    /// <summary>
    /// Класс объекта опцион
    /// </summary>
    public class Option
    {
        public double S { get; set; }
        public double K { get; set; }
        public DateTime Expiry { get; set; }
        public double Sigma { get; set; }
        public double R { get; set; }
        public uint Steps { get; set; } = 100;
        public double Q { get; set; } = 0.0;

        /// <summary>
        /// Конструктор для определения параметров опциона
        /// </summary>
        /// <param name="s">Стоимость актива</param>
        /// <param name="k">Страйк опциона</param>
        /// <param name="expiry">Дата экспирации</param>
        /// <param name="sigma">Волатильность в процентах</param>
        /// <param name="r">Безрисковая ставка в процентах</param>
        /// <param name="q">Дивидендная доходность в процентах (необязательный параметр)</param>
        /// <param name="steps">Количество шагов построения биномиального дерева (необязательный параметр)</param>
        /// <exception cref="ArgumentException">Неверные параметры</exception>
        public Option(double s, double k, DateTime expiry, double sigma, double r, double q = 0, uint steps = 100)
        {
            if (s <= 0 || k <= 0 || steps == 0 || sigma <= 0 || expiry < DateTime.Now || q < 0 || r < 0) 
                throw new ArgumentException("Invalid parameters");
            S = s;
            K = k;
            Expiry = expiry;
            Sigma = sigma/100;
            R = r/100;
            Steps = steps;
            Q = q / 100;
        }

        //Метод для вычисления осноsвных параметров построения биномиального дерева
        internal static (double t, double u, double d, double p) CalculateParameters(Option option)
        {

            double sigma = option.Sigma;
            double r = option.R;
            double q = option.Q;

            double days = (option.Expiry - DateTime.Now).TotalDays;
            double t = days / 365 / option.Steps;

            double u = Math.Exp(sigma * Math.Sqrt(t));
            double d = 1 / u;

            double futureValueFactor = Math.Exp((r - q) * t);
            double p = (futureValueFactor - d) / (u - d);


            return (t, u, d, p);
        }

        //Вычисление последнего ряда значений опционов
        internal static double[] CalculateFinalPriceForOptions(double[] activesPrices, double K, bool isCall)
        {
            double[] optionHighPrices = new double[activesPrices.Length];

            int counter = 0;
            foreach (double s in activesPrices)
            {
                double optionPrice; ;

                if (isCall)
                    optionPrice = s - K;
                else
                    optionPrice = K - s;

                if (optionPrice > 0)
                {
                    optionHighPrices[counter++] = optionPrice;
                }
                else
                    optionHighPrices[counter++] = 0;
            }

            return optionHighPrices;
        }

        //Вычисление последнего ряда дерева значений цен актива
        internal static double[] GetStockPricesForStep(uint step, double u, double d, double S)
        {
            double[] prices = new double[step + 1];

            for (int i = 0; i <= step; i++)
            {
                prices[i] = S * Math.Pow(u, step - i) * Math.Pow(d, i);
            }

            return prices;
        }

        //Вычисление конечного значения европейского опциона любого типа
        internal static double GetFinalOptionEurorean(double[] optionPrices, double p, double r, double deltaT)
        {
            int n = optionPrices.Length;
            if (n == 1)
                return optionPrices[0];

            double[] newOptionPrices = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                newOptionPrices[i] = (p * optionPrices[i] + (1 - p) * optionPrices[i + 1]) * Math.Exp(-1 * r * deltaT);
            }

            return GetFinalOptionEurorean(newOptionPrices, p, r, deltaT);

        }

        //Вычисление конечного значения американского опциона любого типа
        internal static double GetFinalOptionAmerican(double[] optionPrices, double p, double r, double deltaT, double K, double u, double d, double[] currentStockPrices, bool isCall)
        {
            int n = optionPrices.Length;
            if (n == 1)
                return optionPrices[0];

            double[] newOptionPrices = new double[n - 1];
            double[] previousStockPrices = new double[n - 1];

            for (int i = 0; i < n - 1; i++)
            {
                //Возвращение к предыдущему шагу в дереве
                if (i == n - 2)
                    previousStockPrices[i] = currentStockPrices[i] / d;
                else
                    previousStockPrices[i] = currentStockPrices[i] / u;


                double waitValue = (p * optionPrices[i] + (1 - p) * optionPrices[i + 1]) * Math.Exp(-r * deltaT);

                //Проверка выгоды досрочного исполнения 
                double exerciseValue;
                if (isCall)
                    exerciseValue = Math.Max(0, previousStockPrices[i] - K);
                else
                    exerciseValue = Math.Max(0, K - previousStockPrices[i]);

                newOptionPrices[i] = Math.Max(waitValue, exerciseValue);
            }

            return GetFinalOptionAmerican(newOptionPrices, p, r, deltaT, K, u, d, previousStockPrices, isCall);
        }

    }
}
