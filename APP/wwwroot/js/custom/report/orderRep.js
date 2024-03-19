var imgData = 'data:image/png;base64,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';
var dataTable;

$(document).ready(function () {
    loadDataTable();

    $("#orderStatusFilter").on("change", function () {
        var selectedOrderStatus = $(this).val();

        // Use DataTable API to filter data based on the selected order status
        dataTable.columns(8).search(selectedOrderStatus).draw();
    });
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        ajax: {
            url: "/Main/Report/GetAllOrders",
        },
        columns: [
            { data: "orderNo", autoWidth: true },
            { data: "orderDate", autoWidth: true },
            { data: "userName", autoWidth: true },
            { data: "aircraftName", autoWidth: true },
            { data: "qty", autoWidth: true },
            { data: "grandTotal", autoWidth: true },
            { data: "shippingCountry", autoWidth: true },
            { data: "deliveryDate", autoWidth: true },
            { data: "orderStatus", autoWidth: true },
        ],
        dom: "Bfrtip",
        buttons: [
            {
                extend: 'excel',
                className: 'btn  rounded-0',
                text: '<i class="far fa-file-excel"></i>Excel',
                title: 'Order Report',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
                }
            },
            {
                extend: 'pdf',
                className: 'btn rounded-0',
                text: '<i class="far fa-file-pdf"></i>PDF',
                title: 'Order Report',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8],
                    customize: function (doc) {
                        console.log('Image Data:', imgData);
                        doc.content.unshift({
                            image: imgData,
                            width: 500, // Adjust the width as needed
                            alignment: 'right',
                        });
                        console.log('Adding printed date');
                        var now = new Date();
                        var formattedDate = now.getDate() + '/' + (now.getMonth() + 1) + '/' + now.getFullYear();
                        doc.content.push({
                            text: 'Printed Date: ' + formattedDate,
                            margin: [0, 0, 0, 12], // Adjust the margin as needed
                            alignment: 'right',
                        });
                    },
                }
            },
        ],
    });
}
