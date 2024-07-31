
// *********************************************************** For Loader ***********************************************************
function showLoader() {
    document.querySelector(".loader-container").style.display = "flex";
    document.querySelector(".backdrop").style.display = "flex";
}

function hideLoader() {
    document.querySelector(".loader-container").style.display = "none";
    document.querySelector(".backdrop").style.display = "none";
}

hideLoader();

// *********************************************************** Get Dashboard data ***********************************************************
function GetDashboard(status) {
    event.preventDefault();

    showLoader();

    if (status == 0) {
        stauts = $('#statusForName').val();
    }

    console.log(status);

    $.ajax({
        methd: "GET",
        url: "/AdminSide/GetDashboard",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#home-tab-pane').html(result);

            if (status != 1) {
                $('#new-tab').removeClass("active");
            }
            if (status == 2) {
                $('#Pending-tab').addClass("active");
            }
            if (status == 4) {
                $('#Active-tab').addClass("active");
            }
            if (status == 6) {
                $('#Conclude-tab').addClass("active");
            }
            if (status == 3) {
                $('#ToClose-tab').addClass("active");
            }
            if (status == 9) {
                $('#Unpaid-tab').addClass("active");
            }
            //console.log("success in getting dashbord");
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

// *********************************************************** Get Table data on Status  ***********************************************************
function TableRecords(status, requesttypeid, regionid) {
    console.log(status);
    console.log(typeof (status));

    $.ajax({
        method: "POST",
        url: "/AdminSide/TableRecords",
        data: { status: status, requesttypeid: requesttypeid, regionid: regionid },

        success: function (result) {
            $('#tableRecords').html(result);
        },

        //statuscode: {
        //    401: function () { 
        //        alert('failed to authenticate user');

        //    }
        //},

        error: function () {
            alert("eroor loading partial view")
        }

    })
}

// *********************************************************** filter Table data on requesttypeid and regionid  ***********************************************************
function FilterTableRecords(status, requesttypeid, regionid) {

    if (requesttypeid == null) {
        requesttypeid = $('#reqTypeValueId').val();
    }
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    $.ajax({
        method: "POST",
        url: "/AdminSide/FilterTableRecords",
        data: { status: status, requesttypeid: requesttypeid, regionid: regionid },

        success: function (result) {
            $('#requestTable').html(result);
        },

        error: function () {
            alert('Error loading partial view');
        }
    });
}

// ************************** Request DTY Support **********************
function RequestSupport() {
    $.ajax({
        method: "GET",
        url: "/AdminSide/RequestSupport",

        success: function (result) {           
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#requestSupportModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function requestDTYPost() {
    event.preventDefault();

    if ($('#requestDTYFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/RequestDTYPost",
            data: $('#requestDTYFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#requestSupportModalId').modal('hide');
                Swal.fire("Hurrah", "Requested Support", "success");
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

// ****************************Get Send Link Model*******************************
function sendLinkModal(status,callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/sendLinkModal",
        data: { status: status, callId: callId },

        success: function (result) { 
            if (result.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 3) {
                    $('#showProviderCaseModal').html(result);
                }
                else{
                    $('#showCaseModal').html(result);
                }
            }
            $('#sendLinkModal').modal('show');
        },

        error: function () {
            alert("error showing send link modal");

        }
    });
}

// ****************************Post Send Lik Model Of Dashboard*******************************
function SendSubmitLink(status,callId) {
    event.preventDefault();
    if ($('#sendLinkFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SendLink",
            data: $('#sendLinkFormId').serialize(),

            success: function (result) {
                $('#sendLinkModal').modal('hide');
              
                Swal.fire({
                    title: "Hello Doc",
                    text: "Link Sent!",
                    icon: "success",
                });
                if (callId == 3) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard(status);
                }
            },

            error: function () {
                alert("error sending send link");
            }
        });
    }
}

// **************************** Get Create request by admin Page*******************************
function adminCreateRequest(status,callId) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/CreateRequestByAdmin",
        data: { status: status, callId: callId },

        success: function (response) {      
            if (response.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 3) {
                    $('#provider-home-tab-pane').html(response); 
                }
                else {
                    $('#home-tab-pane').html(response);
                }
            }  
        },

        error: function () {
            alert("error showing create request");
        }
    });
}

// **************************** Post Create request by admin Page*******************************
function sendAdminCreateRequest(status,callId) {
    event.preventDefault();

    if ($('#AdminCreateRequestFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/sendAdminCreateRequest",
            data: $('#AdminCreateRequestFormId').serialize(),

            success: function () {
                //console.log("send create request");
                Swal.fire({
                    title: "Hello Doc",
                    text: "Request Created!",
                    icon: "success",
                });
                if (callId == 3) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard(status);
                }
            },
            error: function () {
                alert("error in sending create request");
            }
        });
    }
}

