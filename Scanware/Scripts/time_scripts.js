$(document).ready(function () {
    $('#SelectRoster').change(function () {
        var url = $(this).val();
        //if (url) {
        //    window.location = url;
        //}
        //return false;
        $('.roster').hide();
        $('.' + url).show();
    });

    //datatables/sum datatables
    $('.dataTables').DataTable({
        //responsive: true,
        stateSave: true,
        'footerCallback': function (settings, json) {
            this.api().columns('.sum').every(function () {
                var column = this;

                var sum = column
                    .data()
                    .reduce(function (a, b) {
                        a = parseInt(a, 10);
                        if (isNaN(a)) { a = 0; }

                        b = parseInt(b, 10);
                        if (isNaN(b)) { b = 0; }

                        return a + b;
                    });

                $(column.footer()).html(sum);
            });
        }
    });

    //initiate datepicker
    $('.datepicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true
    });

    //input mask
    $('.start_date').mask('0000-00-00 00:00');
    $('.end_date').mask('0000-00-00 00:00');


    //toggle between Dept and Emp dropdowns
    $('#SelectDept').change(function () {
        $('#SelectEmp').val('hideAll');
        $('#DeptEmp').val('dept');
        var curr_val = $('#SelectDept').val();
        $('#User').val(curr_val);
    });

    $('#SelectEmp').change(function () {
        $('#SelectDept').val('hideAll');
        $('#DeptEmp').val('emp');
        var curr_val = $('#SelectEmp').val();
        $('#User').val(curr_val);
    });

    //approving on payroll summary page
    $('#ApproveAll').click(function () {
        $('.approved').not(':disabled').prop('checked', 'checked');
        $('.approver').each(function () {
            if ($(this).parent().find('.approved').is(':checked')) {
                var myID = $(this).attr('id');
                var myValue = $(this).attr('value');
                var mySplit = myValue.split('|');
                if (mySplit[2] == '0') {
                    $('#' + myID).prop('value', mySplit[0] + '|' + mySplit[1] + '|1');
                }
            }
        });
    });

    $('.approved').change(function () {
        var myID = $(this).next('input').attr('id');
        var myValue = $(this).next('input').attr('value');
        var mySplit = myValue.split('|');
        if (mySplit[2] == '1') {
            $('#' + myID).prop('value', mySplit[0] + '|' + mySplit[1] + '|0');
        }
        else {
            $('#' + myID).prop('value', mySplit[0] + '|' + mySplit[1] + '|1');
        }
    });

    //approve all on Enter Time page
    $('#EnterTimeApproveAll').click(function () {
        $('.approved_by').not(':disabled').prop('checked', 'checked');
        var current_user = $('#CurrentUserName').text();
        $('.approver').each(function () {
            if ($(this).parent().find('.approved_by').is(':checked')) {
                if ($(this).val() == null || $(this).val() == "") {
                    $(this).val(current_user);
                    $(this).parent().find('.this_approved_by').text(current_user)
                }
            }

        });
    });

    $('.approved_by').change(function () {
        var current_user = $('#CurrentUserName').text();
        if ($(this).is(":checked")) {
            $(this).next('input').val(current_user);
            $(this).parent().find('.this_approved_by').text(current_user)
        }
        else {
            $(this).next('input').val('');
            $(this).parent().find('.this_approved_by').text("")
        }
    });

    $('.emp_signed_vis').change(function () {
        var current_user = $('#CurrentUserName').text();
        if ($(this).is(":checked")) {
            $(this).next('input').val(current_user);

        }
        else {
            $(this).next('input').val('');
            $(this).next('.this_approved_by').text("");
        }
    });

    //updating start_date, end_date, and duration when OFF is selected from dropdown
    $('.time_code').change(function () {
        if ($(this).val() != '(D)' && $(this).val() != 'AFU' && $(this).val() != 'DIS' && $(this).val() != 'MED' && $(this).val() != 'MIL' && $(this).val() != 'UA' && $(this).val() != 'VAC') {
            $(this).parent().parent().find('.start_date').removeAttr('disabled');
            $(this).parent().parent().find('.end_date').removeAttr('disabled');
        }
        else if ($(this).val() == 'VAC') {
            $(this).parent().parent().find('.start_date').removeAttr('disabled');
            $(this).parent().parent().find('.end_date').removeAttr('disabled');
            var is_prod = $(this).parent().find('.is_prod').text();
            if (is_prod == 'Y') {
                var date_worked = $(this).parent().parent().find('.date_worked').text();
                $(this).parent().parent().find('.start_date').val(date_worked + ' 08:00');
                $(this).parent().parent().find('.end_date').val(date_worked + ' 18:00');
                $(this).parent().parent().find('.time_worked').text('10.00');
            }
            else {
                var date_worked = $(this).parent().parent().find('.date_worked').text();
                $(this).parent().parent().find('.start_date').val(date_worked + ' 08:00');
                $(this).parent().parent().find('.end_date').val(date_worked + ' 16:00');
                $(this).parent().parent().find('.time_worked').text('8.00');
            }
        }
        else {
            var date_worked = $(this).parent().parent().find('.date_worked').text();
            $(this).parent().parent().find('.start_date').val(date_worked + ' 00:00');
            $(this).parent().parent().find('.end_date').val(date_worked + ' 00:00');
            $(this).parent().parent().find('.time_worked').text('0.00');
            $(this).parent().parent().find('.start_date').attr('disabled', 'disabled');
            $(this).parent().parent().find('.end_date').attr('disabled', 'disabled');
        }

    });

    //ADD ROW BELOW ON ETID PAGE
    $('.addRowBelow').click(function () {
        var user = $(this).parent().parent().prev().find('.user').text();
        var user_alt = user.replace(".", "");
        var date_worked = $(this).parent().parent().prev().find('.date_worked').text();
        var date_worked_html = $(this).parent().parent().prev().find('.date_worked2').html();
        date_worked_html = date_worked_html.replace(/"/g, '');
        var code = $('#TimeCodeDropdown').html();
        var is_prod = $(this).parent().parent().prev().find('.is_prod').text();
        /*var code = $(this).parent().parent().prev().find('.code').html();
        var time_code_val = $(this).parent().parent().parent().find('.code select').val();
        var disabled = "";
        if (time_code_val == "(D)") {
            disabled = "disabled";
        }*/
        var dept = $(this).parent().parent().prev().find('.dept').html();
        var start_date = $(this).parent().parent().prev().find('.end_date').val();
        var end_date = start_date.substr(0, 10);
        var new_date_worked = date_worked.replace(/-/g, '');
        //var is_production = $(this).parent().parent().parent().find('.is_production').first().val();

        $(this).parent().parent().prev().after('<tr class="' + user_alt + '' + new_date_worked + ' formRow"><td class="user">' + user + '</td><td class=\'date_worked2\'>' + date_worked_html + '</td><td class=\'code\'>' + code + '<div class=\'is_prod\' style=\'display: none;\'>' + is_prod + '</div></td><td class=\'dept\'>' + dept + '</td><td><input type="text" class="start_date" name="on_time" value="' + start_date + '" data-mask"0000/00/00 00:00" /></td><td><input type="text" class="end_date" name="end_time" value="' + end_date + ' 00:00"/></td><td class="time_worked"></td><td>REG<input value="REG" class="hour_type" name="hour_type" type="hidden"></td><td><input type="text" name="notes" class="notes"/><input type="hidden" name="pk" value ="1" class="pk pkTemp" /></td><td><center><a href="#" onClick="deleteNewRow(\'' + user_alt + '' + new_date_worked + '\', this);" class="deleteAddedRow btn btn-outline btn-primary btn-xs">Delete</button></center></td></tr>')
        $('.start_date').mask('0000-00-00 00:00');
        $('.end_date').mask('0000-00-00 00:00');

        //updating start_date, end_date, and duration when OFF is selected from dropdown
        $('.time_code').change(function () {
            if ($(this).val() != '(D)' && $(this).val() != 'AFU' && $(this).val() != 'DIS' && $(this).val() != 'MED' && $(this).val() != 'MIL' && $(this).val() != 'UA' && $(this).val() != 'VAC') {
                $(this).parent().parent().find('.start_date').removeAttr('disabled');
                $(this).parent().parent().find('.end_date').removeAttr('disabled');
            }
            else if ($(this).val() == 'VAC') {
                $(this).parent().parent().find('.start_date').removeAttr('disabled');
                $(this).parent().parent().find('.end_date').removeAttr('disabled');
                var is_prod = $(this).parent().find('.is_prod').text();
                if (is_prod == 'Y') {
                    var date_worked = $(this).parent().parent().find('.date_worked').text();
                    $(this).parent().parent().find('.start_date').val(date_worked + ' 08:00');
                    $(this).parent().parent().find('.end_date').val(date_worked + ' 18:00');
                    $(this).parent().parent().find('.time_worked').text('10.00');
                }
                else {
                    var date_worked = $(this).parent().parent().find('.date_worked').text();
                    $(this).parent().parent().find('.start_date').val(date_worked + ' 08:00');
                    $(this).parent().parent().find('.end_date').val(date_worked + ' 16:00');
                    $(this).parent().parent().find('.time_worked').text('8.00');
                }
            }
            else {
                var date_worked = $(this).parent().parent().find('.date_worked').text();
                $(this).parent().parent().find('.start_date').val(date_worked + ' 00:00');
                $(this).parent().parent().find('.end_date').val(date_worked + ' 00:00');
                $(this).parent().parent().find('.time_worked').text('0.00');
                $(this).parent().parent().find('.start_date').attr('disabled', 'disabled');
                $(this).parent().parent().find('.end_date').attr('disabled', 'disabled');
            }

        });

    });

    //ETID ADD ROW ABOVE
    $('.addRowAbove').click(function () {
        var user = $(this).parent().parent().parent().find('.user').first().text();
        var user_alt = user.replace(".", "");
        var date_worked = $(this).parent().parent().prev().find('.date_worked').text();
        var date_worked_html = $(this).parent().parent().prev().find('.date_worked2').html();
        date_worked_html = date_worked_html.replace(/"/g, '');
        var code = $('#TimeCodeDropdown').html();
        var is_prod = $(this).parent().parent().prev().find('.is_prod').text();
        /*var code = $(this).parent().parent().parent().find('.code').first().html();
        var time_code_val = $(this).parent().parent().parent().find('.code select').first().val();
        var disabled = "";
        if (time_code_val == "(D)") {
            disabled = "disabled";
        }*/
        var dept = $(this).parent().parent().parent().find('.dept').first().html();
        var end_date = $(this).parent().parent().parent().find('.start_date').first().val();
        var start_date = end_date.substr(0, 10);
        var new_date_worked = date_worked.replace(/-/g, '');
        //var is_production = $(this).parent().parent().parent().find('.is_production').first().val();

        $(this).parent().parent().parent().prepend('<tr class="' + user_alt + '' + new_date_worked + ' formRow"><td class="user">' + user + '</td><td class=\'date_worked2\'>' + date_worked_html + '</td><td class=\'code\'>' + code + '<div class=\'is_prod\' style=\'display: none;\'>'+is_prod+'</div></td><td class=\'dept\'>' + dept + '</td><td><input type="text" class="start_date" name="on_time" value="' + start_date + ' 00:00" data-mask"0000/00/00 00:00" /></td><td><input type="text" class="end_date" name="end_time" value="' + end_date + '" /></td><td class="time_worked"></td><td>REG<input value="REG" class="hour_type" name="hour_type" type="hidden"></td><td><input type="text" name="notes" class="notes"/><input type="hidden" name="pk" value ="1" class="pk pkTemp" /></td><td><center><a href="#" onClick="deleteNewRow(\'' + user_alt + '' + new_date_worked + '\', this);" class="deleteAddedRow btn btn-outline btn-primary btn-xs">Delete</button></center></td></tr>')
        $('.start_date').mask('0000-00-00 00:00');
        $('.end_date').mask('0000-00-00 00:00');

        //updating start_date, end_date, and duration when OFF is selected from dropdown
        $('.time_code').change(function () {
            if ($(this).val() != '(D)' && $(this).val() != 'AFU' && $(this).val() != 'DIS' && $(this).val() != 'MED' && $(this).val() != 'MIL' && $(this).val() != 'UA' && $(this).val() != 'VAC') {
                $(this).parent().parent().find('.start_date').removeAttr('disabled');
                $(this).parent().parent().find('.end_date').removeAttr('disabled');
            }
            else if ($(this).val() == 'VAC') {
                $(this).parent().parent().find('.start_date').removeAttr('disabled');
                $(this).parent().parent().find('.end_date').removeAttr('disabled');
                var is_prod = $(this).parent().find('.is_prod').text();
                if (is_prod == 'Y') {
                    var date_worked = $(this).parent().parent().find('.date_worked').text();
                    $(this).parent().parent().find('.start_date').val(date_worked + ' 08:00');
                    $(this).parent().parent().find('.end_date').val(date_worked + ' 18:00');
                    $(this).parent().parent().find('.time_worked').text('10.00');
                }
                else {
                    var date_worked = $(this).parent().parent().find('.date_worked').text();
                    $(this).parent().parent().find('.start_date').val(date_worked + ' 08:00');
                    $(this).parent().parent().find('.end_date').val(date_worked + ' 16:00');
                    $(this).parent().parent().find('.time_worked').text('8.00');
                }
            }
            else {
                var date_worked = $(this).parent().parent().find('.date_worked').text();
                $(this).parent().parent().find('.start_date').val(date_worked + ' 00:00');
                $(this).parent().parent().find('.end_date').val(date_worked + ' 00:00');
                $(this).parent().parent().find('.time_worked').text('0.00');
                $(this).parent().parent().find('.start_date').attr('disabled', 'disabled');
                $(this).parent().parent().find('.end_date').attr('disabled', 'disabled');
            }

        });


    });

    //APPLY CHANGES ON ETID
    //VALIDATES DATES, GENERATES FORMS, SUBMITS FORMS
    $('.submitButton').click(function () {
        var form_name = $(this).attr('name');
        if (checkDates(form_name) && orderedDatesSingle(form_name) && checkMaxTime(form_name)) {
            $('.' + form_name).each(function (i) {
                //var i = this.id;
                var x = form_name + i;
                $('#' + x).remove();
                $('#generatedForms').prepend('<form class="' + form_name + '-form" name="' + x + '" id="' + x + '" method="post" action="/Time/EnterTimeUpdateInsert"></form>');
                var user = $(this).find('.user').text();
                $('#' + x).prepend('<input type="hidden" name="user" value="' + user + '">');
                var pay_date = $(this).find('.date_worked').text();
                $('#' + x).append('<input type="hidden" name="pay_date" value="' + pay_date + '">');
                var time_code = $(this).find('.time_code').val();
                $('#' + x).append('<input type="hidden" name="time_code" value="' + time_code + '">');
                var dept = $(this).find('.dept input').val();
                $('#' + x).append('<input type="hidden" name="dept" value="' + dept + '">');
                var start_time = $(this).find('.start_date').val();
                $('#' + x).append('<input type="hidden" name="start_time" value="' + start_time + '">');
                var end_time = $(this).find('.end_date').val();
                $('#' + x).append('<input type="hidden" name="end_time" value="' + end_time + '">');
                var hour_type = $(this).find('.hour_type').val();
                $('#' + x).append('<input type="hidden" name="hour_type" value="' + hour_type + '">');
                var notes = $(this).find('.notes').val();
                $('#' + x).append('<input type="hidden" name="notes" value="' + notes + '">');
                var pk = $(this).find('.pk').val();
                $('#' + x).append('<input type="hidden" name="pk" value="' + pk + '">');
                $('#' + x).append('<input type="hidden" name="redirect" value="/Time/EnterTime' + window.location.search + '">');
            });

            var current_user = $('#CurrentUserName').text();
            ajaxindicatorstart("Please wait while the page updates.");
            var count = $('.' + form_name + '-form').length;
            var x = 0;
            var math = Math.floor((Math.random() * 1000000000) + 1); //used to get unique value. program didn't want to run same URL again and again
            $('.' + form_name + '-form').each(function () {
                var url = $(this).attr('action');

                $.ajax({
                    type: 'POST',
                    url: url,
                    data: $(this).serialize(),
                    success: function (data) {
                        x++;
                        if (x == count) {
                            $.get('/Time/RecalcTime?user=' + current_user + '*' + math, function (data) {
                                $('#GarbageResults').html(data);
                            });
                            setTimeout(ajaxindicatorstop, 2000);
                        };
                    }
                });
            });
        }
        else {
            if (!checkDates(form_name)) {
                $('.alert-danger').first().text('Please fix incorrect date.');
            }
            else if(!orderedDatesMultiple()){
                $('.alert-danger').first().text('Please fix overlapping dates.');
            }
            else if (!checkMaxTime(form_name)){
                $('.alert-danger').first().text('Total time for date exceeds 16 hours.');
            }
            $('.alert-danger').first().fadeIn(300);
            $('html,body').scrollTop(0);
        }

    });


    //SAVE ALL CHANGES ON ETID
    //VALIDATES DATES, GENERATES FORMS, SUBMITS FORMS
    $('#SaveAllChanges').click(function () {
        var form_name = 'formRow';
        if (checkDates(form_name) && orderedDatesMultiple() && checkMaxTime(form_name)) {
            var form_count = $('.' + form_name).length;
            $('.' + form_name).each(function (i) {
                //var i = this.id;
                var x = form_name + i;
                $('#' + x).remove();
                $('#generatedForms').prepend('<form class="' + form_name + '-form" name="' + x + '" id="' + x + '" method="post" action="/Time/EnterTimeUpdateInsert"></form>');
                var user = $(this).find('.user').text();
                $('#' + x).prepend('<input type="hidden" name="user" value="' + user + '">');
                var pay_date = $(this).find('.date_worked').text();
                $('#' + x).append('<input type="hidden" name="pay_date" value="' + pay_date + '">');
                var time_code = $(this).find('.time_code').val();
                $('#' + x).append('<input type="hidden" name="time_code" value="' + time_code + '">');
                var dept = $(this).find('.dept input').val();
                $('#' + x).append('<input type="hidden" name="dept" value="' + dept + '">');
                var start_time = $(this).find('.start_date').val();
                $('#' + x).append('<input type="hidden" name="start_time" value="' + start_time + '">');
                var end_time = $(this).find('.end_date').val();
                $('#' + x).append('<input type="hidden" name="end_time" value="' + end_time + '">');
                var hour_type = $(this).find('.hour_type').val();
                $('#' + x).append('<input type="hidden" name="hour_type" value="' + hour_type + '">');
                var notes = $(this).find('.notes').val();
                $('#' + x).append('<input type="hidden" name="notes" value="' + notes + '">');
                var pk = $(this).find('.pk').val();
                $('#' + x).append('<input type="hidden" name="pk" value="' + pk + '">');
                $('#' + x).append('<input type="hidden" name="redirect" value="/Time/EnterTime' + window.location.search + '">');
            });

            var current_user = $('#CurrentUserName').text();
            ajaxindicatorstart("Please wait while the page updates.");
            var count = $('.' + form_name + '-form').length;
            var x = 0;
            var math = Math.floor((Math.random() * 1000000000) + 1);//used to get unique value. program didn't want to run same URL again and again
            $('.' + form_name + '-form').each(function () {
                var url = $(this).attr('action');
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: $(this).serialize(),
                    success: function () {
                        x++;
                        if (x == count) {
                            $.get('/Time/RecalcTime?user=' + current_user +'*'+ math, function (data) {
                                $('#GarbageResults').html(data);
                            });
                            setTimeout(ajaxindicatorstop, 2000);
                        };
                    }
                });
            });
        }
        else {
            if (!checkDates(form_name)) {
                $('.alert-danger').first().text('Please fix incorrect date.');
            }
            else if(!orderedDatesMultiple()){
                $('.alert-danger').first().text('Please fix overlapping dates.');
            }
            else if (!checkMaxTime(form_name)){
                $('.alert-danger').first().text('Total time for date exceeds 16 hours.');
            }

            $('.alert-danger').first().fadeIn(300);
            $('html,body').scrollTop(0);
        }
    });

    $('.alert-danger').click(function () {
        $(this).fadeOut(400);
        $(this).text("");
    });

    //FOCUS ON TIME AREA ON ETID
    $('.start_date').focus(function () {
        this.setSelectionRange(11, 16);
    });

    $('.end_date').focus(function () {
        this.setSelectionRange(11, 16);
    });

    //SCHEDULE ABSENCE CHECK 
    $('#abs_schedule').click(function () {
        var duration = $('.duration').val();
        var available = $('.available').val();
        duration = parseFloat(duration);
        available = parseFloat(available);

        var start_date = $('.col-sm-12 input[name="start_date"]').val();
        var end_date = $('.col-sm-12 input[name="end_date"]').val();

        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }

        today = yyyy + '-' + mm + '-' + dd;

        if (start_date <= today || end_date <= today) {
            $('.alert-danger').text('The Start or End Date is incorrect.  Please select a set of future dates.');
            $('.alert-danger').fadeIn(300);
        }
        else if (duration <= available) {

            $('#ScheduleAbsence').submit();
        }
        else {
            $('.alert-danger').text('Employee does not have enough time off to cover this duration.  Please select a valid duration.');
            $('.alert-danger').fadeIn(300);
        }


    });

    /*BACK BUTTON ON ETID PAGE*/
    var pathValue = Cookies.get('pathValue');
    
    //alert('"' + pathValue + '" - "' + thisPath);
    if ($('#EnterTimeInDetail').length && pathValue != null) {
        $('#EnterTimeInDetail .col-sm-12').append('<a class="btn btn-primary" href="' + pathValue + '" style="margin-left: 15px;">Back</button>');
    }
    else {
        Cookies.remove('pathValue');
    }


});

