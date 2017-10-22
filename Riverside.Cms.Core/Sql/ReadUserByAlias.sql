﻿SET NOCOUNT ON

SELECT
	cms.[User].TenantId,
	cms.[User].UserId,
	cms.[User].Alias,
	cms.[User].Email,
	cms.[User].Confirmed,
	cms.[User].[Enabled],
	cms.[User].LockedOut,
	cms.[User].PasswordSaltedHash,
	cms.[User].PasswordSalt,
	cms.[User].PasswordChanged,
	cms.[User].LastPasswordFailure,
	cms.[User].PasswordFailures,
	cms.[User].ResetPasswordTokenValue,
	cms.[User].ResetPasswordTokenExpiry,
	cms.[User].ConfirmTokenValue,
	cms.[User].ConfirmTokenExpiry,
	cms.[User].ImageTenantId,
	cms.[User].ThumbnailImageUploadId,
	cms.[User].PreviewImageUploadId,
	cms.[User].ImageUploadId
FROM
	cms.[User]
WHERE
	cms.[User].TenantId = @TenantId AND
	cms.[User].Alias    = @Alias

SELECT
	cms.[Role].RoleId,
	cms.[Role].Name
FROM
	cms.[Role]
INNER JOIN
	cms.[UserRole]
ON
	cms.[Role].RoleId = cms.[UserRole].RoleId
INNER JOIN
	cms.[User]
ON
	cms.[UserRole].TenantId = cms.[User].TenantId AND
	cms.[UserRole].UserId = cms.[User].UserId
WHERE
	cms.[User].TenantId = @TenantId AND
	cms.[User].Alias    = @Alias