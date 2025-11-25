namespace CRR_Model.Classes
{
    public static class CRR
    {

       
        public static double EuropeanCallOption(double S, double K, DateTime T, double σ, double r, uint n = 100, double q = 0.0)
        {
            var (t, u, d, p) = CalculateParameters(S, K, T, σ, r, n, q);

            double[] stockPrices = GetStockPricesForStep(n, u, d, S);
            double[] highOptions = CalculateFinalPriceForOptions(stockPrices, K, true);

            return GetFinalOptionEurorean(highOptions, p, r, t)[0];
        }

        public static double EuropeanPutOption(double S, double K, DateTime T, double σ, double r, uint n = 100, double q = 0.0)
        {
            var (t, u, d, p) = CalculateParameters(S, K, T, σ, r, n, q);

            double[] stockPrices = GetStockPricesForStep(n, u, d, S);
            double[] highOptions = CalculateFinalPriceForOptions(stockPrices, K, false);

            return GetFinalOptionEurorean(highOptions, p, r, t)[0];
        }

        public static double AmericanCallOption(double S, double K, DateTime T, double σ, double r, uint n = 100, double q = 0.0)
        {
            var (t, u, d, p) = CalculateParameters(S, K, T, σ, r, n, q);

            double[] stockPrices = GetStockPricesForStep(n, u, d, S);
            double[] highOptions = CalculateFinalPriceForOptions(stockPrices, K, true);

            return GetFinalOptionAmerican(highOptions, p, r, t, K, u, d, S, true)[0];
        }

        public static double AmericanPutOption(double S, double K, DateTime T, double σ, double r, uint n = 100, double q = 0.0)
        {
            var (t, u, d, p) = CalculateParameters(S, K, T, σ, r, n, q);

            double[] stockPrices = GetStockPricesForStep(n, u, d, S);
            double[] highOptions = CalculateFinalPriceForOptions(stockPrices, K, false);

            return GetFinalOptionAmerican(highOptions, p, r, t, K, u, d, S, false)[0];
        }

        private static (double t, double u, double d, double p) CalculateParameters(double S, double K, DateTime T, double σ, double r, uint n = 100, double q = 0.0)
        {
            //validation
            if (S <= 0 || K <= 0 || n == 0 || σ <= 0)
                throw new ArgumentException("Invalid parameters");

            //calculating the expiration time expressed in years and finding the step length
            double days = (T - DateTime.Now).TotalDays;
            double t = days / 365 / n;

            //calculating the growth and fall coefficient
            double u = Math.Exp(σ * Math.Sqrt(t));
            double d = 1 / u;

            //calculating the risk-neutral probability of a stock rising and falling
            double futureValueFactor = Math.Exp((r - q) * t);
            double p = (futureValueFactor - d) / (u - d);


            return (t,u,d,p);
        }
       

        //верхние значения ОПЦИОНОВ
        private static double[] CalculateFinalPriceForOptions(double[] activesPrices, double K, bool isCall)
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

       //Верхние значения АКТИВОВ
        private static double[] GetStockPricesForStep(uint step, double u, double d, double S)
        {
            double[] prices = new double[step + 1];

            for (int i = 0; i <= step; i++)
            {
                prices[i] = S * Math.Pow(u, step - i) * Math.Pow(d, i);
            }

            return prices;
        }

        private static double[] GetFinalOptionEurorean(double[] optionPrices, double p, double r, double deltaT)
        {
            int n = optionPrices.Length;
            if (n == 1) 
                return optionPrices;

            double[] newOptionPrices = new double[n - 1];
            for (int i = 0; i < n-1; i++)
            {
                newOptionPrices[i] = (p * optionPrices[i] + (1 - p) * optionPrices[i + 1]) * Math.Exp(-1 * r * deltaT);
            }
                
            return GetFinalOptionEurorean(newOptionPrices, p, r, deltaT);
            
        }

        private static double[] GetFinalOptionAmerican(double[] optionPrices, double p, double r, double deltaT, double K, double u, double d, double S, bool isCall)
        {
            int n = optionPrices.Length;
            if (n == 1)
                return optionPrices;

            double[] newOptionPrices = new double[n - 1];

            double[] previousStockPrices = GetStockPricesForStep((uint)(n - 1), u, d, S);

            for (int i = 0; i < n - 1; i++)
            {
                double waitValue = (p * optionPrices[i] + (1 - p) * optionPrices[i + 1]) * Math.Exp(-r * deltaT);

                double exerciseValue;
                if (isCall)
                     exerciseValue = previousStockPrices[i] - K;
                else
                    exerciseValue = K - previousStockPrices[i];

                newOptionPrices[i] = Math.Max(waitValue, exerciseValue);
            }

            return GetFinalOptionAmerican(newOptionPrices, p, r, deltaT, K, u, d, S, isCall);
        }
    }
}
