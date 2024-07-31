
/*****************************************************************************--Get Provider List Page--****************************************************************************************************** */
function ProviderList(regionId) {
    event.preventDefault();
    $.ajax({
        methd: "GET",
        url: "/AdminSide/Provider",
        data: { regionId: regionId },

        success: function (response) {
            console.log("success in getting admin provider");
            $('#Provider-tab-pane').html(response);
            $('#regionValue').val(regionId);
        },

        error: function () {
            alert("errror in getting admin provider");
        }
    });
}

// *********************************************************** Get Contact Provider Modal***********************************************************
function ContactProvider(email) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/ContactProvider",

        success: function () {
            $('#contactProviderId').modal('show');
            $('#contactEmailId').val(email);
        },

        error: function () {
            alert("error in contact provider");
        }
    });
}

// *********************************************************** Send Mail To Provider***********************************************************
function SendEmailToProvider() {
    event.preventDefault();

    if ($('#contactProviderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SendEmailToProvider",
            data: $('#contactProviderForm').serialize(),

            success: function () {
                $('#contactProviderId').modal('hide');
                Swal.fire({
                    title: "HelloDoc",
                    text: "Mail sent!",
                    icon: "success",
                });
            },

            error: function () {
                alert("error in send email to provider");
            }
        });
    }
}

// *********************************************************** Send SMS To Provider***********************************************************
function SendSMSToProvider() {
    event.preventDefault();

    if ($('#contactProviderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SendSMSToProvider",
            data: $('#contactProviderForm').serialize(),

            success: function () {
                $('#contactProviderId').modal('hide');
                Swal.fire({
                    title: "HelloDoc",
                    text: "SMS sent!",
                    icon: "success",
                });
            },

            error: function () {
                alert("error in send sms to provider");
            }
        });
    }
}

// *********************************************************** Stop Notification***********************************************************


function stopNotification(physicianId) {
    $.ajax({
        method: "Post",
        url: "/AdminSide/Notification",
        data: { physicianId: physicianId },

        success: function (result) {
            if (result.success) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Notification Stopped!",
                    icon: "success",
                });
            }
            else {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Notification Unstopped!",
                    icon: "success",
                });
            }
            ProviderList(0);
        },

        error: function () {
            alert("error in edit Administrator information");
        }
    })
}
// *********************************************************** Get Edit Provider Page***********************************************************
function GetEditProvider(aspId, callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetEditProvider",
        data: { aspId: aspId, callId: callId },

        success: function (result) {
            if (callId == 1) {
                $('#Provider-tab-pane').html(result);
            }
            else if (callId == 2) {
                $('#User-tab-pane').html(result);
            }
            else {
                $('#provider-profile-tab-pane').html(result);
            }
            //$('#regionValue').val(regionId);
        },

        error: function () {
            alert("error in get edit provider");
        }
    });
}

// *********************************************************** Get Payrate Page***********************************************************

function Payrate(aspId,PhysicianId,callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/Payrate",
        data: { aspId : aspId, PhysicianId: PhysicianId, callId: callId },

        success: function (result) {
            if (callId == 1) {
                $('#Provider-tab-pane').html(result);
            }
            else if (callId == 2) {
                $('#User-tab-pane').html(result);
            }
            else {
                $('#provider-profile-tab-pane').html(result);
            }
            //$('#regionValue').val(regionId);
        },

        error: function () {
            alert("error in get edit provider");
        }
    });
}

// *********************************************************** Set PayrateData***********************************************************

function SetPayrate(callId) {
    event.preventDefault();

    if ($('#PayrateFrom').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/SetPayrate",
            data: $('#PayrateFrom').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Information Updated!",
                    icon: "success",
                });
                Payrate(result.aspId, result.physicianId, callId);
            },

            error: function () {
                alert("error in edit account information");
            }
        })
    }
}

// *********************************************************** Reset Password Of Provider***********************************************************
function PhysicianProfileResetPassword(callId) {
    event.preventDefault();
    $("#PhysicianAccountForm input,textarea,select").prop("disabled", false);
    if ($('#PhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/PhysicianProfileResetPassword",
            data: $('#PhysicianAccountForm').serialize(),

            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        title: "Hello Doc",
                        text: "Password Changed!",
                        icon: "success",
                    });
                    GetEditProvider(result.aspId, callId);
                }
                else {
                    Swal.fire({
                        title: "Hello Doc",
                        text: "Please Enter Password!",
                        icon: "error",
                    });
                    //GetEditProvider(result.aspId);
                }
            },

            error: function () {
                alert("error in edit account password");
            }
        })
    }
}

