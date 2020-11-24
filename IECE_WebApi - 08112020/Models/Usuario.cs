﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IECE_WebApi.Models
{
    public class Usuario
    {
        [Key]
        public int usu_Id_Usuario { get; set; }
        [Required]
        [Display(Name = "Nombre de usuario")]
        public string usu_Nombre { get; set; }
        [Required]
        [Display(Name = "Usuario")]
        public string usu_Usuario { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        public string usu_Password { get; set; }
        [Required]
        [Display(Name = "Perfil")]
        public int pef_Id_Perfil { get; set; }
        public bool sw_Registro { get; set; }
        public DateTime Fecha_Registro{ get; set; }
        public int usu_Id_Usuario_Registro { get; set; }
    }
}