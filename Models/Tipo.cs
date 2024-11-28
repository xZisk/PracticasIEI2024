﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEIPracticas.Models
{
    public enum Tipo
    {
        [Description("Yacimiento Arqueologico")]
        Yacimiento,
        [Description("Iglesia-Ermita")]
        Iglesia,
        [Description("Monasterio-Convento")]
        Monasterio,
        [Description("Castillo-Fortaleza-Torre")]
        Castillo,
        [Description("Edificio singular")]
        Edificio,
        [Description("Puente")] 
        Puente,
        [Description("Otro")]
        Otro,
    }
}
