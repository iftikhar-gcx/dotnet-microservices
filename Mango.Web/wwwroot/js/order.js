var dataTable;

$(function () {
    var url = window.location.search;

    if (url.includes("status_approved")) {
        loadDataTable("status_approved");
    }
    else if (url.includes("status_pending")) {
        loadDataTable("status_pending");
    }
    else if (url.includes("status_readyforpickup")) {
        loadDataTable("status_readyforpickup");
    }
    else if (url.includes("status_completed")) {
        loadDataTable("status_completed");
    }
    else if (url.includes("status_refunded")) {
        loadDataTable("status_refunded");
    }
    else if (url.includes("status_cancelled")) {
        loadDataTable("status_cancelled");
    }
    else {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        order: [[0, 'desc']],
        ajax: { url: "/order/getall?status=" + status },
        columns: [
            { data: 'orderHeaderId', "width": "5%" },
            { data: 'email', "width": "25%" },
            { data: 'fullName', "width": "20%" },
            { data: 'phone', "width": "10%" },
            {
                data: 'status',
                "render": function (data) {
                    return data.split('_')[1] || data;
                },
                "width": "10%"
            },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'orderHeaderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/order/OrderDetail?orderId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"> </i>
                        </a>
                    </div>`;
                },
                "width": "10%"
            },
            
        ]
    });
}