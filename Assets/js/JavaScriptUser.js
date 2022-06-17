const Confirm = {
    open(options) {
        options = Object.assign({}, {
            title: 'Are you Sure?',
            message: 'if you sure click "Confirm" else click "Cancel"',
            okText: 'Confirm',
            cancelText: 'Cancel',
            onok: function () { },
            oncancel: function () { }
        }, options);

        const html = `
    <div class="c-confirm-area">
        <div class="c-confirm-box">
            <div class="c-confirm-icon">
                <i class="fa fa-question"></i>
            </div>
            <div class="c-confirm-title">
                <h4>${options.title}</h4>
            </div>

            <div class="c-confirm-description">
                ${options.message}
            </div>
            <div class="c-confirm-controlls">
                <button class="btn btn-sm btn-danger" id="c-confirm-cancel"> <i class="fa fa-close"></i> ${options.cancelText}</button>
                <button class="btn btn-sm btn-success" id="c-confirm-confirm"><i class="fa fa-check"></i> ${options.okText}</button>
            </div>
        </div>
    </div> `;

        const template = document.createElement('template');
        template.innerHTML = html;

        // Identifier
        const confirm_box_area = template.content.querySelector('.c-confirm-area');
        const confirmButton = template.content.querySelector('#c-confirm-confirm');
        const cancelButton = template.content.querySelector('#c-confirm-cancel');

        confirm_box_area.addEventListener('click', e => {
            if (e.target === confirm_box_area) {
                options.oncancel();
                this._close(confirm_box_area);
            }
        });

        confirmButton.addEventListener('click', () => {
            options.onok();
            this._close(confirm_box_area);
        });

        cancelButton.addEventListener('click', () => {
            options.oncancel();
            this._close(confirm_box_area);
        });


        document.body.appendChild(template.content);
    },

    _close(confirmEl) {
        confirmEl.classList.add('confirm__close');

        confirmEl.addEventListener('animationend', () => {
            document.body.removeChild(confirmEl);
        });
    }
};

window.addEventListener('online', () => toastr.info("You are internet connection has been restored", "Online", { timOut: 300 }));
window.addEventListener('offline', () => toastr.warning("You are currently offline", "Offline", { timOut: 300 }));


$("body").on('click', 'a.loadPartial', function (e) {
    e.preventDefault();
    var url = $(this).attr('href');
    if ($(this).hasClass("require-confirm")) {
        Confirm.open({
            onok: function () {
                loadPartialbody(url);
            }
        });
    }
    else {
        loadPartialbody(url);
    }


});
$(function () {
    var width = window.innerWidth;
    $(".sidebar-menu").children("ul.menu").children("li.menu-item").click(function () {
        if (width < 576) {
            $("ul.sub-menu").hide();
        }
        if ($(this).has("ul.sub-menu")) {
            $(this).children("ul.sub-menu").slideToggle();
        }
    });

});
$(function () {
    var name = $(".sidebar-user-name span:last-child").html();
    var len = name.length;

    if (len > 12) {
        var si = 21 - (len - 12);
        $(".sidebar-user-name span:last-child").css("font-size", si + "px");
    }

    // Sum Api jquery datatable
    jQuery.fn.dataTable.Api.register('sum()', function () {
        return this.flatten().reduce(function (a, b) {
            if (typeof a === 'string') {
                a = a.replace(/[^\d.-]/g, '') * 1;
            }
            if (typeof b === 'string') {
                b = b.replace(/[^\d.-]/g, '') * 1;
            }
            return a + b;
        }, 0);
    });
});

function UploadImage() {
    /* File Upload validate*/
    $('#ImageUpload').on('change', function () {
        var numb = $(this)[0].files[0].size / 1024 / 1024; //count file size
        var resultid = $(this).val().split(".");
        var gettypeup = resultid[resultid.length - 1]; // take file type uploaded file
        var filetype = $(this).attr('file_types'); // take allowed files from input
        var allowedfiles = filetype.replace(/\|/g, ', '); // string allowed file
        var filesize = 0.5; //2MB
        var onlist = $(this).attr('file_types').indexOf(gettypeup) > -1;
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
                    $('.thumnail').attr('src', e.target.result);
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
        $(".file-upload").on('change', function () {
            readURL(this);
        });
    });


}
function ConfigureChosen() {
    var config = {
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-rtl': { rtl: true },
        '.chosen-select-width': { width: '95%' }
    }
    var ids = [".chosen_select"]
    for (var selector in config) {
        for (var i = 0; i < ids.length; i++) {
            $(ids[i]).chosen(config[ids[i]]);
        }
    }
}

