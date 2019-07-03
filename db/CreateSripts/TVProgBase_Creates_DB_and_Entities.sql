use [master]
go

create database [TVProgBase] on primary
(NAME = 'TVProgBase_data', FILENAME = N'H:\TEMP\Warehouse\TVProgHome\TVProgBase_data.mdf', SIZE = 2304KB, MAXSIZE=UNLIMITED, FILEGROWTH=1024KB)
LOG ON
(NAME = 'TVProgBase_log', FILENAME = N'H:\TEMP\Warehouse\TVProgHome\TVProgBase_log.ldf', SIZE = 1024KB, MAXSIZE=2GB, FILEGROWTH=10%) 
GO

alter database [TVProgBase] set COMPATIBILITY_LEVEL = 100
go

use TVProgBase
go

if (exists(select * from sys.tables where name = 'UsersPrograms'))
drop table dbo.UsersPrograms
if (exists(select * from sys.tables where name = 'GenreClassificator'))
drop table dbo.GenreClassificator
if (exists(select * from sys.tables where name = 'Genres'))
drop table dbo.Genres
if (exists(select * from sys.tables where name = 'RatingClassificator'))
drop table dbo.RatingClassificator
if (exists(select * from sys.tables where name = 'Ratings'))
drop table dbo.Ratings 
if (exists(select * from sys.tables where name = 'UserChannels'))
drop table dbo.UserChannels
if (exists(select * from sys.tables where name = 'Programmes'))
drop table dbo.Programmes
if (exists(select * from sys.tables where name = 'UpdateProgLog'))
drop table dbo.UpdateProgLog
if (exists(select * from sys.tables where name = 'WebResources'))
drop table dbo.WebResources
if (exists(select * from sys.tables where name = 'TypeProg'))
drop table dbo.TypeProg
if (exists(select * from sys.tables where name = 'Channels'))
drop table dbo.Channels
if (exists(select * from sys.tables where name = 'MediaPic'))
drop table dbo.MediaPic
if (exists(select * from sys.tables where name = 'ExtUserSettings'))
drop table dbo.ExtUserSettings
if (exists(select * from sys.tables where name = 'TVProgProviders'))
drop table dbo.TVProgProviders
if (exists(select * from sys.tables where name = 'SearchSettings'))
drop table dbo.SearchSettings
if (exists(select * from sys.tables where name = 'SystemUsers'))
drop table dbo.SystemUsers
if (exists(select * from sys.tables where name = 'LogActions'))
drop table dbo.LogActions
go

-- ������������
create table dbo.SystemUsers
(
	UserID bigint IDENTITY(1,1),
	UserName nvarchar(70) unique not null,
	PassHash nvarchar (100) not null,
	PassExtend nvarchar (100) not null,
	LastName nvarchar(150) not null,
	FirstName nvarchar (150) not null,
	MiddleName nvarchar (150) null,
	CreateDate datetimeoffset default SysDateTimeOffset() not null,
	BirthDate datetime not null,
	Gender bit null,
	Email nvarchar(300) unique not null,
	MobPhoneNumber nvarchar(25) null,
	OtherPhoneNumber1 nvarchar(25) null,
	OtherPhoneNumber2 nvarchar(25) null,
	DateBegin datetime default '1800-01-01' not null,
	DateEnd datetime default '9999-31-12' not null,
	Country nvarchar(40) null,
	Address nvarchar(1000) null,
	GmtZone nvarchar(10) null,
	Status smallint not null,
	Catalog nvarchar(36) not null,
	Constraint PK_SystemUsers Primary Key Clustered
	(
	UserID Asc
	)
)
go

-- ����������
create table dbo.TVProgProviders
(
   TVProgProviderID int IDENTITY(1,1),
   ProviderName nvarchar(150) NOT NULL,
   ProviderWebSite nvarchar(100) NOT NULL,
   ContactName nvarchar(250) NULL,
   ContactEmail nvarchar(100) NULL,
   Rss nvarchar(200) NULL,
   Constraint PK_TVProgProviders Primary Key Clustered
   (
   TVProgProviderID Asc
   )
)
go