// ****************************Check Region is Available or not On Create Request page*******************************
function checkRegionAvailability() {
    event.preventDefault();
    var region = $('#regionId').val();

    $.ajax({
        method: "POST",
        url: "/AdminSide/checkRegionAvailability",
        data: { region: region },

        success: function (result) {
            if (result == true) {
                Swal.fire({
                    title: "Hallo Doc",
                    text: "Region is available!",
                    icon: "success",
                });
            }
            else {
                Swal.fire({
                    title: "Hallo Doc",
                    text: "Please select region!",
                    icon: "error",
                });
                $('#regionValue').val("");
            }
        },
        error: function () {
            alert("error in check region")
        }
    });
}

// **************************** Export and Export All Requests*******************************
function Export(arr, requesttypeid) {

    var regionid = $('#regionId').val();

    requesttypeid = $('#reqTypeValueId').val();
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    var arr2 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

    if (JSON.stringify(arr) == JSON.stringify(arr2)) {
        regionid = 0;
        requesttypeid = null;
    }

    $.ajax({
        method: "POST",
        url: "/AdminSide/Export",
        data: { arr: arr, requesttypeid: requesttypeid, regionid: regionid },
        xhrFields: {
            responseType: 'blob'
        },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }

            var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'RequestData.xlsx';

            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(link.href);

            hideLoader();
            Swal.fire("Hurrah", "File Downloaded", "success");
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

// **************************** Get VieCase Records Page*******************************
function ViewCaseRecords(status, requestid,callId) {
    console.log(requestid);
    $.ajax({
        method: "GET",
        url: "/AdminSide/ViewCaseRecords",
        data: { status: status, requestid: requestid, callId: callId },
        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 1) {
                    $('#home-tab-pane').html(response);
                }
                else if (callId == 2) {
                    $('#Patient-tab-pane').html(response);
                }
                else {
                    $('#provider-home-tab-pane').html(response);
                }

            }
        },
        error: function () {
            alert('Error loading partial view');
        }
    });
}

// **************************** Update Data VieCaseRecords Page*******************************
function UpdateViewCaseRecords(callId) {
    event.preventDefault();
    if ($('#viewCaseDataForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateViewCaseRecords",
            data: $('#viewCaseDataForm').serialize(),

            success: function () {
                /* console.log("success");  */
                Swal.fire({
                    title: "Good job!",
                    text: "Case Updated!",
                    icon: "success"
                });
            },

            error: function () {
                console.log("error")
            }
        });

        document.querySelectorAll('.input-disable ').forEach(function (element) {
            element.disabled = true;
        });
    }

    document.getElementById('submitBtn').style.display = 'none';
    document.getElementById('editBtn').style.display = 'block';
}

// **************************** Get ViewNotes Page*******************************
function ViewNotes(requestid,status,callId) {
    console.log(requestid);
    console.log(status);
    $.ajax({
        method: "GET",
        url: "/AdminSide/ViewNotes",
        data: { requestid: requestid, status: status,callId : callId },
        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 3) {
                    $('#provider-home-tab-pane').html(response);
                    
                }  
                else if (callId == 2) {
                    $('#Patient-tab-pane').html(response);
                }
                else {
                    $('#home-tab-pane').html(response);
                }

            }
        },

        error: function () {
            alert('Error to get viewnots page');
        }

    });

}

// **************************** Update Data On ViewNotes Page*******************************
function UpdateViewNotes() {
    event.preventDefault();

    if ($('#viewNotesForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateViewNotes",
            data: $('#viewNotesForm').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "Good job!",
                    text: "Notes Submitted!",
                    icon: "success"
                });
                ViewNotes(result.reqId, result.status, result.callId);
            },

            error: function () {
                console.log("error");
            }
        });
    }
}

// **************************** Get CancelCase Model*******************************
function CancelModal(requestid,status) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/CancelCase",
        data: { requestid: requestid, status: status },

        success: function (response) {
            console.log("success in cancel modal");
            $('#showCaseModal').html(response);
            $('#cancelModalId').modal('show');
        },

        error: function () {
            console.log("error in cancel modal");
        }
    });
}

// **************************** Post CancelCase Model*******************************
function CancelCase() {
    event.preventDefault();

    var status = $('#statusForName').val();
    if ($('#cancelCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/CancelCase",
            data: $('#cancelCaseForm').serialize(),

            success: function () {
                $('#cancelModalId').modal('hide');
                Swal.fire({
                    title: "Good job!",
                    text: "Case Canceled!",
                    icon: "success"
                });
                GetDashboard(status);
            },

            error: function () {
                console.log("error in cancel case");
            }
        });
    }
}

// **************************** Get AssignCase Model*******************************
function AssignModel(requestid,status) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/AssignCase",
        data: { requestid: requestid , status : status },

        success: function (result) {
            console.log("success in asign modal");
            $('#showCaseModal').html(result);
            $('#assignModalId').modal('show');
        },

        error: function () {
            console.log("error in asign modal");
        }
    });
}

