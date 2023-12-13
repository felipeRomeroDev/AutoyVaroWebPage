
// Control de validaciones
function ValidatorForm(Data) {    
    $.each(Data, function (index, value) {
        $("#" + value.Id).addClass("is-invalid");
        $("#" + value.Id).attr("data-toggle", "tooltip");
        $("#" + value.Id).attr("data-placement", "top");
        $("#" + value.Id).attr("title", value.Mensaje);
        $("#" + value.Id).attr("data-original-title", value.Mensaje);
        $("#" + value.Id).tooltip();
    });
}
function DesmarcarError(Id) {
    $("#" + Id).removeClass("is-invalid");
    $("#" + Id).removeAttr("data-toggle");
    $("#" + Id).removeAttr("data-placement");
    $("#" + Id).removeAttr("data-original-title");

}

function SuccessOk(Data) {
    if (!Data.Error) {
        notie.alert({ type: 1, text: Data.Mensaje, time: 3 })
        $("[id*=Modal]").modal("hide");
        $("[id*=table]").bootstrapTable("refresh");
    } else {
        notie.alert({ type: 3, text: Data.Mensaje, time: 3 })
        ValidatorForm(Data.data);
    }
}
function SuccessNot(Data) {

}

function formatofechaDiaMesAnio(value, row, index) { // return MM-YYYY
    if (value == null || value == "") {
        return "-"
    }
    var fecha = value.replace("/Date(", "").replace(")/", "");// /Date(1385013600000)/
    var date = new Date(parseInt(fecha));
    return pad((date.getDate()), 2) + "/" + pad((date.getMonth() + 1), 2) + "/" + date.getFullYear();
}


function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}


