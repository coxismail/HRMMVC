


var ImportMasterApp = angular.module('UploadFile', []);
// This is for supplier
var supplierapp = ImportMasterApp.controller("SupplierImportController", function ($scope, SupplierData) {

    $scope.SelectedFileForUpload = null; //initially make it null
    $scope.ListData = null;
    $scope.ShowPermission = false;
    $scope.showLoader = false;
    $scope.showLoadef = false;

    $scope.PrepareThisFile = function (files) {
        $scope.$apply(function () {
            $scope.Message = '';
            $scope.SelectedFileForUpload = files[0];
        });
    }
    $scope.ParseExcel = function () {

        var ExcelTableData = new FormData();
        var file = $scope.SelectedFileForUpload;
        ExcelTableData.append('file', file);
        $scope.showLoader = true;   //show spinner
        var response = SupplierData.SendExcelData(ExcelTableData);   //calling service
        response.then(function (d) {
            $scope.ListData = d.data;
            console.log(d.data);
            $("#supplierDataTable").DataTable({
                "responsive": true,
                "data": d.data,
                "paging": false,
                "searching": false,
                "bDestroy": true,
                "data": d.data,
                "columns": [
                    {
                        "data": "Sl",
                        render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    },
                    { "data": "CompanyName" },
                    { "data": "SupplierCode" },
                    { "data": "FirstName" },
                    { "data": "LastName" },
                    { "data": "JobTitle" },
                    { "data": "BussinessPhone" },
                    { "data": "MobilePhone" },
                    { "data": "NationalId" },
                    { "data": "TinNumber" },
                    { "data": "FaxNumber" },
                    { "data": "Country" },
                    { "data": "State" },
                    { "data": "City" },
                    { "data": "Address" },
                    { "data": "ZipOrPostalCode" },
                    { "data": "Email" },
                    { "data": "Website" },
                    { "data": "Notes" }

                ]
            });

            $scope.ShowPermission = true; //showing the table after databinding
            $scope.showLoader = false; //after success hide spinner

        }, function (err) {
            console.log(err.data);
            console.log("error in parse excel");
        });
    }
    $scope.InsertData = function () {
        $scope.showLoadef = true;
        var response = SupplierData.InsertToDB();
        response.then(function (d) {
            var res = d.data;

            if (res.toString().length > 0) {
                $scope.Message = res;
                $scope.ShowPermission = false;   //used to disable the insert button and table after submitting data
                $scope.showLoadef = false;
                angular.forEach(
                    angular.element("input[type='file']"),
                    function (inputElem) {
                        angular.element(inputElem).val(null); //used to clear the file upload after submitting data
                    });
                $scope.SelectedFileForUpload = null;   //used to disable the preview button after inserting data
            }

        }, function (err) {
            console.log(err.data);
            console.log("error in insertdata");
        });
    }
});
var SupplierData = ImportMasterApp.service("SupplierData", function ($http) {
    this.SendExcelData = function (data) {
        var request = $http({
            method: "post",
            withCredentials: true,
            url: '/Setting/ReadExcelAsSupplier',
            data: data,
            headers: {
                'Content-Type': undefined
            },
            transformRequest: angular.identity
        });
        return request;
    }
    this.InsertToDB = function () {
        var request = $http({
            method: "get",
            url: '/Setting/UpdateSupplierTable',
            data: {},
            datatype: 'json'
        });
        return request;
    }
});

// This is for Customer