// **************************** Filter Physicians On Assign Case Model*******************************
function FilterAssignModel(regionid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/FilterAssignCase",
        data: { regionid: regionid },

        success: function (result) {
            console.log(result.success);
            $("#physicianId").empty();
            $("#physicianId").append($("<option></option>").attr("value", "").text("Select Physician").prop("disabled", true).prop("selected", true));
            result.success
                .forEach(obj => {
                    $('#physicianId').append($("<option></option>").attr("value", obj.physicianId).text(obj.firstName + " " + obj.lastName))
                });
        },

        error: function () {
            console.log("error in filter asign modal");
        }
    });
}

// **************************** Post AssignCase Model*******************************
function AssignCase() {
    event.preventDefault();

    var status = $('#statusForName').val();
    if ($('#asignCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/AssignCase",
            data: $('#asignCaseForm').serialize(),

            success: function () {
                $('#assignModalId').modal('hide');
                Swal.fire({
                    title: "Good job!",
                    text: "Case Assigned!",
                    icon: "success"
                });
                GetDashboard(status);
            },

            error: function () {
                console.log("error in asign case");
            }
        });
    }
}

// **************************** Get BlockCase Model*******************************
function BlockModal(requestid ,status ) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/BlockCase",
        data: { requestid: requestid, status : status },

        success: function (response) {
            console.log("success in block modal");
            $('#showCaseModal').html(response);
            $('#blockModalId').modal('show');
        },

        error: function () {
            console.log("error in block modal");
        }
    });
}

// **************************** Post BlockCase Model*******************************
function BlockCase(requestId) {
    event.preventDefault();

    var status = $('#statusForName').val();
    if ($('#blockCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/BlockCase",
            data: $('#blockCaseForm').serialize(),

            success: function (result) {
                $('#blockModalId').modal('hide');
                Swal.fire({
                    title: "Good job!",
                    text: "Case Blocked!",
                    icon: "success"
                });
                GetDashboard(status);
            },

            error: function () {
                console.log("error in block case");
            }
        });
    }
}

// **************************** Get ViewUploads Page*******************************
function ViewUploads(requestid, status, callId) {
    console.log(requestid);
    console.log(status);
    console.log(callId)
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/ViewUploads",
        data: { requestid: requestid, status: status, callId: callId },
        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 1) {
                    $('#home-tab-pane').html(response);
                }
                else if (callId == 2) {
                    $('#Patient-tab-pane').html(response);
                }
                else {
                    $('#provider-home-tab-pane').html(response);

                }  

            }
        },

        error: function () {
            alert('error fetching view upload page');
        }

    });

}

// **************************** Update Data On ViewUploads Page*******************************
function UpdateViewUploads(requestid, status,callId) {

    var formdata = new FormData($('#updateForm')[0]);
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/AdminSide/UploadDocument",
        data: formdata,
        processData: false,
        contentType: false,
        success: function (result) {
            // console.log("success in upload");
            Swal.fire({
                title: "Good job!",
                text: "Document Uploaded!",
                icon: "success"
            });
            ViewUploads(result, status,callId)
        },

        error: function () {
            console.log("error in upload");
        }

    });

}

// **************************** Delete File On ViewUploads Page*******************************
function DeleteFile(requestwisefileid, requestid, status,callId) {
    console.log(requestwisefileid);
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/DeleteFile",
        data: { requestwisefileid: requestwisefileid, requestid: requestid, status: status, callId: callId },

        success: function (result) {
            Swal.fire({
                title: "Good job!",
                text: "Files Deleted!",
                icon: "success"
            });
            ViewUploads(result.reqId, result.status, callId);
        },

        error: function () {
            console.log("eror in deleting document");
        }


    });
}

// **************************** Send File via Mail From ViewUploads Page*******************************
function SendFile(requestwisefileid, requestid, status, callId) {
    console.log(requestwisefileid);
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/SendFile",
        data: { requestwisefileid: requestwisefileid, requestid: requestid, status: status, callId: callId },
        traditional: true,
        success: function (result) {
            Swal.fire({
                title: "Good job!",
                text: "Mail Sent!",
                icon: "success"
            });
            ViewUploads(result.reqId, result.status, callId);
            //console.log('succes in sending mail')
        },

        error: function () {
            alert("eror in sending document");
        }


    })
}

// **************************** Get Send Order Page*******************************
function SendOrder(requestid,status,callId) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/SendOrder",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 3) {
                    $('#provider-home-tab-pane').html(response);

                }
                else {
                    $('#home-tab-pane').html(response);
                } 

            }
        },

        error: function () {
            alert('Error loading send Order partial view');
        }
    });
}

