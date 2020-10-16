﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IECEMembresia.Models;

namespace IECEMembresia.Pages.Personas
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public Personal Personal { get; set; }

        [BindProperty]
        public Hogar Hogar { get; set; }

        [BindProperty]
        public Domicilio Domicilio { get; set; }

        public void OnGet()
        {

        }
    }
}