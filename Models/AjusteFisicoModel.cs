using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoyVaro
{
    public class AjusteFisicoModel
    {
        public int id { get; set; }
        public int EstadoCalaveras { get; set; }
        public int NumeroCalaveras { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionCalaveras { get; set; }
        public decimal CostoCalavera { get; set; }
        public int EstadoCristales { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionCristales { get; set; }
        public decimal CostoCristales { get; set; }
        public int EstadoDefensas { get; set; }
        public int NumeroDefensas { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionDefensas { get; set; }
        public decimal CostoDefensas { get; set; }
        public int EstadoEspejoLaterales { get; set; }
        public int NumeroEspejoLaterales { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionEspejoLaterales { get; set; }
        public decimal CostoEspejoLaterales { get; set; }
        public int EstadoRetrovisor { get; set; }
        public int NumeroRetrovisor { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionRetrovisor { get; set; }
        public decimal CostoRetrovisor { get; set; }
        public int EstadoFaros { get; set; }
        public int NumeroFaros { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionFaros { get; set; }
        public decimal CostoFaros { get; set; }
        public int EstadoFacias { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionFacias { get; set; }
        public decimal CostoFacias { get; set; }
        public int EstadoHojalateria { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionHojalateria { get; set; }
        public decimal CostoHojalateria { get; set; }
        public int EstadoLlantas { get; set; }
        public int NumeroLlantas { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionLlantas { get; set; }
        public decimal CostoLlantas { get; set; }
        public int EstadoParabrisas { get; set; }
        public decimal CostoParabrisas { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionParabrisas { get; set; }
        public int EstadoPintura { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionPintura { get; set; }
        public decimal CostoPintura { get; set; }
        public bool PolizaSeguro { get; set; }
        public bool Radio { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionRadio { get; set; }
        public int EstadoRhines { get; set; }
        public int NumeroRines { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionRines { get; set; }
        public decimal CostoRines { get; set; }
        public int EstadoSalpicaderas { get; set; }
        public int NumeroSalpicaderas { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionSalpicaderas { get; set; }
        public decimal CostoSalpicaderas { get; set; }
        public int EstadoSistemaElectrico { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionSistemaElectrico { get; set; }
        public decimal CostoSistemaElectrico { get; set; }
        public int EstadoTapones { get; set; }
        public int NumeroTapones { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionTapones { get; set; }
        public decimal CostoTapones { get; set; }
        public bool TarjetaDeCirculacion { get; set; }
        public bool Verificacion { get; set; }
        public int EstadoVestiduras { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Sin observaciones")]
        public string ObservacionVestiduras { get; set; }
        public decimal CostoVestiduras { get; set; }
        public decimal TotalDemerito { get; set; }
        public int IdCredito { get; set; }
        public bool Revisado { get; set; }
        public string OtrosObservacion { get; set; }
        public decimal CostoOtros { get; set; }


        public virtual Credito Credito { get; set; }

        [DisplayName("Demerito por condisiones físicas")]
        [DataType(DataType.Currency)]
        public virtual decimal AjusteTotalFisico
        {
            get
            {
                return  CostoCalavera + CostoCristales + CostoDefensas + CostoEspejoLaterales + CostoRetrovisor + CostoFaros + CostoFacias + CostoParabrisas +
                    CostoHojalateria + CostoLlantas + CostoPintura + CostoRines + CostoSalpicaderas + CostoSistemaElectrico + CostoTapones + CostoVestiduras + CostoOtros ;
            }

        }

        [DisplayName("Observaciones Ajuste Físico")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = " ")]
        public virtual String ObservacionAjusteFisico
        {
            get
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append(ObservacionCalaveras).Append(' ', 90);
                sb.Append(ObservacionCristales).Append(' ', 90);
                sb.Append(ObservacionDefensas).Append(' ', 90);
                sb.Append(ObservacionEspejoLaterales).Append(' ', 90);
                sb.Append(ObservacionRetrovisor).Append(' ', 90);
                sb.Append(ObservacionFaros).Append(' ', 90);
                sb.Append(ObservacionFacias).Append(' ', 90);
                sb.Append(ObservacionHojalateria).Append(' ', 90);
                sb.Append(ObservacionLlantas).Append(' ', 90);
                sb.Append(ObservacionParabrisas).Append(' ', 90);
                sb.Append(ObservacionPintura).Append(' ', 90);
                sb.Append(ObservacionRines).Append(' ', 90);
                sb.Append(ObservacionSalpicaderas).Append(' ', 90);
                sb.Append(ObservacionSistemaElectrico).Append(' ', 90);
                sb.Append(ObservacionTapones).Append(' ', 90);
                sb.Append(ObservacionVestiduras).Append(' ', 90);
                return sb.ToString();
            }

        }
    }
}