
var dialog_left = 0;
var dialog_top = 0;
var win = window;
var IsParent = 1;
if (win == win.parent) {
    IsParent = 0;
}
while (win != win.parent) {
    win = win.parent;
}
$(function () {
    $('body').mouseup(function (e) {
        win.isover = 1;
    });
});

function DialogMove(name) {
    if (IsParent == 1) {
        $('body').mouseout(function () {
            dialog_left = 0;
            dialog_top = 0;
            var dDiv = win.___("_DialogDiv_" + name);
            try {
                if (win.isover == 0) {
                    $(dDiv).css({
                        left: ($(dDiv).offset().left),
                        top: ($(dDiv).offset().top)
                    });
                }
            } catch (e)
        { }
        });

        $('body').mousemove(function (e) {
            if (dialog_left == 0 && dialog_top == 0) {
                dialog_left = e.clientX;
                dialog_top = e.clientY;
            }
            try {
                if (win.isover == 0) {
                    var dDiv = win.___("_DialogDiv_" + name);
                    $(dDiv).css({
                        left: ($(dDiv).offset().left + (e.clientX - dialog_left)),
                        top: ($(dDiv).offset().top + (e.clientY - dialog_top))
                    });
                    e.preventDefault();
                }
            } catch (e)
        { }
        });
    }
}
