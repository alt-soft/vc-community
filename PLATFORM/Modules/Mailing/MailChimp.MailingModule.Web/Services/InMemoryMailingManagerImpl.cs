﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MailChimp.MailingModule.Web.Services
{
    public class InMemoryMailingManagerImpl : IMailingManager
    {
        private readonly List<IMailing> _mailings = new List<IMailing>();

        #region IMailingManager Members

        public void RegisterMailing(IMailing mailing)
        {
            if (mailing == null)
            {
                throw new ArgumentNullException("mailing");
            }
            if (_mailings.Any(x => x.Code == mailing.Code))
            {
                throw new OperationCanceledException(mailing.Code + " already registered");
            }
            _mailings.Add(mailing);
        }

        public IEnumerable<IMailing> Mailings
        {
            get { return _mailings.AsReadOnly(); }
        }

        #endregion
    }
}
