﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FillUpApp.Standart
{
    public class Bar
    {
        [Key]
        public string BarName { get; set; }
        public int RatingOfBar { get; set; }
    }
}