--����������� ��� ������������ ��������
create table dbo.MediaPic
(
	IconID bigint not null IDENTITY(1,1),
	FileName nvarchar(256) not null,
	ContentType nvarchar(256) not null,
	ContentCoding nvarchar(256) not null,
	Length int not null,
	Length25 int null,
	IsSystem bit not null,
	PathOrig nvarchar(256) null,
	Path25 nvarchar(256) null,
	Constraint PK_MediaPic Primary Key Clustered
	(
	IconID ASC
	),
	Constraint Unique_FileName unique 
	(
	FileName
	)
)
go

--������
create table dbo.Channels
(
	ChannelID int IDENTITY(1,1),
	TVProgProviderID int not null references dbo.TVProgProviders(TVProgProviderID),
	InternalID int null,
	IconID bigint null references dbo.MediaPic(IconID),
	CreateDate datetimeoffset default SysDateTimeOffset() not null,
	TitleChannel nvarchar(300) not null,
	IconWebSrc nvarchar(550) null,
	Deleted datetime null,
	Constraint PK_Channels Primary Key Clustered
	(
	ChannelID ASC
	)
)
go

-- ���������������� �����
create table dbo.Genres
(
	GenreID bigint IDENTITY(1,1),
	UID bigint null references dbo.SystemUsers(UserID),
	IconID bigint null references dbo.MediaPic(IconID),
	CreateDate datetimeoffset default SysDateTimeOffset() not null,
	GenreName nvarchar(150) not null,
	Visible bit not null,
	DeleteDate datetimeoffset null,
	Constraint PK_Genres Primary Key Clustered
	(
	GenreID Asc
	)
)
GO

-- ������������� ������
create table dbo.GenreClassificator
(
	GenreClassificatorID bigint IDENTITY(1,1),
	GID bigint not null references dbo.Genres(GenreID),
	UID bigint null references dbo.SystemUsers(UserID),
	ContainPhrases nvarchar(350) null,
	NonContainPhrases nvarchar(350) null,
	OrderCol int null,
	DeleteAfterDate datetime null
	Constraint PK_GenreClassificator Primary Key Clustered
	(
	GenreClassificatorID Asc
	)
)
GO

-- ���������������� ��������
create table dbo.Ratings
(
	RatingID bigint IDENTITY(1,1),
	UID bigint null references dbo.Systemusers(UserID),
	IconID bigint null references dbo.MediaPic(IconID),
	CreateDate datetimeoffset default SysDateTimeOffset() not null,
	RatingName nvarchar(150) not null,
	Visible bit not null,
	Constraint PK_Ratings Primary Key Clustered
	(
	RatingID Asc
	)
)
GO

-- ������������� ��������
create table dbo.RatingClassificator
(
	RatingClassificatorID bigint IDENTITY(1,1),
	RID bigint not null references dbo.Ratings(RatingID),
	UID bigint null references dbo.Systemusers(UserID),
	ContainPhrases nvarchar(350) not null,
	NonContainPhrases nvarchar(350) null,
	OrderCol int null,
	DeleteAfterDate datetime null,
	Constraint PK_RatingClassificator Primary Key Clustered
	(
		RatingClassificatorID Asc
	)
)
GO
-- ���������������� ������
create table dbo.UserChannels
(
	UserChannelID int IDENTITY(1,1),
	UID bigint not null references dbo.SystemUsers(UserID),
	CID int not null references dbo.Channels(ChannelID),
	IconID bigint null references dbo.MediaPic(IconID),
	DisplayName nvarchar(300) null,
	OrderCol int null,
	Constraint PK_UserChannels Primary Key Clustered
	(
	UserChannelID Asc
	)
)
GO

