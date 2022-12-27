﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IECE_WebApi.Models
{
    public class PersonaDomicilio
    {
        [Key]
        public int id { get; set; }
        public virtual Persona PersonaEntity { get; set; }
        public virtual HogarDomicilio HogarDomicilioEntity {get; set; }
        public string nvaProfesionOficio1 { get; set; }
        public string nvaProfesionOficio2 { get; set; }
        public string nvoEstado { get; set; }
    }
}