//delete existing row
function deleteNewRow(class_name, el) {
    var count_elements = $('.' + class_name).length;
    if (count_elements == 1) {
        $('.' + class_name).closest('table').prev('.alert-danger').text('You cannot delete this row.');
        $('.' + class_name).closest('table').prev('.alert-danger').fadeIn(400);
    }
    else {
        $(el).parent().parent().parent().remove();
    }
}

//delete newly created row
function deleteRow(url, class_name) {
    var count_elements = $('.' + class_name).length;
    var pk_elements = $('.' + class_name + ' .pkTemp').length;
    if ((count_elements == 1) || (count_elements >= 2 && pk_elements >= 1)) {
        $('.' + class_name).closest('table').prev('.alert-danger').text('You cannot delete this row.');
        $('.' + class_name).closest('table').prev('.alert-danger').fadeIn(400);
    }
    else {
        location.href = '/Time/' + url;
    }
}

//delete scheduled absence
function deleteAbsenceRow(url, class_name) {
    location.href = '/Time/' + url;
}


function ajaxindicatorstart(text, form_count, val) {
    if (jQuery('body').find('#resultLoading').attr('id') != 'resultLoading') {
        jQuery('body').append('<div id="resultLoading" style="display:none"><div><img src="/Images/GCNyjJY.gif" height=42 width=42><div>' + text + '</div></div><div class="bg"></div></div>');
    }

    jQuery('#resultLoading').css({
        'width': '100%',
        'height': '100%',
        'position': 'fixed',
        'z-index': '10000000',
        'top': '0',
        'left': '0',
        'right': '0',
        'bottom': '0',
        'margin': 'auto'
    });

    jQuery('#resultLoading .bg').css({
        'background': '#000000',
        'opacity': '0.7',
        'width': '100%',
        'height': '100%',
        'position': 'absolute',
        'top': '0'
    });

    jQuery('#resultLoading>div:first').css({
        'width': '250px',
        'height': '75px',
        'text-align': 'center',
        'position': 'fixed',
        'top': '0',
        'left': '0',
        'right': '0',
        'bottom': '0',
        'margin': 'auto',
        'font-size': '16px',
        'z-index': '10',
        'color': '#ffffff'

    });

    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeIn(300);
    jQuery('body').css('cursor', 'wait');

    //setTimeout(ajaxindicatorstop, form_count * val);
}

