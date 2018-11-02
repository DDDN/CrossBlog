"use strict";

$(document).ready(function () {
	HighlightActiveLink();
	resizeBg();
});

$(window).on("load", function () {
	resizeBg();
});

function resizeBg() {
	var footer = $("footer");
	if (footer.length === 1) {
		var positionFooter = footer.position();
		var bgdiv = $('.bg-article');
		if (bgdiv.length === 1) {
			bgdiv.css('height', positionFooter.top - 115);
			bgdiv.css('display', 'block');
		}
	}
}