// **************************** Filter Vendors(HealthProfessionals) On SendOrder Page*******************************
function FilterSendOrder(health_professional_id) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/FilterSendOrder",
        data: { health_professional_id: health_professional_id },

        success: function (result) {
            console.log(result.success);
            $("#healthprofessionid").empty();
            $("#healthprofessionid").append($("<option></option>").attr("value", "").text("Business").prop("disabled", true).prop("selected", true));
            result.success
                .forEach(obj => {
                    $('#healthprofessionid').append($("<option></option>").attr("value", obj.vendorId).text(obj.vendorName))
                });
        },

        error: function () {
            console.log("error in filter vendor");
        }
    });
}

// **************************** Get Vendors Data On SendOrder Page*******************************
function VendorData(vendorid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/VendorData",
        data: { vendorid: vendorid },

        success: function (result) {
            console.log(result.success);
            $("#floatingEmail").val(result.success.email);
            $("#floatingcontect").val(result.success.businessContact);
            $("#floatingFaxNum").val(result.success.faxNum);
        },

        error: function () {
            alert('Error loading ajax');
        }
    });
}

// **************************** Update Data On SendOrder page*******************************
function UpdateSendOrder(requestId) {
    event.preventDefault();
    var status = $('#statusForName').val();
    var callId = $('#orderCallId').val();

    if ($('#sendOrderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SendOrder",
            data: $('#sendOrderForm').serialize(),

            success: function (response) {
                /* console.log("success in send order ");*/
                Swal.fire({
                    title: "Good job!",
                    text: "Order Sent!",
                    icon: "success"
                });
                if (callId == 3) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard(status);
                }
            },

            error: function () {
                console.log("error in send order ");
            }
        });
    }
}

// **************************** Get TransferCase Model*******************************
function TransferModel(requestid,status) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/TransferCase",
        data: { requestid: requestid, status : status },

        success: function (result) {
            console.log("success in transfer modal");
            $('#showCaseModal').html(result);
            $('#transferModalId').modal('show');
        },

        error: function () {
            console.log("error in transfer modal");
        }
    });
}

// **************************** Filter Physicians on TransferCase Model*******************************
function FilterTransferModel(regionid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/FilterTransferCase",
        data: { regionid: regionid },

        success: function (result) {
            console.log(result.success);
            $("#physicianId").empty();
            $("#physicianId").append($("<option></option>").attr("value", "").text("Select Physician").prop("disabled", true).prop("selected", true));
            result.success
                .forEach(obj => {
                    $('#physicianId').append($("<option></option>").attr("value", obj.physicianId).text(obj.firstName + " " + obj.lastName))
                });
        },

        error: function () {
            console.log("error in filter transfer modal");
        }
    });
}

// **************************** Post Data Of TransferCase Model*******************************
function TransferCaseAdmin(requestId) {
    event.preventDefault();
    var status = $('#statusForName').val();
    if ($('#transferCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/TransferCase",
            data: $('#transferCaseForm').serialize(),

            success: function (response) {
                $('#transferModalId').modal('hide');
                Swal.fire({
                    title: "Good job!",
                    text: "Case Transfered!",
                    icon: "success"
                });
                GetDashboard(status);
            },

            error: function () {
                console.log("error in transfer case");
            }
        });
    }
}

// **************************** Get ClearCase Model*******************************
function ClearModel(requestid, status) {
    console.log(status);
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/ClearCase",
        data: { requestid: requestid, status: status },

        success: function (response) {
            console.log("success in clear modal");
            $('#showCaseModal').html(response);
            $('#clearModalId').modal('show');
        },

        error: function () {
            console.log("error in clear modal");
        }
    });
}

// **************************** Post Data of ClearCase Model*******************************
function ClearCase() {
    event.preventDefault();
    var status = $('#statusForName').val();
    if ($('#clearCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/ClearCase",
            data: $('#clearCaseForm').serialize(),

            success: function (result) {
                $('#clearModalId').modal('hide');
                Swal.fire({
                    title: "Good job!",
                    text: "Case Cleared!",
                    icon: "success"
                });
                GetDashboard(status);
            },

            error: function () {
                console.log("error in clear modal");
            }
        });
    }
}

// **************************** Get SendAgreeMent Model*******************************
function SendAgreementModel(requestid, status, callId) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/SendAgreement",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (response) {
            if (response.code == 401) {
                location.reload();
            }
            if (callId == 3) {
                $('#showProviderCaseModal').html(response);
            }
            else {
                $('#showCaseModal').html(response);
            }            
            $('#sendAgreeModalId').modal('show');
        },

        error: function () {
            console.log("error in SendAgreement modal");
        }
    });
}

