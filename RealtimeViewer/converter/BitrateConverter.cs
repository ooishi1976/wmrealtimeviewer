using System;
using System.Globalization;

namespace RealtimeViewer.Converter
{
    public class BitrateConverter 
    {
        private BitrateList blist = new BitrateList();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var key = (string)value;
                if (blist.BitrateTextToId.ContainsKey(key))
                {
                    return blist.BitrateTextToId[key];
                }
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int index = (int)value;
                if (index >= 0 && index < blist.Bitrates.Length)
                {
                    return blist.Bitrates[index].Name;
                }
            }
            return String.Empty;
        }
    }
}
