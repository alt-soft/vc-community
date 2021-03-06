angular.module('platformWebApp')
.factory('platformWebApp.toolbarService', function () {
    var toolbarCommandsMap = [];
    var toolbarCustomContentsMap = [];
    return {
        register: function (toolbarItem, toolbarController, isCustomContent) {
            var map = isCustomContent ? toolbarCustomContentsMap : toolbarCommandsMap;
            if (!map[toolbarController]) {
                map[toolbarController] = [];
            }

            map[toolbarController].push(toolbarItem);
            map[toolbarController].sort(function (a, b) {
                return a.index > b.index;
            });
        },
        resolve: function (bladeCommands, toolbarController, isCustomContent) {
            var map = isCustomContent ? toolbarCustomContentsMap : toolbarCommandsMap;
            var externalCommands = map[toolbarController];
            if (externalCommands) {
                bladeCommands = angular.copy(bladeCommands || []);

                _.each(externalCommands, function (newCommand) {
                    if (isCustomContent)
                        bladeCommands.splice(newCommand.index, 0, newCommand.template);
                    else
                        bladeCommands.splice(newCommand.index, 0, newCommand);
                });
            }

            return bladeCommands;
        }
    };
})
.directive('vaBladeContainer', ['$compile', 'platformWebApp.bladeNavigationService', function ($compile, bladeNavigationService) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'Scripts/app/navigation/blade/bladeContainer.tpl.html',
        link: function (scope) {
            scope.blades = bladeNavigationService.stateBlades();
        }
    }
}])
.directive('vaBlade', ['$compile', 'platformWebApp.bladeNavigationService', 'platformWebApp.toolbarService', '$timeout', function ($compile, bladeNavigationService, toolbarService, $timeout) {
    return {
        terminal: true,
        priority: 100,
        link: function (scope, element) {
            element.attr('ng-controller', scope.blade.controller);
            element.attr('id', scope.blade.id);
            element.attr('ng-model', "blade");
            element.removeAttr("va-blade");
            $compile(element)(scope);

            var mainContent = $('.cnt');
            var blade = $(element).parent('.blade');
            var offset = parseInt(blade.offset().left + mainContent.scrollLeft() + blade.width() + 125 - mainContent[0].clientWidth);

            if (!scope.blade.disableOpenAnimation) {
                blade.css('margin-left', '-' + blade.width() + 'px').addClass('__animate');

                setTimeout(function () {
                    blade.animate({ 'margin-left': 0 }, 250, function () {
                        blade.removeAttr('style').removeClass('__animate');
                    });
                }, 0);
            }

            $timeout(function () {
                if (offset > mainContent.scrollLeft()) {
                    mainContent.animate({ scrollLeft: offset + 'px' }, 500);
                }
            }, 0, false);

            scope.bladeMaximize = function () {
                scope.maximized = true;

                var blade = $(element);
                blade.attr('data-width', blade.width());
                var leftMenu = $('.nav-bar');
                var offset = parseInt((blade.offset().left + $('.cnt').scrollLeft()) - leftMenu.width());
                var contentblock = blade.find(".blade-content");
                $(contentblock).animate({ width: (parseInt(window.innerWidth - leftMenu.width()) + 'px') }, 100);
                $(contentblock).find('.inner-block').animate({ width: parseInt(window.innerWidth - leftMenu.width() - 40) + 'px' }, 100);
                $('.cnt').animate({ scrollLeft: offset + 'px' }, 250);
            };

            scope.bladeRestore = function () {
                scope.maximized = false;

                var blade = $(element);
                var blockWidth = blade.data('width');
                var leftMenu = $('.nav-bar');
                blade.removeAttr('data-width');
                var offset = parseInt(blade.offset().left + $('.cnt').scrollLeft() - leftMenu.width());
                var contentblock = blade.find(".blade-content");
                $(contentblock).animate({ width: blockWidth }, 100);
                $(contentblock).find('.inner-block').animate({ width: blockWidth - 40 }, 100);
                $('.cnt').animate({ scrollLeft: offset + 'px' }, 250);
            };

            scope.bladeClose = function (onAfterClose) {
                bladeNavigationService.closeBlade(scope.blade, onAfterClose, function (callback) {
                    blade.addClass('__animate').animate({ 'margin-left': '-' + blade.width() + 'px' }, 125, function () {
                        blade.remove();
                        callback();
                    });
                });
            };

            scope.$watch('blade.toolbarCommands', function (toolbarCommands) {
                scope.resolvedToolbarCommands = toolbarService.resolve(toolbarCommands, scope.blade.controller, false);
            });
            scope.$watch('blade.toolbarCustomTemplates', function (toolbarCustomTemplates) {
                scope.resolvedToolbarCustomTemplates = toolbarService.resolve(toolbarCustomTemplates, scope.blade.controller, true);
            });
        }
    };
}])
.factory('platformWebApp.bladeNavigationService', ['$rootScope', '$timeout', '$state', function ($rootScope, $timeout, $state) {
    var service = {
        blades: [],
        currentBlade: undefined,
        closeBlade: function (blade, callback, onBeforeClosing) {
            //Need in case a copy was passed
            blade = service.findBlade(blade.id);

            // close all children
            service.closeChildrenBlades(blade, function () {
                var idx = service.stateBlades().indexOf(blade);

                var doCloseBlade = function () {
                    if (angular.isFunction(onBeforeClosing)) {
                        onBeforeClosing(doCloseBladeFinal);
                    } else {
                        doCloseBladeFinal();
                    }
                }

                var doCloseBladeFinal = function () {
                    service.stateBlades().splice(idx, 1);
                    //remove blade from children collection
                    if (angular.isDefined(blade.parentBlade)) {
                        var childIdx = blade.parentBlade.childrenBlades.indexOf(blade);
                        if (childIdx >= 0) {
                            blade.parentBlade.childrenBlades.splice(childIdx, 1);
                        }
                    }
                    if (angular.isFunction(callback)) {
                        $timeout(callback);
                    };
                };

                if (idx >= 0) {
                    if (angular.isFunction(blade.onClose)) {
                        blade.onClose(doCloseBlade);
                    }
                    else {
                        doCloseBlade();
                    }
                }
            });
        },
        closeChildrenBlades: function (blade, callback) {
            if (blade && blade.childrenBlades.length > 0) {
                angular.forEach(blade.childrenBlades.slice(), function (child) {
                    service.closeBlade(child, function () {
                        // show only when all children were closed
                        if (blade.childrenBlades.length == 0 && angular.isFunction(callback)) {
                            callback();
                        }
                    });
                });
            } else if (angular.isFunction(callback)) {
                callback();
            }
        },
        hasBlade: function (id) {
            return service.findBlade(id) !== undefined;
        },
        stateBlades: function (stateName) {
            if (angular.isUndefined(stateName)) {
                stateName = $state.current.name;
            }

            if (angular.isUndefined(service.blades[stateName])) {
                service.blades[stateName] = [];
            }

            return service.blades[stateName];
        },
        findBlade: function (id) {
            var found;
            angular.forEach(service.stateBlades(), function (blade) {
                if (blade.id == id) {
                    found = blade;
                }
            });

            return found;
        },
        showBlade: function (blade, parentBlade) {
            blade.isLoading = true;
            blade.parentBlade = parentBlade;
            blade.childrenBlades = [];

            var existingBlade = service.findBlade(blade.id);

            //Show blade in previous location
            if (existingBlade != undefined) {
                //store prev blade x-index
                blade.xindex = existingBlade.xindex;
            } else if (!angular.isDefined(blade.xindex)) {
                //Show blade as last one by default
                blade.xindex = service.stateBlades().length;
            }

            var showBlade = function () {
                if (angular.isDefined(parentBlade)) {
                    blade.xindex = service.stateBlades().indexOf(parentBlade) + 1;
                    parentBlade.childrenBlades.push(blade);
                }
                //show blade in same place where it was
                service.stateBlades().splice(Math.min(blade.xindex, service.stateBlades().length), 0, blade);
                service.currentBlade = blade;
            };

            if (angular.isDefined(parentBlade) && parentBlade.childrenBlades.length > 0) {
                service.closeChildrenBlades(parentBlade, showBlade);
            }
            else if (angular.isDefined(existingBlade)) {
                service.closeBlade(existingBlade, showBlade);
            }
            else {
                showBlade();
            }
        },
        setError: function (msg, blade) {
            blade.isLoading = false;
            blade.error = msg;
        }
    };

    return service;
}]);