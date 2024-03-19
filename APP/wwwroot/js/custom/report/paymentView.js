

$(document).ready(function () {
    var dataTable;
    var selectedMonth = 0; // Initialize with "All Months" selected
    var totalAmount = 0;

    loadDataTable();

    $('#selectMonth').on('change', function () {
        selectedMonth = parseInt($(this).val());
        applyMonthFilter(selectedMonth);
    });

    function applyMonthFilter(month) {
        if (dataTable) {
            dataTable.column(1).search(month).draw();
        }
    }

    function loadDataTable() {
        dataTable = $("#tblData").DataTable({
            ajax: {
                url: "/Main/Report/GetPaymentDetails",
            },
            columns: [
                { data: "payId", autoWidth: true },
                {
                    data: "payDate",
                    autoWidth: true,
                    render: function (data) {
                        // Format the date in the desired format (e.g., MM/DD/YYYY)
                        var date = new Date(data);
                        var options = { year: "numeric", month: "2-digit", day: "2-digit" };
                        return date.toLocaleDateString(undefined, options);
                    },
                    searchable: true, // Enable searching on this column
                },
                { data: "user", autoWidth: true },
                {
                    data: "totalAmount",
                    autoWidth: true,
                    render: function (data) {
                        // Format the amount with two decimal places
                        return $.fn.dataTable.render.number(',', '.', 2,).display(data);
                    }
                },
                {
                    data: "shippingAmount",
                    autoWidth: true,
                    render: function (data) {
                        // Format the amount with two decimal places
                        return $.fn.dataTable.render.number(',', '.', 2,).display(data);
                    }
                },
                {
                    data: "grandTotal",
                    autoWidth: true,
                    render: function (data) {
                        // Format the amount with two decimal places
                        return $.fn.dataTable.render.number(',', '.', 2,).display(data);
                    }
                },
            ],
            pageLength: 20,
            "footerCallback": function (row, data, start, end, display) {
                var api = this.api();
                var column = api.column(5, { page: 'current' });
                var total = column.data().reduce(function (a, b) {
                    return parseFloat(a) + parseFloat(b);
                }, 0);
                console.log(total);
                totalAmount = total.toFixed(2);
                $(column.footer()).html(total.toFixed(2));
            },
            "dom": "Bfrtip",
            "buttons": [
                {
                    extend: 'excel',
                    className: 'btn rounded-0',
                    text: '<i class="far fa-file-excel"></i>Excel',
                    title: 'Payment Report',
                   
                   
                },
                {
                    extend: 'pdf',
                    className: 'btn rounded-0',
                    text: '<i class="far fa-file-pdf"></i>PDF',
                    title: 'Payment Report',
                  
                    customize: function (doc) {
                        // Include the total amount in the PDF
                        doc.content.splice(1, 0, { text: 'Total  :' + totalAmount, alignment: 'center' });
                    },
                  
                },
            ],
        });
    }
});