function loadPartialbody(url) {

    if (url != "#") {
        var width = window.innerWidth;
        if (width < 576) {
            $("ul.sub-menu").hide();
        }
        $("#hiddenlink").val(url);
        $.ajax({
            type: "GET",
            dataType: "html",
            url: url,
            data: {},
            beforeSend: function () {
                ShowLoading(true);
            },
            success: function (response) {

                $("main").html(response);

            },
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                xhr.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var complete = evt.loaded / evt.total * 100;
                        //  var percentComplete = complete + "%";
                        // alert(complete);
                        // Do something with download progress
                        setProgress(complete);
                    }
                }, false);
                return xhr;
            },
            complete: function () {
                setTimeout(function () { ShowLoading(false) }, 500);
                loadPageTitleAndScript();
            }
        });

    }
}
// prevent loading  ============================================================= End =========

function MyProfile() {
    UploadImage();
}


// Set Progress of loading
function setProgress(current_progress) {
    // var interval = setInterval(function () {
    $("#u-progress-bar")
        .css("width", current_progress + "%")
        .attr("aria-valuenow", current_progress)
        .text(current_progress + "% loaded");
    //    if (current_progress >= 100)
    //        clearInterval(interval);
    //}, 100);
}

function loadPageTitleAndScript() {
    var title = $("main").find("#pagetitle").val();
    var jsfunction = $("main").find("#javascript").attr("data-function");


    if (typeof (jsfunction) != 'undefined') {
        window[jsfunction]();
        $("main").find("#javascript").attr("data-function", "");
    }

    $(".searchwrapper").html(title);
}

// Prevent Form submitting loading========================================================== Start
$("body").on('submit', 'form.ajaxform', function (e) {
    e.preventDefault();
    $("#hiddenlink").val("");
    var form = $(this);
    var formData = new FormData(this);
    var url = form.attr('action');
    var type = form.attr('method');

    $("#hiddenlink").val(url);
    $(this).removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
    Confirm.open({
        onok: function () {
            $.ajax({
                type: type,
                url: url,
                data: formData,
                beforeSend: function () {
                    ShowLoading(true);
                },
                //  data: form.serialize(), // serializes the form's elements.
                success: function (res) {
                    console.log(res);
                    if (res.Status === "OK") {
                        toastr.success(res.Message, "Success", timeOut = 3000);
                    }
                    if (res.Status === "Error") {
                        toastr.error(res.Message, "Error", timeOut = 3000);
                    }
                    if (res.Status === "Faild") {
                        toastr.error(res.Message, "Faild", timeOut = 3000);
                    }
                    if (res.Status === "Info") {
                        toastr.info(res.Message, "Information", timeOut = 2500);
                    }
                    if (res.Status === "Warning") {
                        toastr.warning(res.Message, "Warning", timeOut = 3000);
                    }
                    if (typeof (res.url) !== 'undefined') {
                        if (res.url.length > 0) loadPartialbody(res.url);
                    }
                },

                xhr: function (xhr) {
                    var xhr = new window.XMLHttpRequest();

                    // Upload progress
                    xhr.upload.addEventListener("progress", function (evt) {
                        if (evt.lengthComputable) {
                            var percentComplete = evt.loaded / evt.total * 100;
                            var cop = parseInt(percentComplete);
                            //Do something with upload progress
                            $("#cs-loader").children('span').html(cop + "%");
                        }
                    }, false);


                    return xhr;
                },
                error: function (ex) {
                    toastr.error(ex.statusText, "Error", timeOut = 3000);
                },
                complete: function () {
                    ShowLoading(false);
                },
                contentType: false,
                processData: false

            });
        }
    });
});

// Prevent Form submitting loading ======================




