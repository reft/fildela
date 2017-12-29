using Fildela.Data.CustomModels.User;
using Fildela.Data.Database.Models;
using Fildela.Data.Storage.Models;
using System;
using System.Collections.Generic;

namespace Fildela.Data.Repositories.User
{
    public interface IUserRepository
    {
        bool RegisterVerificationExist(string email);

        void InsertRegisterVerification(RegisterVerification registerVerification);

        RegisterVerification GetRegisterVerification(string email, string key);

        void DeleteRegisterVerification(string email);

        bool ResetPasswordExist(string email, bool isUser);

        void InsertResetPassword(ResetPassword resetPassword);

        ResetPassword GetResetPassword(string rowKey, string key, bool isUser);

        void DeleteResetPassword(string rowKey, bool isUser);

        Role GetRole(string roleName);

        bool UserExist(string email);

        bool GuestExist(string email);

        int InsertUser(Database.Models.User user);

        void InsertUserRole(int userID, Role newRole);

        bool IsUserPasswordValid(string email, string password);

        bool IsGuestDeleted(string email);

        bool IsUserDeleted(string email);

        int GetUserID(string email);

        IEnumerable<String> GetUserRoles(int userID);

        void UpdateUserLastActive(int userID);

        void UpdateGuestEmail(int guestID, string newEmail);

        bool IsGuestPasswordValid(string email, string password);

        IEnumerable<Database.Models.User> GetAccountLinksForGuest(string email);

        int GetGuestID(string email);

        bool IsGuestLinkedWithUser(int guestID, int userID);

        void UpdateGuestLastActive(int guestID);

        bool UserExistAndActive(string email);

        string ResetPasswordUser(string email);

        bool GuestExistAndActive(string email);

        string ResetPasswordGuest(string email);

        string GetUserEmail(int userID);

        SignInAccountLinksDTO GetSignInAccountLinks(string guestEmail);

        SignInUserDTO GetSignInUser(string email);

        SignInGuestDTO GetSignInGuest(string guestEmail, int userID);

        int InsertGuest(Guest guest);

        void UpdateUserEmail(int userID, string newEmail);

        void DeleteUser(int userID);

        void DeleteGuest(int guestID);

        void UpdateGuestPassword(int guestID, string newPassword);

        void UpdateUserPassword(int userID, string newPassword);

        void RestoreUser(int userID);

        void RestoreGuest(int guestID);

        bool AccountAuthenticationProviderExist(int accountID, int authenticationProviderID);

        int GetExternalAuthenticationID(string externalAuthenticationName);

        void InsertAccountAuthenticationProvider(int accountID, int authenticationProviderID, string key);

        AccountAuthenticationProvider GetAccountAuthenticationProvider(string email, string authenticationProviderName);

        bool IsAccountAuthenticationProviderKeyValid(int accountID, int authenticationProviderID, string key);

        IEnumerable<AuthenticationProvider> GetAuthenticationProviders();

        IEnumerable<AccountAuthenticationProvider> GetAccountAuthenticationProviders(int accountID);

        bool DeleteAccountAuthenticationProvider(int accountID, int authenticationProviderID);
    }
}