-- ���� ������������
create table dbo.TypeProg
(
	TypeProgID int Identity(1,1),
	TVProgProviderID int not null references dbo.TVProgProviders(TVProgProviderID),
	TypeName nvarchar(150) not null,
	FileFormat nvarchar(5) null,
	Constraint PK_TypeProg Primary Key Clustered
	(
	TypeProgID Asc
	)
)
GO
-- ���-��������� ������������
create table dbo.WebResources
(
	WebResourceID int IDENTITY(1,1),
	TPID int references dbo.TypeProg(TypeProgID) not null,
	FileName nvarchar(300) not null, 
	ResourceName nvarchar(150) not null,
	ResourceUrl nvarchar(500) not null
	Constraint PK_WebResources Primary Key Clustered
	(
	WebResourceID Asc
	)
)
GO
-- ���� ���������� ������������� � ��������
create table dbo.UpdateProgLog
(
	UpdateProgLogID bigint IDENTITY(1,1),
	WRID int references dbo.WebResources(WebResourceID) not null,
	TsUpdateStart datetimeoffset not null,
	TsUpdateEnd datetimeoffset default SysDateTimeOffset() not null,
	UdtElapsedSec int,
	MinProgDate datetimeoffset not null,
	MaxProgDate datetimeoffset not null,
	QtyChans int not null,
	QtyProgrammes int not null,
	QtyNewChans int not null,
	QtyNewProgrammes int not null,
	IsSuccess bit not null,
	ErrorMessage nvarchar(1000) null,
	Constraint PK_UpdateProgLog Primary Key Clustered
	(
		UpdateProgLogID Asc
	)
)
GO
-- ���������
create table dbo.Programmes
(
	ProgrammesID bigint IDENTITY(1,1),
	TID int references dbo.TypeProg(TypeProgID) not null,
	CID int references dbo.Channels(ChannelID) not null,
	InternalChanID int null,
	TsStart datetimeoffset not null,
	TsStop datetimeoffset not null,
	TsStartMO datetime not null,
	TsStopMO datetime not null,
	Title nvarchar(300) not null,
	Descr nvarchar(1000) null,
	Category nvarchar(500) null,
	Constraint PK_Programmes Primary Key Clustered
	(
		ProgrammesID Asc
	)
)
GO
-- ��������� � ����������� �������������
create table dbo.UsersPrograms 
(
	UserProgramsID bigint IDENTITY(1,1),
	UID bigint references dbo.SystemUsers(UserID) not null,
	UCID int references dbo.UserChannels(UserChannelID) not null,
	PID bigint references dbo.Programmes(ProgrammesID) not null,
	GID bigint references dbo.Genres(GenreID) null,
	RID bigint references dbo.Ratings(RatingID) null,
	Anons nvarchar(1000) null,
	Remind bit not null,
	Constraint PK_UsersPrograms Primary Key Clustered
	(
		UserProgramsID Asc
	)
)
GO
-- ��� �������� �������������
create table dbo.LogActions
(
	LogID int Identity(1,1),
	Login nvarchar(70) not null,
	TsAction datetimeoffset default SysDateTimeOffset(),
	TypeAction smallint not null,
	MessageAction nvarchar(1000) not null,
	IP nvarchar(50) not null,
	UserAgent nvarchar(500) null,
	Constraint PK_LogActions Primary Key Clustered
	(
	LogID Asc
	)
)
GO

-- ��������� ������������ ������
create table dbo.SearchSettings
(
	SearchSettingsID int Identity(1,1),
	UID bigint not null references dbo.SystemUsers(UserID),
	LoadSettings bit not null,
	Match nvarchar(1000) null,
	NotMatch nvarchar(1000) null,
	InAnons bit null,
	TsFinalFrom datetime null,
	TsFinalTo datetime null,
	TrackBarFrom int null,
	TrackBarTo int null,
	Constraint PK_SearchSettings Primary Key Clustered
	(
	SearchSettingsID Asc
	)
)
GO

-- �������������� ���������������� ���������
create table dbo.ExtUserSettings
(
	ExtUserSettingsID int Identity(1,1),
	UID bigint not null references dbo.SystemUsers(UserID),
	TVProgProviderID int not null references dbo.TVProgProviders(TVProgProviderID),
	UncheckedChannels smallint NULL,
	Constraint PK_ExtUserSettingsID Primary Key Clustered
	(
	ExtUserSettingsID Asc
	)
)
GO