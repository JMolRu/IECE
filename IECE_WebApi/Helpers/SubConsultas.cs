﻿using IECE_WebApi.Contexts;
using IECE_WebApi.Controllers;
using IECE_WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static IECE_WebApi.Controllers.Registro_TransaccionesController;

namespace IECE_WebApi.Helpers
{
    public class SubConsultas
    {
        private readonly AppDbContext context;
        public SubConsultas(AppDbContext context)
        {
            this.context = context;
        }
        public class movimientosEstadisticosReporteBySector
        {
            public int personasBautizadas { get; set; }
            public int personasNoBautizadas { get; set; }
            public int personasBautizadasAlFinalDelMes { get; set; }
            public int personasNoBautizadasAlFinalDelMes { get; set; }
            public int hogares { get; set; }
            public int hogaresAlFinalDelMes { get; set; }
            public int matrimonios { get; set; }
            public int legalizaciones { get; set; }
            public int presentaciones { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.altas.bautizados altasBautizados { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.altas.noBautizados altasNoBautizados { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.bajas.bautizados bajasBautizados { get; set; }
            public virtual Registro_TransaccionesController.HistTransEstBySectorMes.bajas.noBautizados bajasNoBautizados { get; set; }
        }

        public class HistorialPorFechaSector
        {
            public int hte_Id_Transaccion { get; set; }
            public int ct_Codigo_Transaccion { get; set; }
            public string ct_Grupo { get; set; }
            public string ct_Tipo { get; set; }
            public string ct_Subtipo { get; set; }
            public string per_Nombre { get; set; }
            public string per_Apellido_Paterno { get; set; }
            public string per_Apellido_Materno { get; set; }
            public string per_Apellido_Casada { get; set; }
            public string apellidoPrincipal { get; set; }
            public bool per_Bautizado { get; set; }
            public string per_Categoria { get; set; }
            public string hte_Comentario { get; set; }
            public DateTime? hte_Fecha_Transaccion { get; set; }
            public string dis_Distrito_Alias { get; set; }
            public string sec_Sector_Alia { get; set; }
        }

        public movimientosEstadisticosReporteBySector SubMovimientosEstadisticosReporteBySector(FiltroHistTransEstDelMes fhte)
        {
            // preparacion del mes del cual se solicita el reporte
            var mesSiguienteDelResporte = new DateTime(fhte.year, fhte.mes + 1, 1);
            DateTime mesActualDelReporte = mesSiguienteDelResporte.AddDays(-1);

            // personas del sector activas y vivas hasta el mes de consulta
            var personas = (from p in context.Persona
                            where p.sec_Id_Sector == fhte.sec_Id_Sector
                            && p.per_Vivo == true
                            && p.per_Activo == true
                            select p).ToList();

            // PERSONAS BAUTIZAS Y EN COMUNION HASTA EL MES DE CONSULTA
            var pb = personas.Where(
                p => p.per_Bautizado == true
                && p.per_En_Comunion == true
                && p.per_Fecha_Bautismo <= mesActualDelReporte).ToList();

            int personasBautizadas = pb.Count;

            // hombres bautizados hasta el mes de consulta
            var hb = pb.Where(p => p.per_Categoria == "ADULTO_HOMBRE").ToList();

            // mujeres bautizadas hasta el mes de consulta
            var mb = pb.Where(p => p.per_Categoria == "ADULTO_MUJER").ToList();

            // jovenes hombres bautizados hasta el mes de consulta
            var jhb = pb.Where(p => p.per_Categoria == "JOVEN_HOMBRE").ToList();

            // jovenes mujeres bautizadas hasta el mes de consulta
            var jmb = pb.Where(p => p.per_Categoria == "JOVEN_MUJER").ToList();

            // PERSONAS NO BAUTIZAS HASTA EL MES DE CONSULTA
            var pnb = personas.Where(
                p => p.per_Bautizado == false
                && p.per_En_Comunion == false
                && p.per_Fecha_Nacimiento <= mesActualDelReporte).ToList();

            int personasNoBautizadas = pnb.Count;

            // jovenes hombres no bautizados hasta el mes de consulta
            var jhnb = pnb.Where(p => p.per_Categoria == "JOVEN_HOMBRE").ToList();

            // jovenes mujeres no bautizadas hasta el mes de consulta
            var jmnb = pnb.Where(p => p.per_Categoria == "JOVEN_MUJER").ToList();

            // niños
            var ninos = pnb.Where(p => p.per_Categoria == "NIÑO").ToList();

            // niñas
            var ninas = pnb.Where(p => p.per_Categoria == "NIÑA").ToList();

            // HISTORIAL DE TRANSACCIONES ESTADISTICAS
            // historial transacciones estadisticas del sector y mes de consulta
            var hteDelMesConsultado = (from hte in context.Historial_Transacciones_Estadisticas
                                       where (hte.hte_Fecha_Transaccion >= new DateTime(fhte.year, fhte.mes, 1)
                                       && hte.hte_Fecha_Transaccion <= mesActualDelReporte)
                                       && hte.sec_Sector_Id == fhte.sec_Id_Sector
                                       select hte).ToList();

            // posible duplisidad para futuras altas

            // altas bautizados del mes
            int[] codAlta = { 11001, 11002, 11003, 11004, 11005, 12001, 12002, 12003, 12004 };
            int movAltaBautizado = 0;
            foreach (var ca in codAlta)
            {
                var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                if (a.Count() > 0)
                {
                    movAltaBautizado++;
                }
            }
            // bajas bautizados del mes
            int[] codBaja = { 11101, 11102, 11103, 11004, 11005, 12101, 12102, 12103, 12104, 12105, 12106 };
            int movBajaBautizado = 0;
            foreach (var ca in codBaja)
            {
                var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                if (a.Count() > 0)
                {
                    movBajaBautizado++;
                }
            }

            // altas no bautizados del mes
            int[] codAltaNB = { 12001, 12002, 12003, 12004 };
            int movAltaNB = 0;
            foreach (var ca in codAltaNB)
            {
                var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                if (a.Count() > 0)
                {
                    movAltaNB++;
                }
            }
            // bajas no bautizados del mes
            int[] codBajaNB = { 12101, 12102, 12103, 12104, 12105, 12106 };
            int movBajaNB = 0;
            foreach (var ca in codBajaNB)
            {
                var a = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == ca).ToList();
                if (a.Count() > 0)
                {
                    movBajaNB++;
                }
            }

            // ALTAS BAUTIZADOS
            // alta por bautismo
            int ab = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11001).ToList().Count();

            // alta por restitucion
            var ar = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11002).ToList().Count();

