﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EDDNResponder
{
    class EDDNCommodity
    {
        public string name;
        public int meanPrice;
        public int buyPrice;
        public int stock;
        public dynamic stockBracket;
        public int sellPrice;
        public int demand;
        public dynamic demandBracket;
        public List<string> statusFlags;

        public bool ShouldSerializestatusFlags()
        {
            // Don't serialize status flags if they are empty as the schema requires that if present they contain at least 1 element
            return (statusFlags != null && statusFlags.Count > 0);
        }
    }
}
