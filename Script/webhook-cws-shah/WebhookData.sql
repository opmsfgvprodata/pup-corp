USE [PUPOPMSCORP]
GO

/****** Object:  Table [dbo].[WebhookData]    Script Date: 08/08/2024 20:21:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WebhookData](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Company] [nvarchar](50) NULL,
	[Event] [nvarchar](10) NULL,
	[Action] [nvarchar](20) NULL,
	[Data] [text] NULL,
 CONSTRAINT [PK_WebhookData] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

