using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User.Models;
using System;
using System.Collections.Generic;

namespace Fildela.Business.Domains.User
{
    public interface IUserService
    {
        bool RegisterVerificationExist(string email);

        void InsertRegisterVerification(RegisterVerificationModel registerVerification);

        RegisterVerificationModel GetRegisterVerification(string email, string key);

        void DeleteRegisterVerification(string email);

        bool ResetPasswordExist(string email, bool isUser);

        void InsertResetPassword(ResetPasswordModel resetPassword);

        ResetPasswordModel GetResetPassword(string rowKey, string key, bool isUser);

        void DeleteResetPassword(string rowKey, bool isUser);

        RoleModel GetRole(string roleName);

        bool UserExist(string email);

        bool GuestExist(string email);

        int InsertUser(UserModel user);

        void InsertUserRole(RoleModel newRole, int userID);

        bool IsUserPasswordValid(string email, string password);

        bool IsGuestDeleted(string email);

        bool IsUserDeleted(string email);

        int GetUserID(string email);

        IEnumerable<String> GetUserRoles(int userID);

        void UpdateUserLastActive(int userID);

        bool IsGuestPasswordValid(string email, string password);

        IEnumerable<UserModel> GetAccountLinksForGuest(string email);

        int GetGuestID(string email);

        bool IsGuestLinkedWithUser(int guestID, int userID);

        void UpdateGuestLastActive(int userID);

        bool UserExistAndActive(string email);

        string ResetPasswordUser(string email);

        bool GuestExistAndActive(string email);

        string ResetPasswordGuest(string email);

        string GetUserEmail(int userID);

        void SendEmailRegisterVerification(RegisterVerificationModel registerVerification);

        void SendEmailRegisterSuccess(string email);

        void SendEmailRegisterExternalAuthenticationSuccess(string email, string password, string provider);

        void SendEmailResetPassword(ResetPasswordModel resetPassword);

        void SendEmailResetPasswordNewPassword(string email, string newPassword, bool isUser);

        void SendEmailContact(string senderName, string senderEmail, string categoryName, string message);

        SignInAccountLinksDTOModel GetSignInAccountLinks(string email);

        SignInUserDTOModel GetSignInUser(string email);

        SignInGuestDTOModel GetSignInGuest(string guestEmail, int userId);

        int InsertGuest(GuestModel guest);

        void UpdateUserEmail(int userID, string newEmail);

        void UpdateGuestEmail(int guestID, string newEmail);

        void DeleteUser(int userID);

        void DeleteGuest(int guestID);

        void UpdateGuestPassword(int guestID, string newPassword);

        void UpdateUserPassword(int userID, string newPassword);

        void RestoreUser(int userID);

        void RestoreGuest(int guestID);

        bool AccountAuthenticationProviderExist(int accountID, int authenticationProviderID);

        int GetExternalAuthenticationID(string externalAuthenticationName);

        void InsertAccountAuthenticationProvider(int accountID, int authenticationProviderID, string key);

        AccountAuthenticationProviderModel GetAccountAuthenticationProvider(string email, string authenticationProviderName);

        bool IsAccountAuthenticationProviderKeyValid(int accountID, int authenticationProviderID, string key);

        IEnumerable<AuthenticationProviderModel> GetAuthenticationProviders();

        IEnumerable<AccountAuthenticationProviderModel> GetAccountAuthenticationProviders(int accountID);

        bool DeleteAccountAuthenticationProvider(int accountID, int authenticationProviderID);
    }
}