/*  ============================================================= Attendace controller Start ================================== */
function AttendanaceIndex() {
    loadTableData(null);
    $("#checkAttendance").change(function () {

        var da = $(this).val();
        if (da != "") {
            loadTableData(da);
        }
    });
    function loadTableData(date) {
        $("#attendance_Table").DataTable({
            "processing": true,
            "responsive": true,
            "bDestroy": true,
            "language": {
                processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
            },
            "lengthMenu": [[20, 50, 100, -1], [20, 50, 100, "All"]],
            "ajax": {
                "url": "/Attendance/AttendacesDate?Date=" + date,
                "type": "GET",
                "datatype": "json"
            },

            "columns": [
                {
                    "data": "Sl",
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }

                },

                { "data": "Employee" },
                { "data": "Entry" },
                { "data": "Out" },
                { "data": "Duration" },
                { "data": "EntryCount" },

            ],

            "dom": "lrBftip",
            "buttons": [
                {
                    extend: 'excel',
                    className: 'btn Button',
                    text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Export to Excel'
                }
            ]
        });
    }
}
function AttendanceEntry() {

    $("#SearchButton").click(function () {
        var depart = $("#DepartmentId").find("option:selected").val();
        var shift = $("#ShiftId").find("option:selected").val();
        var designation = $("#DesignationId").find("option:selected").val();
        //var settings = {
        //    url: "/Attendance/GetEmployee",
        //    data: { "DepartmentId": depart, "ShiftId": shift, "DesignationId": designation },
        //    contentType: "html",
        //    type: "GET",
        //    //beforeSend: function () {
        //    //    $(this).addClass('disabled');
        //    //}
        //}
        var furl = '/Attendance/GetEmployee?DepartmentId=' + depart + '&ShiftId=' + shift + '&DesignationId=' + designation;
        $("#searchemployeetable").load(furl);

        //$.ajax(settings).done(function (res) {
        //    $("#searchemployeetable").append(res);
        //});

    });


    $("#searchemployeetable").on('change', 'input.isleave', function () {

        var is = $(this).is(":checked");
        var ref = $(this).parent("td").next("td").children(".reference");
        var out = $(this).parent("td").prev("td").children(".out");
        var entry = $(this).parent("td").prev("td").prev("td").children(".entry");
        if (is) {
            ref.attr('required', 'required');
            ref.show();
            out.hide();
            entry.hide();
        }
        else {
            ref.hide();
            ref.attr('required', false);
            out.show();
            entry.show();
        }
    });

    LoadDesignationdropdown();
}
/*  ============================================= Attendace controller End ================================== */


// ==============================================Setup Controller Start ================================================
function SetupBasic() {

    LoadDepartment();
    LoadDesignation();
    LoadShift();
    loaddropdown();
    LoadAllowance();
    LoadDeduction();

    $("table").on('click', ".updatebtn", function () {
        var type = $(this).attr('data-type');
        var id = $(this).attr('data-id');
        var text = $(this).attr('data-text');
        switch (type) {
            case "Department":
                $("#DepartmentId").val(id);
                $("#DepartmentName").val(text);
                break;
            case "Shift":
                var start = $(this).attr("data-start");
                var end = $(this).attr('data-end');
                $("#shiftId").val(id);
                $("#offficeStart").val(start);
                $("#officeEnd").val(end);
                $("#ShiftName").val(text);
                break;
            case "Allowance":
                $("#AllowanceName").val(text);
                $("#AllowanceId").val(id);
                break;
            case "Deduction":
                $("#DeductionName").val(text);
                $("#DeductionId").val(id);
                break;
            default:
        }
    });
}
function loaddropdown() {
    var settings = {
        url: "/setup/GetData?type=Department",
        type: "GET",
    }
    $.ajax(settings).done(function (res) {
        var s = "<option>--Department--</option>";
        $.each(res.data, function (key, val) {
            s += '<option value="' + val.Id + '">' + val.Name + '</option>';
        });
        $("#DepartmentsId").append(s);
    })
}
function LoadDepartment() {
    $("#departmentTable").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": "/setup/GetData?type=Department",
        "dom": "",
        "columns": [
            { "data": "Id" },
            { "data": "Name" },
            {
                "render": function (data, type, row, meta) {
                    var s = '<button class="updatebtn btn btn-sm" data-id="' + row.Id + '" data-text="' + row.Name + '" data-type="Department"><i class="fa fa-pencil"></i></button>';
                    return s;
                }
            }

        ]
    });
}
function LoadShift() {
    $("#shifttable").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": "/setup/GetData?type=Shift",
        "dom": "",
        "columns": [
            { "data": "Id" },
            { "data": "Name" },
            { "data": "Start" },
            { "data": "End" },
            {
                "render": function (data, type, row, meta) {
                    var s = '<button class="updatebtn btn btn-sm" data-start="' + row.Start + '" data-end="' + row.End + '" data-id="' + row.Id + '" data-text="' + row.Name + '" data-type="Shift"><i class="fa fa-pencil"></i></button>';
                    return s;
                }
            }

        ]
    });
}
function LoadAllowance() {
    $("#allowanceTable").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": "/setup/GetData?type=Allowance",
        "dom": "",
        "columns": [
            { "data": "Id" },
            { "data": "Title" },
            {
                "render": function (data, type, row, meta) {
                    var s = '<button class="updatebtn btn btn-sm"  data-id="' + row.Id + '" data-text="' + row.Title + '" data-type="Allowance"><i class="fa fa-pencil"></i></button>';
                    return s;
                }
            }

        ]
    });
}
function LoadDeduction() {
    $("#deductionTable").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": "/setup/GetData?type=Deduction",
        "dom": "",
        "columns": [
            { "data": "Id" },
            { "data": "Title" },
            {
                "render": function (data, type, row, meta) {
                    var s = '<button class="updatebtn btn btn-sm"  data-id="' + row.Id + '" data-text="' + row.Title + '" data-type="Deduction"><i class="fa fa-pencil"></i></button>';
                    return s;
                }
            }

        ]
    });
}
function LoadDesignation() {
    $("#designationtable").DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": "/setup/GetData?type=Designation",
        "dom": "",
        "columns": [
            { "data": "Id" },
            { "data": "Name" },
            {
                "render": function (data, type, row, meta) {
                    var s = "";
                    $.each(row.Designation, function (index, val) {
                        s += val + "<br>"
                    })

                    return s;
                }
            }

        ]
    });
}
function SetupPerformance() {

}
// ==============================================Setup Controller End ================================================