function ajaxindicatorstop() {
    jQuery('#resultLoading .bg').height('100%');
    jQuery('#resultLoading').fadeOut(300);
    jQuery('body').css('cursor', 'default');
    location.reload();


}

function checkDates(form_name) {
    var result = true;
    $('.' + form_name).each(function () {
        var start_date = $(this).find('.start_date').val();
        var end_date = $(this).find('.end_date').val();
        if (valiDate(start_date)) {
            //alert("true");
        }
        else {
            result = false;
            return false;
        }
        if (valiDate(end_date)) {
        }
        else {
            result = false;
            return false;
        }
    });
    return result;
}

function valiDate(date) {
    var matches = date.match(/^(\d{4})\-(\d{2})\-(\d{2}) (\d{2}):(\d{2})$/);
    var result = true;
    if (matches === null) {
        return false;
    }
    else {
        var year = parseInt(matches[1], 10);
        var month = parseInt(matches[2], 10) - 1;
        var day = parseInt(matches[3], 10);
        var hour = parseInt(matches[4], 10);
        var minute = parseInt(matches[5], 10);
        var date = new Date(year, month, day, hour, minute);
        if (date.getFullYear() !== year
          || date.getMonth() != month
          || date.getDate() !== day
          || date.getHours() !== hour
          || date.getMinutes() !== minute
        ) {
            result = false;
            return false;
        } else {
            return true;
        }
    }
    return result;
}

