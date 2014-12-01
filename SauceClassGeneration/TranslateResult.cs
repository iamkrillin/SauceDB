using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCodeHaven.SauceClassGeneration
{
    public class TranslateResult
    {
        public TranslateResult()
        {
            Success = true;
            Type = "";
        }

        public bool Success { get; set; }
        public string Type { get; set; }
    }
}