/*  ======================================Employee Controller Start===========================================  */

function Employeeindex() {
    $("#employeeTable").DataTable({

        "responsive": true,
        "processing": true,
        "language": {
            processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
        },
        "lengthMenu": [[20, 30, 50, 100, -1], [20, 30, 50, 100, "All"]],
        "ajax": {
            "url": "/Employees/GetEmployeeListData",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "Sl",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "Code" },
            { "data": "Name" },
            { "data": "Designation" },
            {
                "render": function (data, row, type, meta) {
                    var s = type.Phone + ", " + type.Phone1 + ", " + type.Phone2;
                    return s;
                }
            },
            { "data": "JoiningDate" },
            { "data": "LeaveDate" },

            {
                "render": function (data, type, full, meta) {
                    return '<a style="color:#00a65a;" href="/Employees/Details/' + full.Id + '" class="loadPartial"><i class="fa              fa-eye"></i>';
                }
            }
        ],
        "dom": "lrBftip",
        "buttons": [
            {
                extend: 'excel',
                className: 'btn Button',
                text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Export to Excel',
                attr: {
                    title: "This table is export in excel format.",
                }
            }
        ]
    });
}
function EmployeeCreate() {
    LoadDesignationdropdown();
    UploadImage();
}
function EmployeeUpdate() {
    LoadDesignationdropdown();
    UploadImage();
}
function LoadDesignationdropdown() {
    $("#DepartmentId").change(function () {
        $("#DesignationId").empty();
        var id = $(this).find("option:selected").val();
        if (id != "") {
            var settings = {
                url: "/Employees/GetDesignation?Id=" + id,
                type: "GET",
                beforeSend: function () {
                    var s = "<option>Loading...</option>";
                    $("#DesignationId").append(s);
                }
            }
            $.ajax(settings).done(function (data) {
                $("#DesignationId").empty();
                var s = "<option>--Select Designation--</option>";
                $.each(data, function (key, val) {
                    s += '<option value="' + val.Id + '">' + val.Text + '</option>';
                });
                $("#DesignationId").append(s);
            })
        }
    });
}
function EmployeeJobExperience() {
    $("#stillworking").click(function () {
        if ($(this).is(':checked')) {
            $("#ToDate").val("").css('display', 'none');
        }
        else {
            $("#ToDate").val("").css('display', 'block');
        }
    });
}

/*  =====================================Employee Controller End=====================================  */



/*  ==================================Setting Controller Start=======================================  */

function SettingOther() {
    $(".check-box").after('<span class="slider round"></span>');
}