// **************************** Post Data Of SendAgreeMent Model*******************************
function SendLink(callId) {
    if ($('#sendAgreementForm').valid()) {
        event.preventDefault();

        var status = $('#statusForName').val();
        $.ajax({
            method: "POST",
            url: "/AdminSide/sendMailLink",
            data: $('#sendAgreementForm').serialize(),

            success: function () {
                $('#sendAgreeModalId').modal('hide');
                Swal.fire({
                    title: "Good job!",
                    text: "Successfully Sent Agreement!",
                    icon: "success"
                });
                if (callId == 3) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard(status);
                }
            },
            error: function () {
                console.log('eror in sending mail');
            }
        })
    }
}

// **************************** Get Encounter Page*******************************
function Encounter(requestid,status,callId) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/Encounter",
        data: { requestid: requestid, status: status,callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 3) {
                $('#provider-home-tab-pane').html(result);
            }
            else if (callId == 2) {
                $('#Patient-tab-pane').html(result);
            }
            else {
                $('#home-tab-pane').html(result);
            }
        },

        error: function () {
            alert('Error loading partial view of Encounterform');
        }
    });
}

// **************************** Update Data On Encounter Page*******************************
function UpdateEncounter() {
    event.preventDefault();
    console.log($('#statusForName').val());
    var formData = $('#encounterForm').serializeArray();
    formData.push({ name: 'requestId', value: $('#reqid').val() });
    formData.push({ name: 'StatusForName', value: $('#statusForName').val() });
    formData.push({ name: 'CallId', value: $('#callId').val() });
    $.ajax({
        method: "POST",
        url: "/AdminSide/Encounter",
        data: formData,

        success: function (result) {
            /*console.log("success");*/
            Swal.fire({
                title: "Good job!",
                text: "Encounter Form Updated!",
                icon: "success"
            });
            Encounter(result.reqId, result.status,result.callid);
            
        },

        error: function () {
            console.log("error in update encounter form")
        }
    });
    $('#savebtn2').on('click', function (e) {
        e.preventDefault();
        $('.editfield2').attr('disabled', true);
        $(this).hide();
        $('#savebtn2').show();
        $('#cancelbtn2').hide();

    });

}

// **************************** Get CloseCase Page*******************************
function CloseCase(requestid , status) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/CloseCase",
        data: { requestid: requestid, status: status },

        success: function (response) {
            $('#home-tab-pane').html(response);
        },

        error: function () {
            alert('Error loading partial view of CloseCaseForm');
        }
    });
}

// **************************** Update Details On CloseCase Page*******************************
function UpdateCloseCase() {
    event.preventDefault();

    if ($('#closeCaseForm').valid()) {
        $("#closeCaseForm input,textarea,select").prop("disabled", false);
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateCloseCase",
            data: $('#closeCaseForm').serializeArray(),

            success: function (result) {  
                Swal.fire({
                    title: "Good job!",
                    text: "Case Updated!",
                    icon: "success"
                });
                CloseCase(result.reqId, result.status);

            },

            error: function () {
                alert("error in close case update")
            }
        });
    }
}

// **************************** Post  CloseCase Page*******************************
function PostCloseCase(requestId, status) {
    $.ajax({
        method: "POST",
        url: "/AdminSide/CloseCase",
        data: { requestId: requestId },

        success: function () {
            //console.log("success in post close case"); 
            Swal.fire({
                title: "Good job!",
                text: "Case Closed!",
                icon: "success"
            });
            GetDashboard(status);
        },
        error: function () {
            alert("error in post close case");
        }
    });
}


function SendRequestMail() {
    console.log($('#sendsubmitReqForm').serialize())
    event.preventDefault();
    if ($('#sendsubmitReqForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SendRequestMail",
            data: $('#sendsubmitReqForm').serialize(),

            success: function () {
                console.log('succes in sending mail')
                window.location.reload();

            },

            error: function () {
                console.log('eror in sending mail');
            }


        })
    }

}

// **************************** Get AccountAccess Page*******************************
function GetAccountAccess() {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetAccountAccess",
        success: function (result) {  

             $('#Account-tab-pane').html(result);
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// **************************** Get CreateAccess Page*******************************
function GetCreateAccess() {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetCreateAccess",
        success: function (result) {           
            $('#Account-tab-pane').html(result);
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// **************************** Filters  RoleMenu******************************
function FilterRolesMenu(accounttype) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/AdminSide/FilterRolesMenu",
        data: { accounttype: accounttype },
        success: function (result) {
            $('#RolesMenuList').empty();
            result.forEach(obj => {
                $('#RolesMenuList').append(`<div class='form-check form-check-inline px-2 mx-3 my-2 col-auto'><input class='form-check-input d2class' name='AccountMenu' type='checkbox' id='${obj.menuId}' value='${obj.menuId}'/><label class='form-check-label' for='${obj.menuId}'>${obj.name}</label></div>`)
            });
           
        },
        error: function () {
            console.log("Error");
        }
    });
}

// **************************** Filter Edit RolesMenu Page*******************************
function FilterEditRolesMenu(accounttypeid, roleid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/FilterEditRolesMenu",
        data: { accounttypeid: accounttypeid, roleid: roleid },
        success: function (result) {
                $('#EditRoleMenu').html(result);
        },
        error: function () {
            console.log("Error");
        }
    });
}

