
// Sliding Global Title
//(function titleMarquee() {
//    document.title = document.title.substring(1) + document.title.substring(0, 1);
//    setTimeout(titleMarquee, 200);
//})();


//window.onbeforeunload = function (e) {
//    // Cancel the event
//    e.preventDefault();
//    // Chrome requires returnValue to be set
//    e.returnValue = 'Really want to quit the site?';
//};

//Prevent Ctrl+S (and Ctrl+W for old browsers and Edge)





document.onkeydown = function (e) {
    e = e || window.event;//Get event
    if (!e.ctrlKey) return;

    var code = e.which || e.keyCode;//Get key code
    switch (code) {
        case 67: //Block Ctrl+C
        case 65: //Block Ctrl+A
        case 88: //Block Ctrl+X
        case 73: //Block Ctrl+I
        case 80: //Block Ctrl+P
        //  case 86: //Block Ctrl+V
        case 85: //Block Ctrl+U
        case 83: //Block Ctrl+S
        case 87: //Block Ctrl+W -- Not work in Chrome and new Firefox
            e.preventDefault();
            e.stopPropagation();
            break;
    }
};
$(document).ready(function () {
    //  chosen list configure



    $(function () {
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "positionClass": "toast-bottom-right",
            "timeOut": 2000,
            "progressBar": true
        }

        var message = $("#alertmessage").val();
        var info = $("#alertinfo").val();
        var warning = $("#alertwarning").val();
        var error = $("#alerterror").val();
        var success = $("#alertsuccess").val();

        if (info != "") {
            toastr.info(info, "Info");
        }
        if (warning != "") {
            toastr.warning(warning, "Warninig");
        }
        if (error != "") {
            toastr.info(error, "Error");
        }
        if (message != "") {
            toastr.info(message, "Message");
        }
        if (success != "") {
            toastr.success(success, "Success");
        }

    });



    $(".tree_view").children('a').append('<i class="fa fa-angle-right"></i>');

    $(".tree_view").click(function () {
        $(this).children('a').children('i').toggleClass('down');
        $(this).children(".tree_view_menu").slideToggle();

    });
    //$(".openbt").click(function () {
    //    $("#mySidebar").css('visibility', 'visible');
    //    $("#mySidebar").css('width', '250px');
    //    $("#main").css('margin-left', '250px');
    //    $(".openbt").css('display', 'none');
    //});

    //$(".closebtn").click(function () {
    //    $("#mySidebar").css('visibility', 'hidden');
    //    $("#mySidebar").css('width', '0px');
    //    $("#main").css('margin-left', '0px');
    //    $(".openbt").css('display', 'block');
    //});
});
/*==================================================================
       Following Function prevent form submit without confimation
====================================================================*/
$(document).ready(function () {

   

    //  All Chosen list
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    var ids = [
        ".chosen_select",
        "#VendorId         ",
        "#SupplierId       ",
        "#Sales_TeamId     ",
        "#ClientId         ",
        "#closingbalance   ",
        "#Contractor"
    ]
    for (var selector in config) {
        for (var i = 0; i < ids.length; i++) {
            $(ids[i]).chosen(config[ids[i]]);
        }
    }
});
function LoadAddresssBook() {
    $.ajax({
        type: "Post",
        url: "/Country/GetCountryList",
        contentType: "html",
        success: function (response) {
            $("#Country").empty();
            $("#Country").append(response);
        }
    });
    $("#Country").change(function () {
        var countryId = $(this).val();
        $.ajax({
            type: "Post",
            url: "/Country/GetStateList?CountryId=" + countryId,
            contentType: "html",
            success: function (response) {
                $("#State").empty();
                $("#State").append(response);
            }
        });
    });
    $("#State").change(function () {

        var stateId = $(this).val();
        $.ajax({
            type: "Post",
            url: "/Country/GetCityList?StateId=" + stateId,
            contentType: "html",
            success: function (response) {
                $("#City").empty();
                $("#City").append(response);

            }
        });
    });
}


