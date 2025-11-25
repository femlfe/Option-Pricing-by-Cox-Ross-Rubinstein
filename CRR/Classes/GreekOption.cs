
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