//validate start/end dates for single user/date
function orderedDatesSingle(name) {
    var name_count = $('.' + name).length;
    var start_date;
    var end_date;
    var off_date;
    var curr_time_code;
    var result = true;
    var x;
    $('.' + name).each(function (i) {
        x = i + 1;
        if (x != name_count) { //check second+ to next
            end_date = $('.' + name + ':eq(' + i + ')').find('.end_date').val();
            curr_time_code = $('.' + name + ':eq(' + i + ')').find('.time_code').val();
            start_date = $('.' + name + ':eq(' + x + ')').find('.start_date').val();
            off_date = $('.' + name + ':eq(' + x + ')').find('.time_code').val();
            if ((start_date < end_date) && (off_date == '(P)' || off_date == '(W)') && (curr_time_code == '(P)' || curr_time_code == '(W)')) {
                $('.' + name + ':eq(' + i + ')').find('.end_date').addClass('red');
                result = false;

            }
        }
        end_date = $('.' + name + ':eq(' + i + ')').find('.end_date').val();
        start_date = $('.' + name + ':eq(' + i + ')').find('.start_date').val();
        if (end_date < start_date) {
            $('.' + name + ':eq(' + i + ')').find('.end_date').addClass('red');
            $('.' + name + ':eq(' + i + ')').find('.start_date').addClass('red');
            result = false;
        }

    });
    return result;
}