var customerapp = ImportMasterApp.controller("CustomerImportController", function ($scope, CustomerData) {
    $scope.SelectedFileForUpload = null; //initially make it null
    $scope.ListData = null;
    $scope.ShowPermission = false;
    $scope.showLoader = false;
    $scope.Showloaderf = false;

    $scope.PrepareThisFile = function (files) {
        $scope.$apply(function () {
            $scope.Message = '';
            $scope.SelectedFileForUpload = files[0];
        });
    }
    $scope.ParseExcel = function () {
        var ExcelTableData1 = new FormData();
        var file = $scope.SelectedFileForUpload;
        ExcelTableData1.append('file', file);
        $scope.showLoader = true;   //show spinner
        var response = CustomerData.SendExcelData(ExcelTableData1);   //calling service
        response.then(function (d) {

            $scope.ListData = d.data;
            $("#customerDataTable").DataTable({
                "responsive": true,
                "paging": false,
                "bDestroy": true,
                "data": d.data,
                "columns": [
                    {
                        "data": "Sl",
                        render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    },
                    { "data": "CompanyName" },
                    { "data": "Code" },
                    { "data": "FirstName" },
                    { "data": "LastName" },
                    { "data": "BalanceLimist" },
                    { "data": "Phone" },
                    { "data": "WebPage" },
                    { "data": "Email" },
                    { "data": "TinNumber" },
                    { "data": "Country" },
                    { "data": "State" },
                    { "data": "City" },
                    { "data": "Address" },
                    { "data": "NationalId" },





                ]
            });


            console.log(d.data);

            $scope.ShowPermission = true; //showing the table after databinding
            $scope.showLoader = false; //after success hide spinner

        }, function (err) {
            console.log(err.data);
            console.log("error in parse excel");
        });
    }
    $scope.InsertData = function () {
        $scope.Showloaderf = true;
        var response = CustomerData.InsertToDB();
        response.then(function (d) {
            var res = d.data;

            if (res.toString().length > 0) {
                $scope.Message = res;
                $scope.Showloaderf = false;
                $scope.ShowPermission = false;   //used to disable the insert button and table after submitting data
                angular.forEach(
                    angular.element("input[type='file']"),
                    function (inputElem) {
                        angular.element(inputElem).val(null); //used to clear the file upload after submitting data
                    });
                $scope.SelectedFileForUpload = null;   //used to disable the preview button after inserting data
            }

        }, function (err) {
            console.log(err.data);
            console.log("error in insertdata");
        });
    }
});
var CustomerData = ImportMasterApp.service("CustomerData", function ($http) {
    this.SendExcelData = function (data) {
        var request = $http({
            method: "post",
            withCredentials: true,
            url: '/Setting/ReadExcelAsCustomer',
            data: data,
            headers: {
                'Content-Type': undefined
            },
            transformRequest: angular.identity
        });
        return request;
    }
    this.InsertToDB = function () {
        var request = $http({
            method: "get",
            url: '/Setting/UpdateCustomerTable',
            data: {},
            datatype: 'json'
        });
        return request;
    }
});


