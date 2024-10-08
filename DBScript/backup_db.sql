USE [master]
GO
/****** Object:  Database [Curus]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE DATABASE [Curus]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Curus', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Curus.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Curus_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Curus_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Curus] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Curus].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Curus] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Curus] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Curus] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Curus] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Curus] SET ARITHABORT OFF 
GO
ALTER DATABASE [Curus] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Curus] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Curus] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Curus] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Curus] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Curus] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Curus] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Curus] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Curus] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Curus] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Curus] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Curus] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Curus] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Curus] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Curus] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Curus] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [Curus] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Curus] SET RECOVERY FULL 
GO
ALTER DATABASE [Curus] SET  MULTI_USER 
GO
ALTER DATABASE [Curus] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Curus] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Curus] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Curus] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Curus] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Curus] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Curus', N'ON'
GO
ALTER DATABASE [Curus] SET QUERY_STORE = ON
GO
ALTER DATABASE [Curus] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Curus]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BackupChapters]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BackupChapters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Thumbnail] [nvarchar](max) NULL,
	[Order] [int] NOT NULL,
	[Duration] [time](7) NOT NULL,
	[Type] [int] NULL,
	[BackupCourseId] [int] NOT NULL,
	[ChapterId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_BackupChapters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BackupCourses]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BackupCourses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ShortSummary] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Thumbnail] [nvarchar](max) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[OldPrice] [decimal](18, 2) NULL,
	[Status] [nvarchar](max) NOT NULL,
	[Version] [nvarchar](max) NOT NULL,
	[Point] [float] NULL,
	[AllowComments] [bit] NOT NULL,
	[CourseId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_BackupCourses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookmarkedCourses]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookmarkedCourses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_BookmarkedCourses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ParentCategoryId] [int] NULL,
	[Status] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Chapters]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chapters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Thumbnail] [nvarchar](max) NULL,
	[Order] [int] NOT NULL,
	[Duration] [time](7) NOT NULL,
	[Type] [int] NOT NULL,
	[IsStart] [bit] NULL,
	[CourseId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Chapters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CommentCourses]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentCourses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[ByAdmin] [bit] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_CommentCourses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CommentUsers]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommentUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[CommentedById] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[InstructorDataId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_CommentUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseCategories]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseCategories](
	[CourseId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Id] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_CourseCategories] PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC,
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[ShortSummary] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Thumbnail] [nvarchar](max) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[OldPrice] [decimal](18, 2) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[Version] [nvarchar](max) NOT NULL,
	[Point] [float] NULL,
	[Reason] [nvarchar](max) NULL,
	[AllowComments] [bit] NOT NULL,
	[AdminModified] [bit] NOT NULL,
	[InstructorId] [int] NOT NULL,
	[StudentInCourseId] [int] NULL,
	[FeedbackId] [int] NULL,
	[ReportId] [int] NULL,
	[BackupCourseId] [int] NULL,
	[BackupChapterId] [int] NULL,
	[CommentCourseId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Discounts]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiscountPercentage] [decimal](18, 2) NOT NULL,
	[DiscountCode] [nvarchar](max) NOT NULL,
	[ExpireDateTime] [datetime2](7) NULL,
	[isAvalaible] [bit] NOT NULL,
	[DiscountStatus] [int] NULL,
	[CourseId] [int] NULL,
	[InstructorDataId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Discounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedbacks]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedbacks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Ordering] [int] NULL,
	[Attachment] [nvarchar](max) NULL,
	[ReviewPoint] [int] NOT NULL,
	[IsMarkGood] [bit] NOT NULL,
	[CourseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Feedbacks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Footers]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Footers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PhoneNumber] [nvarchar](max) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[WorkingTime] [nvarchar](max) NOT NULL,
	[Privacy] [nvarchar](max) NOT NULL,
	[Term_of_use] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Footers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Headers]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Headers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BranchName] [nvarchar](max) NOT NULL,
	[SupportHotline] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Headers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HistoryCourseDiscounts]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoryCourseDiscounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiscountPercentage] [decimal](18, 2) NULL,
	[InstructorId] [int] NULL,
	[CourseId] [int] NULL,
	[DiscountId] [int] NULL,
	[InstructorDataId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_HistoryCourseDiscounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HistoryCourses]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoryCourses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CourseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_HistoryCourses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InstructorData]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InstructorData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaxNumber] [nvarchar](max) NULL,
	[CardNumber] [nvarchar](max) NULL,
	[CardName] [nvarchar](max) NULL,
	[CardProvider] [nvarchar](max) NOT NULL,
	[Certification] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_InstructorData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InstructorPayouts]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InstructorPayouts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PayoutDate] [datetime2](7) NOT NULL,
	[RequestDate] [datetime2](7) NOT NULL,
	[RejectionDate] [datetime2](7) NULL,
	[PayoutAmount] [decimal](18, 2) NOT NULL,
	[PayoutStatus] [nvarchar](max) NOT NULL,
	[RejectionReason] [nvarchar](max) NULL,
	[InstructorId] [int] NOT NULL,
	[InstructorDataId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_InstructorPayouts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[StudentOrderId] [int] NOT NULL,
	[CoursePrice] [decimal](18, 2) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentAmount] [decimal](18, 2) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[PaymentMethod] [nvarchar](max) NOT NULL,
	[PaymentStatus] [nvarchar](max) NOT NULL,
	[PaymentUrl] [nvarchar](max) NOT NULL,
	[TransactionId] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[StudentOrderId] [int] NULL,
	[InstructorPayoutId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportFeedbacks]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportFeedbacks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportReason] [nvarchar](500) NOT NULL,
	[IsHidden] [bit] NOT NULL,
	[FeedbackId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[InstructorDataId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_ReportFeedbacks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reports]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[Attachment] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[UserId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[ChapterId] [int] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentInCourses]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentInCourses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[InstructorId] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[IsFinish] [bit] NOT NULL,
	[Progress] [decimal](18, 2) NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_StudentInCourses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentOrders]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[OrderStatus] [nvarchar](max) NOT NULL,
	[TotalPrice] [decimal](18, 2) NOT NULL,
	[UserId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedBy] [nvarchar](max) NULL,
	[DeletedDate] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](max) NULL,
	[IsDelete] [bit] NULL,
 CONSTRAINT [PK_StudentOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 8/6/2024 4:45:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[Birthday] [datetime2](7) NULL,
	[VerificationToken] [nvarchar](max) NULL,
	[IsVerified] [bit] NOT NULL,
	[ResetToken] [nvarchar](max) NULL,
	[ResetTokenExpiry] [datetime2](7) NULL,
	[Otp] [nvarchar](max) NULL,
	[OtpExpiryTime] [datetime2](7) NULL,
	[RoleId] [int] NOT NULL,
	[LastActiveTime] [datetime2](7) NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[StudentInCourseId] [int] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_BackupChapters_BackupCourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_BackupChapters_BackupCourseId] ON [dbo].[BackupChapters]
(
	[BackupCourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BackupChapters_ChapterId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BackupChapters_ChapterId] ON [dbo].[BackupChapters]
(
	[ChapterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BackupCourses_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_BackupCourses_CourseId] ON [dbo].[BackupCourses]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookmarkedCourses_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_BookmarkedCourses_CourseId] ON [dbo].[BookmarkedCourses]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_BookmarkedCourses_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_BookmarkedCourses_UserId] ON [dbo].[BookmarkedCourses]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Categories_ParentCategoryId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_ParentCategoryId] ON [dbo].[Categories]
(
	[ParentCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Chapters_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Chapters_CourseId] ON [dbo].[Chapters]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CommentCourses_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_CommentCourses_CourseId] ON [dbo].[CommentCourses]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CommentCourses_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_CommentCourses_UserId] ON [dbo].[CommentCourses]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CommentUsers_CommentedById]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_CommentUsers_CommentedById] ON [dbo].[CommentUsers]
(
	[CommentedById] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CommentUsers_InstructorDataId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_CommentUsers_InstructorDataId] ON [dbo].[CommentUsers]
(
	[InstructorDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CommentUsers_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_CommentUsers_UserId] ON [dbo].[CommentUsers]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CourseCategories_CategoryId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_CourseCategories_CategoryId] ON [dbo].[CourseCategories]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Courses_InstructorId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Courses_InstructorId] ON [dbo].[Courses]
(
	[InstructorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Courses_StudentInCourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Courses_StudentInCourseId] ON [dbo].[Courses]
(
	[StudentInCourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Discounts_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Discounts_CourseId] ON [dbo].[Discounts]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Discounts_InstructorDataId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Discounts_InstructorDataId] ON [dbo].[Discounts]
(
	[InstructorDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Feedbacks_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Feedbacks_CourseId] ON [dbo].[Feedbacks]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Feedbacks_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Feedbacks_UserId] ON [dbo].[Feedbacks]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HistoryCourseDiscounts_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_HistoryCourseDiscounts_CourseId] ON [dbo].[HistoryCourseDiscounts]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HistoryCourseDiscounts_DiscountId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_HistoryCourseDiscounts_DiscountId] ON [dbo].[HistoryCourseDiscounts]
(
	[DiscountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HistoryCourseDiscounts_InstructorDataId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_HistoryCourseDiscounts_InstructorDataId] ON [dbo].[HistoryCourseDiscounts]
(
	[InstructorDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HistoryCourseDiscounts_InstructorId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_HistoryCourseDiscounts_InstructorId] ON [dbo].[HistoryCourseDiscounts]
(
	[InstructorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HistoryCourses_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_HistoryCourses_CourseId] ON [dbo].[HistoryCourses]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_HistoryCourses_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_HistoryCourses_UserId] ON [dbo].[HistoryCourses]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InstructorData_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_InstructorData_UserId] ON [dbo].[InstructorData]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InstructorPayouts_InstructorDataId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_InstructorPayouts_InstructorDataId] ON [dbo].[InstructorPayouts]
(
	[InstructorDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_InstructorPayouts_InstructorId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_InstructorPayouts_InstructorId] ON [dbo].[InstructorPayouts]
(
	[InstructorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_OrderDetails_CourseId] ON [dbo].[OrderDetails]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_StudentOrderId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_StudentOrderId] ON [dbo].[OrderDetails]
(
	[StudentOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_InstructorPayoutId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_InstructorPayoutId] ON [dbo].[Payments]
(
	[InstructorPayoutId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_StudentOrderId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_StudentOrderId] ON [dbo].[Payments]
(
	[StudentOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Payments_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Payments_UserId] ON [dbo].[Payments]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReportFeedbacks_FeedbackId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ReportFeedbacks_FeedbackId] ON [dbo].[ReportFeedbacks]
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReportFeedbacks_InstructorDataId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReportFeedbacks_InstructorDataId] ON [dbo].[ReportFeedbacks]
(
	[InstructorDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ReportFeedbacks_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_ReportFeedbacks_UserId] ON [dbo].[ReportFeedbacks]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reports_ChapterId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Reports_ChapterId] ON [dbo].[Reports]
(
	[ChapterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reports_CourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Reports_CourseId] ON [dbo].[Reports]
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Reports_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Reports_UserId] ON [dbo].[Reports]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StudentOrders_UserId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_StudentOrders_UserId] ON [dbo].[StudentOrders]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_RoleId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Users_RoleId] ON [dbo].[Users]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_StudentInCourseId]    Script Date: 8/6/2024 4:45:52 PM ******/
CREATE NONCLUSTERED INDEX [IX_Users_StudentInCourseId] ON [dbo].[Users]
(
	[StudentInCourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BackupChapters]  WITH CHECK ADD  CONSTRAINT [FK_BackupChapters_BackupCourses_BackupCourseId] FOREIGN KEY([BackupCourseId])
REFERENCES [dbo].[BackupCourses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BackupChapters] CHECK CONSTRAINT [FK_BackupChapters_BackupCourses_BackupCourseId]
GO
ALTER TABLE [dbo].[BackupChapters]  WITH CHECK ADD  CONSTRAINT [FK_BackupChapters_Chapters_ChapterId] FOREIGN KEY([ChapterId])
REFERENCES [dbo].[Chapters] ([Id])
GO
ALTER TABLE [dbo].[BackupChapters] CHECK CONSTRAINT [FK_BackupChapters_Chapters_ChapterId]
GO
ALTER TABLE [dbo].[BackupCourses]  WITH CHECK ADD  CONSTRAINT [FK_BackupCourses_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BackupCourses] CHECK CONSTRAINT [FK_BackupCourses_Courses_CourseId]
GO
ALTER TABLE [dbo].[BookmarkedCourses]  WITH CHECK ADD  CONSTRAINT [FK_BookmarkedCourses_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookmarkedCourses] CHECK CONSTRAINT [FK_BookmarkedCourses_Courses_CourseId]
GO
ALTER TABLE [dbo].[BookmarkedCourses]  WITH CHECK ADD  CONSTRAINT [FK_BookmarkedCourses_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BookmarkedCourses] CHECK CONSTRAINT [FK_BookmarkedCourses_Users_UserId]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY([ParentCategoryId])
REFERENCES [dbo].[Categories] ([Id])
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_Categories_ParentCategoryId]
GO
ALTER TABLE [dbo].[Chapters]  WITH CHECK ADD  CONSTRAINT [FK_Chapters_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Chapters] CHECK CONSTRAINT [FK_Chapters_Courses_CourseId]
GO
ALTER TABLE [dbo].[CommentCourses]  WITH CHECK ADD  CONSTRAINT [FK_CommentCourses_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[CommentCourses] CHECK CONSTRAINT [FK_CommentCourses_Courses_CourseId]
GO
ALTER TABLE [dbo].[CommentCourses]  WITH CHECK ADD  CONSTRAINT [FK_CommentCourses_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[CommentCourses] CHECK CONSTRAINT [FK_CommentCourses_Users_UserId]
GO
ALTER TABLE [dbo].[CommentUsers]  WITH CHECK ADD  CONSTRAINT [FK_CommentUsers_InstructorData_InstructorDataId] FOREIGN KEY([InstructorDataId])
REFERENCES [dbo].[InstructorData] ([Id])
GO
ALTER TABLE [dbo].[CommentUsers] CHECK CONSTRAINT [FK_CommentUsers_InstructorData_InstructorDataId]
GO
ALTER TABLE [dbo].[CommentUsers]  WITH CHECK ADD  CONSTRAINT [FK_CommentUsers_Users_CommentedById] FOREIGN KEY([CommentedById])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[CommentUsers] CHECK CONSTRAINT [FK_CommentUsers_Users_CommentedById]
GO
ALTER TABLE [dbo].[CommentUsers]  WITH CHECK ADD  CONSTRAINT [FK_CommentUsers_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[CommentUsers] CHECK CONSTRAINT [FK_CommentUsers_Users_UserId]
GO
ALTER TABLE [dbo].[CourseCategories]  WITH CHECK ADD  CONSTRAINT [FK_CourseCategories_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CourseCategories] CHECK CONSTRAINT [FK_CourseCategories_Categories_CategoryId]
GO
ALTER TABLE [dbo].[CourseCategories]  WITH CHECK ADD  CONSTRAINT [FK_CourseCategories_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CourseCategories] CHECK CONSTRAINT [FK_CourseCategories_Courses_CourseId]
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD  CONSTRAINT [FK_Courses_StudentInCourses_StudentInCourseId] FOREIGN KEY([StudentInCourseId])
REFERENCES [dbo].[StudentInCourses] ([Id])
GO
ALTER TABLE [dbo].[Courses] CHECK CONSTRAINT [FK_Courses_StudentInCourses_StudentInCourseId]
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD  CONSTRAINT [FK_Courses_Users_InstructorId] FOREIGN KEY([InstructorId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Courses] CHECK CONSTRAINT [FK_Courses_Users_InstructorId]
GO
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_Courses_CourseId]
GO
ALTER TABLE [dbo].[Discounts]  WITH CHECK ADD  CONSTRAINT [FK_Discounts_InstructorData_InstructorDataId] FOREIGN KEY([InstructorDataId])
REFERENCES [dbo].[InstructorData] ([Id])
GO
ALTER TABLE [dbo].[Discounts] CHECK CONSTRAINT [FK_Discounts_InstructorData_InstructorDataId]
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD  CONSTRAINT [FK_Feedbacks_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[Feedbacks] CHECK CONSTRAINT [FK_Feedbacks_Courses_CourseId]
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD  CONSTRAINT [FK_Feedbacks_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feedbacks] CHECK CONSTRAINT [FK_Feedbacks_Users_UserId]
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts]  WITH CHECK ADD  CONSTRAINT [FK_HistoryCourseDiscounts_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts] CHECK CONSTRAINT [FK_HistoryCourseDiscounts_Courses_CourseId]
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts]  WITH CHECK ADD  CONSTRAINT [FK_HistoryCourseDiscounts_Discounts_DiscountId] FOREIGN KEY([DiscountId])
REFERENCES [dbo].[Discounts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts] CHECK CONSTRAINT [FK_HistoryCourseDiscounts_Discounts_DiscountId]
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts]  WITH CHECK ADD  CONSTRAINT [FK_HistoryCourseDiscounts_InstructorData_InstructorDataId] FOREIGN KEY([InstructorDataId])
REFERENCES [dbo].[InstructorData] ([Id])
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts] CHECK CONSTRAINT [FK_HistoryCourseDiscounts_InstructorData_InstructorDataId]
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts]  WITH CHECK ADD  CONSTRAINT [FK_HistoryCourseDiscounts_Users_InstructorId] FOREIGN KEY([InstructorId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HistoryCourseDiscounts] CHECK CONSTRAINT [FK_HistoryCourseDiscounts_Users_InstructorId]
GO
ALTER TABLE [dbo].[HistoryCourses]  WITH CHECK ADD  CONSTRAINT [FK_HistoryCourses_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HistoryCourses] CHECK CONSTRAINT [FK_HistoryCourses_Courses_CourseId]
GO
ALTER TABLE [dbo].[HistoryCourses]  WITH CHECK ADD  CONSTRAINT [FK_HistoryCourses_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HistoryCourses] CHECK CONSTRAINT [FK_HistoryCourses_Users_UserId]
GO
ALTER TABLE [dbo].[InstructorData]  WITH CHECK ADD  CONSTRAINT [FK_InstructorData_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InstructorData] CHECK CONSTRAINT [FK_InstructorData_Users_UserId]
GO
ALTER TABLE [dbo].[InstructorPayouts]  WITH CHECK ADD  CONSTRAINT [FK_InstructorPayouts_InstructorData_InstructorDataId] FOREIGN KEY([InstructorDataId])
REFERENCES [dbo].[InstructorData] ([Id])
GO
ALTER TABLE [dbo].[InstructorPayouts] CHECK CONSTRAINT [FK_InstructorPayouts_InstructorData_InstructorDataId]
GO
ALTER TABLE [dbo].[InstructorPayouts]  WITH CHECK ADD  CONSTRAINT [FK_InstructorPayouts_Users_InstructorId] FOREIGN KEY([InstructorId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InstructorPayouts] CHECK CONSTRAINT [FK_InstructorPayouts_Users_InstructorId]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Courses_CourseId]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_StudentOrders_StudentOrderId] FOREIGN KEY([StudentOrderId])
REFERENCES [dbo].[StudentOrders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_StudentOrders_StudentOrderId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_InstructorPayouts_InstructorPayoutId] FOREIGN KEY([InstructorPayoutId])
REFERENCES [dbo].[InstructorPayouts] ([Id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_InstructorPayouts_InstructorPayoutId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_StudentOrders_StudentOrderId] FOREIGN KEY([StudentOrderId])
REFERENCES [dbo].[StudentOrders] ([Id])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_StudentOrders_StudentOrderId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Users_UserId]
GO
ALTER TABLE [dbo].[ReportFeedbacks]  WITH CHECK ADD  CONSTRAINT [FK_ReportFeedbacks_Feedbacks_FeedbackId] FOREIGN KEY([FeedbackId])
REFERENCES [dbo].[Feedbacks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReportFeedbacks] CHECK CONSTRAINT [FK_ReportFeedbacks_Feedbacks_FeedbackId]
GO
ALTER TABLE [dbo].[ReportFeedbacks]  WITH CHECK ADD  CONSTRAINT [FK_ReportFeedbacks_InstructorData_InstructorDataId] FOREIGN KEY([InstructorDataId])
REFERENCES [dbo].[InstructorData] ([Id])
GO
ALTER TABLE [dbo].[ReportFeedbacks] CHECK CONSTRAINT [FK_ReportFeedbacks_InstructorData_InstructorDataId]
GO
ALTER TABLE [dbo].[ReportFeedbacks]  WITH CHECK ADD  CONSTRAINT [FK_ReportFeedbacks_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ReportFeedbacks] CHECK CONSTRAINT [FK_ReportFeedbacks_Users_UserId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Chapters_ChapterId] FOREIGN KEY([ChapterId])
REFERENCES [dbo].[Chapters] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Chapters_ChapterId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Courses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([Id])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Courses_CourseId]
GO
ALTER TABLE [dbo].[Reports]  WITH CHECK ADD  CONSTRAINT [FK_Reports_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Reports] CHECK CONSTRAINT [FK_Reports_Users_UserId]
GO
ALTER TABLE [dbo].[StudentOrders]  WITH CHECK ADD  CONSTRAINT [FK_StudentOrders_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[StudentOrders] CHECK CONSTRAINT [FK_StudentOrders_Users_UserId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles_RoleId]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_StudentInCourses_StudentInCourseId] FOREIGN KEY([StudentInCourseId])
REFERENCES [dbo].[StudentInCourses] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_StudentInCourses_StudentInCourseId]
GO
USE [master]
GO
ALTER DATABASE [Curus] SET  READ_WRITE 
GO
