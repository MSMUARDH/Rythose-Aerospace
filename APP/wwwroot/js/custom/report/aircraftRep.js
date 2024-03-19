var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        ajax: {
            url: "/Main/Report/GetAllAircraft",
        },
        columns: [
            {
                data: "url",
                render: function (data, type, row) {

                    return '<img src="' + data + '" alt="' + data + '"height="50" width="50"/>';;
                },
                autoWidth: true
            },
            { data: "code", autoWidth: true },
            { data: "name", autoWidth: true },
            { data: "category", autoWidth: true },
            { data: "model", autoWidth: true },
            { data: "price", autoWidth: true },
            { data: "qty", autoWidth: true },
           
        ],
        dom: "Bfrtip",
        buttons: [
            {
                extend: 'excel',
                className: 'btn  rounded-0',
                text: '<i class="far fa-file-excel"></i>Excel',
                title: 'Aircraft Report',
               
            },
            {
                extend: 'pdf',
                className: 'btn rounded-0',
                text: '<i class="far fa-file-pdf"></i>PDF',
                title: 'Aircraft Report',
            }
        ],
    });
}
