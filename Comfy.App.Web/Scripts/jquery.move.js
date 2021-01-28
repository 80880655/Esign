var isover = 1;
(function ($) {
    $.fn.jquerymove = function () {
        var moveobj = $(this);
        var old_position = {};
        var new_position = {};
        var offset = {};
        moveobj.css("cursor", "move");
        moveobj.mousedown(
			function (e) {
			    old_position = { X: e.clientX, Y: e.clientY };
			    new_position = { X: e.clientX, Y: e.clientY };
			    offset = moveobj.offset();
			    isover = 0;
			    $('body').mouseup(
					function (e) {
					    isover = 1;
					}
				);
			    e.preventDefault();
			}
		);

        $('body').mousemove(
			function (e) {
			    if (!isover) {
			        var leftX = offset.left + new_position.X - old_position.X;
			        var topY = offset.top + new_position.Y - old_position.Y;
			        new_position = { X: e.clientX, Y: e.clientY };
			        moveobj.css({
			            left: leftX,
			            top: topY
			        });
                    $("._SelectOp").remove();
                    e.preventDefault();
			    }
			}
		);
    }

})(jQuery);
