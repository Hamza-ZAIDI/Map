﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotSpatialMap.Views
{
    //Base View  
    public interface IView
    {
        event EventHandler Initialize;
        event EventHandler Load;
    }
}
