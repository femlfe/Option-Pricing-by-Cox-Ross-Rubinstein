
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

        public static double Delta(Option option, bool isCall, bool isEuropian)
        {
            return GreekOption.GetDelta(option, isCall, isEuropian);
        }
    }
}
