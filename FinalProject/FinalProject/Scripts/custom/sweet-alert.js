function successAlert(title, message) {
    swal({
        title: title,
        text: message,
        icon: 'success',
        button: true
    });
}

function errorAlert(title, message) {
    swal({
        title: title,
        text: message,
        icon: 'error',
        button: true
    });
}