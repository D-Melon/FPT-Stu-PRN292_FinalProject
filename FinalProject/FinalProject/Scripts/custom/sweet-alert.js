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

function confirmAlert(title, message, func) {
    swal({
        title: title,
        text: message,
        icon: "warning",
        buttons: {
            cancel: {
                text: "Hủy",
                value: "cancel",
                visible: true,
                classname: "",
                closeModel: true
            },
            confirm: {
                text: "Đồng ý",
                value: "confirm",
                visible: true,
                classname: "",
                closeModel: true
            }
        }
    }).then((value) => {
        if (value) {
            func();
        } else {

        }
    });
}