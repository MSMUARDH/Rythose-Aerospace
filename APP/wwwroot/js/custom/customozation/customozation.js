var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        ajax: {
            url: "/Main/Customozation/GetAll",
        },
        columns: [
            { data: "customzationType", autoWidth: true },
            { data: "customzationValue", autoWidth: true },
            { data: "price", autoWidth: true },
           
            {
                data: "id",
                render: function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Main/Customozation/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i> 
                                </a>
                                <a onclick=Delete("/Main/Customozation/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-trash-alt"></i> 
                                </a>
                            </div>
                           `;
                },
                autoWidth: true,
            },
        ],      
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!",
    }).then((willDelete) => {
        if (willDelete.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
            });
        }
    });
}






