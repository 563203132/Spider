USE [SpiderData]
GO
/****** Object:  Table [dbo].[CrawledItem]    Script Date: 2018/1/8 22:49:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CrawledItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Url] [nvarchar](500) NOT NULL,
	[Detail] [nvarchar](max) NULL,
	[CreatedTime] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
