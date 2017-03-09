$(document).ready(function () {
    //var isChanged = false;
    $('input, select, textarea').change(function () {
        //$("input[type='submit']").removeAttr("disabled");
        $("#btnSubmit").removeAttr('disabled');
        $("#frmModified").val("true");
        //isChanged = true;
    });

    $(window).bind('beforeunload', function () {
        if ($("#frmModified").val() == "true") {
            //return 'There are unsaved changed. Do you want to discard Changes and Exit?';
            return "false";
        } else {
            return undefined;
        }
    });

    $("#btnSubmit").click(function () {
        //compilePostValues();
        $("#frmModified").val("false");
        $(this).button('loading');
    });

    $('input').on('keydown keyup change ', function (e) {
        $("#btnSubmit").removeAttr('disabled');
        $("#frmModified").val("true");
    });
});

$(document).ready(function () {
    $("input:text").focus(function () { $(this).select(); });

    //$('.collapse').on('show.bs.collapse', function () {
    //    $(this).parent().find(".glyphicon-collapse-down").removeClass("glyphicon-collapse-down").addClass("glyphicon-minus");
    //}).on('hide.bs.collapse', function () {
    //    $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-collapse-down");
    //});

    $('.datepicker').datepicker({
        dateFormat: 'M dd, yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true
    });

    $("#dialog-confirm").dialog({
        autoOpen: false,
        resizable: false,
        height: 300,
        width: 500,
        modal: true,
        buttons: {
            "Exit": function () {
                $("#frmModified").val("false");
                //isChanged.val("N");
                location.href = "/Employee/Index/";
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });

    $("#btnCancel").click(function (event) {
        if ($("#frmModified").val() == "true") {
            // If form was modified display confirmation message
            $("#dialog-confirm").dialog("open");
        }
        else {
            // Just redirect back to the Employee Listing Page
            location.href = "/Employee/Index/";
        }
    });

    //-------------------------------------------------------------------------------------------------
    //Building Assign Partial View
    //-------------------------------------------------------------------------------------------------
    $("#divLCAsgn").on('click', '#btnAsgnLC', function () {
        moveListItems('unasgndLCList', 'asgndLCList', false);
    });
    $("#divLCAsgn").on('click', '#btnUnasgnLC', function () {
        moveListItems('asgndLCList', 'unasgndLCList', false);
    });
    //-------------------------------------------------------------------------------------------------
    $("#divLCAsgn").on('dblclick', '#unasgndLCList', function () {
        moveListItems('unasgndLCList', 'asgndLCList', false);
    });
    $("#divLCAsgn").on('dblclick', '#asgndLCList', function () {
        moveListItems('asgndLCList', 'unasgndLCList', false);
    });

});

//---------------------------------------------------
//---------------------Functions---------------------
//---------------------------------------------------
function moveListItems(fromListId, toListId, byId) {
    var fromList = document.getElementById(fromListId);
    var toList = document.getElementById(toListId);

    $(fromList).find(':selected').appendTo(toList);

    if (byId == true) {
        sortSelectById(fromList);
        sortSelectById(toList);
    }
    else {
        sortSelect(fromList);
        sortSelect(toList);
    }

    //alert(getAsgndLCListValues());
}

function sortSelect(selElem) {
    //http://stackoverflow.com/questions/278089/javascript-to-sort-contents-of-select-element
    var tmpArr = new Array();
    for (var i = 0; i < selElem.options.length; i++) {
        tmpArr[i] = new Array();
        tmpArr[i][0] = selElem.options[i].text;
        tmpArr[i][1] = selElem.options[i].value;
    }
    tmpArr.sort();

    while (selElem.options.length > 0) {
        selElem.options[0] = null;
    }
    for (var i = 0; i < tmpArr.length; i++) {
        var op = new Option(tmpArr[i][0], tmpArr[i][1]);
        selElem.options[i] = op;
    }
    return;
}

function sortSelectById(selElem) {
    var tmpArr = new Array();
    for (var i = 0; i < selElem.options.length; i++) {
        tmpArr[i] = new Array();
        tmpArr[i][0] = selElem.options[i].text;
        tmpArr[i][1] = parseInt(selElem.options[i].value, 10);
    }
    tmpArr.sort(function (a, b) {
        return a[1] - b[1];
    });

    while (selElem.options.length > 0) {
        selElem.options[0] = null;
    }
    for (var i = 0; i < tmpArr.length; i++) {
        var op = new Option(tmpArr[i][0], tmpArr[i][1]);
        selElem.options[i] = op;
    }
    return;
}

function getAsgndLCListValues() {
    var lcList = document.getElementById("asgndLCList");
    var listResult = "";
    var i;
    for (i = 0; i < lcList.length; i++) {
        listResult = listResult + lcList.options[i].value;
        if (i != lcList.length - 1)
        { listResult = listResult + ","; }
    }
    //alert(lcList);
    return listResult;
};

function compilePostValues() {
    var lcList = getAsgndLCListValues();
    document.getElementById("asgnd_lc_list").setAttribute("Value", lcList);
    //alert(lcList);
    //alert(document.getElementById("asgnd_lc_list").value)
}