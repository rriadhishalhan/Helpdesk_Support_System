
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

            var textTicketSolutionDanFeedback = ``;
            if (resultTicket.solution != null) {
                textTicketSolutionDanFeedback += `<span>`;
                textTicketSolutionDanFeedback += `Solusi dari kami`;
                textTicketSolutionDanFeedback += `</span>`;
                textTicketSolutionDanFeedback += `<div>`;
                textTicketSolutionDanFeedback += `<h4 style="font-weight:bolder; line-height: 35px;">${resultTicket.solution}</h4>`;
                textTicketSolutionDanFeedback += `</div>`;

            }
            $('#soulutionAndFeedbackSection').html(textTicketSolutionDanFeedback);


        }).fail((error) => {
            console.log(error);

        });




        var currentIndex = 1;
        var textTicketHistories = ``;
        //START OF LOOP TICKETHISTORIES
        $.each(result, function (key, val) {

            if (currentIndex == result.length) {
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
//$(document).ready(function () {
//    //BUAT SELECT OPTION ROLE
//    $.ajax({
//        type: "GET",
//        url: "https://localhost:44368/roles/getall",
//        data: {}
//    }).done((result) => {
//        var textRoles = `<option value="hide" style="display: none;">Pick role</option>`;
//        $.each(result, function (key, val) {

//            textRoles += `<option value="${result[key].id}">${result[key].jobRole}</option>`;
//        });
//        $('#inputRole').html(textRoles);
//    }).fail((err) => {
//        console.log(err);
//    });
//    //BUAT SELECT OPTION UNIVERSITAS
//    $.ajax({
//        type: "GET",
//        url: "https://localhost:44368/universities/getall",
//        data: {}
//    }).done((result) => {
//        var textUniversities = `<option value="hide" style="display: none;">Pick universities</option>`;
//        $.each(result, function (key, val) {

//            textUniversities += `<option value="${result[key].id}">${result[key].name}</option>`;
//        });
//        $('#inputUniversities').html(textUniversities);
//    }).fail((err) => {
//        console.log(err);
//    });

//    //BUAT COUNT GENDER
//    $.ajax({
//        type: "GET",
//        url: "https://localhost:44368/employees/GetCountGender",
//        data: {}
//    }).done((result) => {
        
//        var options = {
//            labels: [result[0].labels, result[1].labels],
//            series: [result[0].series, result[1].series],
//            colors: ['#2b5737','#691d57' ],
//            chart: {
//                height: 350,
//                type: 'donut',
//                width: '100%'
//            }
//        };

//        var chart = new ApexCharts(document.querySelector("#pieChartGender"), options);
//        chart.render();

//    }).fail((err) => {
//        console.log(err);
//    });

//    function getLabelsUniv(univs) {
//        var labels = [];

//        for (var i = 0; i < univs.length; i++) {
//            labels.push(univs[i].labels);
//        }

//        return labels;
//    }
//    function getSeriesUniv(univs) {
//        var series = [];
//        for (var i = 0; i < univs.length; i++) {
//            series.push(univs[i].series);

//        }
//        return series;
//    }

//    //BUAT COUNT UNIVERSITIES
//    $.ajax({
//        type: "GET",
//        url: "https://localhost:44368/employees/GetCountUniversities",
//        data: {}
//    }).done((result) => {

//        var options = {
//            xaxis: {
//                categories: getLabelsUniv(result),
//                tickAmount: 1
//            },
//            series: [{
//                name: [
//                    "People count"
//                ],
//                data: getSeriesUniv(result)
//            }],
//            plotOptions: {
//                bar: {
//                    borderRadius: 4,
//                    horizontal: true,
//                }
//            },

//            //colors: ['#2b5737', '#691d57'],
//            chart: {
//                height: 350,
//                type: 'bar',
//                width: '100%'
//            },
            
//        };

//        var chart = new ApexCharts(document.querySelector("#chartUniversities"), options);
//        chart.render();

//    }).fail((err) => {
//        console.log(err);
//    });


//    //BUAT DATATABLE
//    $("#employeeDatatable").DataTable({
//        responsive: true,
//        dom: 'Bfrtip',
//        buttons: [
//            {
//                extend: 'excel',
//                exportOptions: {
//                    columns:[0,1,2,3,4,5]
//                }
//            }
//        ],
//        "ajax": {
//            "url": "https://localhost:44368/employees/GetAllProfile",
//            "dataSrc":"",
//        },
//        "columns": [
//            //{
//            //    "data": null, "sortable": false,
//            //    render: function (data, type, row, meta) {
//            //        return meta.row + meta.settings._iDisplayStart + 1;
//            //    }
//            //},
//            { "data": "nik" },
//            { "data": "fullname" },
//            { "data": "role" },
//            { "data": "email" },
//            { "data": "phone" },
//            { "data": "gender" },
//            {
//                "data": null,
//                render: function (data, type, row) {
//                    var btnDetail = `<button onclick="detailEmployee(${data.nik})" data-target="#DetailDataModal" data-toggle="modal" class="btn btn-info"  title="Show Detail"> <i class="fa fa-info" aria-hidden="true"></i></button>`;
//                    var btnUpdate = `<button onclick="dataUpdate(${data.nik})" data-target="#UpdateDataModal"  data-toggle="modal" class="btn btn-warning" title="Update Data" style="color:white"> <i class="fa fa-pencil-square-o" aria-hidden="true"></i></button >`;
//                    var btnDelete = `<button onclick="deleteEmployee(${data.nik})" class="btn btn-danger"  title="Delete"> <i class="fa fa-trash-o" aria-hidden="true"></i></button >`;
//                    return btnDetail+` ` + btnUpdate+` ` +btnDelete;
//                }
//            },

//        ],
        
//    });
//})
