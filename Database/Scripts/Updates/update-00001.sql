/*
** Description: Add column and foreign key to NavBarTab table
** Date: 20th September 2018
*/

/*----- Add column to NavBarTab table if it does not already exist -----*/

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[element].[NavBarTab]') AND name = 'ParentNavBarTabId')
BEGIN
    ALTER TABLE [element].[NavBarTab] ADD ParentNavBarTabId bigint NULL
END

/*----- Add foreign key if it does not already exist -----*/

IF NOT EXISTS (SELECT * FROM sys.objects o WHERE o.object_id = object_id(N'[element].[FK_NavBarTab_NavBarTab]') AND OBJECTPROPERTY(o.object_id, N'IsForeignKey') = 1)
BEGIN
    ALTER TABLE [element].[NavBarTab] WITH CHECK ADD CONSTRAINT [FK_NavBarTab_NavBarTab] FOREIGN KEY([TenantId], [ElementId], [ParentNavBarTabId]) REFERENCES [element].[NavBarTab] ([TenantId], [ElementId], [NavBarTabId])
END