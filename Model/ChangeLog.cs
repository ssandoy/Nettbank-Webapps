using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ChangeLog
    {
        public int LogID { get; set; }
        public string EventType { get; set; }
        public string ChangedBy { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedTime { get; set; }

        public string toString()
        {
            var sb = new StringBuilder();
            sb.Append("Changed: " + ChangedTime);
            sb.Append(", Changed by: " + ChangedBy);
            sb.Append(", Original value: : " + OriginalValue);
            sb.Append(", New value: " + NewValue);
            sb.Append(", Event type: " + EventType);
            return sb.ToString();
        }

    }
}
