using System;
using System.Collections.Generic;


namespace CRR_Model.Classes
{
    public class Option
    {
         public double S { get; set; }
         public double K { get; set; }
         public DateTime Expiry { get; set; }
         public double Sigma { get; set; }
         public double R { get; set; }
         public uint Steps { get; set; } = 100;
         public double Q { get; set; } = 0.0;

        internal static (double t, double u, double d, double p) CalculateParameters(Option option)
        {
            if (option.S <= 0 || option.K <= 0 || option.Steps == 0 || option.Sigma <= 0)
                throw new ArgumentException("Invalid parameters");

            double days = (option.Expiry - DateTime.Now).TotalDays;
            double t = days / 365 / option.Steps;

            double u = Math.Exp(option.Sigma * Math.Sqrt(t));
            double d = 1 / u;

            double futureValueFactor = Math.Exp((option.R - option.Q) * t);
            double p = (futureValueFactor - d) / (u - d);


            return (t, u, d, p);
        }


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

        internal static double[] GetStockPricesForStep(uint step, double u, double d, double S)
        {
            double[] prices = new double[step + 1];

            for (int i = 0; i <= step; i++)
            {
                prices[i] = S * Math.Pow(u, step - i) * Math.Pow(d, i);
            }

            return prices;
        }

        internal static double[] GetFinalOptionEurorean(double[] optionPrices, double p, double r, double deltaT)
        {
            int n = optionPrices.Length;
            if (n == 1)
                return optionPrices;

            double[] newOptionPrices = new double[n - 1];
            for (int i = 0; i < n - 1; i++)
            {
                newOptionPrices[i] = (p * optionPrices[i] + (1 - p) * optionPrices[i + 1]) * Math.Exp(-1 * r * deltaT);
            }

            return GetFinalOptionEurorean(newOptionPrices, p, r, deltaT);

        }

        internal static double[] GetFinalOptionAmerican(double[] optionPrices, double p, double r, double deltaT, double K, double u, double d, double[] currentStockPrices, bool isCall)
        {
            int n = optionPrices.Length;
            if (n == 1)
                return optionPrices;

            double[] newOptionPrices = new double[n - 1];
            double[] previousStockPrices = new double[n - 1];

            for (int i = 0; i < n - 1; i++)
            {
                if (i == n - 2)
                    previousStockPrices[i] = currentStockPrices[i] / d;
                else
                    previousStockPrices[i] = currentStockPrices[i] / u;


                double waitValue = (p * optionPrices[i] + (1 - p) * optionPrices[i + 1]) * Math.Exp(-r * deltaT);

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
