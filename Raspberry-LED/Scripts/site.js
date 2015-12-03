var $ = jQuery;
$(function () {
    $('.nav li').hover(function () {
        $(this)
         .find('.submenu')
         .stop(true, true)
         .slideDown('fast');
    }, function () {
        $(this)
         .find('.submenu')
         .stop(true, true)
         .slideUp('fast')
    });
});