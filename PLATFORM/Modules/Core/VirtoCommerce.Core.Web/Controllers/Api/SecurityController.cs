﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using VirtoCommerce.CoreModule.Web.Converters;
using VirtoCommerce.CoreModule.Web.Security;
using VirtoCommerce.CoreModule.Web.Security.Models;
using VirtoCommerce.Foundation.Data.Security.Identity;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using VirtoCommerce.Foundation.Security.Model;
using ApiAccount = VirtoCommerce.Foundation.Security.Model.ApiAccount;
using Omu.ValueInjecter;

namespace VirtoCommerce.CoreModule.Web.Controllers.Api
{
    [RoutePrefix("api/security")]
    public class SecurityController : ApiController
    {
        private readonly Func<IFoundationSecurityRepository> _securityRepository;
        private readonly Func<IAuthenticationManager> _authenticationManagerFactory;
        private readonly Func<ApplicationSignInManager> _signInManagerFactory;
        private readonly Func<ApplicationUserManager> _userManagerFactory;
        private readonly IApiAccountProvider _apiAccountProvider;

        public SecurityController(Func<IFoundationSecurityRepository> securityRepository, Func<ApplicationSignInManager> signInManagerFactory,
                                  Func<ApplicationUserManager> userManagerFactory, Func<IAuthenticationManager> authManagerFactory,
                                  IApiAccountProvider apiAccountProvider)
        {
            _securityRepository = securityRepository;
            _signInManagerFactory = signInManagerFactory;
            _userManagerFactory = userManagerFactory;
            _authenticationManagerFactory = authManagerFactory;
            _apiAccountProvider = apiAccountProvider;
        }

