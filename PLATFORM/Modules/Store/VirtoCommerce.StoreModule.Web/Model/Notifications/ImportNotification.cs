﻿
namespace VirtoCommerce.StoreModule.Web.Model.Notifications
{
	public class ImportNotification : JobNotificationBase
	{
		public ImportNotification(string creator)
			: base(creator)
		{
			NotifyType = "StoreImport";
		}
	}
}