// This is for Employee
var employeeapp = ImportMasterApp.controller("EmpoloyeeImportController", function ($scope, EmployeData) {
    $scope.SelectedFileForUpload = null; //initially make it null
    $scope.ListData = null;
    $scope.ShowPermission = false;
    $scope.PrepareThisFile = function (files) {
        $scope.$apply(function () {
            $scope.Message = '';
            $scope.SelectedFileForUpload = files[0];
        });
    }


    $scope.ParseExcel = function () {
        var ExcelTableData = new FormData();
        var file = $scope.SelectedFileForUpload;
        ExcelTableData.append('file', file);
        $scope.showLoader = true;   //show spinner
        var response = EmployeData.SendExcelData(ExcelTableData);   //calling service
        response.then(function (d) {

            $scope.ListData = d.data;
            console.log(d.data);

            $("#EmployeeDataTable").DataTable({
                "responsive": true,
                "paging": false,
                "bDestroy": true,
                "data": d.data,
                "columns": [
                    {
                        "data": "Sl",
                        render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    },
                    { "data": "FirstName" },
                    { "data": "LastName" },
                    { "data": "JobTitle" },
                    {
                        render: function (data, type, full, meta) {
                            return getDateIfDate(full.DateOfBirth);
                        }

                    },
                    {
                        render: function (data, type, full, meta) {
                            return getDateIfDate(full.JoiningDate);
                        }
                    },
                    { "data": "Genders" },
                    { "data": "BloodGroup" },
                    { "data": "HomePhone" },
                    { "data": "BussinessPhone" },
                    { "data": "Country" },
                    { "data": "State" },
                    { "data": "City" },
                    { "data": "Address" },
                    { "data": "ZipOrPostalCode" },
                    { "data": "NationalId" },
                    { "data": "TinNumber" },
                    { "data": "Code" },
                    { "data": "Email" },
                    { "data": "Notes" }

                ]
            });



            $scope.ShowPermission = true; //showing the table after databinding
            $scope.showLoader = false; //after success hide spinner


        }, function (err) {
            console.log(err.data);
            console.log("error in parse excel");
        });
    }
    function getDateIfDate(d) {
        var m = d.match(/\/Date\((\d+)\)\//);
        return m ? (new Date(+m[1])).toLocaleDateString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
    }

    $scope.InsertData = function () {
        var response = EmployeData.InsertToDB();
        response.then(function (d) {
            var res = d.data;

            if (res.toString().length > 0) {
                $scope.Message = res + "  Records Inserted";
                $scope.ShowPermission = false;   //used to disable the insert button and table after submitting data
                angular.forEach(
                    angular.element("input[type='file']"),
                    function (inputElem) {
                        angular.element(inputElem).val(null); //used to clear the file upload after submitting data
                    });
                $scope.SelectedFileForUpload = null;   //used to disable the preview button after inserting data
            }

        }, function (err) {
            console.log(err.data);
            console.log("error in insertdata");
        });
    }
});
var EmployeData = ImportMasterApp.service("EmployeData", function ($http) {
    this.SendExcelData = function (data) {
        var request = $http({
            method: "post",
            withCredentials: true,
            url: '/Setting/ReadExcelAsEmployee',
            data: data,
            headers: {
                'Content-Type': undefined
            },
            transformRequest: angular.identity
        });
        return request;
    }
    this.InsertToDB = function () {
        var request = $http({
            method: "get",
            url: '/Setting/UpdateEmployeeTable',
            data: {},
            datatype: 'json'
        });
        return request;
    }
});



var missingled = ImportMasterApp.controller("SearchLedgerController", function ($scope, missingledData) {
    $scope.isVisible = false;
    $scope.ShowLoader8 = false;
    $scope.isloading = false;
    $scope.LoadMissingLedger = function (val) {
        $scope.isVisible = false;
        var res = missingledData.GetMissingLed(val);
        res.then(function (data) {
            $scope.isVisible = true;
            missingledData.SerializeData(data.data);
        });
    }
    $scope.CreateMissingLedger = function () {
        $scope.ShowLoader8 = true;
        $scope.isloading = true;
        var ids = missingledData.GetIds();
        var type = missingledData.GetType();
        if (ids.length > 0) {
            var resp = missingledData.CreateMissingLed(ids, type);
            resp.then(function (r) {
                var message = r.data;
                console.log(r);
                $scope.Message = message;
                $scope.ShowLoader8 = false;
                $scope.isloading = false;
                $scope.isVisible = false;
            }, function (err) {
                console.log(err);
                console.log("error in database");
            });

            $scope.ShowLoader8 = false;
            $scope.isloading = false;
        }

    }

});
var missingledData = ImportMasterApp.service("missingledData", function ($http) {
    this.GetMissingLed = function (val) {
        var request = $http({
            method: "POST",
            withCredentials: true,
            url: '/Setting/GetMissingLedger?Type=' + val,
            headers: {
                'Content-Type': undefined
            },
            transformRequest: angular.identity
        });
        return request;
    }
    this.CreateMissingLed = function (ids, type) {
        var request = $http({
            method: "POST",
            url: '/Setting/CreateMissingLedger',
            data: { "ids": ids, "Type": type },
            datatype: 'json'
        });
        return request;
    }
    this.SerializeData = function (data) {
        $(".searchResult").empty();
        var s = "";
        $.each(data, function (key, val) {
            s += '<li><input type="checkbox" class="ledger" value="' + val.value + '"/>  ' + val.Text;
        });
        $(".searchResult").append(s);
    }
    this.GetIds = function () {
        var ids = new Array();
        $('input.ledger:checkbox:checked').each(function () {
            ids.push($(this).val());
        });
        return ids;
    }
    this.GetType = function () {
        var type = $("#LedgerType").find("option:selected").val();
        return type;
    }

});