//validate start/end dates for multiple users/dates
function orderedDatesMultiple() {
    var x;
    var start_date;
    var end_date;
    var off_date;
    var result = true;
    var curr_user;
    var next_user;
    var curr_time_code;
    var curr_end_date;
    var curr_start_date;
    var name_count = $('.date_group tr.formRow').length;
    $('.date_group tr.formRow').each(function (i) {
        x = i;
        i = i - 1;
        //x = i + 1;
        if (x != name_count && x > 0) { //check second+ to next
            end_date = $('.formRow:eq(' + i + ')').find('.end_date').val();
            start_date = $('.formRow:eq(' + x + ')').find('.start_date').val();
            off_date = $('.formRow:eq(' + x + ')').find('.time_code').val();
            curr_user = $('.formRow:eq(' + i + ')').find('.user').text();
            next_user = $('.formRow:eq(' + x + ')').find('.user').text();
            curr_time_code = $('.formRow:eq(' + i + ')').find('.time_code').val();
            if ((start_date < end_date) && (off_date == '(P)' || off_date == '(W)') && curr_user == next_user && (curr_time_code == '(P)' || curr_time_code == '(W)')) {
                $('.formRow:eq(' + i + ')').find('.end_date').addClass('red');
                $('.formRow:eq(' + x + ')').find('.start_date').addClass('red');
                result = false;
            }

            curr_end_date = $('.formRow:eq(' + i + ')').find('.end_date').val();
            curr_start_date = $('.formRow:eq(' + i + ')').find('.start_date').val();
            if (curr_end_date < curr_start_date) {
                $('.formRow:eq(' + i + ')').find('.end_date').addClass('red');
                $('.formRow:eq(' + i + ')').find('.start_date').addClass('red');
                result = false;
            }
            
        }
    });
    return result;
}