function Full_Page_Loading_Open() {
    $('body').append('<div class="fullPage_loading">Loading&#8230;</div>');
}

function Full_Page_Loading_Close() {
    $('.fullPage_loading').remove();
}
function OpenDivLoading(divid) {
    $('#' + divid + '').append('<div class="divloading">Loading&#8230;</div>');

}
function CloseDivLoading(divid) {
    $('#' + divid + '').children('.divloading').remove();
}
//=================================== Leads Function goes here =======================================//
function Leads() {
    $("#leads_table").on('click', '.followbtn', function () {
        var id = $(this).attr('data-val');
        var name = $(this).text();
        $("#nameofc").text(name);
        $('#exampleModal').modal('show');
        $("#guid").val(id);
        $("#details").attr('href', '/Leads/Details?Id=' + id);
    });


    $(".submit_btn").click(function () {
        var Id = $("#guid").val();
        var note = $.trim($("#Notes").val());
        var type = $("input[name='Type']:checked").val()
        var date = $("#fdate").val();
        if (Id != "" && note != "" && type != "") {
            Mark(Id, note, type, date);
        }
    });


    function Mark(id, notes, type, date) {
        $.ajax({
            url: "/leads/MarkLeads?Id=" + id + "&Notes=" + notes + "&Type=" + type + "&date=" + date,
            type: "GET",
            dataType: "Json",
        }).done(function (res) {
            alert(res);
            window.location.reload();
        });
    }
    //// Page Open Function
    //$(document).ready(function () {
    //    $('.body_element_container').attr('id', 'bodycontent');
    //    //$("a").click(function (e) {
    //    //    e.preventDefault(0);
    //    //    var text = $(this).attr("href");
    //    //    if (text.length > 1) {
    //    //        $("html").load(text);
    //    //    }
    //    //});
    //});

    //function PageOpen(Ad_Url) {
    //    var segments = Ad_Url.split('/');
    //    var lasturi = segments[5];
    //    //alert(url);  alert(lasturi);
    //    if (lasturi == 'pos_invoice') {
    //        $("#loaded_img").show();
    //        setTimeout(function () {
    //            $("#loaded_img").hide();
    //        }, 700);
    //        $('#bodycontent').load(Ad_Url);
    //        $("#add_item").focus();
    //        $('.sidebar-mini').addClass("sidebar-collapse");
    //        $(".removeContentwraper").removeClass("content-wrapper");
    //    } else {
    //        $("#loaded_img").show();
    //        setTimeout(function () {
    //            $("#loaded_img").hide();
    //        }, 700);
    //        $('#bodycontent').load(Ad_Url);
    //        $(".removeContentwraper").removeClass("content-wrapper");
    //        $('.sidebar-mini').removeClass("sidebar-collapse");
    //        //                    ========= its for auto scrol up ==============
    //        return $("html, body").animate({
    //            scrollTop: 0
    //        }, 600), !1
    //        //                    ============== auto scroll up close =========
    //    }
    //}
    //=========== its for special character remove =========
    function special_character_remove(vtext, id) {
        //                var specialChars = "<>@!#$%^&*()_+[]{}?:;|'\"\\/~`-=";
        var specialChars = "@!#$%^&*()_+[]{}?:;|'`/><";
        var check = function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true
                }
            }
            return false;
        }
        if (check($('#' + id).val()) == false) {
            // Code that needs to execute when none of the above is in the string
        } else {
            alert(specialChars + " these special character are not allows");
            $("#" + id).val('').focus();
            //            $("#customer_name").focus();
        }
    }
    //=========== its for only number allow=========
    function onlynumber_allow(vtext, id) {
        var specialChars = "<>@!#$%^&*()_+[]{}?:;|'\"\\/~`-=abcdefghijklmnopqrstuvwxyz"
        var check = function (string) {
            for (i = 0; i < specialChars.length; i++) {
                if (string.indexOf(specialChars[i]) > -1) {
                    return true
                }
            }
            return false;
        }
        if (check($('#' + id).val()) == false) {
            // Code that needs to execute when none of the above is in the string
        } else {
            alert(specialChars + " these special character are not allows");
            $("#" + id).val('').focus();
        }
    }

}