// ***********************************************************Update Physician Profile Account Info***********************************************************
function PhysicianAccountEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/PhysicianAccountEdit",
            data: $('#PhysicianAccountForm').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Account Information Updated!",
                    icon: "success",
                });
                GetEditProvider(result,callId);
            },

            error: function () {
                alert("error in edit account information");
            }
        })
    }
}

// ***********************************************************Update Physician Profile Administrator Info***********************************************************
function PhysicianAdministratorEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianAdministratorForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/PhysicianAdministratorEdit",
            data: $('#PhysicianAdministratorForm').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Administrator Information Updated!",
                    icon: "success",
                });
                GetEditProvider(result, callId);
            },

            error: function () {
                alert("error in edit Administrator information");
            }
        })
    }
}

// ***********************************************************Update Physician Profile Mailing Info***********************************************************
function PhysicianMailingEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianMailingForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/PhysicianMailingEdit",
            data: $('#PhysicianMailingForm').serialize(),

            success: function (result) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Mailing Information Updated!",
                    icon: "success",
                });
                GetEditProvider(result,callId);
            },

            error: function () {
                alert("error in edit mailing information");
            }
        });
    }
}

// ***********************************************************Update Physician Profile Business Info***********************************************************
function PhysicianBusinessInfoEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianBusinessInfoForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/PhysicianBusinessInfoEdit",
            data: new FormData($('#PhysicianBusinessInfoForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Business Information Updated!",
                    icon: "success",
                });
                GetEditProvider(result,callId);
            },

            error: function () {
                alert("error in edit mailing information");
            }
        });
    }
}

// ***********************************************************Update Physician Profile OnBoarding***********************************************************
function UpdateOnBoarding(callId) {
    event.preventDefault();

    if ($('#OnboardingEditForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateOnBoarding",
            data: new FormData($('#OnboardingEditForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                Swal.fire({
                    title: "Hello Doc",
                    text: "Onboarding Information Updated!",
                    icon: "success",
                });
                GetEditProvider(result,callId);
            },

            error: function () {
                alert("error in edit onboarding");
            }
        });
    }
}

// ***********************************************************Delete Physician Account***********************************************************
function DeletePhysicianAccount(physicianId, callId) {
    $.ajax({
        method: "POST",
        url: "/AdminSide/DeletePhysicianAccount",
        data: { physicianId: physicianId },

        success: function () {
            Swal.fire({
                title: "Hello Doc",
                text: "Physician Deleted!",
                icon: "success",
            });
            if (callId == 1) {
                ProviderList(0);
            }
            else {
                GetUserAccess(0);
            }       
        },

        error: function () {
            alert("error in delete physician account");
        }
    });
}


// ***********************************************************Get Create Provider Account Page***********************************************************
function CreateProviderAccount(callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/CreateProviderAccount",
        data: { callId: callId },

        success: function (result) {
            if (callId == 1) {
                $('#Provider-tab-pane').html(result);
            }
            else {
                $('#User-tab-pane').html(result);
            }
        },

        error: function () {
            alert("error in get create account");
        }
    });
}

// ***********************************************************Update Data On Provider Accout Page***********************************************************
function CreateProviderAccountPost(callId) {
    event.preventDefault();

    if ($('#CreatePhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/CreateProviderAccountPost",
            data: new FormData($('#CreatePhysicianAccountForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        title: "Hello Doc",
                        text: "Physician Created!",
                        icon: "success",
                    });
                    if (callId == 1) {
                        ProviderList(0);
                    }
                    else {
                        GetUserAccess(0);
                    }               
                }
                else {
                    Swal.fire({
                        title: "Hello Doc",
                        text: "Physician Already Exists!",
                        icon: "error",
                    });
                }
                
            },

            error: function () {
                alert("error in post create account");
            }
        });
    }
}

