﻿angular.module('catalogModule.wizards.newProductWizard.review.detail', [])
.controller('editorialReviewDetailWizardStepController', ['$scope', function ($scope)
{
    $scope.bladeActions = "Modules/Catalog/VirtoCommerce.CatalogModule.Web/Scripts/app/catalog/wizards/newProduct/new-product-wizard-ok-action.tpl.html";
    function initializeBlade(data)
    {
        $scope.currentEntity = angular.copy(data);
        $scope.blade.origEntity = data;
        $scope.blade.isLoading = false;
    };

 
    $scope.saveChanges = function ()
    {
        if (!angular.isDefined($scope.blade.parentBlade.currentEntities)) {
            $scope.blade.parentBlade.currentEntities = [];
        }
        var idx = $scope.blade.parentBlade.currentEntities.indexOf($scope.blade.origEntity);
        $scope.blade.parentBlade.currentEntities.splice(idx, idx < 0 ? 0 : 1, $scope.currentEntity);
        
        $scope.bladeClose();
    };

    $scope.bladeToolbarCommands = [
        {
            name: "Delete", icon: 'icon-remove',
            executeMethod: function () {
                if (angular.isDefined($scope.blade.parentBlade.currentEntities))
                {
                    var idx = $scope.blade.parentBlade.currentEntities.indexOf($scope.blade.origEntity);
                    $scope.blade.parentBlade.currentEntities.splice(idx, 1);
                    $scope.bladeClose();
                }
            },
            canExecuteMethod: function ()
            {
                if (angular.isDefined($scope.blade.parentBlade.currentEntities)) {
                    return $scope.blade.parentBlade.currentEntities.indexOf($scope.blade.origEntity) >= 0;
                }
                return false;
            }
        }
    ];

    // on load
    initializeBlade($scope.blade.currentEntity);

}]);