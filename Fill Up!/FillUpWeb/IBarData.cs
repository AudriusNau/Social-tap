﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FillUpWeb
{
    public interface IBarData
    {
        double RateAvg { get; }
        bool Comparison { get; }
    }
}