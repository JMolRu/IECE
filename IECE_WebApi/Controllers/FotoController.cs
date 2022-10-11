﻿using System;
using System.IO;
using System.Linq;
using IECE_WebApi.Contexts;
using IECE_WebApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class FotoController : ControllerBase
    {
        private readonly AppDbContext context;

        public FotoController(AppDbContext context)
        {
            this.context = context;
        }

        //[HttpPost]
        //[EnableCors("AllowOrigin")]
        //public IActionResult Post([FromForm]IFormFile image)
        //{
        //    try
        //    {
        //        if (image != null)
        //        {
        //            // RECOLECTA CARACTERISTICAS DE LA IMAGEN
        //            Foto foto = new Foto
        //            {
        //                guid = Guid.NewGuid().ToString(),
        //                extension = Path.GetExtension(image.FileName),
        //                mimeType = image.ContentType,
        //                size = int.Parse(image.Length.ToString()),
        //                path = "c:\\inetpub\\wwwroot\\" // define donde guardar la imagen
        //            };

        //            // DEFINE EL NOMBRE DEL ARCHIVO PARA GUARDAR LA IMAGEN
        //            string ImageName = foto.guid + foto.extension;

        //            // GUARDAR IMAGEN EN DISCO
        //            //string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", ImageName);
        //            string SavePath = Path.Combine(foto.path + foto.guid + foto.extension);
        //            using (var stream = new FileStream(SavePath, FileMode.Create))
        //            {
        //                image.CopyTo(stream);
        //            }

        //            // AGREGA REGISTRO A LA BASE DE DATOS
        //            context.Foto.Add(foto);
        //            context.SaveChanges();

        //            return Ok(new
        //            {
        //                status = "success",
        //                foto = foto
        //            });
        //        }
        //        else
        //        {
        //            return Ok(new
        //            {
        //                status = "error",
        //                mensaje = "No se cargo niguna imagen"
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new
        //        {
        //            status = "error",
        //            mensaje = ex.Message
        //        });
        //    }
        //}

        [HttpGet("{idPersona}")]
        [EnableCors("AllowOrigin")]
        public IActionResult Get(int idPersona)
        {
            try
            {
                // CONSULTA DATOS DE LA PERSONA
                var p = context.Persona.FirstOrDefault(per => per.per_Id_Persona == idPersona);

                // CONSULTA IMAGEN DE LA FOTO
                var foto = context.Foto.FirstOrDefault(f => f.idFoto == p.idFoto);
                string path = Path.Combine($"{foto.path}{foto.guid}{foto.extension}");
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                return File(imageByteData, foto.mimeType);
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    status = "error",
                    mensaje = ex.Message
                });
            }
        }
    }
}