using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.ClassStructure
{
    public class CboViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public static class FormatDecimal
    {
        static string f = "###,###,###.00";
        public static string Format
        {
            get { return f; }
            set
            {
                f = value;
                // OnPopertyChanged(nameof(AMOUNT)); // Notify that there was a change on this property
            }
        }
    }
}