// **************************** Post CreateAccess Page Data*******************************
function SetCreateAccessAccount() {
    event.preventDefault();
    console.log($('#AccountCreateForm').serialize())
    if ($('#AccountCreateForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SetCreateAccessAccount",
            data: $('#AccountCreateForm').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "HalloDoc",
                    text: "Role Created Successfully",
                    icon: "success",
                    timer: 1500,
                    timerProgressBar: true
                });
                GetAccountAccess();
            },

            error: function () {
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }
        })
    }
}

// ****************************Get Edit Access Page*******************************
function GetEditAccess(accounttypeid, roleid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetEditAccess",
        data: { accounttypeid: accounttypeid, roleid: roleid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// **************************** Post Data Of Edit Access Page*******************************
function SetEditAccessAccount(accounttypeid, roleid) {
    event.preventDefault();

    if ($('#AccountEditForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SetEditAccessAccount",
            data: $('#AccountEditForm').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "HalloDoc", text: "Role Edited Successfully", icon: "success", timer: 1500, timerProgressBar: true
                });

                GetEditAccess(accounttypeid, roleid);
            },



            error: function () {
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }
        })
    }
}

// **************************** Delete Account Acess*******************************
function DeleteAccountAccess(roleid) {

    $.ajax({
        method: "POST",
        url: "/AdminSide/DeleteAccountAccess",
        data: { roleid: roleid },

        success: function (result) {
            Swal.fire({
                title: "HalloDoc", text: "Role Deleted Successfully", icon: "success", timer: 1500, timerProgressBar: true
            });
            GetAccountAccess();
        },
            
        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    })

}

// **************************** Get UserAccess Page*******************************
function GetUserAccess(accountTypeId) {

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetUserAccess",
        data: { accountTypeId: accountTypeId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#User-tab-pane').html(result);
                $('#roleValue').val(accountTypeId);
                if (accountTypeId == 1) {
                    $('#createAdmin').removeClass('d-none');
                }
                if (accountTypeId == 2) {
                    $('#createProvider').removeClass('d-none');
                }
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}


// **************************** Get Schedulig Page*******************************
function GetScheduling(callId) {

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetScheduling",
        data: { callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                if (callId == 3) {
                    $('#provider-schedule-tab-pane').html(result);
                }
                else {
                    $('#Scheduling-tab-pane').html(result);
                }
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// **************************** Get Add New Shift Page(Modal)*******************************
function CreateNewShift(callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/CreateNewShift",
        data: { callId: callId },

        success: function (res) {
            if (res.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                $('#schedulingModals').html(res);
                $('#createShiftModal').modal('show');
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// ****************************Post Data Of Add New Shift Page(Modal)*******************************
function createShiftPost(callId) {
    event.preventDefault();
    var formdata = $('#createShiftForm').serialize();
    if ($('#createShiftForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/createShiftPost",
            data: formdata,

            success: function (res) {
                if (res.code == 401) {
                    setTimeout(function () { location.reload(); }, 2000);
                    Swal.fire("Oops!", "Session Expired !!!", "error");
                } else {
                    if (res) {
                        $('#createShiftModal').modal('hide');
                        Swal.fire({
                            title: "HalloDoc", text: "Shift Created Succesfully ", icon: "success"
                        });
                        GetScheduling(callId);
                    }
                    else {
                        Swal.fire({
                            title: "HalloDoc", text: "This Time Slot is not Available", icon: "error"
                        });
                    }
                }
            },

            error: function () {
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }
        });
    }
}

// ****************************Get Shift On Review Page*******************************
function ShiftReview(regionId, callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/ShiftReview",
        data: { regionId: regionId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                $('#Scheduling-tab-pane').html(result);
                $('#regionValue').val(regionId);
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// ****************************Post Data By cliking on Approve Shift Button*******************************
function ApproveShift(shiftDetailsId, regionId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/AdminSide/ApproveShift",
        data: { shiftDetailsId: shiftDetailsId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                if (shiftDetailsId.length === 0) {
                    Swal.fire("Oops!", "Please Select Any Shift To Proceed Ahead", "error");
                }
                else {
                    Swal.fire("Hallo Doc", "Shifts Approved!!", "success");
                    ShiftReview(regionId);
                }
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// ****************************Post Data By clicking on Delete Selected Button*******************************

function DeleteSelectedShift(shiftDetailsId, regionId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/AdminSide/DeleteSelectedShift",
        data: { shiftDetailsId: shiftDetailsId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                if (shiftDetailsId.length === 0) {
                    Swal.fire("Oops!", "Please Select Any Shift To Proceed Ahead", "error");
                }
                else {
                    Swal.fire("Hallo Doc", "Shifts Deleted!!", "success");
                    ShiftReview(regionId);
                }
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// ****************************Get Providers On Call Page*******************************
function MDOnCall(regionId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/MDOnCall",
        data: { regionId: regionId },

        success: function (result) {
            if (result.code == 401) {
                setTimeout(function () { location.reload(); }, 2000);
                Swal.fire("Oops!", "Session Expired !!!", "error");
            } else {
                $('#Scheduling-tab-pane').html(result);
                $('#MDsRegion').val(regionId);
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// ****************************Get ProviderLocation Page*******************************
function ProviderLocation() {

    $.ajax({
        method: "GET",
        url: "/AdminSide/ProviderLocation",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#provider-location-tab-pane').html(result);
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

// ****************************Get Partners Page*******************************
function Partners(professionid) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/Partners",
        data: { professionid: professionid },
        success: function (result) {
            $("#Partners-tab-pane").html(result);
            if (professionid != 0) {
                $(".ProfessionsDropdown").val(professionid);
            }
        },

        error: function () {
            alert("Error loading partial view");
        },
    });
}

// ****************************Get Create/Edit Business Page*******************************
function AddBusiness(vendorID) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/AddBusiness",
        data: { vendorID: vendorID },
        success: function (result) {
            $("#Partners-tab-pane").html(result);
        },

        error: function () {
            alert("Error loading partial view");
        },
    });
}

// ****************************Post Data On Create Business Page*******************************
function CreateNewBusiness() {
    event.preventDefault();

    if ($("#SubmitInfoForm").valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/CreateNewBusiness",
            data: $("#SubmitInfoForm").serialize(),

            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        title: "Hallo Doc",
                        text: "New Business Added!",
                        icon: "success",
                    });
                    Partners(0);
                } else {
                    Swal.fire({
                        title: "Hallo Doc",
                        text: "Business Already Exists!",
                        icon: "error",
                    });
                }
            },

            error: function () {
                alert("error in post create account");
            },
        });
    }
}

// ****************************Update Data On Edit Business Page*******************************
function UpdateBusiness() {
    event.preventDefault();

    if ($("#SubmitInfoForm").valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateBusiness",
            data: $("#SubmitInfoForm").serialize(),

            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        title: "Hallo Doc",
                        text: "Business Data Updated!",
                        icon: "success",
                    });
                    AddBusiness(result.vendorid);
                } else {
                    Swal.fire({
                        title: "Hallo Doc",
                        text: "Please Update Any One Field",
                        icon: "error",
                    });
                }
            },

            error: function () {
                alert("error in post create account");
            },
        });
    }
}

