using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Data;

namespace E.ExploreDeezer.UWP.ValueConverters
{
    public class NumberFormatValueConverter : IValueConverter
    {
        private string currentLanguage;
        private NumberFormatInfo numberFormatInfo;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Cache culture info as generating this can be quite expensive
            if (language != currentLanguage)
            {
                this.currentLanguage = language;
                this.numberFormatInfo = CultureInfo.CreateSpecificCulture(language)
                                                   .NumberFormat;

                this.numberFormatInfo.NumberDecimalDigits = 0;
            }



            if (value is int intValue)
            {
                return intValue.ToString("N", this.numberFormatInfo);
            }
            else if (value is uint uintValue)
            {
                return uintValue.ToString("N", this.numberFormatInfo);
            }
            else
            {
                throw new Exception("Invalid or unsupport number type given.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
