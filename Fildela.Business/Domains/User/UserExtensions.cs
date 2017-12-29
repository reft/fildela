using Fildela.Business.Domains.Account.Models;
using Fildela.Business.Domains.User.Models;
using Fildela.Data.CustomModels.User;
using Fildela.Data.Database.Models;
using Fildela.Data.Storage.Models;
using System.Linq;

namespace Fildela.Business.Domains.User
{
    public static class UserExtensions
    {
        public static Data.Database.Models.User ToEntity(this UserModel model)
        {
            if (model == null) return null;

            return new Data.Database.Models.User()
            {
                AgreeUserAgreement = model.AgreeUserAgreement,
                DateLastActive = model.DateLastActive,
                DateRegistered = model.DateRegistered,
                Email = model.Email,
                DefaultIpAddress = model.DefaultIpAddress,
                IsDeleted = model.IsDeleted,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.PasswordHash,
                UserID = model.UserID,
                PasswordSalt = model.PasswordSalt,
                DefaultEmailAddress = model.DefaultEmailAddress
            };
        }

        public static UserModel ToModel(this Data.Database.Models.User entity)
        {
            if (entity == null) return null;

            return new UserModel()
            {
                AgreeUserAgreement = entity.AgreeUserAgreement,
                DateLastActive = entity.DateLastActive,
                DateRegistered = entity.DateRegistered,
                Email = entity.Email,
                DefaultIpAddress = entity.DefaultIpAddress,
                IsDeleted = entity.IsDeleted,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PasswordHash = entity.PasswordHash,
                UserID = entity.UserID,
                PasswordSalt = entity.PasswordSalt,
                DefaultEmailAddress = entity.DefaultEmailAddress
            };
        }

        public static GuestDTO ToEntity(this GuestDTOModel model)
        {
            if (model == null) return null;

            return new GuestDTO()
            {
                DateExpires = model.DateExpires,
                DateRegistered = model.DateRegistered,
                DateStart = model.DateStart,
                Email = model.Email,
                PermissionNames = model.PermissionNames,
                UserID = model.UserID
            };
        }

        public static GuestDTOModel ToModel(this GuestDTO entity)
        {
            if (entity == null) return null;

            return new GuestDTOModel()
            {
                DateExpires = entity.DateExpires,
                DateRegistered = entity.DateRegistered,
                DateStart = entity.DateStart,
                Email = entity.Email,
                PermissionNames = entity.PermissionNames,
                UserID = entity.UserID
            };
        }

        public static Guest ToEntity(this GuestModel model)
        {
            if (model == null) return null;

            return new Guest()
            {
                AgreeUserAgreement = model.AgreeUserAgreement,
                DateLastActive = model.DateLastActive,
                DateRegistered = model.DateRegistered,
                Email = model.Email,
                DefaultIpAddress = model.DefaultIpAddress,
                IsDeleted = model.IsDeleted,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = model.PasswordHash,
                UserID = model.UserID,
                PasswordSalt = model.PasswordSalt,
                DefaultEmailAddress = model.DefaultEmailAddress
            };
        }

        public static GuestModel ToModel(this Guest entity)
        {
            if (entity == null) return null;

            return new GuestModel()
            {
                AgreeUserAgreement = entity.AgreeUserAgreement,
                DateLastActive = entity.DateLastActive,
                DateRegistered = entity.DateRegistered,
                Email = entity.Email,
                DefaultIpAddress = entity.DefaultIpAddress,
                IsDeleted = entity.IsDeleted,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PasswordHash = entity.PasswordHash,
                UserID = entity.UserID,
                PasswordSalt = entity.PasswordSalt,
                DefaultEmailAddress = entity.DefaultEmailAddress
            };
        }

