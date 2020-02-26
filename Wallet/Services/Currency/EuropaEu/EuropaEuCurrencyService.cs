using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WalletApp.Services.Currency.EuropaEu
{
    public class EuropaEuCurrencyService : ICurrencyService
    {
        public EuropaEuCurrencyService(int cacheTimeoutInMin)
        {
            _cacheTimeoutInMin = cacheTimeoutInMin;
            _nextUpdateDate = DateTime.MinValue;
            _locker = new object();
        }

        const string SERVICE_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        const string LEAD_CURRENCY = "EUR";
        const decimal LEAD_CURRENCY_RATE = 1m;

        Dictionary<string, decimal> _ratesCache;
        int _cacheTimeoutInMin;
        DateTime _nextUpdateDate;
        object _locker;
        public IEnumerable<string> GetCurrencies()
        {
            RefreshRates();
            return _ratesCache.Keys;
        }

        private decimal GetRate(string code, Dictionary<string, decimal> rates)
        {
            if (_ratesCache.ContainsKey(code))
            {
                return _ratesCache[code];
            }
            throw new EuropaEuCurrencyServiceException($"Нет данных о курсе для {code}");
        }

        public decimal Convert(decimal inValue, string inCode, string outCode)
        {
            try
            {
                if (inCode == null) throw new ArgumentNullException(nameof(inCode));
                if (outCode == null) throw new ArgumentNullException(nameof(outCode));

                //Апаем и тримим коды курсов валют
                inCode = inCode.ToUpper().Trim();
                outCode = outCode.ToUpper().Trim();

                if (inValue <= 0) throw new EuropaEuCurrencyServiceException($"Конвертируемое значение не может быть отрецательным или раным 0");

                if (inCode == outCode) return inValue;

                //Обновляем курсы валют если период кэширования закончился
                RefreshRates();

                //Получаем ссылку на кэш валют для избежания ситуации, когда одна валюта будет из старой версии кэша, а другая из новой
                var ratesCache = _ratesCache;
                var inCurrencyRate = GetRate(inCode, ratesCache);
                var outCurrencyRate = GetRate(outCode, ratesCache);

                var ratio = outCurrencyRate / inCurrencyRate;
                var result = ratio * inValue / LEAD_CURRENCY_RATE;

                return result;
            }
            catch
            {
                throw new EuropaEuCurrencyServiceException($"Ошибка при конвертации валюты");
            }
        }

        private void RefreshRates()
        {
            if (_nextUpdateDate < DateTime.Now)
            {
                lock (_locker) //Лочим потоки для исключения двойного обновления
                {
                    if (_nextUpdateDate < DateTime.Now) //Двойная проверка для исключения повторного обновления обновления
                    {
                        try
                        {
                            var doc = XDocument.Load(SERVICE_URL);
                            var cubeXName = "{http://www.ecb.int/vocabulary/2002-08-01/eurofxref}Cube";
                            var cubes = doc.Root.Element(cubeXName).Element(cubeXName).Elements();
                            var gl = System.Globalization.CultureInfo.GetCultureInfo("usa");
                            //Обновляем курсы валют
                            var newRates = new Dictionary<string, decimal>();
                            newRates[LEAD_CURRENCY] = LEAD_CURRENCY_RATE;

                            foreach (var cube in cubes)
                            {
                                var currencyAttr = cube.Attribute("currency");
                                var rateAttr = cube.Attribute("rate");

                                newRates[currencyAttr.Value] = decimal.Parse(rateAttr.Value, gl);
                            }
                            _nextUpdateDate = DateTime.Now.AddMinutes(_cacheTimeoutInMin);
                            _ratesCache = newRates;
                        }
                        catch (Exception ex)
                        {
                            throw new EuropaEuCurrencyServiceException("Ошибка получения курса валют", ex);
                        }
                    }
                }
            }
        }

        public bool HasCurrencyRate(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            //Апаем и тримим коды курсов валют
            code = code.ToUpper().Trim();
            //Обновляем курсы валют если период кэширования закончился
            RefreshRates();
            return _ratesCache.ContainsKey(code);
        }
    }
}
