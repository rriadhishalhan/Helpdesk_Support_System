﻿
function addTicket() {
    let ticket = new Object();

    ticket.Customer_id = objSession.Id.toString();

    ticket.Issue = $("#inputIssue").val();

    ticket.Category_id = Number($("#inputCategories").val());

    console.log(ticket);

    $("#formInsertTicket").validate({
        rules: {

            Categories: {
                required: true,
            },
            Issue: {
                required: true,
            },
        },

        errorElement: 'span',
        errorPlacement: function (error, element) {
            error.addClass('invalid-feedback');
            element.closest('.form-group').append(error);
        },
        highlight: function (element, errorClass, validClass) {
            $(element).addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).removeClass('is-invalid');
        }
    });
    if ($("#formInsertTicket").valid() && !Number.isNaN(ticket.Category_id)) {

        $.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            type: "POST",
            url: "https://localhost:44376/API/Tickets/createTicket",
            dataType: "json",
            data: JSON.stringify(ticket)
        }).done((result) => {
            Swal.fire({
                icon: 'success',
                title: 'Data berhasil ditambahkan',
            }).then((result) => {
                window.location.reload();
            })

        }).fail((error) => {
            console.log(error);

        })
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Gagal menambahkan, silahkan dicek kembali',
            timer: 2000,
        })
    }

    
}

function KirimFeedback(ticketId) {
    console.log("Masuk ke kirim feedback");
    

    let feedbackTicket = new Object();
    feedbackTicket.Ticket_Id = ticketId;
    feedbackTicket.Feedback = $('#inputFeedback').val();
    console.log(feedbackTicket);

    //START OF AJAX FEEDBACK TIKET
    $.ajax({
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        type: "PUT",
        url: "https://localhost:44376/API/Tickets/customerFeedback",
        dataType: "json",
        data: JSON.stringify(feedbackTicket)
    }).done((result) => {
        console.log("sukses Memberikan Solusi pada tiket");
        Swal.fire({
            icon: 'success',
            title: 'Berhasil menambahkan tanggapan pada Tiket',
        }).then((result) => {
            window.location.reload();
        })

    }).fail((error) => {
        console.log(error);

    });
    //END OF AJAX FEEDBACK TIKET

}

function detailTicket(id, ticketId) {
    //console.log(ticketId)
    let textUrlDetail = "https://localhost:44376/API/Customers/" + id + "/tickets/" + ticketId +"/history";
    let textUrlGetTicket = "https://localhost:44376/API/Tickets/"  + ticketId ;
    console.log(textUrlDetail);

    $.ajax({
        type: "GET",
        url: textUrlDetail,
        data: {}
    }).done((result) => {
        $('#customerTikcetId').html('<h4 style="font-weight:bolder; line-height: 35px;">' + ticketId + '</h4>');

        //GET TICKET BY ID
        $.ajax({
            type: "GET",
            url: textUrlGetTicket,
            data: {}
        }).done((resultTicket) => {
            console.log("Masuk Ajax Get Ticket by ID nih ");
            $('#customerIssue').html('<h4 style="font-weight:bolder; line-height: 35px;">' + resultTicket.issue + '</h4>');



            var textTicketSolution = ``;
            var textTicketFeedback = ``;
            var textBtnFeedback = ``;
            if (resultTicket.solution != null) {
                textTicketSolution += `<Label>`;
                textTicketSolution += `Solusi dari kami`;
                textTicketSolution += `</Label>`;
                textTicketSolution += `<div class="col-12">`;
                textTicketSolution += `<h4 style="font-weight:bolder; line-height: 35px;white-space: pre-wrap;" >${resultTicket.solution}</h4>`;
                textTicketSolution += `</div>`;

                textTicketFeedback += `<Label>`;
                textTicketFeedback += `Tanggapan kamu`;
                textTicketFeedback += `</Label>`;
                textTicketFeedback += `<div class="col-12">`;
                textTicketFeedback += `<textarea id="inputFeedback" class="form-control" rows="3" ></textarea>`;
                textTicketFeedback += `</div>`;

                textBtnFeedback += `<button type="submit" class="btn btn-success" onclick="KirimFeedback('${ticketId}')" title="Anda akan mengirimkan Feedback ke Pegawai kami"> Kirim </button>`
            }
            $('#soulutionSection').html(textTicketSolution);
            $('#feedbackSection').html(textTicketFeedback);
            $('#btnGroupModal').html(textBtnFeedback);


            if (resultTicket.feedback != null) {
                console.log("masuk sini nih");
                textBtnFeedback = ``
                $('#btnGroupModal').html(textBtnFeedback);

                textTicketFeedback = ``;
                textTicketFeedback += `<Label>`;
                textTicketFeedback += `Tanggapan kamu`;
                textTicketFeedback += `</Label>`;
                textTicketFeedback += `<div class="col-12">`;
                textTicketFeedback += `<h4 style="font-weight:bolder; line-height: 35px;white-space: pre-wrap;" >${resultTicket.feedback}</h4>`;
                textTicketFeedback += `</div>`;
                $('#feedbackSection').html(textTicketFeedback);

            }

        }).fail((error) => {
            console.log(error);

        });




        var currentIndex = 1;
        var textTicketHistories = ``;

        console.log(result);
        //START OF LOOP TICKETHISTORIES
        result.reverse();
        console.log(result);

        $.each(result, function (key, val) {

            if (currentIndex == 1) {
                if (result[key].employee_position == null) {
                    textTicketHistories += `<tr>`;
                    textTicketHistories += `<td>${result[key].date}</td>`;
                    textTicketHistories += `<td><i class="fa fa-circle" style="color:#16ba0d"></i> </td>`;
                    textTicketHistories += `<td>${result[key].status}</td>`;
                    textTicketHistories += `</tr>`;
                }
                else {
                    textTicketHistories += `<tr>`;
                    textTicketHistories += `<td>${result[key].date}</td>`;
                    textTicketHistories += `<td><i class="fa fa-circle" style="color:#16ba0d"></i> </td>`;
                    textTicketHistories += `<td>${result[key].status} (${result[key].employee_position})</td>`;
                    textTicketHistories += `</tr>`;
                }
            }
            else {
                if (result[key].employee_position == null) {
                    textTicketHistories += `<tr>`;
                    textTicketHistories += `<td>${result[key].date}</td>`;
                    textTicketHistories += `<td><i class="fa fa-circle" ></i> </td>`;
                    textTicketHistories += `<td>${result[key].status} </td>`;
                    textTicketHistories += `</tr>`;
                }
                else {
                    textTicketHistories += `<tr>`;
                    textTicketHistories += `<td>${result[key].date}</td>`;
                    textTicketHistories += `<td><i class="fa fa-circle" ></i> </td>`;
                    textTicketHistories += `<td>${result[key].status} (${result[key].employee_position})</td>`;
                    textTicketHistories += `</tr>`;
                }
                
            }
            

            currentIndex += 1;

            //console.log(textTicketHistories);
        });

        $('#customerStatus').html('<h4 style="font-weight:bolder; line-height: 35px;">' + result[0].status + '</h4>');
        //END OF LOOP TICKETHISTORIES
        $('#ticketHistoriesTableCustomer').html(textTicketHistories);

    }).fail((err) => {
        console.log(err);
    });

}




