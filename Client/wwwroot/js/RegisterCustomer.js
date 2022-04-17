$("#registerForm").submit(function (event) {
    event.preventDefault();

    const registerData = new Object();
    registerData.First_Name = $("#registerForm #firstName").val()
    registerData.Last_Name = $("#registerForm #lastName").val()
    registerData.Email = $("#registerForm #email").val()
    registerData.Password = $("#registerForm #password").val()
    registerData.Phone_Number = $("#registerForm #phoneNumber").val()

    console.log(registerData);
    $.ajax({
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        type: "POST",
        url: "https://localhost:44376/API/Customers/register",
        dataType: "json",
        data: JSON.stringify(registerData)
    }).done(result => {
        console.log(result)
        Swal.fire({
            icon: 'success',
            title: 'Registrasi Sukses',
            text: "Akun berhasil dibuat"
        })
    }).fail(error => {
        console.log(error)

        if (error.status == 400) {
            Swal.fire({
                icon: 'error',
                title: 'Registrasi Gagal',
                text: error.responseJSON
            })
        }
    })
})