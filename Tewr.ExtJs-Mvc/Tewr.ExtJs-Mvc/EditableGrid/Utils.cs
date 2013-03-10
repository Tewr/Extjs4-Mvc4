using System.Globalization;
using System.Linq;

namespace Tewr.ExtJsMvc.EditableGrid
{
    public class Utils
    {
        private static string _phpStyleDateFormat;

        public static string PhpStyleDateFormat
        {
            get
            {
                // Todo: A Complete translation is needed here, this is very rudimentatry
                // basically go from G pattern (as it is used by NewtonSoft.Json.JsonConvert?) to format specified in extjs docs at
                // docs/#!/api/Ext.Date
                return _phpStyleDateFormat ??
                       (_phpStyleDateFormat =
                        CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns('G').First()
                            .Replace("HH", "X")
                            .Replace("H", "G")
                            .Replace("X", "H")
                            .Replace("mm", "i")
                            .Replace("ss", "s")
                            .Replace("M", "m")
                            .Replace("yyyy", "Y")
                            .Replace("yy", "y")
                            .Replace("dd", "d")
                            .Replace("mm", "m"));
            }
        } 
    }
}