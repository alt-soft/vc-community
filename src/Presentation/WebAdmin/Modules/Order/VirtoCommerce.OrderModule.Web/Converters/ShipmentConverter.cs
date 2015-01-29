﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Omu.ValueInjecter;
using coreModel = VirtoCommerce.Domain.Order.Model;
using webModel = VirtoCommerce.OrderModule.Web.Model;
using VirtoCommerce.Foundation.Frameworks.Extensions;

namespace VirtoCommerce.OrderModule.Web.Converters
{
	public static class ShipmentConverter
	{
		public static webModel.Shipment ToWebModel(this coreModel.Shipment shipment)
		{
			var retVal = new webModel.Shipment();
			retVal.Childrens = new List<webModel.OperationTreeNode>();
			retVal.InjectFrom(shipment);

			retVal.OrganizationId = shipment.SourceAgentId;
			retVal.FulfilmentCenterId = shipment.SourceStoreId;
			retVal.EmployeeId = shipment.TargetAgentId;

			retVal.Organization = retVal.OrganizationId;
			retVal.FulfilmentCenter = retVal.FulfilmentCenterId;
			retVal.Employee = retVal.EmployeeId;

			if (shipment.DeliveryAddress != null)
				retVal.DeliveryAddress = shipment.DeliveryAddress.ToWebModel();

			if (shipment.InPayments != null)
			{
				retVal.InPayments = shipment.InPayments.Select(x => x.ToWebModel()).ToList();
				retVal.Childrens.AddRange(retVal.InPayments);
			}

			if(shipment.Items != null)
			{
				retVal.Items = shipment.Items.Select(x => x.ToWebModel()).ToList();
			}
		
			if(shipment.Discount != null)
			{
				retVal.Discount = shipment.Discount.ToWebModel();
			}
			return retVal;
		}

		public static coreModel.Shipment ToCoreModel(this webModel.Shipment shipment)
		{
			var retVal = new coreModel.Shipment();
			retVal.InjectFrom(shipment);

			retVal.SourceAgentId = shipment.OrganizationId;
			retVal.SourceStoreId = shipment.FulfilmentCenterId;
			retVal.TargetAgentId = shipment.EmployeeId;

			if (shipment.DeliveryAddress != null)
				retVal.DeliveryAddress = shipment.DeliveryAddress.ToCoreModel();
			if (shipment.InPayments != null)
				retVal.InPayments = shipment.InPayments.Select(x => x.ToCoreModel()).ToList();
			if (shipment.Discount != null)
				retVal.Discount = shipment.Discount.ToCoreModel();
			if (shipment.Items != null)
				retVal.Items = shipment.Items.Select(x => x.ToCoreModel()).ToList();

			return retVal;
		}


	}
}