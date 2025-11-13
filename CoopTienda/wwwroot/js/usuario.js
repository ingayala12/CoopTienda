let dataTable;

$(document).ready(() => {
    CargarTabla();
});

const CargarTabla = () => {
    dataTable = $("#tblDatos").DataTable({
        "ajax": {
            "url":"/Admin/Usuario/ObtenerTodos"
        },
        "columns": [
            {"data": "email"},
            {"data": "nombre"},
            {"data": "apellido"},
            {"data": "cedula"},
            {"data": "phoneNumber"},
            { "data": "role" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    let hoy = new Date().getTime();
                    let bloqueo = new Date(data.lockoutEnd).getTime();
                    if (bloqueo > hoy) {
                        // Usuario esta Bloqueado
                        //Usuario esta debloqueado
                        return `<div class="d-flex flex-column gap-2 flex-md-row justify-content-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-danger btn-sm text-white"> <i class="fa-solid fa-lock"></i> </a>
                        </div>`;
                    }
                    else {
                        //Usuario esta debloqueado
                        return `<div class="d-flex flex-column gap-2 flex-md-row justify-content-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-primary btn-sm text-white"> <i class="fa-solid fa-lock-open"></i> </a>
                        </div>`;
                    }

                }
            }
        ]
    })
}

const BloquearDesbloquear = (id) => {

    $.ajax({
        type: "POST",
        url: '/Admin/Usuario/BloquearDesbloquear',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data) {
                dataTable.ajax.reload();
                Swal.fire({
                    title: "Bloqueo",
                    text: "Operacion Exitosa",
                    icon: "success"
                });
            }
        }
    })
}
