﻿using DotSpatial.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotSpatialMap.Models
{
    public interface ITopology:IModel
    {
        bool Validate();
    }
}
