var dataTable1;

$(document).ready(function () {
    loadDataTable1();
});

function loadDataTable1() {
    dataTable1 = $("#tblData1").DataTable({
        ajax: {
            url: "/Main/Order/GetOrderCustomDetail",
        },
        columns: [
            { data: "customzationType", autoWidth: true },
            { data: "customzationValue", autoWidth: true },
        ],      
    });
}








