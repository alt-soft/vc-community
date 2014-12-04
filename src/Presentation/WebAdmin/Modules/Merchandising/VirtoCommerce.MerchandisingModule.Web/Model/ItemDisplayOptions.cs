﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.MerchandisingModule.Web2.Model
{
    /// <summary>
    /// Enum ItemDisplayOptions
    /// </summary>
    [Flags]
    public enum ItemDisplayOptions
    {
        /// <summary>
        /// The item only
        /// </summary>
        ItemOnly = 0,
        /// <summary>
        /// The item price
        /// </summary>
        ItemPrice = 1 << 1,
        /// <summary>
        /// The item availability
        /// </summary>
        ItemAvailability = 1 << 2,
        /// <summary>
        /// The item property sets
        /// </summary>
        ItemPropertySets = 1 << 3,
        /// <summary>
        /// The item small
        /// </summary>
        ItemSmall = ItemOnly | ItemPrice,
        /// <summary>
        /// The item medium
        /// </summary>
        ItemMedium = ItemOnly | ItemPrice | ItemAvailability,
        /// <summary>
        /// The item large
        /// </summary>
        ItemLarge = ItemOnly | ItemPrice | ItemAvailability | ItemPropertySets
    }
}
