﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CatalogModule.Model
{
	public interface ISupportLinks
	{
		ICollection<CategoryLink> Links { get; set; }
	}
}
