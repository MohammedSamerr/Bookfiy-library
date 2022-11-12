var updatedRow;

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

function onMassegesuccess(item) {
    showMassegeSuccessfully();
    $('#Modal').modal('hide');
    if (updatedRow === undefined) {
        $('tbody').append(item);
    }
    else {
        $(updatedRow).replaceWith(item);
        updatedRow = undefined;
    }
    KT.init();
    KT.initHanders();
}

$(document).ready(function () {

    var massage = $('#Message').text();
    if (massage !== '') {
        showMassegeSuccessfully(massage);
    }

    //handle bootstarp modal

    $('body').delegate('.js-render-modal','click', function () {

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


});