function daterange(cb) {
    var start = moment();
    var end = moment();
    $('#reportrange').daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment().endOf('month')],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
            'This Year': [moment().startOf('year'), moment().endOf('year')],
        }
    }, cb);

    cb(start, end);

};
$(document).ready(function () {


    var d = new Date();
    var month = d.getMonth() + 1;
    var day = d.getDate();
    var output = d.getFullYear() + '-' +
        (month < 10 ? '0' : '') + month + '-' +
        (day < 10 ? '0' : '') + day;

    $('input[type="date"]').val(output);

})

function loadTree() {
    $('#treebody').jstree({
        'core': {
            'data': {
                'url': '/AjaxData/Nodes',
                'dataType': 'json'
            },
        },
        'types': {
            "root": {
                "icon": "fa fa-building-o"
            },
            "chart": {
                "icon": "fa fa-folder-open"
            },
            "cat": {
                "icon": "fa fa-clone"
            },
            "ledger": {
                "icon": "fa fa-file"
            },
            "default": {

            }
        },
        plugins: ["search", "themes", "types"]
    })
    var to = false;
    $('#tree_q').keyup(function () {
        if (to) { clearTimeout(to); }
        to = setTimeout(function () {
            var v = $('#tree_q').val();
            $('#treebody').jstree(true).search(v);
        }, 250);
    });

    $('#treebody').on('changed.jstree',
        function (e, data) {
            var i, j, r = [];
            for (i = 0, j = data.selected.length; i < j; i++) {
                r.push(data.instance.get_node(data.selected[i]).text)
            };
            // alert('Selected: ' + r.join(', '));
            // $('#event_result').html('Selected: ' + r.join(', '));
        }).jstree();
}
function loadProdTree() {
    $('#ProdTree').jstree({
        'core': {
            'data': {
                'url': '/Costcenter/Nodes',
                'dataType': 'json'
            },
        },
        'types': {
            "root": {
                "icon": "fa fa-building-o"
            },
            "chart": {
                "icon": "fa fa-folder-open"
            },
            "group": {
                "icon": "fa fa-clone"
            },
            "prod": {
                "icon": "fa fa-file"
            },
            "default": {

            }
        },
        plugins: ["search", "themes", "types"]
    })


    $('#ProdTree').on('changed.jstree',
        function (e, data) {
            var i, j, r = [];
            for (i = 0, j = data.selected.length; i < j; i++) {
                r.push(data.instance.get_node(data.selected[i]).text)
            };
        }).jstree();
}

$("#btnShowModal").click(function () {
    $("#charttree").modal('show');
});
function UploadImage() {
    /* File Upload validate*/
    $('#ImageUpload').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('data-file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 1; //2MB
        var onlist = $(this).attr('data-file_types').indexOf(gettypeup) > -1;
        var checkinputfile = $(this).attr('type');
        numb = numb.toFixed(2);

        if (onlist && numb <= filesize) {
            $('.upload_alert').html('The Image is ready to upload').removeAttr('class').addClass('xd2'); //file OK
        }
        else {
            if (numb >= filesize && onlist) {
                $(this).val(''); //remove uploaded file
                $('.upload_alert').html('Added file is too big \(' + numb + ' MB\) - max file size ' + filesize + ' MB').removeAttr('class').addClass('xd'); //alert that file is too big, but type file is ok
            } else if (numb < filesize && !onlist) {
                $(this).val(''); //remove uploaded file
                $('.upload_alert').html('This file format is not allowed \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            } else if (!onlist) {
                $(this).val(''); //remove uploaded file
                $('.upload_alert').html('This file format is not allowed \(' + gettypeup + ') - allowed formats: ' + allowedfiles).removeAttr('class').addClass('xd'); //wrong type file
            }
        }
    });
    /* File upload preview*/
    $(document).ready(function () {
        var readURL = function (input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    $('.avatar').attr('src', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
        $(".file-upload").on('change', function () {
            readURL(this);
        });
    });


}

