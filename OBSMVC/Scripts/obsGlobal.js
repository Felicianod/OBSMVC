
//Function to replace the browser Confirmation popup with a nicer bootstraps formatted window.
//Requieres "bootstrap-dialog.js" & "bootstrap-dialog.css"
function obsConfirm(myMessage) {
    BootstrapDialog.confirm({
        title: 'CONFIRMATION - Are you Sure?',
        message: myMessage,
        type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
        //closable: false, // <-- Default value is false (Only closable through button click selection)
        draggable: true, // <-- Default value is false
        btnOKLabel: 'YES', // <-- Default value is 'OK',
        btnCancelLabel: 'NO', // <-- Default value is 'Cancel',
        btnOKClass: 'btn-warning', // <-- If you didn't specify it, dialog type will be used,
        callback: function (result) {
            // result will be true if button was click, while it will be false if users close the dialog directly.
            if (result) {
                // DO Something Upon Confirmation
                alert('Thanks for Confirming!');
            } else {
                //alert('Nope.');
            }
        },
    });
};

//Function to replace the browser Alert popup with a nicer bootstraps formatted alert.
function obsAlert(myMessage) {
    BootstrapDialog.show({
        title: 'Observations PRO Alert',
        size: BootstrapDialog.SIZE_LARGE,   // options: BootstrapDialog.SIZE_NORMAL, .SIZE_SMALL, .SIZE_WIDE, .SIZE_LARGE
        //type: BootstrapDialog.TYPE_PRIMARY,  //TYPE_PRIMARY is the default
        //cssClass: 'myStyle',        //If we want to give the modal dialog window a custom style (Must define the class "myStyle" in the STYLES section as ".myStyle .modal-dialog" )
        message: myMessage,
        //closable: true,                // "true" is the default
        //animate: false,                // "true" is the default (Animate as a slide down fade in)
        //onshow: function (dialog) {
        //    dialog.getButton('buttonName').disable();
        //},
        draggable: true,    //Or false as needed (Default is false)
        buttons: [{
            label: 'OK',
            hotkey: 13, // This button will respond to the "Enter" Key  
            cssClass: 'btn-primary',
            autospin: true,    // "false" is the default. Turn button to autospin when clicked
            //action: function (dialog) {
            //    //dialog.setTitle('Title 1');
            //    //dialog.setMessage('Message 2');
            //    //dialog.setType(type);  //"type" is [BootstrapDialog.TYPE_DEFAULT, BootstrapDialog.TYPE_INFO, BootstrapDialog.TYPE_PRIMARY, BootstrapDialog.TYPE_SUCCESS, BootstrapDialog.TYPE_WARNING or BootstrapDialog.TYPE_DANGER];    
            //    //dialog.realize();
            //    //dialog.getModalHeader().hide();
            //    //dialog.getModalFooter().hide();
            //    //dialog.getModalBody().css('background-color', '#0088cc');
            //    //dialog.getModalBody().css('color', '#fff');
            //},
            action: function (dialogItself) {
                //dialogItself.setClosable(false);     //To make dialog unclosable

                //dialogItself.enableButtons(false);  //Disable all modal buttons
                //dialogItself.getModalBody().html('Clossing in 2 seconds...');  //Change the Modal window Text
                //setTimeout(function () {
                //    dialogItself.close();
                //}, 2000);

                dialogItself.close();
            }
        }]
    });
};

