﻿namespace MilEmpleos.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;    
    public class ViewRespuestasCalculadora
    {
   
        public int id { get; set; }
        [Display(Name = "Razón Social")]
        [Required(ErrorMessage = "Campo Razón social es obligatorio.")]
        public string RazonSocial { get; set; }
        [Display(Name = "Nit")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Debe ser un entero")]
        [Required(ErrorMessage = "Campo Nit es obligatorio.")]
        public string Nit { get; set; }
        [Display(Name = "Teléfono Empresa")]
        [RegularExpression(@"^\d{3}\D?\d{2}\D?\d{2}$", ErrorMessage = "Debe ser un Teléfono")]
        [Required(ErrorMessage = "Campo Teléfono Empresa es obligatorio.")]
        public int TelefonoEmpresa { get; set; }
        [Display(Name = "Nombre Contacto")]
        [Required(ErrorMessage = "Campo Nombre Contacto es obligatorio.")]
        public string NombreContacto { get; set; }
        [Display(Name = "Descripción Vacante")]
        [StringLength(800, ErrorMessage = "Máximo 800 caracteres")]
        [Required(ErrorMessage = "Campo Descripción Vacante es obligatorio.")]
        public string DescripcionVacante { get; set; }
        [Display(Name = "Nivel Educativo Requerido Vacante")]
        [Required(ErrorMessage = "Campo Nivel Educativo Requerido Vacante es obligatorio.")]
        public int NivelEducativoRequeridoVacante { get; set; }
        [Display(Name = "Número Puestos Trabajo")]
        [Required(ErrorMessage = "Campo Número Puestos Trabajo es obligatorio.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Debe ser un entero")]
        [MinValue(1, ErrorMessage = "Introduzca un valor mayor o igual que 1.")]
        public int NumeroPuestosTrabajo { get; set; }
        [Display(Name = "Oficina Recursos Humanos")]
        [Required(ErrorMessage = "Campo Oficina Recursos Humanos es obligatorio.")]
        public bool OficinaRecursosHumanos { get; set; }
        [Display(Name = "Tamaño Planta Empresa")]
        [Required(ErrorMessage = "Campo Tamaño Planta Empresa Trabajo es obligatorio.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Debe ser un entero")]
        [MinValue(1, ErrorMessage = "Introduzca un valor mayor o igual que 1.")]
        public int TamanoPlantaEmpresa { get; set; }
        [Display(Name = "Habilidad Requerida")]
        [Required(ErrorMessage = "Campo Habilidad Requerida es obligatorio.")]
        public int HabilidadRequerida { get; set; }
        [Display(Name = "Destrezas Requeridas")]
        [Required(ErrorMessage = "Campo Destrezas Requeridas es obligatorio.")]
        public int DestrezasRequeridas { get; set; }
        [Display(Name = "Tipo Conocimiento Requerido")]
        [Required(ErrorMessage = "Campo Tipo Conocimiento Requerido es obligatorio.")]
        public int TipoConocimientoRequerido { get; set; }
        [Display(Name = "Actividades Puesto De Trabajo")]
        [Required(ErrorMessage = "Campo Actividades Puesto De Trabajo es obligatorio.")]
        public int ActividadesPuestoDeTrabajo { get; set; }
        [Display(Name = "Jóvenes Por Tutor")]
        [Required(ErrorMessage = "Campo Jóvenes Por Tutor es obligatorio.")]
        public int JovenesPorTutor { get; set; }
        [Display(Name = "Vacante Nueva")]
        [Required(ErrorMessage = "Campo es una Vacante Nueva es obligatorio.")]
        public bool VacanteNueva { get; set; }
        [Display(Name = "Puesto Trabajo Permanente")]
        [Required(ErrorMessage = "Campo Puesto Trabajo Permanente es obligatorio.")]
        public bool PuestoTrabajoPermanente { get; set; }
        [Display(Name = "Contenido Actividad Formación")]
        [Required(ErrorMessage = "Campo Contenido Actividad Formación es obligatorio.")]
        public int ContenidoActividadFormacion { get; set; }
        [Display(Name = "Salario Adicional")]
        [Required(ErrorMessage = "Campo Salario Adicional es obligatorio.")]
        public bool SalarioAdicional { get; set; }
        public int SalarioAdicionalRango { get; set; }
        public string Municipio { get; set; }
        public string UserId { get; set; }
        public int CentroId { get; set; }
        public System.DateTime FechaRegistro { get; set; }
    
        public virtual ActividadesPuestoDeTrabajo ActividadesPuestoDeTrabajo1 { get; set; }
        public virtual ContenidoActividadFormacion ContenidoActividadFormacion1 { get; set; }
        public virtual Destrezas Destrezas { get; set; }
        public virtual Habilidad Habilidad { get; set; }
        public virtual JovenesPorTutor JovenesPorTutor1 { get; set; }
        public virtual municipality municipality { get; set; }
        public virtual NivelEducativo NivelEducativo { get; set; }
        public virtual SalarioAdicionalRango SalarioAdicionalRango1 { get; set; }
        public virtual TipoConocimientoRequerido TipoConocimientoRequerido1 { get; set; }
        public virtual ICollection<ResultadoCalculadora> ResultadoCalculadora { get; set; }
        public virtual Centros Centros { get; set; }

        public class MinValueAttribute : ValidationAttribute, IClientValidatable
        {
            private readonly double _minValue;

            public MinValueAttribute(double minValue)
            {
                _minValue = minValue;
                ErrorMessage = "Introduzca un valor mayor o igual que " + _minValue;
            }

            public MinValueAttribute(int minValue)
            {
                _minValue = minValue;
                ErrorMessage = "Introduzca un valor menor o igual que " + _minValue;
            }

            public override bool IsValid(object value)
            {
                return Convert.ToDouble(value) >= _minValue;
            }

            public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
            {
                var rule = new ModelClientValidationRule();
                rule.ErrorMessage = ErrorMessage;
                rule.ValidationParameters.Add("min", _minValue);
                rule.ValidationParameters.Add("max", Double.MaxValue);
                rule.ValidationType = "range";
                yield return rule;
            }

        }
    }
}


