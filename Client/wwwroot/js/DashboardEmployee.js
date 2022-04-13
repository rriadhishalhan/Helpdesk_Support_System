
function dataDetailTicket(employeeId, TicketId, role) {
    console.log(role);
    $.ajax({
        type: "GET",
        async: false,
        url: "https://localhost:44376/API/tickets/" + TicketId,
        data: {}
    }).done((result) => {
        //console.log(result);
        $('#updateTicketId').html('<input id="FormUpdateTicketId" type="text" class="form-control input-group-sm" name="ticketId" placeholder="' + result.id + '" value="' + result.id + '" disabled>');
        $('#UpdateIssue').html(' <textarea id="FormUpdateIssue" class="form-control" rows="3" placeholder="' + result.issue + '" value="' + result.issue + '" disabled></textarea>');
        if (role =="Server Engineer") {
            var textBtn = `<button type="submit" class="btn btn-success" onclick="KirimSolusi('${employeeId}','${result.id}','${role}')" title="Anda akan mengirimkan solusi ke customer"> Kirim </button>`;
            console.log(textBtn);
            $('#formBtn').html(textBtn);
        }
        else {
            var textBtn = `<button type="submit" class="btn btn-danger" onclick="EskalasiTicket('${employeeId}','${result.id}','${role}')" style="margin-right:8%" title="Tiket ini akan diserahkan ke Engineer"> Eskalasi </button>`;
            textBtn += `<button type="submit" class="btn btn-success" onclick="KirimSolusi('${employeeId}','${result.id}','${role}')" title="Anda akan mengirimkan solusi ke customer"> Kirim </button>`;
            console.log(textBtn);
            $('#formBtn').html(textBtn);
        }

        console.log(result.solution);
        if (result.solution != null) {
            console.log("Masuk solution tidak null");
            $('#UpdateSolution').html(' <textarea  id="inputSolution" class="form-control" rows="3" placeholder="' + result.solution + '" value="' + result.solution + '" disabled></textarea>');
            texBtnSolutionNotNull = `<button class="btn btn-secondary" type="button" data-bs-dismiss="modal" > Close </button>`;
            console.log(texBtnSolutionNotNull);
            $('#formBtn').html(texBtnSolutionNotNull);
        }
        else {
            console.log("Masuk solution null");
            $('#UpdateSolution').html(' <textarea  id="inputSolution" class="form-control" rows="3"></textarea>');

        }


        if (role == "Admin") {
            console.log("admin neh klik baris tiket");
            console.log(result.priority_Id);
            if (result.priority_Id != null) {
                console.log("masuk sini");
                var strPriority;

                switch (result.priority_Id) {
                    case 1:
                        strPriority = "Low";
                        break;
                    case 2:
                        strPriority = "Medium";
                        break;
                    case 3:
                        strPriority = "High";
                        break;
                }
                console.log(strPriority);

                $('#updatePriority').html('<input id="FormSelectPriority" name="Priority" type="text" class="form-control input-group-sm"  placeholder="' + strPriority + '" value="' + strPriority + '" disabled>');
            }
            else {
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
           

        }
        else {
            console.log("Developer neh klik baris tiket");
            var strPriority;

            switch (result.priority_Id) {
                case 1:
                    strPriority = "Low";
                    break;
                case 2:
                    strPriority = "Medium";
                    break;
                case 3:
                    strPriority = "High";
                    break;
            }
            console.log(strPriority);

            $('#updatePriority').html('<input id="FormSelectPriority" name="Priority" type="text" class="form-control input-group-sm"  placeholder="' + strPriority + '" value="' + strPriority + '" disabled>');

        }

    }).fail((err) => {
        console.log(err);
    });
}



function EskalasiTicket(EmployeeId, TicketId, role) {

    let OpenTicket = new Object();
    OpenTicket.Ticket_Id = TicketId;
    OpenTicket.Opener_Employee_Id = EmployeeId;

    let ForwardTicket = new Object();
    ForwardTicket.Ticket_Id = TicketId;


    if (role == "Admin") {
        console.log("masuk admin eskalasi");
        let Priority = new Object();
        Priority.Ticket_Id = TicketId;
        Priority.Priority_Id = Number($("#FormSelectPriority").val());


        console.log("Object buat Priority"  );
        console.log( Priority);
        console.log("Object buat Open"  );
        console.log(OpenTicket);
        console.log("Object buat Forward");
        console.log(ForwardTicket);

        //START OF CHECK PRIOIRTY = NAN
        if (!Number.isNaN(Priority.Priority_Id)) {
            //START OF AJAX SET PRIORITY
            $.ajax({
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                type: "PUT",
                url: "https://localhost:44376/API/Tickets/setPriority",
                dataType: "json",
                data: JSON.stringify(Priority)
            }).done((result) => {
                console.log("sukses atur priority tiket")
                //START OF AJAX OPEN TICKET
                $.ajax({
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    type: "POST",
                    url: "https://localhost:44376/API/Tickets/open",
                    dataType: "json",
                    data: JSON.stringify(OpenTicket)
                }).done((result) => {

                    console.log("sukses Membuka tiket")
                    //START OF AJAX FORWARD TIKET
                    $.ajax({
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json'
                        },
                        type: "POST",
                        url: "https://localhost:44376/API/Tickets/forward",
                        dataType: "json",
                        data: JSON.stringify(ForwardTicket)
                    }).done((result) => {
                        console.log("sukses Meneruskan Tiket ");
                        console.log(result);
                        Swal.fire({
                            icon: 'success',
                            title: 'Tiket berhasil diteruskan ke ' + result.employee_Name + '( ' + result.employee_Position+' )',
                        }).then((result) => {
                            window.location.reload();
                        })

                    }).fail((error) => {
                        console.log(error);

                    });
                    //END OF AJAX FORWARD TIKET


                }).fail((error) => {
                    console.log(error);

                });
                //END OF AJAX OPEN TICKET

            }).fail((error) => {
                //alert pemberitahuan jika gagal
                console.log(error);

            });
            //END OF AJAX SET PRIORITY
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Priority belum dipilih, silahkan dicek kembali',
                timer: 2000,
            })
        }
        //END OF CHECK PRIOIRTY = NAN
        
    }
    else {
        console.log("masuk engineer eskalasi");

        console.log("Object buat Open");
        console.log(OpenTicket);
        console.log("Object buat Forward");
        console.log(ForwardTicket);

        //START OF AJAX OPEN TICKET
        $.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            type: "POST",
            url: "https://localhost:44376/API/Tickets/open",
            dataType: "json",
            data: JSON.stringify(OpenTicket)
        }).done((result) => {

            console.log("sukses Membuka tiket")
            //START OF AJAX FORWARD TIKET
            $.ajax({
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                type: "POST",
                url: "https://localhost:44376/API/Tickets/forward",
                dataType: "json",
                data: JSON.stringify(ForwardTicket)
            }).done((result) => {
                console.log("sukses Meneruskan Tiket ");
                Swal.fire({
                    icon: 'success',
                    title: 'Tiket berhasil diteruskan ke ' + result.employee_Name + ' ( ' + result.employee_Position + ' )',
                }).then((result) => {
                    window.location.reload();
                })

            }).fail((error) => {
                console.log(error);

            });
            //END OF AJAX FORWARD TIKET


        }).fail((error) => {
            console.log(error);

        });
        //END OF AJAX OPEN TICKET

    }
}

