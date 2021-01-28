function WebButtonEdit(name) {
    var buttonEditName = name;
    $(function() {
        $("#" + buttonEditName + "ClickDiv").mouseover(function() {
            $(this).css({ "cursor": "pointer", "background-color": "#FFC488" });
        });
        $("#" + buttonEditName + "ClickDiv").mouseout(function() {
            $(this).css({ "background-color": "#CBE1FB" });
        });
    });


    this.SetText = function(val) {
        $("#" + buttonEditName).val(val);
    }

    this.GetText = function() {
        return $("#" + buttonEditName).val();
    }
};
