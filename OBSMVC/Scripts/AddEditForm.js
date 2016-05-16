
//==========================================================================================================================
// ++++++++++++++++++++++++++++++++++++++++ JAVASCRIPT FUNCTIONS +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// ++++   Must be Available before any element is rendered and they are outside the "document.ready" section      ++++++++++
//==========================================================================================================================
function isCascadeEvent(){
    //Check if the control field is blank, if so, this is the first time this event if fired
    if ($("#frmEventCounter").val() == "0") {
        $("#frmEventCounter").val(new Date());
        return false;    
    }

    var currentDate = new Date();
    var lastDate = new Date($("#frmEventCounter").val());
    var elapsedSeconds = (currentDate - lastDate) / 1000
    alert("Comparing Current: " + currentDate + "\nVersus Last: " + lastDate);
    alert("Time Elapsed: " + elapsedSeconds + " Seconds.");
    $("#frmEventCounter").val(new Date());   // Reset the Control Field
    if (elapsedSeconds > 20) {
        return false;
    }
    return true;
}
function loadTemplate(callerBtn, selAnslist) {
    // Function to load the selected template values into the Edit Mode ANswer Types

    //Create an array of values (From parameter selAnslist value) to apply to the text boxes
    var templateList = selAnslist.split(",");
    var textBoxCounter = 0;
    $("#newQATsection").find(".selAnswerTextb").each(function () {
        // Loops through all textboxes of the upper level parent DIV and set their values accordingly
        $(this).val(templateList[textBoxCounter]);
        textBoxCounter = textBoxCounter + 1;
    });

    //$('.saveSA').prop('disabled', false);
    $("#btnSaveNewSelAnswer").removeAttr('disabled');
    $("#btnSaveNewSelAnswer").button('reset');
    $("#newErrorMsg").html("");
}
//-----------------------------------------------------------------------------------------------
function clearNewSAbox() {
    $("#divNewSA").html("");
    $.ajax({
        //url: '@Url.Action("addFormQAInfo", "colFormTemplate")',
        url: '/colFormTemplate/addFormQAInfo',
        method: "GET",
        cache: false,
        //data: { question_id: questionId, dropdownID: ddlId},
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Failed to reload Selectable Answer Data Entry Form!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and give an alert of any errors if they occurred
        }
    }).done(function (d) {
        $("#divNewSA").html(d);
    });
    $("#eventCounter").val("0");
}
//-----------------------------------------------------------------------------------------------
function addNewTextBox() {
    var divToAdd = '<div class="row form-inline newTB"> <input type="text" value="" class="form-control selAnswerTextb" placeholder="Enter a Value" /></div>'
    var delButton = $('<button  type="button" ><span class="glyphicon glyphicon-trash"></span></button>').click(function () {
        $(this).parent().remove();
    });
    $('#multiList').append(divToAdd).children(".newTB").last().append(delButton);
}
//-----------------------------------------------------------------------------------------------
function enableDropDown(dropdown) {
    //var $drop_down = $(this);
    var selectedId = dropdown.val();
    var selectedText = dropdown.children("option:selected").text();
    var ddlId = dropdown.prop("id");

    var selansDiv = dropdown.parents(".divFormQuestion").first().find(".selAnsSection").first();
    var qid = dropdown.parents(".divFormQuestion").first().find("#qid").val();
    //alert("Selected Text is: " + selectedText);
    //alert("Before opening modal window, the ddl id is: " + ddlId);
    if (selectedId == '') {
        selectedId = "-1";
        selansDiv.html("");
    }
    else if (selectedId == 'New') {
        //alert("A New Question request has been issued!");
        $("#popupStatus").val("");
        $(this).parents(".divFormQuestion").first().find(".selAnsSection").first().html("");  // Reset Sel Answers
        //Reset the modal Window contents
        clearNewSAbox();

        // Following code not used anymore as we will no longer keep a list of AT to add (New functionality
        // will only save one AT at a time)
        ////Set the Hidden Field Values that will be used in Edit Mode
        //if ($("#popupDDLid").val() != ddlId)
        //{  // This indicates that a new Dropdown is being selected for edition!
        //    //Reset the old values from the "added" section, as they do not need to be kept from a previous different Selectable Answer
        //    $("#divaddedSA").html("");
        //}

        $("#popupDDLid").val(ddlId);
        var questionText = dropdown.parents(".divFormQuestion").first().find(qText).text();
        $("#selQText").html(questionText);
        //$("#selQText").text(questionText);
        $("#editQATWindow").modal();
        //selansDiv.html("");
    } else {
        if (selectedText.substring(0, 2) == "YN" || selectedText.substring(0, 2) == "Fr") {
            selansDiv.html('<span style="color:blue">No Selectable Answers Required for this Selection</span>')
        }
        else {
            //alert("Selected ID "+selectedId);
            $.ajax({
                //url: '@Url.Action("GetSelectableAnswers", "ColFormTemplate")',
                url: '/ColFormTemplate/GetSelectableAnswers',
                method: "GET",
                cache: false,
                data: { qat_id: selectedId },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert("Failed to retrieve data for this question!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
                }
            }).done(function (d) {
                //alert("Retrieved Data is: " + d);
                selansDiv.html(d);
            });
        }
    }

    $(this).css({
        "color": "black",
        "background-color": "white"
    });
}
//-----------------------------------------------------------------------------------------------
function resetSelAns(qId, ddlId) {

    $qatDIV = $(ddlId).parents(".divFormQuestion").first();
    $qatDIV.find("#ddlSelValue").val($(ddlId).find('option:selected').val())

    var selectedText = $(ddlId).children("option:selected").text();
    var selectedQATid = $(ddlId).children("option:selected").val();

    //alert("Refreshing SelAns: Selected Text: " + selectedText + " and QAT id is: " + selectedQATid);

    if (selectedText.substring(0, 2) == "YN" || selectedText.substring(0, 2) == "Fr") {
        $qatDIV.find(".selAnsSection").first().html('<span style="color:blue">No Selectable Answers Required for this Selection</span>')
    }
    else {
        //alert("Selected ID "+selectedId);
        $.ajax({
            //url: '@Url.Action("GetSelectableAnswers", "ColFormTemplate")',
            url: '/ColFormTemplate/GetSelectableAnswers',
            method: "GET",
            cache: false,
            data: { qat_id: selectedQATid },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Can't find data for this question!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //alert("Retrieved Data is: " + d);
            $qatDIV.find(".selAnsSection").first().html(d);
        });
    }
};
//-----------------------------------------------------------------------------------------------
function showAlert(alertTitle, alertMessage) {
    //Set the values to display in the alert message
    $("#alertTitle").html(alertTitle)
    $("#divAlertText").html(alertMessage)
    //Show the pop up alert
    $("#alertPopup").modal();
}
//-----------------------------------------------------------------------------------------------
function getATcategory() {
    var selectedText = $("#fullSelATlist").find('option:selected').text().substring(0, 5);
    if (selectedText == "3 Val") {
        return "3 Val Range"
    }
    else if (selectedText == "5 Val") {
        return "5 Val Range";
    }
    else {
        return "Other"
    }
}
function validateForm() {
    var errorMsg = "";
    var sectionCounter = 0;
    //----------------------- Perform Front-End Form Validation ----------------------------------------
    //Process all the form Sections to get Data to post
    $("div.formSection").each(function () {
        var formSection = $(this);
        //var sectionTitleBox = $($(this).find("input.sectionTitle").first()).val();
        var sectionTitleBox = formSection.find("input.sectionTitle").first();
        var sectionName = sectionTitleBox.val().trim();
        //Loopt thorugh all Dropdowns in this section to ensure that they have a valid selection
        formSection.find(".viewDropDown").each(function () {
            if ($(this).val() == "" || $(this).val() == "New") {
                $(this).css({
                    "color": "white",
                    "background-color": "red"
                });
                errorMsg = errorMsg + "One Dropdown has an invalid Selection\n";
            }
        });

        //var dropdownArray = formSection.find(".viewDropDown");
        //alert("Section " + sectionName + " has " + dropdownArray + " dropdowns.")

        sectionCounter++;
        if (sectionName == "") {
            errorMsg = errorMsg + " Form Section " + sectionCounter + " is missing the Title.\n";
            sectionTitleBox.addClass("invalidField");
            sectionTitleBox.focus();
        }
        else {
            formSection.find('.divFormQuestion').each(function () {
                // Process data for each of the QATs in the form
                var qatDDL = $(this).find(".viewDropDown").first();
                var selectedQAT = qatDDL.val();
                var isOptional = $(this).find("#cbOptional").prop("checked");

                if (isOptional) { isOptional = "Y"; }
                else { isOptional = "N" }
                //var trtrt = ""; //hdnFormQuestion
                //alert("A checkbox was found. Value : " + isOptional);
                var qatHiddenField = $(this).find(".hdnFormQuestion").first();
                qatHiddenField.val(selectedQAT + "~" + sectionName + "~" + isOptional);
                //alert("A question was found!\nSelected QATid = " + selectedQAT + "\nSection Name = " + sectionName + "\nFull Posting String: " + qatHiddenField.val());
                //alert("Question value to post = " + qatHiddenField.val());
            });
        }
    });  // Finished Processing all Form Sections

    // ---- Validate that the start Date must not be in the past -----
    var todaysDate = new Date();                              // It includes the current Time
    var effStartDate = new Date($('#cft_eff_st_dt').val());   // It does not contain a time
    //var todayDateOnly = new Date(todaysDate.getFullYear(),todaysDate.getMonth(),todaysDate.getDate()); //This will write a Date with time set to 00:00:00 so you kind of have date only
    todaysDate.setHours(0, 0, 0, 0);          //Clear the times so the comparison will be based on date only
    effStartDate.setHours(0, 0, 0, 0);

    if (effStartDate < todaysDate) {
        errorMsg = errorMsg + "Warning!\nThe Effective Form Start Date [" + effStartDate.toString().substring(0, 16) + "] \nMust be greater or equal to Today [" + todaysDate.toString().substring(0, 16) + "].\n";
        $('#cft_eff_st_dt').addClass("invalidField");
        $('#cft_eff_st_dt').focus();
    }

    //Validate the Observation Type
    if (document.getElementById("cft_obsType").value == "") {
        errorMsg = errorMsg + "The Observation Type is invalid.\n";
        $("#cft_obsType").addClass("invalidField");
        $("#cft_obsType").focus();
    }

    //Validate the Form Title
    if (document.getElementById("cft_Title").value.trim() == "") {
        errorMsg = errorMsg + "The form Title is Required\n";
        $("#cft_Title").addClass("invalidField");
        $("#cft_Title").focus();
    }

    if (errorMsg != "") {
        //If any validation error is found give alert and reset the Save Button
        $('#btnSaveForm').button('reset');
        $("#btnPublishForm").button('reset');
        alert(errorMsg);
        return false;
    }
    else {
        //Validation Passes.
        return true;
    }
}
function PostForm() {
    //Post the Form
    //isChanged = false;  // To prevent the alert of leaving the page from showing up
    $("#formChanged").val("N");   // To prevent the alert of leaving the page from showing up
    document.getElementById("CreateForm").submit();
}
function showConfirmation(alertTitle, alertMessage, confBtnText, funcName) {
    //Set the values to display in the alert message
    $("#confTitle").html(alertTitle)
    $("#divConfText").html(alertMessage)
    //$("#btnAlertConfirm").prop('value', confBtnText);
    $("#btnAlertConfirm").html('Delete Form');

    $("#funcToExec").val(funcName);
    //$("#confirmationResult").val('true');

    //Show the Confirmation Popup Form
    $("#confPopUp").modal();
}
function removeQuestion(qRowId) {
    $("#" + qRowId).remove();
}
//-----------------------------------------------------------------------------------------------
function getTestData(questionId) {
    return "[You Selected Question Id: " + questionId + "]";
}
//-----------------------------------------------------------------------------------------------
function removeSection(qSectionId) {
    $("#" + qSectionId).remove();
}
//-----------------------------------------------------------------------------------------------
function leavePage() {
    var editMode = "@Model.manageAction";
    $("#formChanged").val("N");   //To prevent the browser to present the confirm-exit dialog.

    // When in "MANAGE-EDIT" mode, we need to Reset the published Date/Time via Ajax to it's previous state
    if (editMode == "MANAGE-EDIT") {
        var publishedDateTime = "@Model.originalPublishDate";
        var publishedUser = "@Model.originalyPublishedBy";
        var formCftid = "@Model.cft_id";
        $.ajax({
            //url: '@Url.Action("republishFormOnCancel", "ColFormTemplate")',
            url: '/ColFormTemplate/republishFormOnCancel',
            method: "POST",
            cache: false,
            data: { cft_id: formCftid, publishedBy: publishedUser, publishedOn: publishedDateTime },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Failed to reset Changes...\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            if (d == "Success") {
                alert("Edit mode cancelled! Form has been republished.");
                //location.href = "/ColFormTemplate/AddEditForm/" + formCftid;
            }
            else {
                alert("ERROR Resetting Published Date/Time: " + d);
            }
        });
        //--- Finished Resetting the ajax values
    }

    //Finally, just leave the page and redirect to the Form Index page
    location.href = "/ColFormTemplate/";
};
//-----------------------------------------------------------------------------------------------
function delForm(){
    // Perform an Ajax call to either hard or soft delete the current form
    var cft_id = $("#cft_id").val();
    alert("Deleting...");
    $.ajax({
        //url: '@Url.Action("deleteForm", "ColFormTemplate")',
        url: '/ColFormTemplate/deleteForm',
        method: "POST",
        cache: false,
        data: { cft_id: cft_id },     //<---- Data Parameters (if not already passed in the Url)
        //data: JSON.stringify({ 'Options': someData }),           //<--- In case we wanted to post a JSON data object
        //dataType: "json",                                        //<--- In case we wanted to post a JSON data object
        //contentType: "application/json; charset=utf-8",          //<--- In case we wanted to post a JSON data object
        //traditional: true,                                       //<--- In case we wanted to post a JSON data object

        //--- On error, execute this function ------
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Failed to Remove Form id " + cft_id+ " from Database !!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
        }
    }).done(function (d) {
        //Execute this code After the Ajax call completes successfully
        //alert("Form Id: " + @Model.cft_id + " has been Deleted!");
        //close modal Popup
        $('#confPopUp').modal('toggle');
        location.href = "/ColFormTemplate/";
    });
}
//-----------------------------------------------------------------------------------------------   

