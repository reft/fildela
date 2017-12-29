using Fildela.Data.CustomModels.User;
using Fildela.Data.Database.DataLayer;
using Fildela.Data.Database.Models;
using Fildela.Data.Helpers;
using Fildela.Data.Storage.Models;
using Fildela.Data.Storage.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Fildela.Data.Repositories.User
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(DataLayer db, CloudStorageServices storage) : base(db, storage) { }

        public bool RegisterVerificationExist(string email)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable registerVerificationTable = Storage.GetCloudRegisterVerificationTable();
            TableQuery<RegisterVerification> registerVerificationQuery = registerVerificationTable.CreateQuery<RegisterVerification>();

            bool registerVerificationExist = (from r in registerVerificationTable.ExecuteQuery(registerVerificationQuery)
                                              where r.PartitionKey == "registerverifications" &&
                                              r.RowKey == email.ToLower().Trim() && (r.DateExpires - currentTime).TotalDays > 0
                                              select r).SingleOrDefault() != null;

            return registerVerificationExist;
        }

        public void InsertRegisterVerification(RegisterVerification registerVerification)
        {
            CloudTable registerVerificationTable = Storage.GetCloudRegisterVerificationTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(registerVerification);
            registerVerificationTable.Execute(insertOperation);
        }

        public RegisterVerification GetRegisterVerification(string email, string key)
        {
            CloudTable registerVerificationTable = Storage.GetCloudRegisterVerificationTable();
            TableQuery<RegisterVerification> registerVerificationQuery = registerVerificationTable.CreateQuery<RegisterVerification>();

            RegisterVerification registerVerificationEntity = (from r in registerVerificationTable.ExecuteQuery(registerVerificationQuery)
                                                               where r.PartitionKey == "registerverifications" && r.RowKey == email.ToLower().Trim() &&
                                                               r.Key == key.Trim()
                                                               select r).SingleOrDefault();

            return registerVerificationEntity;
        }

        public void DeleteRegisterVerification(string email)
        {
            CloudTable registerVerificationTable = Storage.GetCloudRegisterVerificationTable();
            TableQuery<RegisterVerification> registerVerificationQuery = registerVerificationTable.CreateQuery<RegisterVerification>();

            RegisterVerification registerVerificationEntity = (from r in registerVerificationTable.ExecuteQuery(registerVerificationQuery)
                                                               where r.PartitionKey == "registerverifications" && r.RowKey == email.ToLower().Trim()
                                                               select r).SingleOrDefault();

            if (registerVerificationEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(registerVerificationEntity);
                registerVerificationTable.Execute(deleteOperation);
            }
        }

        public bool ResetPasswordExist(string email, bool isUser)
        {
            DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

            CloudTable resetPasswordVerificationTable = Storage.GetCloudResetPasswordTable();
            TableQuery<ResetPassword> resetPasswordVerificationQuery = resetPasswordVerificationTable.CreateQuery<ResetPassword>();

            bool resetPasswordVerificationExist = (from r in resetPasswordVerificationTable.ExecuteQuery(resetPasswordVerificationQuery)
                                                   where r.PartitionKey == "resetpasswords" &&
                                                   r.Email == email.ToLower().Trim() &&
                                                   (r.DateExpires - currentTime).TotalDays > 0
                                                   && r.IsUser == isUser
                                                   select r).SingleOrDefault() != null;

            return resetPasswordVerificationExist;
        }

        public void InsertResetPassword(ResetPassword resetPassword)
        {
            CloudTable resetPasswordVerificationTable = Storage.GetCloudResetPasswordTable();

            TableOperation insertOperation = TableOperation.InsertOrReplace(resetPassword);
            resetPasswordVerificationTable.Execute(insertOperation);
        }

        public ResetPassword GetResetPassword(string rowKey, string key, bool isUser)
        {
            CloudTable resetPasswordTable = Storage.GetCloudResetPasswordTable();
            TableQuery<ResetPassword> resetPasswordQuery = resetPasswordTable.CreateQuery<ResetPassword>();

            ResetPassword resetPasswordEntity = (from r in resetPasswordTable.ExecuteQuery(resetPasswordQuery)
                                                 where r.PartitionKey == "resetpasswords" &&
                                                 r.RowKey == rowKey &&
                                                 r.Key == key &&
                                                 r.IsUser == isUser
                                                 select r).SingleOrDefault();

            return resetPasswordEntity;
        }

        public void DeleteResetPassword(string rowKey, bool isUser)
        {
            CloudTable resetPasswordTable = Storage.GetCloudResetPasswordTable();
            TableQuery<ResetPassword> resetPasswordQuery = resetPasswordTable.CreateQuery<ResetPassword>();

            ResetPassword resetPasswordEntity = (from r in resetPasswordTable.ExecuteQuery(resetPasswordQuery)
                                                 where r.PartitionKey == "resetpasswords" &&
                                                 r.RowKey == rowKey &&
                                                 r.IsUser == isUser
                                                 select r).SingleOrDefault();

            if (resetPasswordEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(resetPasswordEntity);
                resetPasswordTable.Execute(deleteOperation);
            }
        }

        public Role GetRole(string roleName)
        {
            return DB.Role.Where(m => m.Name == roleName).SingleOrDefault();
        }

        public bool UserExist(string email)
        {
            return DB.Account.Where(m => m.Email == email.ToLower().Trim() && m is Database.Models.User).Any();
        }

        public bool GuestExist(string email)
        {
            return DB.Account.Where(m => m.Email == email.ToLower().Trim() && m is Database.Models.Guest).Any();
        }

        public int InsertUser(Database.Models.User user)
        {
            DB.Account.Add(user);
            DB.SaveChanges();

            return user.UserID;
        }

        public void InsertUserRole(int userID, Role newRole)
        {
            var user = (from u in DB.Account
                        where u.UserID == userID &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            Role role = (from r in DB.Role
                         where r.RoleID == newRole.RoleID && r.Name == newRole.Name && r.Name != "Guest"
                         select r).SingleOrDefault();

            if (user != null && role != null)
            {
                UserRole userRole = new UserRole(userID, role.RoleID);

                DB.UserRole.Add(userRole);
                DB.SaveChanges();
            }
        }

        public bool IsUserPasswordValid(string email, string password)
        {
            bool isValid = false;

            var users = (from ao in DB.Account
                         where ao.Email == email.ToLower().Trim() &&
                                ao is Database.Models.User
                         select ao).SingleOrDefault();

            if (users != null)
            {
                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();

                if (users.PasswordHash == crypto.Compute(password.Trim(), users.PasswordSalt))
                    isValid = true;
            }

            return isValid;
        }

        public bool IsGuestDeleted(string email)
        {
            bool existAndDeleted = false;

            var guest = (from g in DB.Account
                         where g.Email == email.ToLower().Trim() && g.IsDeleted &&
                         g is Guest
                         select g).SingleOrDefault();

            if (guest != null)
                existAndDeleted = true;

            return existAndDeleted;
        }

        public bool IsUserDeleted(string email)
        {
            bool existAndDeleted = false;

            var users = (from g in DB.Account
                         where g.Email == email.ToLower().Trim() && g.IsDeleted &&
                                g is Database.Models.User
                         select g).SingleOrDefault();

            if (users != null)
                existAndDeleted = true;

            return existAndDeleted;
        }

        public int GetUserID(string email)
        {
            int userID = 0;

            var user = (from ao in DB.Account
                        where ao.Email == email.ToLower().Trim() &&
                                ao is Database.Models.User
                        select ao).SingleOrDefault();

            if (user != null)
                userID = user.UserID;

            return userID;
        }

        public IEnumerable<string> GetUserRoles(int userID)
        {
            var user = DB.User.Where(m => m.UserID == userID && m is Database.Models.User).SingleOrDefault();

            if (user != null && user.UserRoles != null && user.UserRoles.Count > 0)
            {
                var roles = user.UserRoles.Select(o => o.Role.Name);

                return roles;
            }
            else
                return null;
        }

        public void UpdateUserLastActive(int userID)
        {
            var user = (from ao in DB.Account
                        where ao.UserID == userID &&
                                ao is Database.Models.User
                        select ao).SingleOrDefault();

            if (user != null)
            {
                user.DateLastActive = DataTimeZoneExtensions.GetCurrentDate();
                DB.SaveChanges();
            }
        }

        public bool IsGuestPasswordValid(string email, string password)
        {
            bool isValid = false;

            var guest = (from g in DB.Account
                         where g.Email == email.ToLower().Trim() &&
                         g is Guest
                         select g).SingleOrDefault();

            if (guest != null)
            {
                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();

                if (guest.PasswordHash == crypto.Compute(password.Trim(), guest.PasswordSalt))
                    isValid = true;
            }

            return isValid;
        }

        public IEnumerable<Database.Models.User> GetAccountLinksForGuest(string email)
        {
            List<Database.Models.User> users = new List<Database.Models.User>();

            var guest = (from g in DB.Account
                         where g.Email == email.ToLower().Trim() &&
                         g is Guest
                         select g).SingleOrDefault();

            if (guest != null)
            {
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                List<AccountLink> accountLinks = (from ao in DB.AccountLink.Include("User")
                                                  where ao.GuestID == guest.UserID &&
                                                  SqlFunctions.DateDiff("second", currentTime, ao.DateExpires) > 0 &&
                                                  SqlFunctions.DateDiff("second", currentTime, ao.DateStart) < 0
                                                  select ao).ToList();

                if (accountLinks != null && accountLinks.Count > 0)
                {
                    foreach (var item in accountLinks)
                    {
                        if (item.User != null)
                        {
                            Database.Models.User userTemp = new Database.Models.User()
                            {
                                UserID = item.User.UserID,
                                Email = item.User.Email
                            };

                            users.Add(userTemp);
                        }
                    }
                }
            }

            return users;
        }

        public int GetGuestID(string email)
        {
            return DB.Account.Where(m => m.Email == email.ToLower().Trim() && m is Guest).Select(o => o.UserID).SingleOrDefault();
        }

        public bool IsGuestLinkedWithUser(int guestID, int userID)
        {
            bool isLinked = false;

            var user = (from u in DB.Account
                        where u.UserID == userID &&
                                u is Database.Models.User &&
                                !u.IsDeleted
                        select u).SingleOrDefault();

            if (user != null)
            {
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                isLinked = (from ac in DB.AccountLink
                            where ac.GuestID == guestID &&
                            ac.UserID == userID &&
                            SqlFunctions.DateDiff("second", currentTime, ac.DateExpires) > 0 &&
                            SqlFunctions.DateDiff("second", currentTime, ac.DateStart) < 0
                            select ac).Any();
            }

            return isLinked;
        }

        public void UpdateGuestLastActive(int userID)
        {
            var guest = (from g in DB.Account
                         where g.UserID == userID && g is Guest
                         select g).SingleOrDefault();

            if (guest != null)
            {
                guest.DateLastActive = DataTimeZoneExtensions.GetCurrentDate();
                DB.SaveChanges();
            }
        }

        public bool UserExistAndActive(string email)
        {
            return DB.Account.Where(m => m.Email == email.ToLower().Trim() && m is Database.Models.User && !m.IsDeleted).Any();
        }

        public string ResetPasswordUser(string email)
        {
            string newPassword = string.Empty;

            var user = (from ao in DB.Account
                        where ao.Email == email.ToLower().Trim() && ao is Database.Models.User
                        select ao).SingleOrDefault();

            if (user != null)
            {
                newPassword = Guid.NewGuid().ToString("N");

                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                string passwordHash = crypto.Compute(newPassword);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = crypto.Salt;

                DB.SaveChanges();
            }

            return newPassword;
        }

        public bool GuestExistAndActive(string email)
        {
            return DB.Account.Where(m => m.Email == email.ToLower().Trim() && m is Guest && !m.IsDeleted).Any();
        }

        public string ResetPasswordGuest(string email)
        {
            string newPassword = string.Empty;

            var guest = (from ao in DB.Account
                         where ao.Email == email.ToLower().Trim() && ao is Guest
                         select ao).SingleOrDefault();

            if (guest != null)
            {
                newPassword = Guid.NewGuid().ToString("N");

                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                string passwordHash = crypto.Compute(newPassword);

                guest.PasswordHash = passwordHash;
                guest.PasswordSalt = crypto.Salt;

                DB.SaveChanges();
            }

            return newPassword;
        }

        public string GetUserEmail(int userID)
        {
            return DB.Account.Where(m => m.UserID == userID && m is Database.Models.User).Select(o => o.Email).SingleOrDefault();
        }

        public SignInAccountLinksDTO GetSignInAccountLinks(string guestEmail)
        {
            SignInAccountLinksDTO signInModel = new SignInAccountLinksDTO();

            var guest = (from g in DB.Account
                         where g.Email == guestEmail.ToLower().Trim() && g is Guest
                         select g).SingleOrDefault();

            //Add isDeleted to model
            signInModel.IsDeleted = guest.IsDeleted;

            if (guest != null)
            {
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                List<AccountLink> accountLinks = (from ao in DB.AccountLink.Include("User")
                                                  where ao.GuestID == guest.UserID &&
                                                  SqlFunctions.DateDiff("second", currentTime, ao.DateExpires) > 0 &&
                                                  SqlFunctions.DateDiff("second", currentTime, ao.DateStart) < 0
                                                  select ao).ToList();

                if (accountLinks != null && accountLinks.Count > 0)
                {
                    List<Database.Models.User> users = new List<Database.Models.User>();

                    foreach (var item in accountLinks)
                    {
                        if (item.User != null)
                        {
                            Database.Models.User userTemp = new Database.Models.User()
                            {
                                UserID = item.User.UserID,
                                Email = item.User.Email
                            };

                            users.Add(userTemp);
                        }
                    }

                    //Add users to model
                    signInModel.Users = users;
                }
            }

            return signInModel;
        }

        public SignInUserDTO GetSignInUser(string email)
        {
            SignInUserDTO signInModel = new SignInUserDTO();

            Database.Models.User user = (from g in DB.User.Include("UserRoles.Role")
                                         where g.Email == email.ToLower().Trim() && g is Database.Models.User
                                         select g).SingleOrDefault();

            if (user != null)
            {
                signInModel.IsDeleted = user.IsDeleted;
                signInModel.UserID = user.UserID;

                if (user.UserRoles != null && user.UserRoles.Count() > 0)
                    signInModel.UserRoles = user.UserRoles.Select(o => o.Role.Name).ToList();

                if (!user.IsDeleted)
                {
                    user.DateLastActive = DataTimeZoneExtensions.GetCurrentDate();

                    DB.SaveChanges();
                }
            }

            return signInModel;
        }

        public SignInGuestDTO GetSignInGuest(string guestEmail, int userID)
        {
            SignInGuestDTO signInModel = new SignInGuestDTO();

            Fildela.Data.Database.Models.Account guest = (from g in DB.Account
                                                          where g.Email == guestEmail.ToLower().Trim() && g is Guest
                                                          select g).SingleOrDefault();

            if (guest != null)
            {
                //Add isDeleted to model
                signInModel.IsDeleted = guest.IsDeleted;
                //Add guestID to model
                signInModel.GuestID = guest.UserID;

                //Add isLinked to model
                DateTime currentTime = DataTimeZoneExtensions.GetCurrentDate();

                signInModel.IsGuestLinkedWithUser = (from ac in DB.AccountLink
                                                     where ac.GuestID == guest.UserID &&
                                                     ac.UserID == userID &&
                                                     SqlFunctions.DateDiff("second", currentTime, ac.DateExpires) > 0 &&
                                                     SqlFunctions.DateDiff("second", currentTime, ac.DateStart) < 0
                                                     select ac).Any();

                //Update last active
                if (!guest.IsDeleted)
                {
                    guest.DateLastActive = DataTimeZoneExtensions.GetCurrentDate();

                    DB.SaveChanges();
                }
            }

            return signInModel;
        }

        public int InsertGuest(Guest guest)
        {
            DB.Account.Add(guest);
            DB.SaveChanges();

            return guest.UserID;
        }

        public void UpdateUserEmail(int userID, string newEmail)
        {
            var user = (from u in DB.Account
                        where u.UserID == userID &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            if (user != null)
            {
                user.Email = newEmail.ToLower().Trim();
                DB.SaveChanges();
            }
        }

        public void UpdateGuestEmail(int guestID, string newEmail)
        {
            var guest = (from u in DB.Account
                         where u.UserID == guestID &&
                         u is Database.Models.Guest
                         select u).SingleOrDefault();

            if (guest != null)
            {
                guest.Email = newEmail.ToLower().Trim();
                DB.SaveChanges();
            }
        }

        public void DeleteUser(int userID)
        {
            var user = (from u in DB.Account
                        where u.UserID == userID &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            if (user != null)
            {
                user.IsDeleted = true;

                DB.SaveChanges();
            }
        }

        public void DeleteGuest(int guestID)
        {
            var user = (from u in DB.Account
                        where u.UserID == guestID &&
                        u is Guest
                        select u).SingleOrDefault();

            if (user != null)
            {
                user.IsDeleted = true;

                DB.SaveChanges();
            }
        }

        public void UpdateGuestPassword(int guestID, string newPassword)
        {
            var user = (from u in DB.Account
                        where u.UserID == guestID &&
                        u is Guest
                        select u).SingleOrDefault();

            if (user != null)
            {
                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                string passwordHash = crypto.Compute(newPassword);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = crypto.Salt;
                DB.SaveChanges();
            }
        }

        public void UpdateUserPassword(int userID, string newPassword)
        {
            var user = (from u in DB.Account
                        where u.UserID == userID &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            if (user != null)
            {
                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
                string passwordHash = crypto.Compute(newPassword);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = crypto.Salt;
                DB.SaveChanges();
            }
        }

        public void RestoreUser(int userID)
        {
            var user = (from u in DB.Account
                        where u.UserID == userID &&
                        u is Database.Models.User
                        select u).SingleOrDefault();

            if (user != null && user.IsDeleted)
            {
                user.IsDeleted = false;

                DB.SaveChanges();
            }
        }

        public void RestoreGuest(int guestID)
        {
            var user = (from u in DB.Account
                        where u.UserID == guestID &&
                        u is Guest
                        select u).SingleOrDefault();

            if (user != null && user.IsDeleted)
            {
                user.IsDeleted = false;

                DB.SaveChanges();
            }
        }

        public bool AccountAuthenticationProviderExist(int accountID, int authenticationProviderID)
        {
            return DB.AccountAuthenticationProvider.Any(m => m.AccountID == accountID && m.AuthenticationProviderID == authenticationProviderID);
        }

        public int GetExternalAuthenticationID(string externalAuthenticationName)
        {
            return DB.AuthenticationProvider.FirstOrDefault(m => m.Name == externalAuthenticationName).AuthenticationProviderID;
        }

        public void InsertAccountAuthenticationProvider(int accountID, int authenticationProviderID, string key)
        {
            SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();
            string keyHash = crypto.Compute(key);

            AccountAuthenticationProvider provider = new AccountAuthenticationProvider()
            {
                AccountID = accountID,
                AuthenticationProviderID = authenticationProviderID,
                KeyHash = keyHash,
                KeySalt = crypto.Salt
            };

            DB.AccountAuthenticationProvider.Add(provider);
            DB.SaveChanges();
        }

        public AccountAuthenticationProvider GetAccountAuthenticationProvider(string email, string authenticationProviderName)
        {
            var account = DB.User.Include("AccountAuthenticationProvider.AuthenticationProvider").FirstOrDefault(m => m.Email == email.ToLower().Trim());

            if (account != null && account.AccountAuthenticationProvider != null)
                return account.AccountAuthenticationProvider.FirstOrDefault(m => m.AuthenticationProvider.Name == authenticationProviderName);
            else
                return null;
        }

        public bool IsAccountAuthenticationProviderKeyValid(int accountID, int authenticationProviderID, string key)
        {
            var accountAuthenticationProvider = DB.AccountAuthenticationProvider.FirstOrDefault(m => m.AccountID == accountID && m.AuthenticationProviderID == authenticationProviderID);

            if (accountAuthenticationProvider != null)
            {
                SimpleCrypto.PBKDF2 crypto = new SimpleCrypto.PBKDF2();

                if (accountAuthenticationProvider.KeyHash == crypto.Compute(key.Trim(), accountAuthenticationProvider.KeySalt))
                    return true;
            }

            return false;
        }

        public IEnumerable<AuthenticationProvider> GetAuthenticationProviders()
        {
            return DB.AuthenticationProvider;
        }

        public IEnumerable<AccountAuthenticationProvider> GetAccountAuthenticationProviders(int accountID)
        {
            return DB.AccountAuthenticationProvider.Where(m => m.AccountID == accountID);
        }

        public bool DeleteAccountAuthenticationProvider(int accountID, int authenticationProviderID)
        {
            var accountAuthenticationProvider = DB.AccountAuthenticationProvider.FirstOrDefault(m => m.AccountID == accountID && m.AuthenticationProviderID == authenticationProviderID);

            if (accountAuthenticationProvider != null)
            {
                DB.AccountAuthenticationProvider.Remove(accountAuthenticationProvider);
                DB.SaveChanges();

                return true;
            }
            else
                return false;
        }
    }
}