        private ApplicationUserManager _userManager;
        private ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = _userManagerFactory();
                }
                return _userManager;
            }
        }
        #region Internal Web admin actions

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(UserLogin model)
        {
            if (await _signInManagerFactory().PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true) == SignInStatus.Success)
            {
                return Ok(await GetUserExtended(model.UserName));
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }

        [HttpGet]
        [Route("usersession")]
        [ResponseType(typeof(ApplicationUserExtended))]
        public async Task<IHttpActionResult> GetCurrentUserSession()
        {
            return Ok(await GetUserExtended(User.Identity.Name));
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            _authenticationManagerFactory().SignOut();
            return Ok(new { status = true });
        }

        #endregion

        #region Public methods

        [HttpGet]
        [Route("usersession/{userName}")]
        public async Task<ApplicationUserExtended> GetUserSession(string userName)
        {
            return await GetUserExtended(userName);
        }

        #region Methods needed to integrate identity security with external user store that will call api methods

        #region IUserStore<ApplicationUser> Members

        [Route("users/id/{userId}")]
        [HttpGet]
        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await UserManager.FindByIdAsync(userId);
        }

        [Route("users/name/{userName}")]
        [HttpGet]
        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        [Route("users/email/{email}")]
        [HttpGet]
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await UserManager.FindByEmailAsync(email);
        }

        #endregion

        /// <summary>
        ///  GET: api/security/apiaccounts/new
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Security.Models.ApiAccount))]
        [Route("apiaccounts/new")]
        public IHttpActionResult GenerateNewApiAccount()
        {
            var apiAccount = _apiAccountProvider.GenerateApiCredentials();
			apiAccount.IsActive = true;
            var result = apiAccount.ToWebModel();
            return Ok(result);
        }


        /// <summary>
        ///  GET: api/security/users/jo@domain.com
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ApplicationUserExtended))]
        [Route("users/{name}")]
        public async Task<IHttpActionResult> GetUserByName(string name)
        {
            var retVal = await GetUserExtended(name);
            return Ok(retVal);
        }

        /// <summary>
        /// POST: api/security/users/jo@domain.com/changepassword
        /// </summary>
        /// <param name="name"></param>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(IdentityResult))]
        [Route("users/{name}/changepassword")]
        public async Task<IHttpActionResult> ChangePassword(string name, [FromBody] ChangePasswordInfo changePassword)
        {
            var user = await GetUserExtended(name);
            if (user == null)
            {
                return NotFound();
            }
            var retVal = await UserManager.ChangePasswordAsync(user.Id, changePassword.OldPassword, changePassword.NewPassword);
            return Ok(retVal);
        }



        /// <summary>
        ///  GET: api/security/users?q=ddd&amp;start=0&amp;count=20
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(UserSearchResult))]
        [Route("users")]
        public async Task<IHttpActionResult> SearchUsersAsync([ModelBinder(typeof(UserSearchCriteriaBinder))] UserSearchCriteria criteria)
        {
            var query = UserManager.Users;
            var retVal = new UserSearchResult
            {
                TotalCount = query.Count(),
                Users = new List<ApplicationUserExtended>()
            };

            var result = query.OrderBy(x => x.UserName)
                             .Skip(criteria.Start)
                             .Take(criteria.Count)
                             .ToArray();

            foreach (var user in result)
            {
                var userExt = await GetUserExtended(user.UserName);
                retVal.Users.Add(userExt);
            }

            return Ok(retVal);
        }

        /// <summary>
        /// DELETE: api/security/users?names=21
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("users")]
        public async Task<IHttpActionResult> DeleteAsync([FromUri] string[] names)
        {
            foreach (var name in names)
            {
                var dbUser = await UserManager.FindByNameAsync(name);
                if (dbUser == null)
                {
                    return NotFound();
                }
                await UserManager.DeleteAsync(dbUser);
                using (var repository = _securityRepository())
                {
                    var account = repository.GetAccountByName(name);
                    if (account != null)
                    {
                        repository.Remove(account);
                        repository.UnitOfWork.Commit();
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// PUT: api/security/users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("users")]
        public async Task<IHttpActionResult> UpdateAsync(ApplicationUserExtended user)
        {
            var dbUser = await UserManager.FindByIdAsync(user.Id);

			dbUser.InjectFrom(user);
          

            var result = await UserManager.UpdateAsync(dbUser);

            if (result.Succeeded)
            {
                using (var repository = _securityRepository())
                {
                    var acount = repository.GetAccountByName(user.UserName);
                    if (acount == null)
                    {
                        return BadRequest("Acount not found");
                    }
                    acount.RegisterType = (int)user.UserType;
                    acount.AccountState = (int)user.UserState;
					acount.MemberId = user.MemberId;
					acount.StoreId = user.StoreId;
                    if (user.ApiAcounts != null)
                    {
                        var source = new ObservableCollection<ApiAccount>(user.ApiAcounts.Select(x => x.ToFoundation()));
                        var inventoryComparer = AnonymousComparer.Create((ApiAccount x) => x.ApiAccountId);
                        acount.ApiAccounts.ObserveCollection(x => repository.Add(x), x => repository.Remove(x));
                        source.Patch(acount.ApiAccounts, inventoryComparer, (sourceAccount, targetAccount) => sourceAccount.Patch(targetAccount));
                    }

                    repository.UnitOfWork.Commit();
                }

                return Ok();
            }

            return BadRequest(String.Join(" ", result.Errors));
        }

        /// <summary>
        /// POST: api/security/users/create
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/create")]
        public async Task<IHttpActionResult> CreateAsync(ApplicationUserExtended user)
        {
			var dbUser = new ApplicationUser();

			dbUser.InjectFrom(user);

            IdentityResult result;
            if (!string.IsNullOrEmpty(user.Password))
            {
                result = await UserManager.CreateAsync(dbUser, user.Password);
            }
            else
            {
                result = await UserManager.CreateAsync(dbUser);
            }

            if (result.Succeeded)
            {
                 using (var repository = _securityRepository())
                {
                    var account = new Account
                    {
                        UserName = user.UserName,
                        AccountId = user.Id,
						MemberId = user.MemberId,
                        AccountState = AccountState.Approved.GetHashCode(),
                        RegisterType = user.UserType.GetHashCode(),
                        StoreId = user.StoreId
                    };

                    repository.Add(account);
                    repository.UnitOfWork.Commit();
                }

                return Ok();
            }
            else
            {
                return BadRequest(String.Join(" ", result.Errors));
            }
        }

        #endregion

        #endregion

        #region Helpers

        private async Task<ApplicationUserExtended> GetUserExtended(string userName)
        {
            ApplicationUserExtended retVal = null;
            var applicationUser = await UserManager.FindByNameAsync(userName);
            if (applicationUser != null)
            {
				retVal = new ApplicationUserExtended();
 				retVal.InjectFrom(applicationUser);

                using (var repository = _securityRepository())
                {
					
                    var user = repository.GetAccountByName(userName);
					retVal.InjectFrom(user);
                    if (user != null)
                    {
                        var permissions =
                            user.RoleAssignments.Select(x => x.Role)
                                .SelectMany(x => x.RolePermissions)
                                .Select(x => x.Permission);

                        retVal.UserState = (UserState)user.AccountState;
                        retVal.UserType = (UserType)user.RegisterType;
                        retVal.Permissions = permissions.Select(x => x.PermissionId).Distinct().ToArray();
                        retVal.ApiAcounts = user.ApiAccounts.Select(x => x.ToWebModel()).ToList();
                    }
                }
            }

            return retVal;
        }

        #endregion
    }
}