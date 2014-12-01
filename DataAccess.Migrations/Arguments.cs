using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Migrations
{
    public class Arguments
    {
        public string DllPath { get; set; }
        public bool ExceptionOnStepError { get; set; }

        public Arguments(string[] args)
        {
            foreach(string s in args)
            {
                string[] parts = s.Replace("-", "").Split(new char[] { ':' }, 2);
                Console.WriteLine("Argument: {0} -> {1}", parts[0], parts[1]);


                switch(parts[0].ToUpper())
                {
                    case "PATH":
                        DllPath = parts[1];
                        break;
                    case "EXCEPTIONONERROR":
                        ExceptionOnStepError = Convert.ToBoolean(parts[1]);
                        break;
                }
            }
        }
    }
}
