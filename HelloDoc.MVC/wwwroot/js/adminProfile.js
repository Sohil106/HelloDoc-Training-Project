// *********************************************************** Get Admin Profile***********************************************************
function AdminProfile(aspId, callId) {
    event.preventDefault();   
    $.ajax({
        methd: "GET",
        url: "/AdminSide/AdminProfile",
        data: { aspId: aspId, callId: callId },

        success: function (response) {
            //console.log("success in getting admin profile");
            //$('#profile-tab-pane').html(response);


            if (response.code == 401) {
                window.location.reload();
            }
            else {
                if (callId == 1) {
                    $('#profile-tab-pane').html(response);
                }
                if (callId == 2) {
                    $('#User-tab-pane').html(response);
                }

            }
        },

        error: function () {
            alert("errror in getting admin profile");
        }
    });
}

// *********************************************************** Update Mailing Form On Profile***********************************************************
function UpdateMailingForm(callId) {
    //console.log($('#aspnetuserid').val())
    event.preventDefault();
    if ($('#mailingForm').valid())
    {
        $("#mailingForm input,textarea,select").prop("disabled", false);
        var formData = $('#mailingForm').serialize();

        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateMailingForm",
            data: formData,

            success: function (result) {
                Swal.fire({
                    title: "HelloDoc",
                    text: "Mailing Information Updated Successfully",
                    icon: "success",
                    timer: 1500,
                    timerProgressBar: true
                });
                AdminProfile(result, callId);
            },

            error: function () {
                console.log('eror in update mailing form');
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }


        });
    }

} 

// *********************************************************** Update Info Form On Admin Profile***********************************************************
function UpdateInfoForm(callId) {
    /*console.log($('#netuserid').val())*/
    event.preventDefault();

    if ($('#admininfoForm').valid())
    {
        $("#admininfoForm input,textarea,select").prop("disabled", false);
        var formData = $('#admininfoForm').serialize();
        //formData.push({ name: 'aspnetuserid', value: $('#netuserid').val() });
        //formData.push({ name: 'adminid', value: $('#adminId').val() });
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdateInfoForm",
            data: formData,

            success: function (result) {
                Swal.fire({
                    title: "HelloDoc",
                    text: "Administrator Information Updated Successfully",
                    icon: "success",
                    timer: 1500,
                    timerProgressBar: true
                });
                AdminProfile(result, callId);
            },

            error: function () {
                console.log('eror in update info form');
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }


        });
    }

} 

// *********************************************************** Update Password Form On Admin Profile***********************************************************
function UpdatePassword(callId) {


    event.preventDefault();
    if ($('#resetpassForm').valid())
    {
        $("#resetpassForm input,textarea,select").prop("disabled", false);
        var formData = $('#resetpassForm').serialize();
        $.ajax({
            method: "POST",
            url: "/AdminSide/UpdatePassword",
            data: formData,

            success: function (result) {
                Swal.fire({
                    title: "HelloDoc",
                    text: "Password Updated Successfully",
                    icon: "success",
                    timer: 1500,
                    timerProgressBar: true
                });
                AdminProfile(result, callId);
            },

            error: function () {
                console.log('eror in update Password');
                Swal.fire("Oops!", "Something is Wrong !!!", "error");
            }


        });
    }

} 

// *********************************************************** Get Creat Admin Account Page***********************************************************
function GetCreateAdminAccount(callId) {
    $.ajax({
        method: "GET",
        url: "/AdminSide/GetCreateAdminAccount",
        data: { callId: callId },

        success: function (result) {
            if (callId == 1) {
                $('#Admin-tab-pane').html(result);
            }
            if (callId == 2) {
                $('#User-tab-pane').html(result);
            }

        },

        error: function () {
            alert("error in get create admin account");
        }
    });
}

// *********************************************************** Post Create Admin Account Form***********************************************************
function CreateAdminAccountPost(callId) {
    event.preventDefault();

    if ($('#CreateAdminAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/AdminSide/CreateAdminAccountPost",
            data: $('#CreateAdminAccountForm').serialize(),

            success: function (result) {
                if (result.success) {
                    Swal.fire({
                        title: "Hallo Doc",
                        text: "Admin Created!",
                        icon: "success",
                    });
                    if (callId == 1) {
                        GetDashboard(0);
                    }
                    if (callId == 2) {
                        GetUserAccess(0);
                    }
                }
                else {
                    Swal.fire({
                        title: "Hallo Doc",
                        text: "Admin Already Exists!",
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

// *********************************************************** Delete Admin Account***********************************************************
function DeleteAdminAccount(adminId) {
    $.ajax({
        method: "POST",
        url: "/AdminSide/DeleteAdminAccount",
        data: { adminId: adminId },

        success: function () {
            Swal.fire({
                title: "Hallo Doc", 
                text: "Admin Deleted!",
                icon: "success",
            });
            GetUserAccess(0);
        },

        error: function () {
            alert("error in delete admin account");
        }
    });
}

