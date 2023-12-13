using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace AutoyVaro
{
    public partial class AjusteFisico
    {
        public virtual decimal AjusteTotalFisico
        {
            get
            {

                return CostoCalavera + CostoCristales + CostoDefensas + CostoEspejoLaterales + CostoRetrovisor + CostoFaros + CostoFacias + CostoParabrisas +
                    CostoHojalateria + CostoLlantas + CostoPintura + CostoRines + CostoSalpicaderas + CostoSistemaElectrico + CostoTapones + CostoVestiduras + CostoOtros;
            }

        }


        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = " ")]
        [DisplayName("Observaciones")]
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