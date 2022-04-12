
function dataDetailTicket(employeeId,TicketId, role) {
    $.ajax({
        type: "GET",
        async: false,
        url: "https://localhost:44376/API/tickets/" + TicketId,
        data: {}
    }).done((result) => {
        console.log(result);
        $('#updateTicketId').html('<input id="FormUpdateTicketId" type="text" class="form-control input-group-sm" name="ticketId" placeholder="' + result.id + '" value="' + result.id + '" disabled>');
        $('#UpdateIssue').html(' <textarea id="FormUpdateIssue" class="form-control" rows="3" placeholder="' + result.issue + '" value="' + result.issue + '" disabled></textarea>');
        var textBtn = `<button type="submit" class="btn btn-danger" onclick="EskalasiTicket('${employeeId}','${result.id}','${role}')" style="margin-right:8%"> Eskalasi </button>`;
        textBtn += `<button type="submit" class="btn btn-success" onclick=""> Kirim </button>`;
        console.log(textBtn);
        $('#formBtn').html(textBtn);

        if (role = "Admin") {
            console.log("admin neh klik baris tiket");

            $('#updatePriority').html('<select id="FormSelectPriority" name="Priority" class="form-control "  ></select >');
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
                $('#FormSelectPriority').html(textPriorities);
            }).fail((err) => {
                console.log(err);
            });


        }

    }).fail((err) => {
        console.log(err);
    });
}

function getVacantEmployee(FromEmployeeId) {
    console.log("Masuk Get Vacant");
    $.ajax({
        type: "GET",
        async: false,
        url: "https://localhost:44376/API/employees/" + FromEmployeeId,
        data: {}
    }).done((resultFromEmployee) => {

        console.log("Masuk ajax get employee by id");
        console.log(resultFromEmployee);

        $.ajax({
            type: "GET",
            async: false,
            url: "https://localhost:44376/API/employees",
            data: {}
        }).done((resultAllEmployee) => {

            console.log("Masuk ajax get employee all");
            var toEmployeeId = "";

            for (var i = 0; i < resultAllEmployee.length; i++) {
                console.log("MASUK FOR");
                if (resultAllEmployee[i].id != resultFromEmployee.id &&
                    resultAllEmployee[i].workload < 10 &&
                    resultAllEmployee[i].position_id == resultFromEmployee.position_id + 1) {
                    console.log("MASUK IF");

                    toEmployeeId = resultAllEmployee[i].id;
                    break;
                }
                else {
                    console.log("MASUK ELSE");

                }
            };
            console.log(toEmployeeId);
            return toEmployeeId;


        }).fail((err) => {
            console.log(err);
        });



    }).fail((err) => {
        console.log(err);
    });
}

function EskalasiTicket(EmployeeId, TicketId, role) {

    if (role = "Admin") {
        console.log("masuk admin eskalasi");
        let Priority = new Object();
        Priority.Ticket_Id = TicketId;
        Priority.Priority_Id = Number($("#FormSelectPriority").val());

        let OpenTicket = new Object();
        OpenTicket.Ticket_Id = TicketId ;
        OpenTicket.Opener_Employee_Id = EmployeeId;

        let toEmployeeId;
        try {
            toEmployeeId = getVacantEmployee(EmployeeId);
            console.log(toEmployeeId);

        } catch (e) {
            console.log(e);
        }


        let ForwardTicket = new Object();
        ForwardTicket.Ticket_Id = TicketId;
        ForwardTicket.To_Employee_Id = toEmployeeId;


        console.log("Object buat Priority"  );
        console.log( Priority);
        console.log("Object buat Open"  );
        console.log(OpenTicket);
        console.log("Object buat Forward");
        console.log(ForwardTicket);


        //if (!Number.isNaN(Priority.Priority_Id)) {
        //    $.ajax({
        //        headers: {
        //            'Accept': 'application/json',
        //            'Content-Type': 'application/json'
        //        },
        //        type: "PUT",
        //        url: "https://localhost:44376/API/Tickets/setPriority",
        //        dataType: "json",
        //        data: JSON.stringify(Priority)
        //    }).done((result) => {
        //        console.log("sukses atur priority tiket")

        //        $.ajax({
        //            headers: {
        //                'Accept': 'application/json',
        //                'Content-Type': 'application/json'
        //            },
        //            type: "POST",
        //            url: "https://localhost:44376/API/Tickets/open",
        //            dataType: "json",
        //            data: JSON.stringify(OpenTicket)
        //        }).done((result) => {
        //            console.log("sukses Membuka tiket")



        //        }).fail((error) => {
        //            console.log(error);

        //        })

        //    }).fail((error) => {
        //        //alert pemberitahuan jika gagal
        //        console.log(error);

        //    })
        //}
        //else {
        //    Swal.fire({
        //        icon: 'error',
        //        title: 'Oops...',
        //        text: 'Priority belum dipilih, silahkan dicek kembali',
        //        timer: 2000,
        //    })
        //}

        
    }
    else {

    }
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
        async:false,
        url: "https://localhost:44376/api/Employees/" + objSession.Id+"/Tickets/",
        data: {}
    }).done((result) => {



        var textTicket = ``;
        if (result.length != 0) {
            $.each(result, function (key, val) {
                textTicket += `<tr data-bs-toggle="modal" data-bs-target="#ModalTicketDetail" onclick="dataDetailTicket('${objSession.Id }','${result[key].ticket_Id}','${objSession.Role }')">`;
                textTicket += `<th scope="row">${key + 1}</th>`;
                textTicket += `<td>${result[key].ticket_Id}</td>`;
                textTicket += `<td>${result[key].category}</td>`;
                textTicket += `<td>${result[key].issue}</td>`;
                textTicket += `</tr>`;
                //console.log(textTicket);
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



