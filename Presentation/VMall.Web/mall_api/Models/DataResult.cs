﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.JWT.Models
{
    public class DataResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

    }
}