﻿CREATE TABLE [element].[PageList] (
	[TenantId] [bigint] NOT NULL,
	[ElementId] [bigint] NOT NULL,
	[PageTenantId] [bigint] NULL,
	[PageId] [bigint] NULL,
	[DisplayName] [nvarchar](256) NULL,
	[SortBy] [int] NOT NULL,
	[SortAsc] [bit] NOT NULL,
	[ShowRelated] [bit] NOT NULL,
	[ShowDescription] [bit] NOT NULL,
	[ShowImage] [bit] NOT NULL,
	[ShowBackgroundImage] [bit] NOT NULL,
	[ShowCreated] [bit] NOT NULL,
	[ShowUpdated] [bit] NOT NULL,
	[ShowOccurred] [bit] NOT NULL,
	[ShowComments] [bit] NOT NULL,
	[ShowTags] [bit] NOT NULL,
	[ShowPager] [bit] NOT NULL,
	[MoreMessage] [nvarchar](256) NULL,
	[Recursive] [bit] NOT NULL,
	[PageType] [int] NOT NULL,
	[PageSize] [int] NOT NULL,
	[NoPagesMessage] [nvarchar](max) NULL,
	[Preamble] [nvarchar](max) NULL,
 CONSTRAINT [PK_PageList] PRIMARY KEY CLUSTERED ([TenantId] ASC, [ElementId] ASC),
 CONSTRAINT [FK_PageList_Element] FOREIGN KEY([TenantId], [ElementId]) REFERENCES [cms].[Element] ([TenantId], [ElementId]),
 CONSTRAINT [FK_PageList_Page] FOREIGN KEY([PageTenantId], [PageId]) REFERENCES [cms].[Page] ([TenantId], [PageId])
)