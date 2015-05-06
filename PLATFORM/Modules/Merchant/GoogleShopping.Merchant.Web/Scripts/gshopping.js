﻿//Call this to register our module to main application
var moduleName = "virtoCommerce.gshoppingModule";

if (AppDependencies != undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [
])

.run(
  ['$rootScope', 'mainMenuService', 'widgetService', '$state', function ($rootScope, mainMenuService, widgetService, $state) {
     
      //Register widgets in catalog item details
      /*widgetService.registerWidget({
          controller: 'virtoCommerce.gshoppingModule.gshoppingWidgetController',
          template: 'Modules/$(GoogleShopping.Merchant)/Scripts/widgets/gshoppingWidget.tpl.html'
      }, 'catalogDetail');*/

      widgetService.registerWidget({
          controller: 'virtoCommerce.gshoppingModule.gshoppingSyncCatWidgetController',
          template: 'Modules/$(GoogleShopping.Merchant)/Scripts/widgets/gshoppingWidget.tpl.html'
      }, 'categoryDetail');
  }]);