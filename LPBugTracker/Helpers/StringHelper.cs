using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class StringHelper
    {

        public static string TruncString(string myStr, int THRESHOLD)
        {
            if (myStr.Length > THRESHOLD)
                return myStr.Substring(0, THRESHOLD) + "...";
            return myStr;
        }
    }
}