// ****************************Delete Venor/HealthProfessional/Partner*******************************

function DelPartner(VendorID) {
    $.ajax({
        method: "POST",
        url: "/AdminSide/DelPartner",
        data: { VendorID: VendorID },
        success: function () {
            Swal.fire({
                title: "HalloDoc",
                text: "Business Partner Deleted Successfully",
                icon: "success",
                timer: 1500,
                timerProgressBar: true,
            });
            Partners(0);
        },
        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        },
    });
}

// *********************************************************** Records Tab ***********************************************************

// ****************************Get Patient History Page*******************************
function GetRecordsTab() {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetRecordsTab",
        data: $('#patientRecordsform').serialize(),

        success: function (res) {
            $('#Patient-tab-pane').html(res);
        },

        error: function () {
        }
    });
}

// ****************************Filter Records On Patient History Page*******************************
function patientRecordsFilter() {
    event.preventDefault();
    var formdata = $('#patientRecordsform').serialize();

    $.ajax({
        method: "POST",
        url: "/AdminSide/GetRecordsTab",
        data: formdata,

        success: function (res) {

            $('#Patient-tab-pane').html(res);

        },
        error: function () {

        }
    });

}

// ****************************Get Explore Page*******************************
function GetPatientRecordExplore(UserId) {

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetPatientRecordExplore",
        data: { UserId: UserId },

        success: function (res) {
            $('#Patient-tab-pane').html(res);
        },

        error: function () {
        }
    });
}

// ****************************Get Search Records Page*******************************
function GetSearchRecords() {
    var formdata = $('#searchRecordsForm').serialize();

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetSearchRecords",
        data: formdata,

        success: function (res) {
            $('#Search-tab-pane').html(res);
        },

        error: function () {
        }
    });
}

// ****************************Fiter Records On Search Records Page*******************************
function searchRecordsFilter(callId) {
    event.preventDefault();

    var formData;

    if (callId != 1) {
        formData = $('#searchRecordsForm').serialize();
    }

    $.ajax({
        method: "POST",
        url: "/AdminSide/GetSearchRecords",
        data: formData,

        success: function (response) {
            $('#Search-tab-pane').html(response);
        },
        error: function () {
            console.log('error');
        }
    });

}

