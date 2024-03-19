var dataTable;
let airid = $("#script").attr("data-airid");
/*alert(airid);*/
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        ajax: {
          
            url: "/Main/Aircraft/GetAircraftCustom?airid=" + airid,
        },
        columns: [
            { data: "customzationType", autoWidth: true },
            { data: "customzationValue", autoWidth: true },
            { data: "price", autoWidth: true },
           
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






