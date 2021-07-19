// Call the dataTables jQuery plugin
$(document).ready(function() {
  $('#dataTable').DataTable();
    $('#add-row').DataTable({
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'excel',
                oriented: 'potrait',
                pageSize: 'Legal',
                title: ' Data Pasien',
                download: 'open'

            },
            'copy',
            'excel',
            'csv',
            'pdf'
        ]
    });
});