function SettingActivity() {
    $('#activity_log_table').DataTable({
        "responsive": true,
        "processing": true,
        "language": {
            processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
        },
        "lengthMenu": [[10, 20, 50, 100, -1], [10, 20, 50, 100, "All"]],
        "pageLength": 20,
        "dom": "lrBftip",
        "buttons": [
            {
                extend: 'excel',
                className: 'btn Button',
                text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Export to Excel'
            }
        ],
        "ajax": {
            "url": "/Setting/Get_Activity_logs",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "Sl",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "date" },
            { "data": "user" },
            { "data": "activity" },
            { "data": "browser" },
            { "data": "ip" },
        ],
    });
}
function SettingLoginHistory() {
    $("#login_his_table").DataTable({
        "responsive": true,
        "processing": true,
        "language": {
            processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
        },
        "lengthMenu": [[10, 20, 50, 100, -1], [10, 20, 50, 100, "All"]],
        "pageLength": 20,
        "dom": "lrBftip",
        "buttons": [
            {
                extend: 'excel',
                className: 'btn Button',
                text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Export to Excel'
            }
        ],
        "ajax": {
            "url": "/Setting/Get_Login_History",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "Sl",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { "data": "Date" },
            { "data": "User" },
            { "data": "Ip" },
            { "data": "Device" },
            { "data": "Browser" },

        ],

    });
}
/*  =============================================Setting Controller End====================================  */

/*  ==================================Payrolls Controller Start=======================================  */
function PayrollSetup() {
    $("#add_allowence").click(function (e) {
        var al_Id = $("#AllowenceId").find('option:selected').val();
        var amount = $("#amount").val();
        var ifp = $("#AlPercent").find("Option:selected").val();
        if (ifp > 0) {
            if (amount > 100 || amount <= 0) {
                alert("Percentage value is not valid!");
                return false;
            }
        }
        if (al_Id == null || amount <= 0) {
            alert("Please Select Allowence and enter amount");
            return false;
        }
        var setting = {
            url: "/Payrolls/Set_Allowence?Id=" + al_Id + "&Amount=" + amount + "&Percent=" + ifp,
            type: "Json",
            method: "POST",
        }
        $.ajax(setting).done(function (data) {
            console.log(data);
            $("#allowence_table tbody").empty();
            $.each(data, function (key, value) {
                var tr = "<tr><td></td><td>" + value.Title + "</td><td>" + value.Amount + "</td><td>" + checkP(value.Percent) + "</td><td><a type='button' href='#' value='" + value.Id + "' id='deletethis'><i class='fa fa-window-close' aria-hidden='true'></i></a></td></tr>";
                $("#allowence_table tbody").append(tr);
            });
            autoserial2("allowence_table");
        })
    });
    $("#add_deduction").click(function (e) {
        var al_Id = $("#DeductionId").find('option:selected').val();
        var amount = $("#damount").val();
        var ifp = $("#ddPercent").find("option:selected").val();
        if (ifp > 0) {
            if (amount > 100 || amount <= 0) {
                alert("Percentage value is not valid!");
                return false;
            }
        }
        if (al_Id == null || amount <= 0) {
            alert("Please Select Deduction and enter amount");
            return false;
        }
        var setting = {
            url: "/Payrolls/Set_Deduction?Id=" + al_Id + "&Amount=" + amount + "&Percent=" + ifp,
            type: "Json",
            method: "POST",
        }
        $.ajax(setting).done(function (data) {
            console.log(data);
            $("#deduction_table tbody").empty();
            $.each(data, function (key, value) {
                var tr = "<tr><td></td><td>" + value.Title + "</td><td>" + value.Amount + "</td><td>" + checkP(value.Percent) + "</td><td><a type='button' href='#' value='" + value.Id + "' id='ddeletethis'><i class='fa fa-window-close' aria-hidden='true'></i></a></td></tr>";
                $("#deduction_table tbody").append(tr);
            });
            autoserial2("deduction_table");
        })
    });


    $("#allowence_table").on("click", "#deletethis", function () {

        var val = $(this).attr("value");
        var setting = {
            url: "/Payrolls/DeleteAllowence?Id=" + val,
            type: "POST",
        }
        $.ajax(setting).done(function (data) {
            $("#allowence_table tbody").empty();
            $.each(data, function (key, value) {
                var tr = "<tr><td></td><td>" + value.Title + "</td><td>" + value.Amount + "</td><td>" + checkP(value.Percent) + "</td><td><a type='button' href='#' value='" + value.Id + "' id='deletethis'><i class='fa fa-window-close' aria-hidden='true'></i></a></td></tr>";
                $("#allowence_table tbody").append(tr);
            });
            autoserial2();
        });
    });

    $("#deduction_table").on("click", "#ddeletethis", function () {

        var val = $(this).attr("value");
        var setting = {
            url: "/Payrolls/DeleteDeduction?Id=" + val,
            type: "POST",
        }
        $.ajax(setting).done(function (data) {
            $("#deduction_table tbody").empty();
            $.each(data, function (key, value) {
                var tr = "<tr><td></td><td>" + value.Title + "</td><td>" + value.Amount + "</td><td>" + checkP(value.Percent) + "</td><td><a type='button' href='#' value='" + value.Id + "' id='deletethis'><i class='fa fa-window-close' aria-hidden='true'></i></a></td></tr>";
                $("#deduction_table tbody").append(tr);
            });
            autoserial2("deduction_table");
        });
    });
    function checkP(val) {
        if (val == 1) {
            return "% of Basic Salary";
        }
        else if (val == 3) {
            return " % Gross Salary ";
        }
        else {
            return " Fixed Amount ";
        }
    };
    function autoserial2(tableid) {
        $('#' + tableid + ' tbody tr').each(function (index) {
            $(this).find('td:nth-child(1)').html(index + 1);
        });
    };
}
/*  ==================================Payrolls Controller End=======================================  */

