var dataTable;

$(function () {
    loadDataTable();
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        order: [[0, 'desc']],
        ajax: { url: "/order/getall" },
        columns: [
            { data: 'orderHeaderId', "width": "5%" },
            { data: 'email', "width": "25%" },
            { data: 'fullName', "width": "20%" },
            { data: 'phone', "width": "10%" },
            { data: 'status', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'orderHeaderId',
                "render": function (data) {
                    console.log('Header Data: ' + data);
                    return '';
                },
                "width": "10%"
            }
        ]
    });
}