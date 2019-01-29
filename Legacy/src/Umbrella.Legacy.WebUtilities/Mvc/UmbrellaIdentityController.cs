//TODO: Move into own package
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Extensions.Logging;
//using Microsoft.Owin.Security;
//using System;
//using System.Web;
//using System.Web.Mvc;

//namespace Umbrella.Legacy.WebUtilities.Mvc
//{
//	public class UmbrellaIdentityController<TUserManager, TUser, TKey> : UmbrellaController
//		where TUserManager : UserManager<TUser, TKey>
//		where TUser : class, IUser<TKey>
//		where TKey : IEquatable<TKey>
//	{
//		#region Private Members
//		private TUserManager m_UserManager;
//		private IAuthenticationManager m_AuthenticationManager;
//        #endregion

//        #region Constructors
//        public UmbrellaIdentityController(ILogger logger) : base(logger)
//        {
//        }
//        #endregion

//        #region Protected Properties
//        protected TUserManager UserManager
//		{
//			get
//			{
//				if (m_UserManager == null)
//					m_UserManager = Request.GetOwinContext().GetUserManager<TUserManager>();

//				return m_UserManager;
//			}
//			set
//			{
//				m_UserManager = value;
//			}
//		}

//		protected IAuthenticationManager AuthenticationManager
//		{
//			get
//			{
//				if (m_AuthenticationManager == null)
//					m_AuthenticationManager = Request.GetOwinContext().Authentication;

//				return m_AuthenticationManager;
//			}
//			set
//			{
//				m_AuthenticationManager = value;
//			}
//		}
//		#endregion

//		#region Overridden Methods
//		protected override void Dispose(bool disposing)
//		{
//			if (disposing && UserManager != null)
//			{
//				UserManager.Dispose();
//				UserManager = null;
//			}
//			base.Dispose(disposing);
//		}
//		#endregion
//	}
//}