/*  ==================================Salary sheets  Controller Start=======================================  */
function salarysheetsperformance() {
    $("#SearchButton").click(function () {
        var depart = $("#DepartmentId").find("option:selected").val();
        var shift = $("#ShiftId").find("option:selected").val();
        var designation = $("#DesignationId").find("option:selected").val();
        var settings = {
            url: "/Salarysheets/GetEmployee",
            data: { "DepartmentId": depart, "ShiftId": shift, "DesignationId": designation },
            type: "GET",
            contentType: "html",
            beforeSend: function () {
                $(this).addClass('disabled');
            }
        }
        $.ajax(settings).done(function (res) {
            $("#loademployeetable").empty();
            if (res.length > 0) {
                $("#submit-button").removeClass('disabled').attr('type', 'submit');
            }
            $("#loademployeetable").append(res);
        });

    });
}

function SalarySheetsAdvanceHistory() {
    $("#advanceandloanTable").DataTable({
        "processing": true,
        "responsive": true,
        "language": {
            processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
        },
        "lengthMenu": [[20, 50, 100, -1], [20, 50, 100, "All"]],
        "pageLength": 20,
        "dom": "lrBftip",
        "buttons": [
            {
                extend: 'excel',
                className: 'btn Button',
                text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Export to Excel'
            }
        ],
        "ajax": {
            "url": "/Salarysheets/Get_Advance_Salary_Data",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "Sl",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },

            { "data": "Name" },
            { "data": "position" },
            { "data": "Join_Date" },
            { "data": "Total_Advance" },
            { "data": "Paid" },
            {
                render: function (data, type, row, meta) {
                    var ad = parseFloat(row.Total_Advance, 2);
                    var p = parseFloat(row.Paid, 2);
                    return ad - p;
                }
            }
        ],
    });
}
/*  ==================================Salary sheets  Controller End=======================================  */


/*  ================================== Leaves Controller Start=======================================  */
function LeavesEntry() {
    $("#EmployeeId").change(function () {
        loadLeaveData();
    });
    $("#LeaveTypeId").change(function () {
        loadLeaveData();
    });


    function loadLeaveData() {
        var EmployeeId = $("#EmployeeId").find('option:selected').val();
        var LeaveTypeId = $("#LeaveTypeId").find('option:selected').val();
        if (EmployeeId != "" && LeaveTypeId != "") {
            $("#leavetakenmessge").html("");
            $.ajax("/Leaves/GetLeavesRec?EmployeeId=" + EmployeeId + "&Type=" + LeaveTypeId).done(function (res) {
                if (res.length > 0) {
                    $("#leavetakenmessge").html(res);
                }
            });
        }
    }
}
/*  ================================== Leaves Controller End=======================================  */

