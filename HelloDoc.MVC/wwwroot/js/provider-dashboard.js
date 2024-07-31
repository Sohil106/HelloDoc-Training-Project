

// *********************************************************** Get Dashboard data ***********************************************************
function GetProviderDashboard() {
    event.preventDefault();

    showLoader();

    var physicianId = $('#sessionId').val();
    var lastStatus = $('#lastStatus').val();


    $.ajax({
        methd: "GET",
        url: "/ProviderSide/GetProviderDashboard",
        data: { physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#provider-home-tab-pane').html(result);

            if (lastStatus != 1) {
                $('#new-tab').removeClass("active");
            }
            if (lastStatus == 2) {
                $('#Pending-tab').addClass("active");
            }
            if (lastStatus == 4) {
                $('#Active-tab').addClass("active");
            }
            if (lastStatus == 6) {
                $('#Conclude-tab').addClass("active");
            }

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


// **************** fetch table data based on status ***********************
function ProviderTableRecords(status, requesttypeid) {

    var physicianId = $('#sessionId').val();
    $('#lastStatus').val(status);

    $.ajax({
        method: "POST",
        url: "/ProviderSide/ProviderTableRecords",
        data: { status: status, requesttypeid: requesttypeid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#tableRecords').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}


// *********** filter table based on request type  ************
function ProviderFilterTable(status, requesttypeid) {

    if (requesttypeid == null) {
        requesttypeid = $('#reqTypeValueId').val();
    }
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    var physicianId = $('#sessionId').val();
    $('#lastStatus').val(status);

    $.ajax({
        method: "POST",
        url: "/ProviderSide/ProviderFilterTable",
        data: { status: status, requesttypeid: requesttypeid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#providerRequestTable').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}


// ************************ get Accept case modal *****************************
function AcceptCaseModal(requestid) {

    $.ajax({
        methd: "GET",
        url: "/ProviderSide/AcceptCaseModal",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#acceptModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

// ************************ Post Data of Accept case modal *****************************
function AcceptCase() {
    event.preventDefault();

    var requestId = $('#clearRequestId').val();
    var physicianId = $('#sessionId').val();


    $.ajax({
        method: "POST",
        url: "/ProviderSide/AcceptCase",
        data: { requestId: requestId, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#acceptModalId').modal('hide');
            Swal.fire("Hurrah", "Case Accepted", "success");

            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}


// ************************** Get Transfer Case Model***************************
function Transfer(requestid) {

    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "GET",
        url: "/ProviderSide/Transfer",
        data: { requestid: requestid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#providerTransferModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ Update Data Of TransferCase modal *****************************
function TransferCase() {
    event.preventDefault();

    if ($('#providerTransferCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/ProviderSide/TransferCase",
            data: $('#providerTransferCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#providerTransferModalId').modal('hide');
                Swal.fire("Hurrah", "Case Transfer To Admin", "success");

                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

// **************************Get Encounter Model***************************
function EncounterModal(requestid) {

    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "GET",
        url: "/ProviderSide/EncounterModal",
        data: { requestid: requestid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#EncounterModalBox').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ Post Data of Encounter modal *****************************
function PostEncounterCare() {
    event.preventDefault();

    if ($('#EncounterModalForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/ProviderSide/PostEncounterCare",
            data: $('#EncounterModalForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#EncounterModalBox').modal('hide');
                GetProviderDashboard();
                if (result) {
                    Swal.fire("Hurrah", "Care type will be House call", "success");
                }
                else {
                    Swal.fire("Hurrah", "Care type will be Conclude", "success");
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}

// **********************Finalize Encounter Model which will open when we click Finalize Button On Encounter Form *****************************
function finalizeEncounterModal(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/ProviderSide/finalizeEncounterModal",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#EncounterFinalizeModal').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ After Opening Finalize Encounter Model there is a Finalized button clicking on that bellow ajax will run *****************************
function finalizeEncounter(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/ProviderSide/finalizeEncounter",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#EncounterFinalizeModal').modal('hide');
            Swal.fire("Hurrah", "Case Finalized", "success");
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ get Download Encountor Modal After Finalize Form *****************************
function DownloadEncounter(requestId,callId) {

    $.ajax({
        method: "GET",
        url: "/ProviderSide/DownloadEncounter",
        data: { requestId: requestId},

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            else {
                if (callId == 3) {
                    $('#showProviderCaseModal').html(result);
                }
                else {
                    $('#showCaseModal').html(result);
                }
            }       
            $('#DownloadFinalizeModal').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ clicking On Download button on Downloadencounter Modal, This action Will return  *****************************
function FinalizeDownload(requestid) {
    event.preventDefault();  
    window.location.href = './GeneratePDF?requestid=' + requestid;
}


// ************************ get Housecall Modal by clicking on HouseCall Button*****************************
function HouseCall(requestId) {

    $.ajax({
        method: "GET",
        url: "/ProviderSide/HouseCall",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#houseCallModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ Post Data Of  Housecall Modal*****************************
function HouseCallPost(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/ProviderSide/HouseCallPost",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#houseCallModalId').modal('hide');
            Swal.fire("Hurrah", "Case House Called", "success");
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

// ************************ Get ConcludeCare Page*****************************
function GetConcludeCare(requestid) {

    $.ajax({
        methd: "GET",
        url: "/ProviderSide/GetConcludeCare",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-home-tab-pane').html(result);

            setTimeout(function () {
                hideLoader();
            }, 300);

        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

// ************************ Upload Documnet On Conclude Care Page****************************
function UploadConcludeDocument() {
    event.preventDefault();

    var formdata = new FormData($('#concludeDocumentFormId')[0]);

    $.ajax({
        method: "POST",
        url: "/ProviderSide/UploadConcludeDocument",
        data: formdata,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire("Hurrah", "Document Uploaded", "success");

            GetConcludeCare(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

// ************************ Delete Uploaded Document On Conclude Care Page*****************************
function DeleteConcludeFile(requestwisefileid, requestid) {
    event.preventDefault();

    $.ajax({
        methd: "POST",
        url: "/ProviderSide/DeleteConcludeFile",
        data: { requestwisefileid: requestwisefileid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire("Hurrah", "Document Deleted", "success");

            GetConcludeCare(requestid);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })
}

// ************************Post Conclude Care *****************************
function ConcludeCare() {
    event.preventDefault();

    if ($('#ConcludeFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/ProviderSide/ConcludeCare",
            data: $('#ConcludeFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire("Hurrah", "Case Concluded", "success");
                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}

// ************************ Send Request For Update Provider Profile Data to Admin via Mail*****************************
function SendRequestToAdmin() {
    event.preventDefault();

    if ($('#requestToAdminFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/ProviderSide/SendRequestToAdmin",
            data: $('#requestToAdminFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#requestToAdminModal').modal('hide');
                Swal.fire("Hurrah", "Requested sent to Admin", "success");

                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

// ************************ Invoicing*****************************

function GetProviderInvoicing() {
    showLoader();

    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        methd: "GET",
        url: "/ProviderSide/GetProviderInvoicing",
        data: { dateSelected: dateSelected },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-invoicing-tab-pane').html(result);
            if (dateSelected != undefined) {
                $('#searchByTimeperiod').val(dateSelected);
            }
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

function OpenFinalizeTimeSheet() {
    showLoader();

    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        methd: "GET",
        url: "/ProviderSide/OpenFinalizeTimeSheet",
        data: { dateSelected: dateSelected },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-invoicing-tab-pane').html(result);

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

function PostFinalizeTimesheet(callId) {
    event.preventDefault();

    var formData = $('#finalizeSheetForm').serializeArray();

    $.ajax({
        method: "POST",
        url: "/ProviderSide/PostFinalizeTimesheet",
        data: formData,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (result) {
                Swal.fire("Hurrah", "Timesheet Updated", "success");
                if (callId == 1) {
                GetAdminInvoicing();
                }
                else if (callId == 2) {
                GetProviderInvoicing();
                }
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function GetAddReceipts(timeSheetDetailId,callId) {

    $.ajax({
        methd: "GET",
        url: "/ProviderSide/GetAddReceipts",
        data: { timeSheetDetailId: timeSheetDetailId,callId : callId},
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#AddReceiptsContainer').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function PostAddReceipt(counter) {
    event.preventDefault();

    var formData = new FormData();

    var timeSheetDetailId = $(`#inputTimeSheetDetailId${counter}`).val();
    var item = $(`#inputItem${counter}`).val();
    var amount = $(`#inputAmount${counter}`).val();
    var file = $(`#inputFile${counter}`)[0].files[0];

    formData.append("timeSheetDetailId", timeSheetDetailId);
    formData.append("item", item);
    formData.append("amount", amount);
    formData.append("file", file);

    $.ajax({
        method: "POST",
        url: "/ProviderSide/PostAddReceipt",
        data: formData,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Receipt Updated",
                icon: "success",
            })
            $('#AddRecieptBtn').click();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function DeleteReceipt(timeSheetDetailId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/ProviderSide/DeleteReceipt",
        data: { timeSheetDetailId: timeSheetDetailId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Receipt Deleted",
                icon: "success",
            })
            $('#AddRecieptBtn').click();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}


function ConfirmFinalizeTimeSheet(timeSheetId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/ProviderSide/ConfirmFinalizeTimeSheet",
        data: { timeSheetId: timeSheetId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Timesheet Finalized",
                icon: "success",
            })
            GetProviderInvoicing();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
