

function dataDetailTicket(id, role) {
    $.ajax({
        type: "GET",
        url: "https://localhost:44376/API/tickets/" + id,
        data: {}
    }).done((result) => {
        console.log(result);
        $('#updateTicketId').html('<input id="FormUpdateTicketId" type="text" class="form-control input-group-sm" name="ticketId" placeholder="' + result.id + '" value="' + result.id + '" disabled>');
        $('#UpdateIssue').html(' <textarea id="FormUpdateIssue" class="form-control" rows="3" placeholder="' + result.issue + '" value="' + result.issue + '" disabled></textarea>');

        if (role = "Admin") {
            console.log("admin neh");

            $('#updatePriority').html('<select name="Priority" class="form-control " id="selectPriority" ></select >');
            //BUAT SELECT OPTION PRIORITY
            $.ajax({
                type: "GET",
                url: "https://localhost:44376/API/priorities",
                data: {}
            }).done((result) => {
                var textPriorities = `<option value="hide" style="display: none;">Pick Priority</option>`;
                $.each(result, function (key, val) {

                    textPriorities += `<option value="${result[key].id}">${result[key].name}</option>`;
                });
                console.log(textPriorities);
                $('#selectPriority').html(textPriorities);
            }).fail((err) => {
                console.log(err);
            });


        }

        //$('#updateFirstName').html('<input id="FormUpdateFirstName" type="text" class="form-control input-group-sm" name="FirstName" placeholder="' + result.firstName + '" value="' + result.firstName +'" disabled>');
        //$('#updateLastName').html('<input id="FormUpdateLastName" type="text" class="form-control input-group-sm" name="LastName" placeholder="' + result.lastName + '" value="' + result.lastName +'" disabled>');
        //$('#updatePhoneNumber').html('<input id="FormUpdatePhoneNumber" type="text" class="form-control input-group-sm" name="PhoneNumber" placeholder="' + result.phone + '" value="' + result.phone +'" disabled>');
        //gender = result.gender;
        //if (gender == 0) {
        //    var inputMale = `<input type='radio' id="updateMale" name="GenderRadioUpdate" value="0"  disabled checked='checked'>`;
        //    var labelMale = `<label for='updateMale'>Male</label>`;
        //    var inputFemale = `<input type='radio' id="updateFemale" name="GenderRadioUpdate" value="1" disabled>`;
        //    var labelFemale = `<label for='updateFemale'>Female</label>`;
        //    $(`#updateGender`).html(inputMale + labelMale + inputFemale + labelFemale);
        //}
        //else {
        //    var inputMale = `<input type='radio' id="updateMale" name="GenderRadioUpdate" value="0"  disabled>`;
        //    var labelMale = `<label for='updateMale'>Male</label>`;
        //    var inputFemale = `<input type='radio' id="updateFemale" name="GenderRadioUpdate" value="1" disabled checked='checked' >`;
        //    var labelFemale = `<label for='updateFemale'>Female</label>`;
        //    $(`#updateGender`).html(inputMale + labelMale + inputFemale + labelFemale);
        //}
        //$('#updateDate').html('<input id="UpdateFormBirthDate" type="text" class="form-control input-group-sm" name="BirthDate" placeholder="' + result.birthDate + '" value="' + result.birthDate + '" disabled>');
        //$('#updateSalary').html('<input id="UpdateFormSalary" type="number" class="form-control input-group-sm" name="Salary" placeholder="' + result.salary + '" value="" >');
        //$('#updateEmail').html('<input id="UpdateFormEmail" type="text" class="form-control input-group-sm" name="Email" placeholder="' + result.email + '" value="' + result.email + '" disabled>');

    }).fail((err) => {
        console.log(err);
    });
}

function updateEmployee() {
    let obj = new Object();

    obj.Nik = $("#FormUpdateNik").val();
    obj.FirstName = $("#FormUpdateFirstName").val();
    obj.LastName = $("#FormUpdateLastName").val();
    obj.Phone = $("#FormUpdatePhoneNumber").val();

    var listGender = document.getElementsByName('GenderRadioUpdate');
    for (var i = 0; i < listGender.length; i++) {
        if (listGender[i].checked) {
            obj.gender = parseInt(listGender[i].value);
            break;
        }
    }
    obj.BirthDate = $("#UpdateFormBirthDate").val();
    obj.Salary = Number($("#UpdateFormSalary").val());
    obj.Email = $("#UpdateFormEmail").val();

    $.ajax({
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        type: "PUT",
        url: "https://localhost:44368/employees/put/",
        dataType: "json",
        data: JSON.stringify(obj)
    }).done((result) => {
        Swal.fire({
            icon: 'success',
            title: 'Data dengan NIK '+obj.Nik +' berhasil diubah',
        }).then((result) => {
            window.location.reload();
        })

    }).fail((error) => {
        //alert pemberitahuan jika gagal
        console.log(error);

    })
}

function deleteEmployee(nik) {
    Swal.fire({
        title: 'Apakah anda yakin?',
        text: "Data dengan NIK "+nik+" akan dihapus",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Ya',
        cancelButtonText: 'Tidak',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: "https://localhost:44368/employees/delete/" + nik,
                data: {}
            }).done((result) => {
                Swal.fire(
                    'Deleted!',
                    'Data dengan NIK '+nik+" telah dihapus",
                    'success'
                ).then((result) => {
                    window.location.reload();
                })
            }).fail((err) => {
                console.log(err);
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'Something went wrong!'+err,
                })
            });
            
        }
    })

}


//GET SESSION DATA FROM HTML//
let objSession = new Object();
objSession.Id = $("#hdnSessionId").data('value');
objSession.Name = $("#hdnSessionName").data('value');
objSession.Email = $("#hdnSessionEmail").data('value');
objSession.Role = $("#hdnSessionRole").data('value');
console.log(objSession);


$(document).ready(function () {


    $.ajax({
        type: "GET",
        url: "https://localhost:44376/api/Employees/" + objSession.Id+"/Tickets/",
        data: {}
    }).done((result) => {



        var textTicket = ``;
        if (result.length != 0) {
            $.each(result, function (key, val) {
                textTicket += `<tr data-bs-toggle="modal" data-bs-target="#ModalTicketDetail" onclick="dataDetailTicket('${result[key].ticket_Id}','${objSession.Role }')">`;
                textTicket += `<th scope="row">${key + 1}</th>`;
                textTicket += `<td>${result[key].ticket_Id}</td>`;
                textTicket += `<td>${result[key].category}</td>`;
                textTicket += `<td>${result[key].issue}</td>`;
                textTicket += `</tr>`;
                console.log(textTicket);
            });
        } else {
            textTicket += `<tr>`;
            textTicket += `<td colspan= "4"> Belum ada tiket </td>`;
            textTicket += `</tr>`;

            console.log(textTicket);

        }
        
        $('#ticketTableEmployee').html(textTicket);
    }).fail((err) => {
        console.log(err);
    });





})



