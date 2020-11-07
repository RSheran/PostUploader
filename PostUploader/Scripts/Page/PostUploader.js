$(document).ready(function () {

    $("#btnUploadPost").click(UploadPostContent);
    $("#btnResetPost").click(ResetControls);
    HideErrorMsg();

    $("#dv-UploadedPost").html("");
    //alert('Hi!..the max content length is: ' + GetMaxContentLength() +'characters');

});

function ValidatePhraseIsNullOrEmpty(phraseParam) {
    if (phraseParam.length > 0 && phraseParam != null && typeof (phraseParam) !== 'undefined') {
        return true;
    }
    else {
        return false;
    }
}

function ValidatePhraseMaxSize(phraseParam) {
    if (phraseParam.length <= GetMaxContentLength()) {
        return true;
    }
    else {
        return false;
    }
}

function ValidateSpecialCharacters(phraseParam) {
    var splChars = "*|,\":<>[]{}`\';()@&$#%+-^)(%!='";
    var totalCharLength = phraseParam.length;
    var splCharLength = 0;
    var phraseParamCharArr = phraseParam.split('');
    for (var i = 0; i < phraseParamCharArr.length; i++) {
        if (splChars.indexOf(phraseParamCharArr[i]) != -1) {

            splCharLength += 1;
        }
    }

    if (totalCharLength == splCharLength) {
        return false;
    }
    else {
        return true;
    }
}

function GetMaxContentLength() {
    var maxLengthForContent = $("#hdnMaxContentLength").val();

    return maxLengthForContent;
}

function GetHtml(template) {
    return template.join('\n');
}

function SetBackgroundColourForElements() {
    var elements_Pos = document.getElementsByClassName('clsPositive');
    var elements_Neg = document.getElementsByClassName('clsNegative');

    for (var i = 0; i < elements_Pos.length; i++) {
        elements_Pos[i].style.backgroundColor = "lightgreen";
    }

    for (var i = 0; i < elements_Neg.length; i++) {
        elements_Neg[i].style.backgroundColor = "lightcoral";
    }
}


function UploadPostContent() {
    var postContent = $("#txtPostContent").val();
    var divContent = "";
    if (ValidatePhraseIsNullOrEmpty(postContent) == true) {
        HideErrorMsg();

        if (ValidatePhraseMaxSize(postContent) == true) {
            HideErrorMsg();

            if (ValidateSpecialCharacters(postContent) == true) {
                HideErrorMsg();
                objParam = JSON.stringify({ 'phraseToUpload': postContent });

                $.ajax({
                    url: '/PostUploader/PredictSentimentForPhrase',
                    type: 'post',
                    data: objParam,
                    contentType: 'application/json',
                    success: function (inputParam) {

                        if (inputParam != "") {

                            if (inputParam.toLowerCase().indexOf('negative') != -1) {

                                swal({
                                    title: "",
                                    text: "The content you are about to upload cannot be posted because it contains hate speech and violates the regulations of posting content. ",
                                    type: "error",
                                    timer: 5000,
                                    showConfirmButton: false
                                });


                             //   swal({
                             //       title: "Hate speech warning",
                             //       text: "The content you are about to upload contains hate speech.Are you sure to continue?",
                             //       type: "warning",
                             //       showCancelButton: true,
                             //       confirmButtonColor: "#01DF74",
                             //       confirmButtonText: "Yes,upload..",
                             //       showLoaderOnConfirm: true,
                             //       closeOnConfirm: false,
                             //       height: "100px",
                             //       width: "100px"
                             //   },
                             // function () {

                             //     UploadConfirmedContent(postContent,'neg');

                             //});


                               
                            }
                            else {
                               
                                UploadConfirmedContent(postContent, 'pos');
                               
                            }                          
                           

                            ResetControls();

                            
                        }
                        else {

                            swal({
                                title: "",
                                text: "Oops!..An error occured.",
                                type: "error",
                                timer: 3000,
                                showConfirmButton: false
                            });
                        }


                    }

                });
            }
            else {
                DisplayErrorMsg('Content cannot contain ONLY special characters.')
            }
        }
        else {
            DisplayErrorMsg('The content has exceeded the maximum allowed character limit of ' + GetMaxContentLength() + ' characters');
        }
    }
    else {
        DisplayErrorMsg('Cannot upload empty content..!');
    }

}

function UploadConfirmedContent(phrase,sentimentType)
{
    var currentdate = new Date();
    var ampmVal = currentdate.getHours() >= 12 ? 'pm' : 'am';

    var datetime =   currentdate.getDate() + "/"
                    + (currentdate.getMonth() + 1) + "/"
                    + currentdate.getFullYear() + " "
                    + currentdate.getHours() + ":"
                    + currentdate.getMinutes() + ":"
                    + currentdate.getSeconds()+" "
                    + ampmVal;
  
    swal({
        title: "",
        text: "Post has been uploaded. ",
        type: "success",
        timer: 3000,
        showConfirmButton: false
    });

    var divContent = "";

    if (sentimentType == 'pos')
    {
        var currentFrame = "";
        currentFrame = [
            '<div class="thumbnail">',
            '<p style="font-size:small;font-family:Segoe UI;">' + phrase + '</p><br/>',
            '<p style="font-size:small;font-family:Segoe UI;text-align:left"><b>' + datetime + '</b></p>',
            '</div><br/>'
        ];
    }
    else
    {
        var currentFrame = "";
        currentFrame = [
            '<div class="thumbnail">',
            '<p style="font-size:small;font-family:Segoe UI;">' + phrase + '</p>',
            '<p style="font-size:small;font-family:Segoe UI;text-align:left"><b>' + datetime + '</b></p>',
            '</div><br/>'
        ];
       
    }

    

    divContent += GetHtml(currentFrame);
    $("#dv-UploadedPost").append(divContent);

    SetBackgroundColourForElements();
   
}

function ResetControls() {
    $("#txtPostContent").val('');
}


function DisplayErrorMsg(message) {
    //HideSuccessMsg();
    $("#ErrorMessage").text(message);
    $("#showError").css('display', 'block');
    $(window).scrollTop($('#showError').offset().top);
}

function HideErrorMsg() {
    $("#ErrorMessage").text('');
    $("#showError").css('display', 'none');
}

