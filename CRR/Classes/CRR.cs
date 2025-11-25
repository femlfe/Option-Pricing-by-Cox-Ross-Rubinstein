
namespace CRR_Model.Classes
{
    public static class CRR
    {

       
        public static double EuropeanCallOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, true);

            return Option.GetFinalOptionEurorean(highOptions, p, option.R, t)[0];
        }

        public static double EuropeanPutOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, false);

            return Option.GetFinalOptionEurorean(highOptions, p, option.R, t)[0];
        }

        public static double AmericanCallOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, true);

            return Option.GetFinalOptionAmerican(highOptions, p, option.R, t, option.K, u, d, stockPrices, true)[0];
        }

        public static double AmericanPutOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, false);

            return Option.GetFinalOptionAmerican(highOptions, p, option.R, t, option.K, u, d, stockPrices, false)[0];
        }

        public static (double delta, double gamma, double theta, double vega, double rho) GetGreeks(Option option,bool isCall, bool isEuropian)
        {
            double delta = GreekOption.GetDelta(option, isCall, isEuropian);
            double gamma = GreekOption.GetGamma(option, isEuropian);
            double theta = GreekOption.GetTheta(option, isCall, isEuropian);
            double vega = GreekOption.GetVega(option, isEuropian);
            double rho = GreekOption.GetRho(option, isCall, isEuropian);

            return (delta, gamma, theta, vega, rho);
        }

    }
}
