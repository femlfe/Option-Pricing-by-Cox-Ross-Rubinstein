
namespace CRR_Model.Classes
{
    internal static class GreekOption
    {
        internal static double GetDelta(Option option, bool isCall, bool isEuropean) 
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(1, u, d, option.S);
            double S_up = stockPrices[0];
            double S_down = stockPrices[1];

            Option optionUp = new Option
            {
                S = S_up,
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma,
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            Option optionDown = new Option
            {
                S = S_down,
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma,
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            double priceUp = GetOptionPrice(optionUp, isCall, isEuropean);
            double priceDown = GetOptionPrice(optionDown, isCall, isEuropean);

            return (priceUp - priceDown) / (S_up - S_down);
        }


        internal static double GetGamma(Option option, bool isEuropean)
        {
            double upDown = option.S * 0.01;
            double S_up = option.S + upDown;
            double S_down = option.S - upDown;

            Option optionUp = new Option
            {
                S = S_up,  
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma,
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            Option optionDown = new Option
            {
                S = S_down, 
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma,
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            double priceUp = GetDelta(optionUp, true, isEuropean);
            double priceDown = GetDelta(optionDown, true, isEuropean);

            return (priceUp - priceDown) / (2* upDown);
        }

        internal static double GetVega(Option option, bool isEuropean)
        {
            double dSigma = 0.01; // Изменение волатильности на 1%

            // Опцион с увеличенной волатильностью
            Option optionVolUp = new Option
            {
                S = option.S,
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma + dSigma,  
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            // Опцион с уменьшенной волатильностью
            Option optionVolDown = new Option
            {
                S = option.S,
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma - dSigma,
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            double priceUp = GetOptionPrice(optionVolUp, true, isEuropean);
            double priceDown = GetOptionPrice(optionVolDown, true, isEuropean);

            return (priceUp - priceDown) / (2 * dSigma);
        }

        internal static double GetTheta(Option option, bool isCall, bool isEuropean)
        {
            double days = 1;
            Option optionTimeDown = new Option
            {
                S = option.S,
                K = option.K,
                Expiry = option.Expiry.AddDays(-days),
                Sigma = option.Sigma,
                R = option.R,
                Steps = option.Steps,
                Q = option.Q
            };

            double priceToday = GetOptionPrice(option, isCall, isEuropean);
            double priceTomorrow = GetOptionPrice(optionTimeDown, isCall, isEuropean);

            return (priceTomorrow - priceToday)*365;
        }

        internal static double GetRho(Option option, bool isCall, bool isEuropean)
        {
            double dR = 0.01; // Изменение ставки на 1% (0.01)

            // Опцион с увеличенной ставкой
            Option optionRateUp = new Option
            {
                S = option.S,
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma,
                R = option.R + dR,  // r + 1%
                Steps = option.Steps,
                Q = option.Q
            };

            // Опцион с уменьшенной ставкой
            Option optionRateDown = new Option
            {
                S = option.S,
                K = option.K,
                Expiry = option.Expiry,
                Sigma = option.Sigma,
                R = option.R - dR,  // r - 1%
                Steps = option.Steps,
                Q = option.Q
            };

            double priceUp = GetOptionPrice(optionRateUp, isCall, isEuropean);
            double priceDown = GetOptionPrice(optionRateDown, isCall, isEuropean);

            return (priceUp - priceDown) / (2 * dR);
        }

        private static double GetOptionPrice(Option option, bool isCall, bool isEuropean)
        {
            if (isCall && isEuropean)
                return CRR.EuropeanCallOption(option);
            if (isCall && !isEuropean)
                return CRR.AmericanCallOption(option);
            if (!isCall && isEuropean)
                return CRR.EuropeanPutOption(option);
            else
                return CRR.AmericanPutOption(option);
        }
    }
}
