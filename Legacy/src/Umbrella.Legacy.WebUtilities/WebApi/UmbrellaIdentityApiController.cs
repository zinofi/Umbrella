using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Umbrella.Legacy.WebUtilities.WebApi
{
	public abstract class UmbrellaIdentityApiController<TUserManager, TUser, TKey> : UmbrellaApiController
		where TUserManager : UserManager<TUser, TKey>
		where TUser : class, IUser<TKey>
		where TKey : IEquatable<TKey>
	{
		#region Private Members
		private TUserManager m_UserManager;
		private IAuthenticationManager m_AuthenticationManager;
        #endregion

        #region Constructors
        public UmbrellaIdentityApiController(ILogger logger) : base(logger)
        {
        }
        #endregion

        #region Protected Properties
        protected TUserManager UserManager
		{
			get
			{
				if (m_UserManager == null)
					m_UserManager = Request.GetOwinContext().GetUserManager<TUserManager>();

				return m_UserManager;
			}
			set
			{
				m_UserManager = value;
			}
		}

		protected IAuthenticationManager AuthenticationManager
		{
			get
			{
				if (m_AuthenticationManager == null)
					m_AuthenticationManager = Request.GetOwinContext().Authentication;

				return m_AuthenticationManager;
			}
			set
			{
				m_AuthenticationManager = value;
			}
		}
		#endregion

		#region Overridden Methods
		protected override void Dispose(bool disposing)
		{
			if (disposing && UserManager != null)
			{
				UserManager.Dispose();
				UserManager = null;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
}