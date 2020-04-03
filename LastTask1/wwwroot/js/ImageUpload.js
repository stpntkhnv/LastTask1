function upload(file) {
	if (!file || !file.type.match(/image.*/)) return;
	var fd = new FormData();
	fd.append("image", file);
	fd.append("title", "pashapidor");
	fd.append("client_secret", "08f15a0f06dcdc42252b5edf1caa234889f3617e");

	var xhr = new XMLHttpRequest();
	xhr.open("POST", "https://api.imgur.com/3/image");
	xhr.setRequestHeader('Authorization', 'Client-ID e75c1034c633a9f');
	xhr.onload = function () {
		var link = 'url(' + (JSON.parse(xhr.responseText).data.link) + ')';

		$("#imgur_link").val(JSON.parse(xhr.responseText).data.link);

		$('#drag').find('p').css('display', 'none');
		$('#drag').css('background', link);
		$('#drag').css('background-size', 'cover');
		$('#drag').css('background-position', 'center');
		$('#drag').css('height', '400px !important');

		$('.image-small').css('height', '400px');
		$('.image-small').css('width', '75%');
		$('.image-small').css('border-radius', '5%');
		$('.image-small').css('background', link);
		$('.image-small').css('background-size', 'cover');
		$('.image-small').css('background-position', 'center');


		alert("Ваше фото успешно загружено");
	}
	xhr.send(fd);
}