        public static ResetPassword ToEntity(this ResetPasswordModel model)
        {
            if (model == null) return null;

            return new ResetPassword()
            {
                DateCreated = model.DateCreated,
                DateExpires = model.DateExpires,
                Email = model.Email,
                IsUser = model.IsUser,
                Key = model.Key,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static ResetPasswordModel ToModel(this ResetPassword entity)
        {
            if (entity == null) return null;

            return new ResetPasswordModel()
            {
                DateCreated = entity.DateCreated,
                DateExpires = entity.DateExpires,
                Email = entity.Email,
                IsUser = entity.IsUser,
                Key = entity.Key,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static Role ToEntity(this RoleModel model)
        {
            if (model == null) return null;

            return new Role()
            {
                Name = model.Name,
                RoleID = model.RoleID
            };
        }

        public static RoleModel ToModel(this Role entity)
        {
            if (entity == null) return null;

            return new RoleModel()
            {
                Name = entity.Name,
                RoleID = entity.RoleID
            };
        }

        public static UserDTO ToEntity(this UserDTOModel model)
        {
            if (model == null) return null;

            return new UserDTO()
            {
                DateLastActive = model.DateLastActive,
                DateRegistered = model.DateRegistered,
                Email = model.Email,
                IsDeleted = model.IsDeleted,
                UserID = model.UserID,
                IsPremium = model.IsPremium,
                UserRolesToString = model.UserRolesToString
            };
        }

        public static UserDTOModel ToModel(this UserDTO entity)
        {
            if (entity == null) return null;

            return new UserDTOModel()
            {
                DateLastActive = entity.DateLastActive,
                DateRegistered = entity.DateRegistered,
                Email = entity.Email,
                IsDeleted = entity.IsDeleted,
                UserID = entity.UserID,
                IsPremium = entity.IsPremium,
                UserRolesToString = entity.UserRolesToString
            };
        }

        public static UserRole ToEntity(this UserRoleModel model)
        {
            if (model == null) return null;

            return new UserRole()
            {
                RoleID = model.RoleID,
                UserID = model.UserID
            };
        }

        public static UserRoleModel ToModel(this UserRole entity)
        {
            if (entity == null) return null;

            return new UserRoleModel()
            {
                RoleID = entity.RoleID,
                UserID = entity.UserID
            };
        }

        public static RegisterVerification ToEntity(this RegisterVerificationModel model)
        {
            if (model == null) return null;

            return new RegisterVerification()
            {
                AgreeUserAgreement = model.AgreeUserAgreement,
                DateExpires = model.DateExpires,
                DateRegistered = model.DateRegistered,
                DefaultIpAddress = model.DefaultIpAddress,
                FirstName = model.FirstName,
                IsDeleted = model.IsDeleted,
                Key = model.Key,
                LastName = model.LastName,
                Password = model.Password,
                PasswordSalt = model.PasswordSalt,
                RowKey = model.RowKey,
                PartitionKey = model.PartitionKey,
                ETag = model.ETag
            };
        }

        public static RegisterVerificationModel ToModel(this RegisterVerification entity)
        {
            if (entity == null) return null;

            return new RegisterVerificationModel()
            {
                AgreeUserAgreement = entity.AgreeUserAgreement,
                DateExpires = entity.DateExpires,
                DateRegistered = entity.DateRegistered,
                DefaultIpAddress = entity.DefaultIpAddress,
                FirstName = entity.FirstName,
                IsDeleted = entity.IsDeleted,
                Key = entity.Key,
                LastName = entity.LastName,
                Password = entity.Password,
                PasswordSalt = entity.PasswordSalt,
                RowKey = entity.RowKey,
                PartitionKey = entity.PartitionKey,
                ETag = entity.ETag
            };
        }

        public static SignInAccountLinksDTOModel ToModel(this SignInAccountLinksDTO entity)
        {
            if (entity == null) return null;

            return new SignInAccountLinksDTOModel()
            {
                Users = entity.Users.Select(o => o.ToModel()),
                IsDeleted = entity.IsDeleted
            };
        }

        public static SignInUserDTOModel ToModel(this SignInUserDTO entity)
        {
            if (entity == null) return null;

            return new SignInUserDTOModel()
            {
                UserID = entity.UserID,
                IsDeleted = entity.IsDeleted,
                UserRoles = entity.UserRoles
            };
        }

        public static SignInGuestDTOModel ToModel(this SignInGuestDTO entity)
        {
            if (entity == null) return null;

            return new SignInGuestDTOModel()
            {
                GuestID = entity.GuestID,
                IsDeleted = entity.IsDeleted,
                IsGuestLinkedWithUser = entity.IsGuestLinkedWithUser
            };
        }

        public static AccountDTO ToEntity(this AccountDTOModel model)
        {
            if (model == null) return null;

            return new AccountDTO()
            {
                DateLastActive = model.DateLastActive,
                DateRegistered = model.DateRegistered,
                Email = model.Email,
                IsDeleted = model.IsDeleted,
                AccountID = model.AccountID,
                IsPremium = model.IsPremium,
                UserRolesToString = model.UserRolesToString
            };
        }

        public static AccountDTOModel ToModel(this AccountDTO entity)
        {
            if (entity == null) return null;

            return new AccountDTOModel()
            {
                DateLastActive = entity.DateLastActive,
                DateRegistered = entity.DateRegistered,
                Email = entity.Email,
                IsDeleted = entity.IsDeleted,
                AccountID = entity.AccountID,
                IsPremium = entity.IsPremium,
                UserRolesToString = entity.UserRolesToString
            };
        }

        public static AuthenticationProvider ToEntity(this AuthenticationProviderModel model)
        {
            if (model == null) return null;

            return new AuthenticationProvider
            {
                Name = model.Name,
                AuthenticationProviderID = model.AuthenticationProviderID,
                IconClass = model.IconClass,
                IconColor = model.IconColor
            };
        }

        public static AuthenticationProviderModel ToModel(this AuthenticationProvider entity)
        {
            if (entity == null) return null;

            return new AuthenticationProviderModel
            {
                Name = entity.Name,
                AuthenticationProviderID = entity.AuthenticationProviderID,
                IconClass = entity.IconClass,
                IconColor = entity.IconColor
            };
        }

        public static AccountAuthenticationProvider ToEntity(this AccountAuthenticationProviderModel model)
        {
            if (model == null) return null;

            return new AccountAuthenticationProvider()
            {
                AccountID = model.AccountID,
                KeyHash = model.KeyHash,
                KeySalt = model.KeySalt,
                AuthenticationProviderID = model.AuthenticationProviderID
            };
        }

        public static AccountAuthenticationProviderModel ToModel(this AccountAuthenticationProvider entity)
        {
            if (entity == null) return null;

            return new AccountAuthenticationProviderModel()
            {
                AccountID = entity.AccountID,
                KeyHash = entity.KeyHash,
                KeySalt = entity.KeySalt,
                AuthenticationProviderID = entity.AuthenticationProviderID
            };
        }
    }
}
