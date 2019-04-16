/*
** Description: Add google site verification and head script to [cms].[Web] table
** Date: 16th April 2019
*/

/*----- Add columns to Web table if they do not already exist -----*/

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[cms].[Web]') AND name = 'GoogleSiteVerification')
BEGIN
    ALTER TABLE [cms].[Web] ADD GoogleSiteVerification [nvarchar](max) NOT NULL DEFAULT('')
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[cms].[Web]') AND name = 'HeadScript')
BEGIN
    ALTER TABLE [cms].[Web] ADD HeadScript [nvarchar](max) NOT NULL DEFAULT('')
END
GO