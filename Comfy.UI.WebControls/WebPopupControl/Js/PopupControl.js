function PopupControl(name) {



    this.ReceiveDataFromServer = function(valueReturnFromServer, context) {
        return;
    };


    this.Hide = function() {
        $("._TopPanel").remove();
        $("div[id$='" + name + "_" + name+"']").hide('slow');
      //  eval(name + "_CallBackToTheServer('hide', '')");
        // $("#DivHidenField").val("0")
    };

    this.Show = function () {
        $("._TopPanel").remove();
        if ($.browser.msie && $.browser.version == "6.0") {
            $("<iframe id='_TopGaiDiv' class='_TopPanel' style='margin:0;padding:0'>").width($(document).width()).height($(document).height()).css({ "position": "absolute", "left": 0, "top": 0, "background-color": "#CCC", "opacity": "0.0", "z-index": "10" }).appendTo(document.body); //加多一層Iframe，因為IE6下div遮不住select
        }
        $("<div id='_TopGaiDiv1' class='_TopPanel' style='margin:0;padding:0'>").width($(document).width()).height($(document).height()).css({ "position": "absolute", "left": 0, "top": 0, "background-color": "#CCC", "opacity": "0.4", "z-index": "11" }).appendTo(document.body);

        $("div[id$='" + name + "_" + name + "']").css({
            "position": "absolute",
            "z-index": "100",
            "top": Math.max(($(window).height() / 2 + $(window).scrollTop() - $("div[id$='" + name + "_" + name + "']").outerHeight() / 2), 0) + "px",
            "left": Math.max(($(document.body).width() / 2 + $(window).scrollLeft() - $("div[id$='" + name + "_" + name + "']").outerWidth() / 2), 0) + "px"
        });

        $("div[id$='" + name + "_" + name + "']").show('slow');
        // eval(name + "_CallBackToTheServer('show', '')");
        //  $("#DivHidenField").val("1");
    };

    $(function() {

        $("div[id$='" + name + "_" + name + "head']").css({ "width": ($("div[id$='" + name + "_" + name + "']").outerWidth() - 7) });
        $("div[id$='" + name + "_" + name + "']").jquerymove();

        $("img[id$='" + name + "_" + name + "closeImg']").mousedown(function (event) {
            event.stopPropagation();
        });
        $("div[id$='" + name + "_" + name + "context']").mousedown(function (event) {
            event.stopPropagation();
        });
        //        if ($("#DivHidenField").val() == 1) {
        //            $("#DivF").show();
        //        }
    });

}