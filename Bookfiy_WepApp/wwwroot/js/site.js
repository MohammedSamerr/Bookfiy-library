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

$(document).ready(function () {

    var massage = $('#Message').text();
    if (massage !== '') {
        showMassegeSuccessfully(massage);
    }

});