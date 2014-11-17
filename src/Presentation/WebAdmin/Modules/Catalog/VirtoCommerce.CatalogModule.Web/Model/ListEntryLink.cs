﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CatalogModule.Web.Model
{
	public class ListEntryLink
	{
		public string ListEntryId { get; set; }
		public string ListEntryType { get; set; }

		public string CatalogId { get; set; }
		public string CategoryId { get; set; }
	}
}
