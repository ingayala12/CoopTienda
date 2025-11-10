let dataTable;

$(document).ready(() => {
    CargarTabla();
});

const CargarTabla = () => {
    dataTable = $("#tblDatos").DataTable({
        "ajax": {
            "url":"/Admin/Producto/ObtenerTodos"
        },
        "columns": [
            {"data": "serial"},
            {"data": "descripcion"},
            {"data": "categoria.nombre"},
            {"data": "marca.nombre"},
            {
                "data": "precio",
                "render": function (data, type, row) {
                    if (type === 'display' || type === 'filter') {
                        // Convertir a número por si viene como texto
                        const valor = parseFloat(data) || 0;
                        // Formatear a moneda sin mostrar "RD"
                        return `$${valor.toLocaleString('es-DO', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        })}`;
                    }
                    return data;
                }
            },
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
                            <a href="/Admin/Producto/Editar/${data}" class="btn btn-primary btn-sm"><i class="fa-solid fa-pen-to-square"></i></a>
                            <button onclick=Delete("/Admin/Producto/Delete/${data}") class="btn btn-danger btn-sm text-white"><i class="fa-solid fa-trash"></i></button>
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
