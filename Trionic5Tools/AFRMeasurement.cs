﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trionic5Tools
{
    public class AFRMeasurement
    {
        private double _afrValue = 0;

        public double AfrValue
        {
            get { return _afrValue; }
            set { _afrValue = value; }
        }
    }
}
