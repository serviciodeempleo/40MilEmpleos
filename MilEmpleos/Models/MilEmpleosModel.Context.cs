﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MilEmpleos.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MilEmpleosEntities : DbContext
    {
        public MilEmpleosEntities()
            : base("name=MilEmpleosEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ActividadesPuestoDeTrabajo> ActividadesPuestoDeTrabajo { get; set; }
        public virtual DbSet<ActividadNivEduDes> ActividadNivEduDes { get; set; }
        public virtual DbSet<ActividadNivEduHab> ActividadNivEduHab { get; set; }
        public virtual DbSet<ActNivEdu> ActNivEdu { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Centros> Centros { get; set; }
        public virtual DbSet<Consulta> Consulta { get; set; }
        public virtual DbSet<ContenidoActividadFormacion> ContenidoActividadFormacion { get; set; }
        public virtual DbSet<department> department { get; set; }
        public virtual DbSet<Destrezas> Destrezas { get; set; }
        public virtual DbSet<Habilidad> Habilidad { get; set; }
        public virtual DbSet<JovenesPorTutor> JovenesPorTutor { get; set; }
        public virtual DbSet<LogsAudit> LogsAudit { get; set; }
        public virtual DbSet<municipality> municipality { get; set; }
        public virtual DbSet<NivelEducativo> NivelEducativo { get; set; }
        public virtual DbSet<PassPrestadores> PassPrestadores { get; set; }
        public virtual DbSet<Pila> Pila { get; set; }
        public virtual DbSet<Ponderacion> Ponderacion { get; set; }
        public virtual DbSet<PonderacionMunicipio> PonderacionMunicipio { get; set; }
        public virtual DbSet<RespuestaPila> RespuestaPila { get; set; }
        public virtual DbSet<ResultadoCalculadora> ResultadoCalculadora { get; set; }
        public virtual DbSet<SalarioAdicionalRango> SalarioAdicionalRango { get; set; }
        public virtual DbSet<TipoConocimientoRequerido> TipoConocimientoRequerido { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<WSPila> WSPila { get; set; }
        public virtual DbSet<actulizacionPila> actulizacionPila { get; set; }
        public virtual DbSet<RespuestasCalculadora> RespuestasCalculadora { get; set; }
    }
}
