//------------------------------------------------- Chat With ------------------------------------//
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (message, userId, time) {
    console.log(`${userId} says ${message}`);
    if ($("#receiverId").val() == userId) {
        $("#message-body").append(`<div class="d-flex w-100 flex-column gap-1 text-start message-div mb-3"><div><span class="bg-primary-subtle rounded p-2 fs-5">${message}</span></div><span class="message-sent-time">${time}</span></div>`);
    }
});

$(document).on('click', "#sendButton", function () {
    var message = $.trim($("#typedMessage").val());
    if (message.length != 0) {
        $("#typedMessage").val("");
        var time = new Date().toLocaleTimeString().split(" ");
        var sentTime = time[0].split(":")[0] + ":" + time[0].split(":")[1] + " " + time[1];
        if ($("#message-body .date-partition span").last().text() != 'today') {
            $("#message-body").append(`<div class="d-flex align-items-center position-relative justify-content-center mb-2 date-partition"><hr class="w-100 position-absolute"
                                                                ><span class="text-secondary px-2" style="z-index: 1050;background: var(--bs-body-bg);">today</span></div>`);
        }
        $("#message-body").append(`<div class="d-flex w-100 flex-column gap-1 text-end message-div mb-3"><div><span class="bg-success-subtle rounded p-2 fs-5">${message}</span></div><span class="message-sent-time">${sentTime}</span></div>`);
        connection.invoke("SendMessage", $("#receiverId").val(), message).catch(function (err) {
            return console.error(err.toString());
        });
    }
});


$(document).on('click', ".chatWithOther", function () {
    $("#chatWith").offcanvas('show');
    connection.invoke("GetUserInfo", $(this).data('receiver'), $(this).closest('tr').data("rid")).catch(function (err) {
        return console.error(err.toString());
    });
});

connection.on("ReceiveUserInfo", function (userId, userName, messages) {
    $("#receiverId").val(userId);
    $("#receiverName").text(userName);
    $("#message-body").html("");
    var x = JSON.parse(messages);
    var date;
    if (x.length != 0) {
        for (let i = 0; i < x.length; i++) {
            var message = x[i].Message;
            var time = x[i].time;
            var textStartEnd = x[i].Sender == userId ? "text-start" : "text-end";
            var messageColor = x[i].Sender == userId ? "primary" : "success";
            var messageDate = new Date(parseInt(x[i].SentDate.split("-")[2]), parseInt(x[i].SentDate.split("-")[1]) - 1, parseInt(x[i].SentDate.split("-")[0]));
            if (date == null || date.getTime() != messageDate.getTime()) {
                date = messageDate;
                var dateshow = date.toLocaleDateString('en-GB') == new Date().toLocaleDateString('en-GB') ? "today" : date.toLocaleDateString('en-GB');
                $("#message-body").append(`<div class="d-flex align-items-center position-relative justify-content-center mb-2 date-partition"><hr class="w-100 position-absolute"
                                                        ><span class="text-secondary px-2" style="z-index: 1050;background: var(--bs-body-bg);">${dateshow}</span></div>`);
            }
            $("#message-body").append(`<div class="d-flex w-100 flex-column gap-1 ${textStartEnd} message-div mb-3"><div><span class="bg-${messageColor}-subtle rounded p-2 fs-5">${message}</span></div><span class="message-sent-time">${time}</span></div>`);
        }
    }
});