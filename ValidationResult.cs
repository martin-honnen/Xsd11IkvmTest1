using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xsd11IkvmTest1
{
    public class ValidationResult
    {
        public string Errors { get; set; }
        public bool Success { get; set; }
        public string Result { get; internal set; }
    }
}
