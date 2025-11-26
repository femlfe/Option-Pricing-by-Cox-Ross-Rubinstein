
namespace CRR_Model.Classes
{
    internal static class GreekOption
    {
        //Получение дельты опциона
        internal static double GetDelta(Option option, bool isCall, bool isEuropean)
        {

            double h = option.S * 0.001;
            double S_up = option.S + h;
            double S_down = option.S - h;

            Option optionUp = new Option(S_up, option.K, option.Expiry, option.Sigma*100, option.R * 100, option.Q * 100, option.Steps);
            Option optionDown = new Option(S_down, option.K, option.Expiry, option.Sigma * 100, option.R * 100, option.Q * 100, option.Steps);

            double priceUp = GetOptionPrice(optionUp, isCall, isEuropean);
            double priceDown = GetOptionPrice(optionDown, isCall, isEuropean);

            return (priceUp - priceDown) / (2 * h);
        }

        //Получение гаммы опциона
        internal static double GetGamma(Option option,bool isCall, bool isEuropean)
        {
            //Изменение цены актива на 1%
            double upDown = option.S * 0.01;
            double S_up = option.S + upDown;
            double S_down = option.S - upDown;

            // Опцион с увеличенной ценой актива
            Option optionUp = new Option(S_up, option.K, option.Expiry, option.Sigma * 100, option.R * 100, option.Q * 100, option.Steps);


            // Опцион с уменьшенной волатильностью
            Option optionDown = new Option(S_down, option.K, option.Expiry, option.Sigma * 100, option.R * 100, option.Q * 100, option.Steps);


            double priceUp = GetDelta(optionUp, isCall, isEuropean);
            double priceDown = GetDelta(optionDown, isCall, isEuropean);

            return (priceUp - priceDown) / (2* upDown);
        }

        //Получение веги опциона
        internal static double GetVega(Option option, bool isEuropean)
        {
            // Изменение волатильности на 1%
            double dSigma = 0.01; 

            // Опцион с увеличенной волатильностью
            Option optionVolUp = new Option(option.S, option.K, option.Expiry, (option.Sigma + dSigma) * 100, option.R * 100, option.Q * 100, option.Steps);

            // Опцион с уменьшенной волатильностью
            Option optionVolDown = new Option(option.S, option.K, option.Expiry, (option.Sigma - dSigma) * 100, option.R * 100, option.Q * 100, option.Steps);

            double priceUp = GetOptionPrice(optionVolUp, true, isEuropean);
            double priceDown = GetOptionPrice(optionVolDown, true, isEuropean);

            return (priceUp - priceDown) / (2 * dSigma);
        }

        //Получение тетты опциона
        internal static double GetTheta(Option option, bool isCall, bool isEuropean)
        {
            //Величина изменения даты экспирации - 1 день
            double days = 1;

            //Опцион с измененной датой экспирации
            Option optionTimeDown = new Option(option.S, option.K, option.Expiry.AddDays(-days), option.Sigma * 100, option.R * 100, option.Q * 100, option.Steps);

            double priceToday = GetOptionPrice(option, isCall, isEuropean);
            double priceTomorrow = GetOptionPrice(optionTimeDown, isCall, isEuropean);

            //Годовое значение тетты
            return (priceTomorrow - priceToday)*365;
        }

        //Получение грека ро
        internal static double GetRho(Option option, bool isCall, bool isEuropean)
        {
            // Изменение ставки на 1%
            double dR = 0.01; 

            // Опцион с увеличенной ставкой
            Option optionRateUp = new Option(option.S, option.K, option.Expiry, option.Sigma * 100, (option.R + dR) * 100, option.Q * 100, option.Steps);

            // Опцион с уменьшенной ставкой
            Option optionRateDown = new Option(option.S, option.K, option.Expiry, option.Sigma * 100, (option.R - dR) * 100, option.Q * 100, option.Steps);

            double priceUp = GetOptionPrice(optionRateUp, isCall, isEuropean);
            double priceDown = GetOptionPrice(optionRateDown, isCall, isEuropean);

            return (priceUp - priceDown) / (2 * dR);
        }

        //Вспомогательный метод для получения цены опциона любого типа
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
