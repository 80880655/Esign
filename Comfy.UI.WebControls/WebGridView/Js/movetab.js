//
var dragenable = false;
var x;
var y;
var w;
var h;
var obj;
var backCursor;
var table_left = 0;

var MoveTableUtil = new Object();

MoveTableUtil.init = function(objTable) {
    x = event.clientX + document.body.scrollLeft;
    obj = event.srcElement;
    w = event.srcElement.offsetWidth;

    var x1 = event.srcElement.offsetLeft + event.srcElement.offsetWidth + table_left - 5;
    var x2 = event.srcElement.offsetLeft + event.srcElement.offsetWidth + table_left;
    //document.getElementById("cursor_position").value ='x=' + x + ' x1=' + x1 + ' x2=' + x2 ;
    if (x > x1 && x < x2) {
        obj.setCapture();
        dragenable = true;
        obj.style.cursor = 'e-resize';
    }
};

MoveTableUtil.drag = function(objTable) {
    var cx = event.clientX + document.body.scrollLeft;
    if (table_left === 0) {
        table_left = MoveTableUtil.calculateTableLeft(objTable);
    }
    var x1 = event.srcElement.offsetLeft + event.srcElement.offsetWidth + table_left - 5;
    var x2 = event.srcElement.offsetLeft + event.srcElement.offsetWidth + table_left;

    //document.getElementById("cursor_position").value ='left=' + left + 'cx=' + cx + ' x1=' + x1 + ' x2=' + x2 + ' x1-x=' + (x1 - cx) +  ' parent=' + event.srcElement.offsetParent.offsetParent.offsetLeft;
    if (cx > x1 && cx < x2) {
        event.srcElement.style.cursor = 'e-resize';
    }
    else {
        if (event.srcElement.tagName == 'TH') {
            event.srcElement.style.cursor = 'default';
        }
        else {
            event.srcElement.style.cursor = 'pointer';
        }

    }
    if (dragenable === true) {
        var j;
        if (event.clientX + document.body.scrollLeft - x + w > 0) {
            var m = obj.cellIndex;

            for (j = 0; j < obj.parentNode.parentNode.rows.length; j = j + 1) {
               try {
                        obj.parentNode.parentNode.rows[j].cells[m].width = event.clientX + document.body.scrollLeft - x + w; 
               }
               catch (e) {
                 }
            }
        }
        else {
            var n = obj.cellIndex;
            for (j = 0; j < obj.parentNode.parentNode.rows.length; j = j + 1) {
             try {
                obj.parentNode.parentNode.rows[j].cells[n].width = 1;
            }
            catch (e) {
            }
            }
        }
    }
};

MoveTableUtil.end = function(objTable) {
    dragenable = false;
    obj.releaseCapture();
    if (event.srcElement.tagName == 'TH') {
        obj.style.cursor = 'default';
    }
    else {
        obj.style.cursor = 'pointer';
    }
    //obj.style.cursor = backCursor;
};

MoveTableUtil.calculateTableLeft = function(obj) {
    var left = 0;
    if (obj != null) {
        if (obj.offsetLeft != null) {
            left = obj.offsetLeft;
            if (obj.parentNode != null) {
                left += MoveTableUtil.calculateTableLeft(obj.parentNode);
            }
        }
    }

    return left;
};

