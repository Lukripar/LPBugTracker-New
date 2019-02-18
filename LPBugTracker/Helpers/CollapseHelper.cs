using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LPBugTracker.Helpers
{
    public class CollapseHelper
    {
        public static string uncollapseFirst(int count)
        {
            var result = "";
            if (count != 1)
            {
                result = "collapsed";
            }
            return result;
        }
    }
}