//make sure total hours worked for single day does not exceed 16
function checkMaxTime(this_class) {
    var sumTime = 0;
    var startTime;
    var endTime;
    var timeBetween = 0;
    var time_code;
    var result = true;
    if (this_class == "formRow") {
        $('.' + this_class).each(function () {
            sumTime = 0;
            timeBetween = 0;
            $(this).parent().find('.formRow').each(function () {
                time_code = $(this).find('.time_code').val();

                if (time_code == "(P)" || time_code == "(W)") {
                    startTime = $(this).find('.start_date').val();
                    startTime = startTime.replace("-", "/");
                    endTime = $(this).find('.end_date').val();
                    endTime = endTime.replace("-", "/");
                    timeBetween = new Date(endTime) - new Date(startTime);
                    sumTime = sumTime + Math.round(timeBetween / 60000);
                   // alert(this_class + ' ' + sumTime + ' ' + startTime + ' ' + endTime + ' ' + timeBetween + ' ' + Math.round(timeBetween / 60000));
                }
            });

            //960 = 16 hours
            if (sumTime > 960) {
                result = false;
                $(this).addClass('errorRow');
            }
            else {
                $(this).removeClass('errorRow');
            }
        });
    }
    else {
        $('.' + this_class).each(function () {
            time_code = $(this).find('.time_code').val();
            if (time_code == "(P)" || time_code == "(W)") {
                startTime = $(this).find('.start_date').val();
                startTime = startTime.replace("-", "/");
                endTime = $(this).find('.end_date').val();
                endTime = endTime.replace("-", "/");
                timeBetween = new Date(endTime) - new Date(startTime);
                sumTime = sumTime + Math.round(timeBetween / 60000);
                //alert(this_class + ' ' + sumTime + ' ' + startTime + ' ' + endTime + ' ' + timeBetween + ' ' + Math.round(timeBetween / 60000));
            }
        });

        //960 = 16 hours
        if (sumTime > 960) {
            result = false;
            $('.' + this_class).addClass('errorRow');
        }
        else {
            $('.' + this_class).removeClass('errorRow');
        }
    }



    return result;
}

function recalcTime(this_class) {
    var split_class = this_class.split(' ');
    this_class = split_class[0];
    var reg = 0;
    var ot = 0;
    var abs = 0;
    var val = 0;
    var type;
    var time_code;
    $('.' + this_class).each(function () {
        val = parseFloat($(this).find('.time_worked').text());
        type = $(this).find('.hour_type').val();
        time_code = $(this).find('.time_code').val();
        if (time_code == 'ABS') {
            abs = abs + val;
        }
        else if (type == 'REG') {
            reg = reg + val;
        }
        else if (type == 'OT') {
            ot = ot + val;
        }
    });
    $('.' + this_class).parent().find('.reg').text(parseFloat(Math.round(reg * 100) / 100).toFixed(2));
    $('.' + this_class).parent().find('.ot').text(parseFloat(Math.round(ot * 100) / 100).toFixed(2));
    $('.' + this_class).parent().find('.abs').text(parseFloat(Math.round(abs * 100) / 100).toFixed(2));
}