            // alta bautizado por cambio de domicilio interno
            var abcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11003).ToList().Count();

            // alta bautizado por cambio de domicilio externo
            var abcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11004).ToList().Count();

            // BAJAS BAUTIZADOS
            // defuncion
            var bd = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11101).ToList().Count();

            // baja excomunion temporal
            var bet = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11102).ToList().Count();

            // baja excomunion
            var be = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11103).ToList().Count();

            // baja cambio de domicilio interno
            var bcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11004).ToList().Count();

            // baja cambio de domicilio externo
            var bcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 11005).ToList().Count();

            // ALTAS NO BAUTIZADOS
            // nuevo ingreso
            var anbni = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12001).ToList().Count();

            // reactivacion
            var anbr = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12004).ToList().Count();

            // alta no bautizado por cambio de domicilio interno
            var anbcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12002).ToList().Count();

            // alta bautizado por cambio de domicilio externo
            var anbce = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12003).ToList().Count();

            // BAJAS NO BAUTIZADOS
            // defuncion
            var bnbd = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12101).ToList().Count();

            // alejamiento
            var bnba = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12102).ToList().Count();

            // baja cambio de domicilio interno
            var bnbcdi = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12103).ToList().Count();

            // baja cambio de domicilio externo
            var bnbcde = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12104).ToList().Count();

            // baja porque pasa a bautizado
            var bnbpb = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12105).ToList().Count();

            // baja por baja de padres
            var bnbbp = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 12106).ToList().Count();

            // HOGARES
            // hogares por sector
            var hogares = (from p in personas
                           join hp in context.Hogar_Persona on p.per_Id_Persona equals hp.per_Id_Persona
                           where hp.hp_Jerarquia == 1 && p.sec_Id_Sector == fhte.sec_Id_Sector
                           select hp).ToList().Count();
            // alta hogares
            var ah = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 31001).ToList().Count();
            // baja hogares
            var bh = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 31102).ToList().Count();

            var matrimonios = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 21001).ToList().Count();
            var legalizacion = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 21102).ToList().Count();
            var presentaciones = hteDelMesConsultado.Where(hte => hte.ct_Codigo_Transaccion == 23203).ToList().Count();

            // construye resultado de la consulta
            HistTransEstBySectorMes.altas.bautizados altasBautizados = new HistTransEstBySectorMes.altas.bautizados
            {
                BAUTISMO = ab,
                RESTITUCIÓN = ar,
                CAMBIODEDOMINTERNO = abcdi,
                CAMBIODEDOMEXTERNO = abcde,
                ALTAHOGAR = ah
            };

            HistTransEstBySectorMes.altas.noBautizados altasNoBautizados = new HistTransEstBySectorMes.altas.noBautizados
            {
                NUEVOINGRESO = anbni,
                REACTIVACION = anbr,
                CAMBIODEDOMINTERNO = anbcdi,
                CAMBIODEDOMEXTERNO = anbce
            };

            HistTransEstBySectorMes.bajas.bautizados bajasBautizados = new HistTransEstBySectorMes.bajas.bautizados
            {
                DEFUNCION = bd,
                EXCOMUNIONTEMPORAL = bet,
                EXCOMUNION = be,
                CAMBIODEDOMINTERNO = bcdi,
                CAMBIODEDOMEXTERNO = bcde,
                BAJAHOGAR = bh
            };

            HistTransEstBySectorMes.bajas.noBautizados bajasNoBautizados = new HistTransEstBySectorMes.bajas.noBautizados
            {
                DEFUNCION = bnbd,
                ALEJAMIENTO = bnba,
                CAMBIODEDOMICILIOINTERNO = bnbcdi,
                CAMBIODEDOMICILIOEXTERNO = bnbcde,
                PASAAPERSONALBAUTIZADO = bnbpb,
                PORBAJADEPADRES = bnbbp
            };

            movimientosEstadisticosReporteBySector resultado = new movimientosEstadisticosReporteBySector();
            resultado.personasBautizadas = personasBautizadas;
            resultado.personasNoBautizadas = personasNoBautizadas;
            resultado.personasBautizadasAlFinalDelMes = personasBautizadas + movAltaBautizado - movBajaBautizado;
            resultado.personasNoBautizadasAlFinalDelMes = personasNoBautizadas + movAltaNB - movBajaNB;
            resultado.hogares = hogares;
            resultado.hogaresAlFinalDelMes = hogares + ah - bh;
            resultado.matrimonios = matrimonios;
            resultado.legalizaciones = legalizacion;
            resultado.presentaciones = presentaciones;
            resultado.altasBautizados = altasBautizados;
            resultado.altasNoBautizados = altasNoBautizados;
            resultado.bajasBautizados = bajasBautizados;
            resultado.bajasNoBautizados = bajasNoBautizados;

            // agregar 
            // sucesos estadisticos y
            // desglose de movimientos estadisticos: Historial_Transacciones_EstadisticasController.HistorialPorFechaSector

            return resultado;
        }

        public List<HistorialPorFechaSector> SubHistorialPorFechaSector(Historial_Transacciones_EstadisticasController.FechasSectorDistrito fsd)
        {
            List<HistorialPorFechaSector> resultado = new List<HistorialPorFechaSector>();
            var query = (from hte in context.Historial_Transacciones_Estadisticas
                         join cte in context.Codigo_Transacciones_Estadisticas
                         on hte.ct_Codigo_Transaccion equals cte.ct_Codigo
                         join per in context.Persona on hte.per_Persona_Id equals per.per_Id_Persona
                         where hte.sec_Sector_Id == fsd.idSectorDistrito
                         && (hte.hte_Fecha_Transaccion >= fsd.fechaInicial && hte.hte_Fecha_Transaccion <= fsd.fechaFinal)
                         orderby cte.ct_Tipo ascending
                         select new
                         {
                             hte.hte_Id_Transaccion,
                             hte.ct_Codigo_Transaccion,
                             cte.ct_Grupo,
                             cte.ct_Tipo,
                             cte.ct_Subtipo,
                             per.per_Nombre,
                             per.per_Apellido_Paterno,
                             per.per_Apellido_Materno,
                             per.per_Apellido_Casada,
                             apellidoPrincipal = (per.per_Apellido_Casada == "" || per.per_Apellido_Casada == null) ? per.per_Apellido_Paterno : (per.per_Apellido_Casada + "* " + per.per_Apellido_Paterno),
                             per.per_Bautizado,
                             per.per_Categoria,
                             hte.hte_Comentario,
                             hte.hte_Fecha_Transaccion,
                             hte.dis_Distrito_Alias,
                             hte.sec_Sector_Alias
                         }).ToList();
            foreach(var q in query)
            {
                resultado.Add(new HistorialPorFechaSector
                {
                    hte_Id_Transaccion = q.hte_Id_Transaccion,
                    ct_Codigo_Transaccion = q.ct_Codigo_Transaccion,
                    ct_Grupo = q.ct_Grupo,
                    ct_Tipo = q.ct_Tipo,
                    ct_Subtipo = q.ct_Subtipo,
                    per_Nombre = q.per_Nombre,
                    per_Apellido_Paterno = q.per_Apellido_Paterno,
                    per_Apellido_Materno = q.per_Apellido_Materno,
                    per_Apellido_Casada = q.per_Apellido_Casada,
                    apellidoPrincipal = (q.per_Apellido_Casada == "" || q.per_Apellido_Casada == null) ? q.per_Apellido_Paterno : (q.per_Apellido_Casada + "* " + q.per_Apellido_Paterno),
                    per_Bautizado = q.per_Bautizado,
                    per_Categoria = q.per_Categoria,
                    hte_Comentario= q.hte_Comentario,
                    hte_Fecha_Transaccion = q.hte_Fecha_Transaccion,
                    dis_Distrito_Alias = q.dis_Distrito_Alias,
                    sec_Sector_Alia = q.sec_Sector_Alias
                });
            }
            return resultado;
        }
    }
}