//-----------------------------------------------------------------------------------------------   


//==========================================================================================================================
// ++++++++++++++++++++++++++++++ DOCUMENT READY JAVASCRIPT EVENT LISTENERS SECTION   ++++++++++++++++++++++++++++++++++++++
//==========================================================================================================================
$(document).ready(function () {
    //alert("Javascript Code File is connected");
   
    $("#editQATWindow").on('click', '#addNewTextBoxControl', function (ev) {
        //ev.preventDefault();
        //ev.stopPropagation();
        //Somehow the event gets always triggered twice. Only execute the action onthe first execution an ignore the second
        if ($("#eventCounter").val() == "0") {
            addNewTextBox();
            $("#eventCounter").val("1");  //Increase the counter so the next time the events is fired it will be ignored
        }
        else {
            $("#eventCounter").val("0");   //Reset the counter for next time a button on the pop up is clicked
        }
    });
    // If the form was recently saved/posted successfuly, this indicates a change on the AT list
    // reload the drop down with the new values
    $("#editQATWindow").on('hidden.bs.modal', function () {
        var dropdownId = "#" + $("#popupDDLid").val();

        if ($("#popupStatus").val() == "saved") {
            //alert('Selectable Answer Edit Mode completed for drop down control id ' + modalddlId + "\nList is being refreshed...");
            var questionId = $("#popupQID").val();
            var modalddlId = $("#popupDDLid").val();

            //alert("Modal is closed, question ID = " + questionId + ", drpdown Id=" + modalddlId + " and selected QATid= '" + $("#savedQATid").val() + "' will be reloaded");

            $.ajax({
                //url: '@Url.Action("reloadQuestionDropdown", "ColFormTemplate")',
                url: '/ColFormTemplate/reloadQuestionDropdown',
                method: "GET",
                cache: false,
                data: { question_id: questionId, dropdownID: modalddlId, selectedQATid: $("#savedQATid").val() },
                error: function () {
                    alert("Failed to reload the Answer Type Values!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
                }
            }).done(function (d) {
                //alert("Retrieved Data is: " + d);

                $(dropdownId).parents(".reloadDDL").first().html(d);
                //alert("Dropdown Id handle is: " + $(dropdownId).attr("id"));

                //After Dropdown is loaded, retrieve the selectable answers for the item that is selected on the dropdown
                resetSelAns(questionId, dropdownId);
            });
        }
        else {
            //Modal Form was nor saved, so we just need to restore the previusly selected value on the dropdown list
            var dropDownIdused = "#" + $("#popupDDLid").val();
            var dropDownStoredValue = $("#ddlSelValue").val();
            //var stringUsed = dropDownIdused + " option[value='" + dropDownStoredValue + "']";
            //alert(stringUsed);
            //alert("Changes aborted... Restoring value for dropdown '" + dropDownIdused + "' to: " + dropDownStoredValue );

            //$(stringUsed).attr('selected', 'selected'); // added single quotes
            //For NOw default the selected Option to the first index
            $(dropDownIdused + " option:contains(" + "Answer" + ")").attr('selected', 'selected');  //Reset dropdown to the first index

            $(dropdownId).parents(".divFormQuestion").first().find(".selAnsSection").first().html("");
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#confPubDates").on('hidden.bs.modal', function () {
        // Reset the Publish Button
        //alert("Popup was closed!");
        $("#btnPublishForm").button('reset');
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $(".modal-wide").on("show.bs.modal", function () {
        var height = $(window).height() - 200;
        $(this).find(".modal-body").css("max-height", height);
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('.collapse').on('show.bs.collapse', function () {
        $(this).parent().find(".glyphicon-collapse-down").removeClass("glyphicon-collapse-down").addClass("glyphicon-minus");
    }).on('hide.bs.collapse', function () {
        $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-collapse-down");
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("input:text").focus(function () { $(this).select(); });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('input, select, textarea').not(".qInput").change(function () {
        //$("input[type='submit']").removeAttr("disabled");

        if ($('#btnSaveForm').val() != "Save Form") {
            $('#btnSaveForm').button('reset');
        }
        $("#btnSaveForm").removeAttr('disabled');
        $("#btnCancel").removeAttr('disabled');
        $("#btnReset").removeAttr('disabled');
        //alert('Button Text is:' + btnText);
        $("#formChanged").val("Y");   // Re-enable the form "changed" status
        //isChanged = true;
        $(this).removeClass("invalidField");
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnCancel").click(function () {
        if ($("#formChanged").val() == "Y") {
            if ($("#bootstrapBtnReset").val() == "N") {
                $.fn.bootstrapBtn = $.fn.button.noConflict();  // To restore the "X" close button of the alert window. Must be done only once (at most) to prevent js errors
                $("#bootstrapBtnReset").val("Y");   // Set it to "Y" so we don't try to reset it again which would cause an error
            }

            $("#dialog-confirm-exit").dialog({
                resizable: false,
                height: 250,
                width: 400,
                modal: true,
                buttons: {
                    "Discard Changes": function () {
                        leavePage();
                    },
                    "Continue Working": function () {
                        $(this).dialog("close");
                    }
                }
            });
        }
        else {
            leavePage();
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#cft_eff_st_dt').datepicker({
        dateFormat: 'M dd, yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        minDate: -0,
        //minDate: -0,
        //maxDate: "+1M +10D",
        onClose: function (selectedDate) {
            $("#cft_eff_end_dt").datepicker("option", "minDate", selectedDate);
        },
        useCurrent: false //Important! See issue #1075
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#cft_eff_end_dt').datepicker({
        dateFormat: 'M dd, yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        minDate: -0,
        onClose: function (selectedDate) {
            $("#cft_eff_st_dt").datepicker("option", "maxDate", selectedDate);
        },
        useCurrent: false //Important! See issue #1075
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#conf_cft_eff_st_dt').datepicker({
        dateFormat: 'M dd, yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        minDate: -0,
        //minDate: -0,
        //maxDate: "+1M +10D",
        onClose: function (selectedDate) {
            $("#conf_cft_eff_end_dt").datepicker("option", "minDate", selectedDate);
        },
        useCurrent: false //Important! See issue #1075
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#conf_cft_eff_end_dt').datepicker({
        dateFormat: 'M dd, yy',
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        minDate: -0,
        onClose: function (selectedDate) {
            $("#conf_cft_eff_st_dt").datepicker("option", "maxDate", selectedDate);
        },
        useCurrent: false //Important! See issue #1075
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnSaveDateConf").click(function(){
        $("#cft_eff_st_dt").val($("#conf_cft_eff_st_dt").val());
        $("#cft_eff_end_dt").val($("#conf_cft_eff_end_dt").val());
        $('#confPubDates').modal('toggle');
        PostForm();
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('#CreateForm').keypress(function (e) {
        if (e.keyCode == 13)
        {
            $('#doSearch').click();
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('[data-toggle="tooltip"]').tooltip();
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnAddQuestionPopup").click(function () {
        $("#addQuestionWindow").modal();
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('textarea').on('keydown keyup change focus blur', function(e) {
        if (e.type === 'change') {
            // this event is triggered when the text is changed through drag and drop too,
            // or by pasting something inside the textarea;

            // remove commas with empty
            $(this).val($(this).val().replace(/,/,"","g"));
            // remove carriage returns (\r) and newlines (\n) with spaces:
            $(this).val($(this).val().replace(/\r?\n/g, ' '));
            //$textarea.val($textarea.val().replace(/[,\n\r]/," ","g");
        }
        // the enter key has been pressed, avoid producing a carriage return from it:
        if (e.which === 13 || e.which === 188) {
            e.preventDefault();
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('input').not(".searchBox").on('keydown keyup change ', function (e) {
        $("#btnSaveForm").removeAttr('disabled');
        $("#btnCancel").removeAttr('disabled');
        $("#formChanged").val("Y");   // Re-enable the form "changed" status
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('input').not(".searchBox").not(".qInput").on('keydown keyup change ', function (e) {
        $("#btnSaveForm").removeAttr('disabled');
        $("#btnCancel").removeAttr('disabled');
        $("#formChanged").val("Y");   // Re-enable the form "changed" status
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $('.qInput').on('keydown keyup change ', function (e) {
        $("#btnSaveLibQuestion").removeAttr('disabled');
        $("#msgAddQ").hide();
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnCancelLibQuestion").click(function(){
        $(".qInput").val("");
        $("#btnSaveLibQuestion").prop('disabled', true);
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnDeleteForever").click(function(){
        //Have user Confirm the Delete Action
        showConfirmation("Confirm Form Deletion", "Are you sure you want to delete this Form?", "Delete Form", "delForm");
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnAlertConfirm").click(function () {
        ////Once the user confirms the Alert execution, call the function that was set
        //// Retrieve String Name of function we want to run and find the object
        //var funcToExecute = $("#funcToExec").val();
        //var fn = window[funcToExecute];
        //alert("You are about to Execute Funcion: " + funcToExecute);
        //// If object a function, execute it?
        //if (typeof fn === "function") fn();

        var cft_id = $("#cft_id").val();
        // Perform an Ajax call to either hard or soft delete the current form
        $.ajax({
            //url: '@Url.Action("deleteForm", "ColFormTemplate")',
            url: '/ColFormTemplate/deleteForm',
            method: "POST",
            cache: false,
            data: { cft_id: cft_id },     //<---- Data Parameters (if not already passed in the Url)
            //data: JSON.stringify({ 'Options': someData }),           //<--- In case we wanted to post a JSON data object
            //dataType: "json",                                        //<--- In case we wanted to post a JSON data object
            //contentType: "application/json; charset=utf-8",          //<--- In case we wanted to post a JSON data object
            //traditional: true,                                       //<--- In case we wanted to post a JSON data object

            //--- On error, execute this function ------
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Failed to Remove Form id " + cft_id + " from Database !!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
            }
        }).done(function (d) {
            //Execute this code After the Ajax call completes successfully
            if (d == "True") {
                //alert("Form Id: " + cft_id + " has been Deleted!");
                //close modal Popup
                $('#confPopUp').modal('toggle');
                location.href = "/ColFormTemplate/";
            }
            else {
                alert("ERROR: Form could not be removed from the Database!\Error: " + d);
                location.href = "/ColFormTemplate/AddEditForm/" + cft_id;
            }
        });
    });
    //-----------------------------------------------------------------------------------------------   

    $(".sectionTitle").on("change", function(){
        if ($(this).val() == "")
        {
            $(this).css({
                "color": "white",
                "background-color": "red"
            });
        }
        else{
            //Perform 'error reset' steps after a valid selection is done from the autocomplete list
            $('#btnSaveForm').button('reset');
            $(this).css({
                "color": "black",
                "background-color": "white"
            });
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $(window).bind('beforeunload', function () {
        if($("#formChanged").val() == "Y") {
            //if the form has been changed
            return 'There are unsaved changed. Do you want to discard Changes and Exit?';
            //var response =
            //$( "#dialog-confirm-exit" ).dialog({
            //    resizable: false,
            //    height:250,
            //    modal: true,
            //    buttons: {
            //        "Discard Changes": function() {
            //            //$("#formChanged").val("N");
            //            //isChanged.val("N");
            //            response = true;
            //        },
            //        Cancel: function() {
            //            //$( this ).dialog( "close" );
            //            response = false;
            //        }
            //    }
            //});
            //return response;
        }
        else {
            return undefined;
        }
    });
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnSaveNewSelAnswer").click(function () {
        // **** SAVE ACTION FROM MODAL POPUP QUESTION MAINTENANCE ****

        //Somehow this even is always executed twice when invoked 
        //Workaround, set an event counter and just execute it when the value is zero (First time)
        if ($("#eventCounter").val() == "0") {
            // ------------------- Before Saving data, validate the user input -------------------------------------------------
            var formValidated = true;
            var emptyBoxes = 0;
            var textValues = "";
            var counter = 0;
            var validationMessage = "";

            var selCathegory = getATcategory();

            //alert("Checking... " + $("#eventCounter").val());
            // ----------- First verify that no boxes have been left blank and compile the New Answer String for posting ---------------
            //var comparisonNewString = $("#divNewSA").find("#fullSelATlist").first().children("option").filter(":selected").text();  //Save the answer Type that is being added
            $('#newQATsection').find('.selAnswerTextb').each(function () {
                counter = counter + 1;
                if ($(this).val() != "") {
                    textValues = textValues + $(this).val().trim().toUpperCase() + "~";
                    //comparisonNewString = comparisonNewString + $(this).val().trim().toUpperCase();
                }
                else {
                    emptyBoxes++;
                }
            });

            //Check if we have empty boxes, then the form is not validated.
            var selectedText = $("#fullSelATlist").find('option:selected').text().substring(0, 5);
            if (selCathegory == "3 Val Range" && emptyBoxes > 2) {
                formValidated = false;
                validationMessage = "No textbox values can be left blank";
            }
            else if (selCathegory == "5 Val Range" && emptyBoxes > 0) {
                formValidated = false;
                validationMessage = "No textbox values can be left blank";
            }
            else if (selectedText == "Multi" || selectedText == "Singl") {
                //If user is entering a List, verify that the list contains at least two item
                var filledInBoxes = 0;
                $("#multiList").find(".selAnswerTextb").each(function () {
                    if ($(this).val().trim() != "") {
                        filledInBoxes = filledInBoxes + 1;
                    }
                });
                if (filledInBoxes < 2) {
                    formValidated = false;
                    validationMessage = "A valid list must contain at least two elements.";
                }
            }
            // ------- Finished looking for boxes left blank------------------------------------------------

            // If the form input is valid, procced with saving data, else just display the Error message
            if (formValidated) {
                // Proceed to Save the new Answer Type
                //Parameter to post are:  QuestionId~AnswerTypeId~defaultT/F~answerlist

                $(this).button('loading');  // Disable the button until posting is complete

                //Build the posting parameters: "Question Id" plus "Answer Type Id" and hardcode default="false" plus sel answers from "texvalues" variable
                var postData = $("#popupQID").val() + "~" + $("#fullSelATlist").val() + "~false~" + textValues;

                //$(".qatDIV").each(function(){
                //    var postAnsTypeId = $(this).find("#ans_type_id").first().val();
                //    var postIsDefault = "false";
                //    var postSelAnswers = $(this).find("#ans_type_selAns").first().val();
                //    postData = postData + postQid +  "~" + postAnsTypeId + "~" + postIsDefault + "~" + postSelAnswers + ",";
                //});

                //alert("Saving New Answer Types...\n" + postData);
                $.ajax({
                    //url: '@Url.Action("saveNewSelAnswer", "ColFormTemplate")',
                    url: '/ColFormTemplate/saveNewSelAnswer',
                    method: "POST",
                    cache: false,
                    data: { ans_type_list: postData },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert("ERROR: Failed to Save the New Question Answer Type Information.!!\nError:" + textStatus + "," + errorThrown);  //<-- Trap and alert of any errors if they occurred
                    }
                }).done(function (d) {
                    if (d.substring(0, 3) == "QAT") {
                        //alert("Data posted successfully!\n" + d);
                        // Set the popupstatus to "saved" to indicate that the post succeded and that the drop down needs to be updated.
                        $("#popupStatus").val("saved");
                        // Reset the list of answers that have already been saved so they do not need to be displayed anymore
                        $("#divaddedSA").html("");
                        $("#savedQATid").val(d.substring(6));
                        $("#editQATWindow").modal('hide');
                    }
                    else {
                        //alert("Server Response: " + d);
                        $("#newErrorMsg").html("<span>ERROR SAVING INFORMATION:</span><br/><span> " + d + "</span>");
                        $("#newErrorMsg").show();
                    }
                    //else { alert("WARNING: Failed to post!\nPlease review your input and try again: " + d); }
                });

                // ----------- Finished Saving New Answer Type and Answer values ----------------
            }
            else {
                var errorMessage = "<span> VALIDATION ERROR: Please review your input:</span><br/>&nbsp;" + validationMessage + "<span>";
                //errorMessage = errorMessage.concat("</span><br/>&nbsp;&nbsp;<span>You are comparing Answer Type Id and values:</span><br/>");
                //errorMessage = errorMessage.concat("<span>'" + comparisonNewString + "'</span>" + "<span> against: </span>");
                //errorMessage = errorMessage.concat("<span> '" + comparisonOldString + "'</span>");
                $("#newErrorMsg").html(errorMessage);
                $("#newErrorMsg").show();
            }

            // Finally, whether the save action failed or posted correctly, reset the "SAVE" button to avoid leaving it stcuk in "Loading..."
            $(this).button('reset');

            //--------------------------------------------------------------------------------------------------------
            $("#eventCounter").val("1");  //Increase the counter when finished so the next time the events is fired it will be ignored
            //--------------------------------------------------------------------------------------------------------
        }
        else {
            //--------------------------------------------------------------------------------------------------------
            $("#eventCounter").val("0");   //Reset the counter for next time a button on the pop up is clicked it will execute
            //--------------------------------------------------------------------------------------------------------
        }
    });

    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    $("#btnPublishForm").click(function(){
        $(this).button('loading');
        document.getElementById("isPublished").value = "true"      //We are Publishing the form

        // Validate that a valid Start Date is entered when the Form is being published
        if ($('#cft_eff_st_dt').val() == "") {
            $('#btnPublishForm').button('reset');
            alert("The Effective Start Date is required when Publishing a Form");
            $('#cft_eff_st_dt').addClass("invalidField");
            $('#cft_eff_st_dt').focus();
        }
        else{
            //Set the Pop up dates to match what is currently selected on the Form before showing the popup
            if (validateForm())  // Validate the Form. If it's Valid proceed
            {
                $("#conf_cft_eff_st_dt").val($("#cft_eff_st_dt").val());
                $("#conf_cft_eff_end_dt").val($("#cft_eff_end_dt").val());
                $("#confPubDates").modal();
            }
        }
    });

    $("#accordion").on

});

//--------------------------------------------------------------------------------------------------------
//WARNING: AJAX calls used in Razor MVC cannot be changed to .js external file unless they are modifed
//.js does not reconnize inline HTML
//As long as you use inline razor syntax like @(Url.Action(...) )    you can't move it to js file
//You can do it in either specifying url like
//    url: '/Project/ProjectPartial',        
//    or in View.cshtml
//    <script type="text/javascript">
//    var projectUrl="@(Url.Action("ProjectPartial", "Project"))"
//    </script>
// And reference the variable on the .js side
//    in RenderParial.js
//url: projectUrl,
//--------------------------------------------------------------------------------------------------------
