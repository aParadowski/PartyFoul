$(document).ready(function() {
				$("#submitBtn").click(function(){
					var firstName = $.trim($('#nameInput').val().split(',')[1]);
					var lastName = $.trim($('#nameInput').val().split(',')[0]);
					var status = $('#statusInput').val();
					passInData(firstName,lastName,status);
				});
});

function passInData(firstN, lastN ,status)
{
	$("#personTable")
    .prepend($('<tr>')
        .append($('<td>')
            .append(lastN + ", " + firstN)
        )
        .append($('<td>')
            .append(status)
        )
    );

	//$('#personTable').html("<tr>name </tr>,<tr>status</tr>");
	//alert(name + ", " + status);
}