// ****************************Export Data In Excel Of SearRecords Data Page*******************************
function ExportSearchRecords() {
    $.ajax({
        method: "POST",
        url: "/AdminSide/ExportSearchRecords",
        data: $('#searchRecordsForm').serialize(),
        xhrFields: {
            responseType: 'blob'
        },

        success: function (data) {

            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'Requests.xlsx';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
        error: function () {
            alert('Error exporting data');
        }
    });


}

// ****************************Delete Record Page*******************************
function deletRequest(requestId) {

    $.ajax({
        method: "GET",
        url: "/AdminSide/deletRequest",
        data: { requestId: requestId },

        success: function (res) {
            $('#Search-tab-pane').html(res);
            Swal.fire({
                title: "Hallo Doc",
                text: "Request Deleted!",
                icon: "success",
            });
            GetSearchRecords();
        },
        error: function () {
        }
    });
}

// ****************************Get Email/SMS Logs Page*******************************
function GetEmailSmsLog(tempId) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetEmailSmsLog",
        data: { tempId: tempId },

        success: function (res) {
            if (tempId == 0) {
                $('#Email-tab-pane').html(res);
            }
            else {
                $('#SMS-tab-pane').html(res);
            }

        },

        error: function () {
        }
    });
}

// ****************************Fiter Records On Email/SMS Logs Page*******************************
function emailSmsLogFilter() {
    event.preventDefault();

    var formdata = $('#logsRecordFilter').serialize();
    var tempid = $('#tempidsmsemaillog').val();

    $.ajax({
        method: "POST",
        url: "/AdminSide/emailSmsLogFilter",
        data: formdata,

        success: function (res) {
            if (tempid == 0) {
                console.log('yesss1');
                $('#Email-tab-pane').html(res);
            }
            else {
                console.log('yesss2');
                $('#SMS-tab-pane').html(res);
            }


        },
        error: function () {
            console.log('yesss3');
        }
    });

}

// ****************************Get BlockRequest Page*******************************
function GetBlockedRequest() {
    event.preventDefault();
    var formdata = $('#BlockedRecordsform').serialize();

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetBlockedRequest",
        data: formdata,

        success: function (res) {
            $('#Block-tab-pane').html(res);

        },

        error: function () {
        }
    });
}

// ****************************Fiter Records BlockRequest Page*******************************
function BlockedRecordsFilter() {
    event.preventDefault();
    var formdata = $('#BlockedRecordsform').serialize();

    $.ajax({
        method: "POST",
        url: "/AdminSide/GetBlockedRequest",
        data: formdata,

        success: function (res) {

            $('#Block-tab-pane').html(res);

        },
        error: function () {

        }
    });

}

// ****************************Unblock Request*******************************
function UnblockRequest(requestId) {

    $.ajax({
        method: "GET",
        url: "/AdminSide/UnblockRequest",
        data: { requestId: requestId },

        success: function (res) {
            $('#Block-tab-pane').html(res);
            Swal.fire({
                position: "top-end",
                icon: "success",
                title: "Request Unblocked",
                showConfirmButton: false,
                timer: 1000
            });
            GetBlockedRequest();
        },

        error: function () {
        }
    });
}


// *********************************************************** Invoicing ***********************************************************

function GetAdminInvoicing() {
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetAdminInvoicing",

        success: function (response) {
            $('#Invoicing-tab-pane').html(response);
        },
        error: function () {
            Swal.fire("Oops", "Error in Getting Invoicing Page", "error");
        }
    });
}

function GetTimeSheetDetail() {
    var phyid = $('#phyDropdown').val();
    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        method: "GET",
        url: "/AdminSide/GetTimeSheetDetail",
        data: { phyid: phyid, dateSelected: dateSelected },

        success: function (response) {
            $('#Invoicing-tab-pane').html(response);
            $('#phyDropdown').val(phyid);
            $('#searchByTimeperiod').val(dateSelected);
        },
        error: function () {
            Swal.fire("Oops", "Error in Getting Invoicing Page", "error");
        }
    });
}

function GetAdminFinalizeTimeSheet() {
    var dateSelected = $('#searchByTimeperiod').val();
    var phyid = $('#phyDropdown').val();
    $.ajax({
        methd: "GET",
        url: "/AdminSide/GetAdminFinalizeTimeSheet",
        data: { dateSelected: dateSelected, phyid: phyid},

        success: function (result) {
            $('#Invoicing-tab-pane').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ConfirmApproveTimeSheet(timeSheetId) {
    event.preventDefault();

    var bonus = $('#bonusAmount').val();
    var notes = $('#adminNotes').val();

    $.ajax({
        method: "POST",
        url: "/AdminSide/ConfirmApproveTimeSheet",
        data: { timeSheetId: timeSheetId, bonus: bonus, notes: notes },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire("Hurrah", "Timesheet Approved", "success");
            GetAdminInvoicing();

        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