function KirimSolusi(EmployeeId, TicketId, role) {
    console.log("Masuk Kirim Solusi");

    let OpenTicket = new Object();
    OpenTicket.Ticket_Id = TicketId;
    OpenTicket.Opener_Employee_Id = EmployeeId;


    let solutionTicket = new Object();
    solutionTicket.Ticket_Id = TicketId;
    solutionTicket.Employee_Id = EmployeeId;
    solutionTicket.Solution = $('#inputSolution').val();

    console.log("Object buat Open Ticket");
    console.log(OpenTicket);
    console.log("Object buat Respond Ticket");
    console.log(solutionTicket);

    if (role == "Admin") {
        console.log("masuk admin eskalasi");
        let Priority = new Object();
        Priority.Ticket_Id = TicketId;
        Priority.Priority_Id = Number($("#FormSelectPriority").val());

        console.log("Object buat Priority");
        console.log(Priority);

        //START OF CHECK PRIOIRTY = NAN
        if (!Number.isNaN(Priority.Priority_Id)) {
            //START OF AJAX SET PRIORITY
            $.ajax({
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                type: "PUT",
                url: "https://localhost:44376/API/Tickets/setPriority",
                dataType: "json",
                data: JSON.stringify(Priority)
            }).done((result) => {
                console.log("sukses atur priority tiket")
                //START OF AJAX OPEN TICKET
                $.ajax({
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    type: "POST",
                    url: "https://localhost:44376/API/Tickets/open",
                    dataType: "json",
                    data: JSON.stringify(OpenTicket)
                }).done((result) => {

                    console.log("sukses Membuka tiket")
                    //START OF AJAX RESPOND TIKET
                    $.ajax({
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json'
                        },
                        type: "PUT",
                        url: "https://localhost:44376/API/Tickets/respond",
                        dataType: "json",
                        data: JSON.stringify(solutionTicket)
                    }).done((result) => {
                        console.log("sukses Memberikan Solusi pada tiket");
                        Swal.fire({
                            icon: 'success',
                            title: 'Berhasil memberikan solusi pada Tiket',
                        }).then((result) => {
                            window.location.reload();
                        })

                    }).fail((error) => {
                        console.log(error);

                    });
                    //END OF AJAX RESPOND TIKET


                }).fail((error) => {
                    console.log(error);

                });
            //END OF AJAX OPEN TICKET

            }).fail((error) => {
                //alert pemberitahuan jika gagal
                console.log(error);

            });
            //END OF AJAX SET PRIORITY
        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Priority belum dipilih, silahkan dicek kembali',
                timer: 2000,
            })
        }
        //END OF CHECK PRIOIRTY = NAN


    }
    else {
        if (solutionTicket.Solution != "") {

            //START OF AJAX OPEN TICKET
            $.ajax({
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                type: "POST",
                url: "https://localhost:44376/API/Tickets/open",
                dataType: "json",
                data: JSON.stringify(OpenTicket)
            }).done((result) => {

                console.log("sukses Membuka tiket")
                //START OF AJAX RESPOND TIKET
                $.ajax({
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    type: "PUT",
                    url: "https://localhost:44376/API/Tickets/respond",
                    dataType: "json",
                    data: JSON.stringify(solutionTicket)
                }).done((result) => {
                    console.log("sukses Memberikan Solusi pada tiket");
                    Swal.fire({
                        icon: 'success',
                        title: 'Berhasil memberikan solusi pada Tiket',
                    }).then((result) => {
                        window.location.reload();
                    })

                }).fail((error) => {
                    console.log(error);

                });
                //END OF AJAX RESPOND TIKET


            }).fail((error) => {
                console.log(error);

            });
            //END OF AJAX OPEN TICKET

        }
        else {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Solusi belum diisi, silahkan dicek kembali',
                timer: 2000,
            })
        }
    }

    

};

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



