/*
** Description: Add title to page database table
** Date: 23rd November 2018
*/

/*----- Add Title column to Page table if it does not already exist -----*/

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[cms].[Page]') AND name = 'Title')
BEGIN
    ALTER TABLE [cms].[Page] ADD Title nvarchar(256) NULL
END