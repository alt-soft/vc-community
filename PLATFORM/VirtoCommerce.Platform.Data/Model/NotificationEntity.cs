﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Model
{
	public class NotificationEntity : AuditableEntity
	{
		/// <summary>
		/// Must be made sending
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Type of notification
		/// </summary>
		[StringLength(128)]
		public string Type { get; set; }

		/// <summary>
		/// Notification was successfully sent
		/// </summary>
		public bool IsSuccessSend { get; set; }

		/// <summary>
		/// Subject of notification
		/// </summary>
		[StringLength(512)]
		public string Subject { get; set; }

		/// <summary>
		/// Body of notification
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Sender info (e-mail, phone number and etc.) of notification
		/// </summary>
		[StringLength(128)]
		public string Sender { get; set; }

		/// <summary>
		/// Recipient info (e-mail, phone number and etc.) of notification
		/// </summary>
		[StringLength(128)]
		public string Recipient { get; set; }

		/// <summary>
		/// Channel of sending notification
		/// </summary>
		[StringLength(128)]
		public string SendingGateway { get; set; }

		/// <summary>
		/// Number of current attemp
		/// </summary>
		public int AttemptCount { get; set; }

		/// <summary>
		/// Maximum number of attempts to send a message
		/// </summary>
		public int MaxAttemptCount { get; set; }

		/// <summary>
		/// Last fail attempt error message
		/// </summary>
		public string LastFailAttemptMessage { get; set; }

		/// <summary>
		/// Date of last fail attempt
		/// </summary>
		public DateTime? LastFailAttemptDate { get; set; }

		/// <summary>
		/// Date of start sending notification
		/// </summary>
		public DateTime? StartSendingDate { get; set; }

		/// <summary>
		/// Date of success sent result
		/// </summary>
		public DateTime? SentDate { get; set; }
	}
}
