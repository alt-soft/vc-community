﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Web.Model
{
	public class Operation : AuditableEntity
	{
	
		public string OperationType { get; set; }

		
		public string Number { get; set; }
		public bool IsApproved { get; set; }
		public string Status { get; set; }
	

		public string Comment { get; set; }
		/// <summary>
		/// Currecy code
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public CurrencyCodes Currency { get; set; }
		public bool TaxIncluded { get; set; }
		public decimal Sum { get; set; }
		public decimal Tax { get; set; }
		
		public bool IsCancelled { get; set; }
		public DateTime? CancelledDate { get; set; }
		public string CancelReason { get; set; }

		public string ParentOperationId { get; set; }

		public IEnumerable<Operation> ChildrenOperations { get; set; }
		public IEnumerable<OperationProperty> Properties { get; set; }


	}
}