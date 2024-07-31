// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function CreateFormModal() {
    $.ajax({
        method: "GET",
        url: "/Student/CreateFormModal",

        success: function (result) {
            //console.log("success showing support modal");
            $('#showCaseModal').html(result);
            $('#createModal').modal('show');
        },

        error: function () {
            alert("error showing createModal");
        }
    });
}

function PostCreateForm() {
    event.preventDefault();
    if ($('#CreateForm').valid())
    {
        $.ajax({
            method: "POST",
            url: "/Student/PostCreateForm",
            data: $('#CreateForm').serialize(),

            success: function () {
                //console.log("send create request");
                alert("data posted");
                window.location.reload();
            },
            error: function () {
                alert("error in create form");
            }
        });
    }
}

function EditFormModal(studentId) {
    $.ajax({
        method: "GET",
        url: "/Student/EditFormModal",
        data: { studentId: studentId },

        success: function (result) {
            //console.log("success showing support modal");
            $('#showCaseModal').html(result);
            $('#editModal').modal('show');
        },

        error: function () {
            alert("error showing createModal");
        }
    });
}

function PostEditForm() {
    event.preventDefault();
    if ($('#EditForm').valid())
    {
        $.ajax({
            method: "POST",
            url: "/Student/PostEditForm",
            data: $('#EditForm').serialize(),

            success: function () {
                //console.log("send create request");
                alert("data Edited");
                window.location.reload();
            },
            error: function () {
                alert("error in edit form");
            }
        });
    }
}

function DeleteRow(studentId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Student/DeleteRow",
        data: { studentId, studentId },

        success: function () {
            //console.log("send create request");
            alert("row deleted");
            window.location.reload();
        },
        error: function () {
            alert("error in delte method");
        }
    });
}
