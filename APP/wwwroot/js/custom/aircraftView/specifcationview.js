var dataTable1;
let airid1 = $("#script1").attr("data-air1id");

$(document).ready(function () {
    loadDataTable1();
});

function loadDataTable1() {
    dataTable1 = $("#tblData1").DataTable({
        ajax: {
            url: "/Main/Aircraft/GetAircraftSpe?airid=" + airid1,
        },
        columns: [
            { data: "title", autoWidth: true },
            { data: "description", autoWidth: true },          
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