/*DRAG AND DROP*/
(function () {

    //exclude older browsers by the features we need them to support
    //and legacy opera explicitly so we don't waste time on a dead browser
    if 
    (
        !document.querySelectorAll
        ||
        !('draggable' in document.createElement('span'))
        ||
        window.opera
    )
    { return; }

    //get the collection of draggable targets and add their draggable attribute
    for (var
        targets = document.querySelectorAll('[data-draggable="target"]'),
        len = targets.length,
        i = 0; i < len; i++) {
        targets[i].setAttribute('aria-dropeffect', 'none');
    }

    //get the collection of draggable items and add their draggable attributes
    for (var
        items = document.querySelectorAll('[data-draggable="item"]'),
        len = items.length,
        i = 0; i < len; i++) {
        items[i].setAttribute('draggable', 'true');
        items[i].setAttribute('aria-grabbed', 'false');
        items[i].setAttribute('tabindex', '0');
    }



    //dictionary for storing the selections data 
    //comprising an array of the currently selected items 
    //a reference to the selected items' owning container
    //and a refernce to the current drop target container
    var selections =
    {
        items: [],
        owner: null,
        droptarget: null
    };

    //function for selecting an item
    function addSelection(item) {
        //if the owner reference is still null, set it to this item's parent
        //so that further selection is only allowed within the same container
        if (!selections.owner) {
            selections.owner = item.parentNode;
        }

            //or if that's already happened then compare it with this item's parent
            //and if they're not the same container, return to prevent selection
        else if (selections.owner != item.parentNode) {
            return;
        }

        //set this item's grabbed state
        item.setAttribute('aria-grabbed', 'true');

        //add it to the items array
        selections.items.push(item);
    }

    //function for unselecting an item
    function removeSelection(item) {
        //reset this item's grabbed state
        item.setAttribute('aria-grabbed', 'false');

        //then find and remove this item from the existing items array
        for (var len = selections.items.length, i = 0; i < len; i++) {
            if (selections.items[i] == item) {
                selections.items.splice(i, 1);
                break;
            }
        }
    }

    //function for resetting all selections
    function clearSelections() {
        //if we have any selected items
        if (selections.items.length) {
            //reset the owner reference
            selections.owner = null;

            //reset the grabbed state on every selected item
            for (var len = selections.items.length, i = 0; i < len; i++) {
                selections.items[i].setAttribute('aria-grabbed', 'false');
            }

            //then reset the items array        
            selections.items = [];
        }
    }

    //shorctut function for testing whether a selection modifier is pressed
    function hasModifier(e) {
        return (e.ctrlKey || e.metaKey || e.shiftKey);
    }


    //function for applying dropeffect to the target containers
    function addDropeffects() {
        //apply aria-dropeffect and tabindex to all targets apart from the owner
        for (var len = targets.length, i = 0; i < len; i++) {
            if 
            (
                targets[i] != selections.owner
                &&
                targets[i].getAttribute('aria-dropeffect') == 'none'
            ) {
                targets[i].setAttribute('aria-dropeffect', 'move');
                targets[i].setAttribute('tabindex', '0');
            }
        }

        //remove aria-grabbed and tabindex from all items inside those containers
        for (var len = items.length, i = 0; i < len; i++) {
            if 
            (
                items[i].parentNode != selections.owner
                &&
                items[i].getAttribute('aria-grabbed')
            ) {
                items[i].removeAttribute('aria-grabbed');
                items[i].removeAttribute('tabindex');
            }
        }
    }

    //function for removing dropeffect from the target containers
    function clearDropeffects() {
        //if we have any selected items
        if (selections.items.length) {
            //reset aria-dropeffect and remove tabindex from all targets
            for (var len = targets.length, i = 0; i < len; i++) {
                if (targets[i].getAttribute('aria-dropeffect') != 'none') {
                    targets[i].setAttribute('aria-dropeffect', 'none');
                    targets[i].removeAttribute('tabindex');
                }
            }

            //restore aria-grabbed and tabindex to all selectable items 
            //without changing the grabbed value of any existing selected items
            for (var len = items.length, i = 0; i < len; i++) {
                if (!items[i].getAttribute('aria-grabbed')) {
                    items[i].setAttribute('aria-grabbed', 'false');
                    items[i].setAttribute('tabindex', '0');
                }
                else if (items[i].getAttribute('aria-grabbed') == 'true') {
                    items[i].setAttribute('tabindex', '0');
                }
            }
        }
    }

    //shortcut function for identifying an event element's target container
    function getContainer(element) {
        do {
            if (element.nodeType == 1 && element.getAttribute('aria-dropeffect')) {
                return element;
            }
        }
        while (element = element.parentNode);

        return null;
    }



    //mousedown event to implement single selection
    document.addEventListener('mousedown', function (e) {
        //if the element is a draggable item
        if (e.target.getAttribute('draggable')) {
            //clear dropeffect from the target containers
            clearDropeffects();

            //if the multiple selection modifier is not pressed 
            //and the item's grabbed state is currently false
            if 
            (
                !hasModifier(e)
                &&
                e.target.getAttribute('aria-grabbed') == 'false'
            ) {
                //clear all existing selections
                clearSelections();

                //then add this new selection
                addSelection(e.target);
            }
        }

            //else [if the element is anything else]
            //and the selection modifier is not pressed 
        else if (!hasModifier(e)) {
            //clear dropeffect from the target containers
            clearDropeffects();

            //clear all existing selections
            clearSelections();
        }

            //else [if the element is anything else and the modifier is pressed]
        else {
            //clear dropeffect from the target containers
            clearDropeffects();
        }

    }, false);

    //mouseup event to implement multiple selection
    document.addEventListener('mouseup', function (e) {
        //if the element is a draggable item 
        //and the multipler selection modifier is pressed
        if (e.target.getAttribute('draggable') && hasModifier(e)) {
            //if the item's grabbed state is currently true
            if (e.target.getAttribute('aria-grabbed') == 'true') {
                //unselect this item
                removeSelection(e.target);

                //if that was the only selected item
                //then reset the owner container reference
                if (!selections.items.length) {
                    selections.owner = null;
                }
            }

                //else [if the item's grabbed state is false]
            else {
                //add this additional selection
                addSelection(e.target);
            }
        }

    }, false);

    //dragstart event to initiate mouse dragging
    document.addEventListener('dragstart', function (e) {
        //if the element's parent is not the owner, then block this event
        if (selections.owner != e.target.parentNode) {
            e.preventDefault();
            return;
        }

        //[else] if the multiple selection modifier is pressed 
        //and the item's grabbed state is currently false
        if 
        (
            hasModifier(e)
            &&
            e.target.getAttribute('aria-grabbed') == 'false'
        ) {
            //add this additional selection
            addSelection(e.target);
        }

        //we don't need the transfer data, but we have to define something
        //otherwise the drop action won't work at all in firefox
        //most browsers support the proper mime-type syntax, eg. "text/plain"
        //but we have to use this incorrect syntax for the benefit of IE10+
        e.dataTransfer.setData('text', '');

        //apply dropeffect to the target containers
        addDropeffects();

    }, false);



    //keydown event to implement selection and abort
    document.addEventListener('keydown', function (e) {
        //if the element is a grabbable item 
        if (e.target.getAttribute('aria-grabbed')) {
            //Space is the selection or unselection keystroke
            if (e.keyCode == 32) {
                //if the multiple selection modifier is pressed 
                if (hasModifier(e)) {
                    //if the item's grabbed state is currently true
                    if (e.target.getAttribute('aria-grabbed') == 'true') {
                        //if this is the only selected item, clear dropeffect 
                        //from the target containers, which we must do first
                        //in case subsequent unselection sets owner to null
                        if (selections.items.length == 1) {
                            clearDropeffects();
                        }

                        //unselect this item
                        removeSelection(e.target);

                        //if we have any selections
                        //apply dropeffect to the target containers, 
                        //in case earlier selections were made by mouse
                        if (selections.items.length) {
                            addDropeffects();
                        }

                        //if that was the only selected item
                        //then reset the owner container reference
                        if (!selections.items.length) {
                            selections.owner = null;
                        }
                    }

                        //else [if its grabbed state is currently false]
                    else {
                        //add this additional selection
                        addSelection(e.target);

                        //apply dropeffect to the target containers    
                        addDropeffects();
                    }
                }

                    //else [if the multiple selection modifier is not pressed]
                    //and the item's grabbed state is currently false
                else if (e.target.getAttribute('aria-grabbed') == 'false') {
                    //clear dropeffect from the target containers
                    clearDropeffects();

                    //clear all existing selections
                    clearSelections();

                    //add this new selection
                    addSelection(e.target);

                    //apply dropeffect to the target containers
                    addDropeffects();
                }

                    //else [if modifier is not pressed and grabbed is already true]
                else {
                    //apply dropeffect to the target containers    
                    addDropeffects();
                }

                //then prevent default to avoid any conflict with native actions
                e.preventDefault();
            }

            //Modifier + M is the end-of-selection keystroke
            if (e.keyCode == 77 && hasModifier(e)) {
                //if we have any selected items
                if (selections.items.length) {
                    //apply dropeffect to the target containers    
                    //in case earlier selections were made by mouse
                    addDropeffects();

                    //if the owner container is the last one, focus the first one
                    if (selections.owner == targets[targets.length - 1]) {
                        targets[0].focus();
                    }

                        //else [if it's not the last one], find and focus the next one
                    else {
                        for (var len = targets.length, i = 0; i < len; i++) {
                            if (selections.owner == targets[i]) {
                                targets[i + 1].focus();
                                break;
                            }
                        }
                    }
                }

                //then prevent default to avoid any conflict with native actions
                e.preventDefault();
            }
        }

        //Escape is the abort keystroke (for any target element)
        if (e.keyCode == 27) {
            //if we have any selected items
            if (selections.items.length) {
                //clear dropeffect from the target containers
                clearDropeffects();

                //then set focus back on the last item that was selected, which is 
                //necessary because we've removed tabindex from the current focus
                selections.items[selections.items.length - 1].focus();

                //clear all existing selections
                clearSelections();

                //but don't prevent default so that native actions can still occur
            }
        }

    }, false);



    //related variable is needed to maintain a reference to the 
    //dragleave's relatedTarget, since it doesn't have e.relatedTarget
    var related = null;

    //dragenter event to set that variable
    document.addEventListener('dragenter', function (e) {
        related = e.target;

    }, false);

    //dragleave event to maintain target highlighting using that variable
    document.addEventListener('dragleave', function (e) {
        //get a drop target reference from the relatedTarget
        var droptarget = getContainer(related);

        //if the target is the owner then it's not a valid drop target
        if (droptarget == selections.owner) {
            droptarget = null;
        }

        //if the drop target is different from the last stored reference
        //(or we have one of those references but not the other one)
        if (droptarget != selections.droptarget) {
            //if we have a saved reference, clear its existing dragover class
            if (selections.droptarget) {
                selections.droptarget.className =
                    selections.droptarget.className.replace(/ dragover/g, '');
            }

            //apply the dragover class to the new drop target reference
            if (droptarget) {
                droptarget.className += ' dragover';
            }

            //then save that reference for next time
            selections.droptarget = droptarget;
        }

    }, false);

    //dragover event to allow the drag by preventing its default
    document.addEventListener('dragover', function (e) {
        //if we have any selected items, allow them to be dragged
        if (selections.items.length) {
            e.preventDefault();
        }

    }, false);



    //dragend event to implement items being validly dropped into targets,
    //or invalidly dropped elsewhere, and to clean-up the interface either way
    document.addEventListener('dragend', function (e) {
        //if we have a valid drop target reference
        //(which implies that we have some selected items)
        if (selections.droptarget) {
            //append the selected items to the end of the target container
            for (var len = selections.items.length, i = 0; i < len; i++) {
                selections.droptarget.appendChild(selections.items[i]);
            }

            //prevent default to allow the action            
            e.preventDefault();
        }

        //if we have any selected items
        if (selections.items.length) {
            //clear dropeffect from the target containers
            clearDropeffects();

            //if we have a valid drop target reference
            if (selections.droptarget) {
                //reset the selections array
                clearSelections();

                //reset the target's dragover class
                selections.droptarget.className =
                    selections.droptarget.className.replace(/ dragover/g, '');

                //reset the target reference
                selections.droptarget = null;
            }
        }

    }, false);

})();