/*  ================================== Accounting Controller Start=======================================  */
function ManageCategory() {
    ConfigureChosen();
}
function ManageLedger() {
    ConfigureChosen();
}
function newTransactionVoucher() {
    $("#trans-submit-btn").hide();
    ConfigureChosen();
    $("#add-button").click(function () {
        var lid = $("#LedgerId").find('option:selected').val();
        var amount = $("#transAmount").val();
        var Type = $(".transtype:checked").val();
        if (lid == "") {
            toastr.warning("Please select ledger", "Wrong", { timeOut: 3000 });
        }
        else if (amount <= 0 || amount == "") {
            toastr.warning("Please enter valid amount", "Wrong", { timeOut: 3000 });
        }
        else if (typeof (Type) == 'undefined') {
            toastr.warning("Please select Debit / Credit", "Wrong", { timeOut: 3000 });
        }
        else {
            var settings = {
                url: "/Accounting/StoreTrans",
                data: { "LedgerId": lid, "Amount": amount, "Type": Type },
                type: "POST",
                beforeSend: function () {
                    ShowLoading(true);
                }
            }
            $.ajax(settings).done(function (resp) {
                FormatTransactonVoucherTable(resp);
                ShowLoading(false);
            }).fail(function (ex) {
                console.log(ex);
                ShowLoading(false);
            });
        }
    });
    $("#transvouchertable tbody").on('click', '.remove', function () {
        var id = $(this).attr('data-id');
        if (id != "") {
            var settings = {
                url: "/Accounting/RemoveTrans?LedgerId=" + id,
                type: "Post",
                beforeSend: function () {
                    ShowLoading(true);
                }
            }
            $.ajax(settings).done(function (res) {
                FormatTransactonVoucherTable(res);
                ShowLoading(false);
            }).fail(function (ex) {
                console.log(ex);
            })
        }
    })

    function FormatTransactonVoucherTable(data) {
        $("#transvouchertable tbody").empty();
        var i = 0;
        var s = '';
        var toD = 0;
        var toC = 0;
        $.each(data, function (k, v) {
            i += 1;
            toD += v.Debit;
            toC += v.Credit;
            s += '<tr><td>' + i + '</td><td>' + v.Name + '</td><td class="text-right">' + v.Debit + '</td><td class="text-right">' + v.Credit + '</td><td class="text-center"><button type="button" class="btn btn-sm btn-danger remove" data-id="' + v.Id + '">&times;</button></td></tr>';
        });
        $("#transvouchertable tbody").append(s);
        $("#totalDebit").html(toD);
        $("#totalCredit").html(toC);
        if (toD == toC) {
            $("#trans-submit-btn").show();
        }
    }

}

function AccountingIndex() {
    ConfigureChosen();
    hideExtraInput();
    $("#account_index_go_to").change(function () {
        hideExtraInput();
        var val = $(this).find('option:selected').val();
        switch (val) {
            case "balance":
                $(".to-date-field").slideDown(500);
                $("#account_index_go_button").show();
                break;
            case "profitloss":
                $(".from-date-field").slideDown(500);
                $(".to-date-field").slideDown(500);
                $("#account_index_go_button").show();
                break;
            case "ledger":
                $("#Ledger").next("div").slideDown(1000);
                $(".from-date-field").slideDown(500);
                $(".to-date-field").slideDown(500);
                $("#account_index_go_button").show();
                break;
            case "category":
                $("#LedgerCategory").next("div").slideDown(500);
                $(".from-date-field").slideDown(500);
                $(".to-date-field").slideDown(500);
                $("#account_index_go_button").show();
                break;
            case "choa":
                $("#ChartofAccount").slideDown(500);
                $(".from-date-field").slideDown(500);
                $(".to-date-field").slideDown(500);
                $("#account_index_go_button").show();
                break;
            case "AllVoucher":
                loadPartialbody("/Reports/All_Voucher");
                break;
            default:
                $("#account_index_go_button").hide();
                break;
        }
    });

    function hideExtraInput() {
        
        $("#account_index_go_button").hide();
        $("#ChartofAccount").hide();
        $("#LedgerCategory").hide().next("div").hide();
        $("#Ledger").hide().next("div").hide();
        $(".from-date-field").hide();
        $(".to-date-field").hide();
    }

    $("#account_index_go_button").click(function () {
       
        var type = $("#account_index_go_to").find("option:selected").val();
        var from = $("#fromDate").val();
        var to = $("#toDate").val();
        var led = $("#Ledger").find("option:selected").val();
        var ledCat = $("#LedgerCategory").find("option:selected").val();
        var chart = $("#ChartofAccount").find("option:selected").val();
        switch (type) {
            case "balance":
                if (to == "" || to == null || typeof (to) === 'undefined') {
                    toastr.warning("Please Selected Date", "Date is Required", { timeOut: 3000 });
                }
                else {
                    loadPartialbody("/Reports/Balancesheet?To=" + to);
                }
                break;
            case "profitloss":
                if (to == "" || to == null || from =="" || from == null || typeof (to) === 'undefined' || typeof (from) === 'undefined' ) {
                    toastr.warning("Please Selected Date properly, from Date must be less than to date", "Date Missing", { timeOut: 3000 });
                }
                else {
                    loadPartialbody("/Reports/ProfitAndLoss?From=" + from+"&To="+to);
                }
                break;
            case "ledger":
                if (led == "" || led == null || typeof (led) === 'undefined') {
                    toastr.warning("Please Selected Ledger", "Ledger is Required", { timeOut: 3000 });
                }
                else if (to == "" || to == null || from == "" || from == null || typeof (to) === 'undefined' || typeof (from) === 'undefined') {
                    toastr.warning("Please Selected Date properly, from Date must be less than to date", "Date Missing", { timeOut: 3000 });
                }
                else {
                    loadPartialbody("/Reports/LedgerReport?Id=" + led+"&From="+from+"&To="+to);
                }
                break;
            case "category":
                if (ledCat == "" || ledCat == null || typeof (ledCat) === 'undefined') {
                    toastr.warning("Please Selected Ledger", "Ledger is Required", { timeOut: 3000 });
                }
                else if (to == "" || to == null || from == "" || from == null || typeof (to) === 'undefined' || typeof (from) === 'undefined') {
                    toastr.warning("Please Selected Date properly, from Date must be less than to date", "Date Missing", { timeOut: 3000 });
                }
                else {
                    loadPartialbody("/Reports/LedgerCategory?LedCategoryId="+ledCat+"&From="+from+"&To="+ to);
                }
                break;
            case "choa":
                if (chart == "" || chart == null || typeof (chart) === 'undefined') {
                    toastr.warning("Please Selected Chart", "Chart of Account is Required", { timeOut: 3000 });
                }
                else if (to == "" || to == null || from == "" || from == null || typeof (to) === 'undefined' || typeof (from) === 'undefined') {
                    toastr.warning("Please Selected Date properly, from Date must be less than to date", "Date Missing", { timeOut: 3000 });
                }
                else {
                    loadPartialbody("/Reports/ChartofAccount?Id="+chart+"&From="+from+"&To=" + to);
                }
                break;
            default:
                $("#account_index_go_button").hide();
                break;
        }
    });
}
/*  ================================== Accounting Controller End=======================================  */



