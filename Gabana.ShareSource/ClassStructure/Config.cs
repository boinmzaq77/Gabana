using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.ClassStructure
{
    public class Config
    {

    }
    public class ParamSlip
    {
        public List<KeyValuePair<string, string>> Header { get; set; }
        public List<List<KeyValuePair<string, string>>> Footer { get; set; }
        public List<List<KeyValuePair<string, string>>> Details { get; set; }
    }
    public enum Parity
    {
        None = 0,
        Odd = 1,
        Even = 2,
        Mark = 3,
        Space = 4,
        NotSet = -1
    }
    public enum StopBits
    {
        One = 1,
        OnePointFive = 3,
        Two = 2,
        NotSet = -1
    }
}
