using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AutoyVaro.Enums
{
    public enum TipoCargo : int
    {
        CondonacionVencida = 1,
        Contrato = 2,
        Comprobantes = 3,
        Tenencia = 4
    };

    public enum EstatusCondonacion : int
    {
        Vencida = 1,
        Contrato = 2,
        Comprobantes = 3,
        Tenencia = 4,
        Aplicada = 5,
        Contabilizada = 6

    };

    public enum TipoCondonacion : int
    {
        Mora = 1,
        Contrato = 2,
        Comprobantes = 3,
        Tenencia = 4
    };

    public enum CatTipoCondonacion : int
    {
        PorMora =1,
        NotadeCrédito=2,
        CapitalCondonado=3,
        GPS=4,
        Pensión=5,
        Seguro =6,
        Interes =7
    }

    public enum TipoDetallePago : int
    {
        GPS = 1,
        Pension = 2,
        GastosAdministrativos = 3,
        GastosCobrenza = 4,
        CostoGPS = 5,
        Otros = 6,
        Seguro = 7,
        Interes = 8,
        Moratorios = 9,
        Amortización = 10,
        Capital = 11,
    }

    public enum CatTipoCargo : int
    {
        GastosAdmon = 1,
        GastosOperación = 2,
        GastosCobranza = 3,
        PagoGastosAdmon = 5
    };

    public enum TipoDePago : int
    {
        PagoNormal = 1,
        PagoACapital = 2,
        PagoGastosAdmon = 3,
        PagoAutCondonacionCapital=4,
        PagoAutNotaCredito = 5

    };

    public enum EstatusCobranza : int
    {
        Todos=1,
        Vencidos = 2,
        Vigente = 3,
        Adjudicado = 4,
        Prejuridico = 5,
        Juridico = 6,
        Liquidado = 7,
        Incobrable = 8,
        Fraude = 9
    }
}