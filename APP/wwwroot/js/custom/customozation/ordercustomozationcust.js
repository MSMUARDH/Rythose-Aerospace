var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        ajax: {
            url: "/Public/Home/GetOrderCustomDetail",
        },
        columns: [
            { data: "customzationType", autoWidth: true },
            { data: "customzationValue", autoWidth: true },
        ],      
    });
}








