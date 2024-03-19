var dataTable;
var dataTable1;

$(document).ready(function () {
    loadDataTable();
    loadDataTableJob();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        ajax: {
            url: "/Main/Order/GetActiveTeams",
        },
        columns: [
            { data: "code", autoWidth: true },
            { data: "name", autoWidth: true },
            { data: "noOfEmployees", autoWidth: true },
            { data: "curStatus", autoWidth: true },
           
            {
                data: "id",
                render: function (data, type, row) {
                    return `
                            <div class="text-center">
                                <a onclick="SelectEmp('${data}','${row.name}')"; class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-plus"></i>
                                </a>
                            </div>
                           `;
                },
                autoWidth: true,
            },
        ],      
    });
}

function SelectEmp(teamId, name) {
   
    Swal.fire({
        title: `Select ${name} For Job?`,
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Select",
    }).then((select) => {
        if (select.isConfirmed) {
            AddTeamToJob(teamId);
        }
    });
}

function AddTeamToJob(teamId) {
   

    var date = new Date($('#startDate').val());
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    /*alert(date);*/
    var selectedDate = date.toISOString();
    /*var selectedDate = day + "/" + month + "/" + year*/
    var url = "/Main/Order/AddTeamToJob";
    jQuery.getJSON(url, { fetch: teamId, selectedDate}, function (data) {
        if (data.success) {
            toastr.success(data.message);
            dataTable.ajax.reload();
            dataTable1.ajax.reload();
        } else {
            toastr.error(data.message);
        }
    });
}


function loadDataTableJob() {
    dataTable1 = $("#tblData1").DataTable({
        ajax: {
            url: "/Main/Order/GetJobAssignedTeams",
        },
        columns: [
            { data: "code", autoWidth: true },
            { data: "name", autoWidth: true },
            { data: "noOfEmployees", autoWidth: true },
            { data: "curStatus", autoWidth: true },

            {
                data: "id",
                render: function (data, type, row) {
                    return `
                             <div class="text-center">
                    <a onclick=Delete("/Main/Order/RemoveTeamToJob/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                        <i class="fas fa-trash-alt"></i>
                    </a>
                  </div>`;
                },
                autoWidth: true,
            },
        ],
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure you want to remove the Team ?",
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
                        dataTable1.ajax.reload();
                        
                    } else {
                        toastr.error(data.message);
                    }
                },
            });
        }
    });
}