//GET SESSION DATA FROM HTML//
let objSession = new Object();
objSession.Id = $("#hdnSessionId").data('value');
objSession.Name = $("#hdnSessionName").data('value');
objSession.Email = $("#hdnSessionEmail").data('value');
objSession.Role = $("#hdnSessionRole").data('value');
console.log(objSession);


$(document).ready(function () {

    //GET ALL TICKET FROM CUSTOMER ID

    $("#ticketDataTable").DataTable({
        responsive: true,
        dom: 'Bfrtip',
        "ajax": {
            "url": "https://localhost:44376/api/Customers/" + objSession.Id + "/Tickets/",
            "dataSrc": "",
        },
        "columns": [
            {
                "data": null, "sortable": false,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "ticket_Id" },
            { "data": "category" },
            { "data": "issue" },
            {
                "data": null,
                render: function (data, type, row) {
                    var btnDetail = `<button data-bs-toggle="modal" data-bs-target="#ModalTicketHistories" onclick="detailTicket( ${objSession.Id}, '${data.ticket_Id}' )" class="btn btn-info"  title="Show Detail"> <i class="fa fa-info" aria-hidden="true"></i></button>`;
                    
                    return btnDetail;
                }
            },

        ],
    });

    //$.ajax({
    //    type: "GET",
    //    url: "https://localhost:44376/api/Customers/" + objSession.Id+"/Tickets/",
    //    data: {}
    //}).done((result) => {

    //    var textTicket = ``;
    //    if (result.length != 0) {
    //        $.each(result, function (key, val) {
    //            textTicket += `<tr data-bs-toggle="modal" data-bs-target="#ModalTicketHistories" onclick="detailTicket( ${objSession.Id}, '${result[key].ticket_Id}' )">`;
    //            textTicket += `<th scope="row">${key + 1}</th>`;
    //            textTicket += `<td>${result[key].ticket_Id}</td>`;
    //            textTicket += `<td>${result[key].category}</td>`;
    //            textTicket += `<td>${result[key].issue}</td>`;
    //            textTicket += `</tr>`;
    //            console.log(textTicket);
    //        });
    //    } else {
    //        textTicket += `<tr>`;
    //        textTicket += `<td colspan= "4"> Belum ada tiket </td>`;
    //        textTicket += `</tr>`;

    //        console.log(textTicket);

    //    }
        
    //    $('#ticketTableCustomer').html(textTicket);
    //}).fail((err) => {
    //    console.log(err);
    //});

    //BUAT SELECT OPTION CATEGORIES
    $.ajax({
        type: "GET",
        url: "https://localhost:44376/API/Categories",
        data: {}
    }).done((result) => {
        var textRoles = `<option value="hide" style="display: none;">Pick Categories</option>`;
        $.each(result, function (key, val) {

            textRoles += `<option value="${result[key].id}">${result[key].name}</option>`;
        });
        $('#inputCategories').html(textRoles);
    }).fail((err) => {
        console.log(err);
    });



})



$('#customerShowStatusDetail').click(function () {
    $('#TableTrackHistories').slideDown(200);
    $('#customerShowStatusDetail').hide(0);
    $('#customerHideStatusDetail').show(0);
});
$('#customerHideStatusDetail').click(function () {
    $('#TableTrackHistories').slideUp(200);
    $('#customerShowStatusDetail').show(0);
    $('#customerHideStatusDetail').hide(0);
});

