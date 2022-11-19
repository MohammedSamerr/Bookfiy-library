var table;
var datatable;
var updatedRow;
var exportedColumns = [];

function showMassegeSuccessfully(massage = 'Saved successfully!') {
    Swal.fire({
        icon: 'success',
        title: 'success',
        text: massage,
        customClass: {
            confirmButton: "btn btn-primary"
        }

    });
}

function showMassegeErorr(massage = 'Something went wrong!') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: massage,
        customClass: {
            confirmButton: "btn btn-primary"
        }

    });
}

//on model begin
function onModelBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator','on');
}



function onMassegesuccess(row) {
    showMassegeSuccessfully();
    $('#Modal').modal('hide');

    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }

    var newRow = $(row);
    datatable.row.add(newRow).draw();

    KTMenu.init();
    KTMenu.initGlobalHandlers();
}

// on model complete
function onModelComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}


//handle datatable
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedColumns.push(i);
});

// Class definition
var KTDatatables = function () {
    // Shared variables


    // Private functions
    var initDatatable = function () {

        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10,
        });
    }

    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $('.js-datatable').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedColumns
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedColumns
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedColumns
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedColumns
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatable');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

$(document).ready(function () {
    //select2
    //$('.js-select2').select2();
    $('.js-select2').select2();
    //$('.js-select2').on('select2:select', function (e) {
    //    var select = $(this);
    //    $('form').validate().element('#' + select.attr('id'));
    //});
    $('.js-select2').on('select2:select', function (e) {
        var select = $(this);
        $('form').validate().element('#' + select.attr('id'));
    });

    //datepicker
    $('.js-datepicker').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear : 1901,
        maxDate : new Date(),
        autoApply: true,
        drops : 'up'
    });
    //sweet alerts
    var massage = $('#Message').text();
    if (massage !== '') {
        showMassegeSuccessfully(massage);
    }
    //handel datatable
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });
    //handle bootstarp modal

    $('body').delegate('.js-render-modal', 'click', function () {

        var btn = $(this);

        var modal = $('#Modal');

        //render title

        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {

            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {

                modal.find('.modal-body').html(form);
                //apply validation to ajax form
                $.validator.unobtrusive.parse(modal);

            },
            error: function () {
                showMassegeErorr();
            }

        });

        modal.modal('show');

    });

    //handle toggle status

    $('body').delegate('.js-toggle-status', 'click', function () {

        var btn = $(this);

        bootbox.confirm({
            message: "Are you sure that you want to change this category status?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        url: btn.data('url'),
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newstatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newstatus).toggleClass('badge-light-danger badge-light-success');


                            row.find('.js-updated-on').html(lastUpdatedOn);
                            row.addClass('animate__animated animate__flash')

                            showMassegeSuccessfully();
                        },
                        error: function () {
                            showMassegeErorr();
                        }
                    });

                }
            }
        });
    });



});