/** ========================================== Report Controller Start ================================= */
function LedgerReports() {
    $("#ledgerReportDetails").DataTable({
        "processing": true,
        "ordering":false,
        "ajax": "/Reports/LedgerDetailsReport",
        "dom": "",
        "columns": [
            { "data": "Date" },
            { "data": "Type" },
            { "data": "VoucherNo" },
            {
                "render": function (row, meta, full, an) {
                    if (full.Debit == null || full.Debit == 0) {
                        return "Dr. " + full.Particular;
                    }
                    else {
                        return "Cr. " + full.Particular;
                    }
                }
            },
            { "data": "Debit" },
            { "data": "Credit" },
            { "data": "Balance" },
        ]
    });
}
function All_Voucher() {
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var today = month + "/" + day + "/" + year;
    var to = month + "/" + day + "/" + year;
    LoadTableData("", today, to);

    $("#searchVoucher").click(function () {
        var type = $("#Type").find("option:selected").val();
        var from = $("#from").val();
        var to = $("#to").val();
        var fromdate = new Date(from);
        var todate = new Date(to);
        LoadTableData(type, from, to);
    });
    function LoadTableData(type, from, to) {
        $("#all_voucher_table").DataTable({
            "processing": true,
            "responsive": true,
            "language": {
                processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
            },
            "bDestroy": true,
            "lengthMenu": [[20, 50, 100, -1], [20, 50, 100, "All"]],
            "ajax": {
                "url": "/Reports/Get_All_Voucher?Type="+type+"&from=" + from + "&to=" + to,
                "type": "GET",
                "datatype": "json"
            },

            "columns": [
                {
                    "data": "Sl",
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { "data": "Date" },
                { "data": "No" },
                { "data": "Type" },
                { "data": "Amount" },
                { "data": "Narration" },
                {
                    "render": function (data, type, full, meta) {
                        return '<a href="/Reports/Details?id=' + full.Id + '" class="loadPartial"><i class="fa fa-eye"></i></a> ';
                    }
                }
            ],
            "drawCallback": function () {
                var api = this.api();
                $(api.column(4).footer()).html(
                    api.column(4, { page: 'current' }).data().sum().toFixed(3)
                );
            },
            "dom": "lrBftip",
            "buttons": [
                {
                    extend: 'excel',
                    className: 'btn Button',
                    text: '<i class="fa fa-file-excel-o" aria-hidden="true"></i> Export to Excel'
                }
            ]
        });
    }
}
/** ========================================== Report Controller End ================================= */



// Loader Start
const cm = document.querySelector("#c-loader");
function ShowLoading(show = true) {
    cm.style.visibility = show ? 'visible' : 'hidden';
}

// Loader end