$(function () {

    /* Navbar */
    WebAdmin.NavbarItemHome.on('click', function (event) {
        event.preventDefault();

        WebAdmin.NavbarActiveMenuItem($(this));
    });

    WebAdmin.NavbarDropdownClose.on('click', function(){
        WebAdmin.NavbarItem.removeClass('__active');
        WebAdmin.NavbarDropdown.removeClass('__opened');
    });

    /* Dashboard user panel */
    WebAdmin.DashboardAccount.on('click', function (event) {
        WebAdmin.DashboardOpenUser($(this));
    });

    /* Horizontal scroll for blades  */
    WebAdmin.HorizontalScrollForBlades('off');

    $(window).on('resize', function () {
        WebAdmin.HorizontalScrollForBlades('off');
    });

    $('.__items .list-item, .table .table-item').on('click', function () {
        $(this).addClass('__selected').siblings().removeClass('__selected');
    });

    $('.__images-list .tile').on('click', function () {
        $(this).toggleClass('__selected');
    });

    $('.list.__items').on('contextmenu', function () {
        return false;
    });

    $('.list.__items').on('mousedown', function (event) {
        var leftPos = event.pageX,
            topPos = event.pageY;

        if (event.button == 2) {
            $('.menu.__context').remove();
            $(this).find('.list-item').prepend('<ul class="menu __context" style="left: ' + leftPos + 'px; top: ' + topPos + 'px;"><li class="menu-item"><i class="menu-ico fa fa-edit"></i> Manage</li><li class="menu-item"><i class="menu-ico fa fa-trash-o"></i> Delete</li></ul>');
        }
    });

    $('.form-input.__number .down, .form-input.__number .up').on('click', function () {
        var step = $(this).parents('.form-input.__number').data('step'),
            value = $(this).parents('.form-input.__number').find('input').val();

            console.log(value)

        if($(this).hasClass('down')) {
            console.log(Math.floor(value + step))
            //$(this).parents('.form-input.__number').find('input').val(value-step);
        }
        else {
            //$(this).parents('.form-input.__number').find('input').val(value+step)
        }
    });

    $('*').on('mouseenter', function (event) {
        var blade = $(this).parents('.blade'),
            bladeI = blade.find('.blade-inner'),
            bladeH = bladeI.height(),
            bladeIh = blade.find('.inner-block').height(),
            dashboard = $(this).parents('.dashboard'),
            dashboardA = dashboard.find('.dashboard-area'),
            dashboardH = dashboardA.height(),
            dashboardIh = dashboard.find('.dashboard-inner').height();

        if (blade.length) {
            if (bladeH <= bladeIh) {
                WebAdmin.HorizontalScrollForBlades('off');
            }
            else {
                WebAdmin.HorizontalScrollForBlades('on');
            }
        }

        if (dashboard.length) {
            if (dashboardH <= dashboardIh) {
                WebAdmin.HorizontalScrollForBlades('off');
            }
            else {
                WebAdmin.HorizontalScrollForBlades('on');
            }
        }
    });

    $('.dashboard-head, .dashboard-head *, .blade-head, .blade-head *, .static, .static *').on('mouseenter', function (event) {
        WebAdmin.HorizontalScrollForBlades('on');
    });

    /* Close dropdown if click in other area window */
    $(document).on("click", function (event) {
        if (!$('.nav-bar').is(event.target) && !$('.nav-bar').has(event.target).length) {
            if(WebAdmin.NavbarFlag) {
                WebAdmin.NavbarDropdown.removeClass('__opened');
                WebAdmin.NavbarItem.removeClass('__active');
            }

            WebAdmin.NavbarFlag = false;
        }

        if (!$('.dashboard .dashboard-account').is(event.target) && !$('.dashboard .dashboard-account').has(event.target).length) {
            if(WebAdmin.DashboardFlag) {
                WebAdmin.DashboardAccount.removeClass('__opened');
            }

            WebAdmin.DashboardFlag = false;
        }

        if (!$('.menu.__context').is(event.target) && !$('.menu.__context').has(event.target).length) {
            $('.menu.__context').remove();
        }
    });

});

var WebAdmin = {

    NavbarFlag: false,
    DashboardFlag: false,
    Speed: null,

    /* Elements for Navbar */
    NavbarItem: $('.nav-bar .menu-item'),
    NavbarItemHome: $('.nav-bar .menu-item:not(.__home)'),
    NavbarDropdown: $('.nav-bar .dropdown'),
    NavbarDropdownClose: $('.dropdown .dropdown-close'),

    /* Dashboard user */
    DashboardAccount: $('.dashboard .dashboard-account'),

    NavbarActiveMenuItem: function(self) {
        var name = $('.name', self);

        if (self.hasClass('__active')) {
            self.removeClass('__active');
            this.NavbarDropdown.removeClass('__opened');
        }
        else {
            this.NavbarItem.removeClass('__active');
            self.addClass('__active');
            this.NavbarDropdown.addClass('__opened');
        }

        this.NavbarFlag = true;
    },

    DashboardOpenUser: function(self) {
        if (self.hasClass('__opened')) {
            self.removeClass('__opened');
        }
        else {
            self.addClass('__opened');
        }

        this.DashboardFlag = true;
    },

    HorizontalScrollForBlades: function(flag) {
        this.Speed = (window.navigator.platform == 'MacIntel' ? .5 : 40);
        
        if (flag != 'off') {
            $('.cnt').off('mousewheel').on('mousewheel', function (event, delta) {
                this.scrollLeft -= (delta * WebAdmin.Speed);
                event.preventDefault();
            });
        }
        else {
            $('.cnt').unmousewheel();
        }
    }

};