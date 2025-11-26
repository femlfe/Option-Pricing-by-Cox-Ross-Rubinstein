
namespace CRR_Model.Classes
{
    public static class CRR
    {
        /// <summary>
        /// Статическй метод для получения справедливой цены европейского call опциона с дивидендами и без
        /// </summary>
        /// <param name="option">Объект опциона</param>
        /// <returns>справедливая цена опциона</returns>
        public static double EuropeanCallOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, true);

            return Math.Round(Option.GetFinalOptionEurorean(highOptions, p, option.R, t),4);
        }

        /// <summary>
        /// Статическй метод для получения справедливой цены европейского put опциона с дивидендами и без
        /// </summary>
        /// <param name="option">Объект опциона</param>
        /// <returns>справедливая цена опциона</returns>
        public static double EuropeanPutOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, false);

            return Math.Round(Option.GetFinalOptionEurorean(highOptions, p, option.R, t),4);
        }

        /// <summary>
        /// Статическй метод для получения справедливой цены американского call опциона с дивидендами и без
        /// </summary>
        /// <param name="option">Объект опциона</param>
        /// <returns>справедливая цена опциона</returns>
        public static double AmericanCallOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, true);

            return Math.Round(Option.GetFinalOptionAmerican(highOptions, p, option.R, t, option.K, u, d, stockPrices, true),4);
        }

        /// <summary>
        /// Статическй метод для получения справедливой цены американского put опциона с дивидендами и без
        /// </summary>
        /// <param name="option">Объект опциона</param>
        /// <returns>справедливая цена опциона</returns>
        public static double AmericanPutOption(Option option)
        {
            var (t, u, d, p) = Option.CalculateParameters(option);

            double[] stockPrices = Option.GetStockPricesForStep(option.Steps, u, d, option.S);
            double[] highOptions = Option.CalculateFinalPriceForOptions(stockPrices, option.K, false);

            return Math.Round(Option.GetFinalOptionAmerican(highOptions, p, option.R, t, option.K, u, d, stockPrices, false),4);
        }

        /// <summary>
        /// Метод для получения всех греков для опциона любого типа
        /// </summary>
        /// <param name="option">Объект опциона</param>
        /// <param name="isCall">Тип опциона true - call, false - put</param>
        /// <param name="isEuropian">Вид опциона true - европейский, false - американский</param>
        /// <returns>5 double значений всех греков последовательно дельта, гамма, тетта, вега, ро</returns>
        public static (double delta, double gamma, double theta, double vega, double rho) GetGreeks(Option option,bool isCall, bool isEuropian)
        {
            double delta = Math.Round(GreekOption.GetDelta(option, isCall, isEuropian),4);
            double gamma = Math.Round(GreekOption.GetGamma(option, isCall, isEuropian),4);
            double theta = Math.Round(GreekOption.GetTheta(option, isCall, isEuropian),4);
            double vega = Math.Round(GreekOption.GetVega(option, isEuropian),4);
            double rho = Math.Round(GreekOption.GetRho(option, isCall, isEuropian),4);

            return (delta, gamma, theta, vega, rho);
        }

    }
}
