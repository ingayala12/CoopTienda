let dataTable;

$(document).ready(() => {
    CargarTabla();
});

const CargarTabla = () => {
    dataTable = $("#tblDatos").DataTable({
        "ajax": {
            "url":"/Admin/Marca/ObtenerTodos"
        },
        "columns": [
            {"data": "nombre"},
            {
                "data": "estado",
                "render": function (data) {
                    if (data) {
                        return `<span class="badge text-bg-success">Activo</span>`
                    } else {
                        return `<span class="badge text-bg-danger text-white">Inactivo</span>`
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="d-flex flex-column gap-2 flex-md-row justify-content-center">
                            <a href="/Admin/Marca/Editar/${data}" class="btn btn-primary btn-sm"><i class="fa-solid fa-pen-to-square"></i></a>
                            <button onclick=Delete("/Admin/Marca/Delete/${data}") class="btn btn-danger btn-sm text-white"><i class="fa-solid fa-trash"></i></button>
                        </div>`;
                }
            }
        ]
    })
}
const Delete = (url) => {
    Swal.fire({
        title: "¿Estás seguro?",
        text: "¡No podrás revertir esto!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#15803d",
        cancelButtonColor: "#d33",
        confirmButtonText: "SI, Eliminar!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data) {
                        dataTable.ajax.reload();
                        Swal.fire({
                            title: "Eliminado",
                            text: "Tu archivo ha sido eliminado.",
                            icon: "success"
                        });
                    }
                }
            })
        }
    });
}
