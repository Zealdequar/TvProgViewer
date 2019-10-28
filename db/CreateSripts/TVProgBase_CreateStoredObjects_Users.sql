use TVProgBase;
GO

if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnIsUserNameExists'))
drop function fnIsUserNameExists;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnIsEmailExists'))
drop function fnIsEmailExists;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'spUserStart'))
drop procedure spUserStart;
if (exists(select * from INFORMATION_SCHEMA.ROUTINES where SPECIFIC_SCHEMA = N'dbo' and SPECIFIC_NAME = N'fnGetUserIDByUserName'))
drop function fnGetUserIDByUserName;
GO

-- �������� ����������������� ����� �� ���������� � �������
create function fnIsUserNameExists (@UserName nvarchar(70))
returns bit
as 
begin
	if (exists(select 1	
	from dbo.SystemUsers su
	where su.UserName Like @UserName))
		return 1;
	else
		return 0;
	return 0;
end
go

-- �������� �� ���������� ������ �� email � �������
create function fnIsEmailExists(@Email nvarchar(300))
returns bit
as
begin
	if (exists(select 1
	from dbo.SystemUsers su
	where su.Email Like @Email))
		return 1;
	else 
	return 0;
	return 0;		
end;
go

-- ��������� ����������� ������ ������������ � ������� 
create procedure [dbo].[spUserStart] (
@UserName nvarchar(70), 
@PassHash nvarchar(100),
@PassExtend nvarchar(100),
@LastName nvarchar(150),
@FirstName nvarchar(150),
@MiddleName nvarchar(150),
@BirthDate datetime,
@Gender bit,
@Email nvarchar(300),
@MobPhoneNumber nvarchar(25),
@OtherPhoneNumber1 nvarchar(25),
@OtherPhoneNumber2 nvarchar(25),
@Address nvarchar(1000),
@GmtZone nvarchar(10),
@Catalog nvarchar(36),
@ErrCode int out
)
as 
declare 
@cntEmail int,
@UID bigint,
@GID bigint
begin
/*ErrCodes: 2 - ����� ��� ������������ ��� ����������. ������� ������ ��� ������������.;
			3 - ������������ � ����� e-mail ��� ��������������� � �������. ������� ������ e-mail. 
			70 � ��������� ������ ��� ����������� � �������. ���������� ��������� ����������� �������. 
			72 - ������ ��� ������� ������ ��� ������� ������
			73 - ������ ��� ������� ��������� ��� ������� ���������
			*/	
	set @ErrCode = 0;			
	if (dbo.fnIsUserNameExists(@UserName) = 1)
	begin
		set @ErrCode = 2;
		return;
	end;	
	
	if (dbo.fnIsEmailExists(@Email) = 1)
	begin
		set @ErrCode = 3;
		return;
	end;
	
	begin try
	insert into dbo.SystemUsers (
	UserName, 
	PassHash, 
	PassExtend, 
	LastName, 
	FirstName, 
	MiddleName, 
	BirthDate,
	Gender,
	Email,
	MobPhoneNumber,
	OtherPhoneNumber1,
	OtherPhoneNumber2,
	Address,
	GmtZone,
	Status,
	Catalog
	)
	values
	(
	@UserName,
	@PassHash,
	@PassExtend,
	@LastName,
	@FirstName,
	@MiddleName,
	@BirthDate,
	@Gender,
	@Email,
	@MobPhoneNumber,
	@OtherPhoneNumber1,
	@OtherPhoneNumber2,
	@Address,
	@GmtZone,
	3,
	@Catalog
	);
	end try
	begin catch
		select @ErrCode = 70;
	end catch
	set @UID = SCOPE_IDENTITY();
	
	begin try
	insert dbo.ExtUserSettings (UID, TVProgProviderID, UncheckedChannels)
	values (@UID, 1, 1);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 1, '��� ����', 1);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 2, '�������������� �����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '�����;���;�/�', 1, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�.�', '', 2, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�. �', '', 3, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�. �', '', 4, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 5,NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�/�', '', 6, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�\\�', '', 7, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 8, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 3, '������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�/�', '�/�', 9, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����������', '', 10, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '�/;�����;������', 11, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 4, '�����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 12, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 13, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 14, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 15, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� ����, ������', '', 16, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�� 16', '', 17, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 18,  NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 19, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 5, '�����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������','', 20, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����-���','', 21, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������','', 22, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����������','', 23, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������','', 24, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 25, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 26, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������','', 27, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������','', 28, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������','', 29, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 30, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ��� ������','', 31, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������','', 32, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������','', 33, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���','', 34, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����','', 35, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������','', 36, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����','', 37, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���','', 38, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����','�����', 39, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������','', 40, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������','', 41, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������','', 42, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����','', 43, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 44, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 45, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 46, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 47, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ �����', '', 48, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 49, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 6, '�������������� �����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���.', '', 50, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�. �', '', 51, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 52, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 53, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�. �.', '', 54, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�/�', '', 55, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�/�', '', 56, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 7, '������. ���������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '�����', 57, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '� ��������', 58, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '���;������', 59, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '������', 60, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� ���������', '', 61, NULL)
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 62, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 63, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 64, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 65, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'News', '', 66, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 67, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ ����', '', 68, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� �� ����', '', 69, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 70, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '24', '', 71, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 72, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ �����', '', 73, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������������', '', 74, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ � ���', '', 75, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 76, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 8, '����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 77, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 78, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '', 79, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� ������� � ����������', '', 80, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������;�������', '', 81, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 82, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 83, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 84, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 85, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�. �. �.', '', 86, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �������', '', 87, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 88, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 89, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 90, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 91, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 92, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 9, '����-���', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '', 93, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 94, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ���� ��������', '', 95, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ ��������', '', 96, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����� ���������!', '', 97, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����� �������', '', 98, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� 2', '', 99, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ����', '', 100, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 101, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 10, '����-����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '�������;�����;�������', 102, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� �����', '', 103, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '�����', 104, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ � ������', '', 105, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ ���������', '', 106, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ����� ����� �����������', '', 107, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���? ���? �����?', '', 108, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� ����', '', 109, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 11, '����� � ����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 110, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 111, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '������;������', 112, NULL)
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 113, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 114, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 115, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 116, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Stop! �����!', '', 117, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ ������', '', 118, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �����', '', 119, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��.', '', 120, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 121, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 122, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 12, '����������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�/', '', 123, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�. �', '', 124, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�/ ������', '', 125, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 126, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 127, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 128, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 129, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Fox Kids', '', 130, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 13, '��������', 1	);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 131, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 132, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 133, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 134, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 135, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 136, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� �������', '', 137, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� �����', '', 138, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������������ ������������',  '', 139, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 140, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����������', '', 141, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 142, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������������', '', 143, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 144, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������������� ���������', '', 145, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 14, '������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '����', 146, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 147, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 148, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 149, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '�����', 150, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 151, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 152, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '', 153, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 154, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� �����', '', 155, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� �����', '', 156, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 157, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '', 158, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 159, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 160, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 161, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 162, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� 10-��', '', 163, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� 10-��', '', 164, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 165, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������!', '', 166, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Extra', '', 167, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Impulse', '', 168, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 169, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �������', '', 170, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� ������', '', 171, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '10 �����', '', 172, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 15, '��������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 173, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '.Ru', '', 174, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '.NET', '', 175, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 176, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� �������', '', 177, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 178, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'www', '', 179, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 16, '� ��������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 180, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '', 181, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 182, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 183, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '� � ��� ������', '', 184, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� � �������', '', 185, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 186, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� � �����', '', 187, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 188, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 189, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 190, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 191, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 17, '������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 192, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� �������', '', 193, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 18, '��������� � ��������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '����', 194, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 195, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 196, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 197, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����������', '', 198, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 199, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� �����', '', 200, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� ����', '', 201, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 202, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 19, '�������� � ����������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 203, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����������', '', 204, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 205, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 206, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������������', '', 207, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '��������', 208, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 20, '������-���������� ��������', 1);
	set @GID = SCOPE_IDENTITY(); 	
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'X������', '', 209, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 210, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 211, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 212, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 213, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 214, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 21, '����������� � �����������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����','', 215, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 216, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� �������', '', 217, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� �� �����', '', 218, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 219, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������� �����', '', 220, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �����', '', 221, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ �����', '', 222, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� ��� ��������', '', 223, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 224, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����������', '', 225, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����� ������', '', 226, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� � ���', '', 227, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 228, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� ��������', '', 229, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�� �����', '', 230, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 231, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 22, '�������', 1); 
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '� ������', '', 232, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������������', '', 233, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���� ��� ����', '', 234, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 235, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� ������', '', 236, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �', '', 237, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����� ������������� �����', '', 238, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� � �������', '', 239, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 240, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ���������', '', 241, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ ������', '', 242, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 23, '�������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 243, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 244, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 245, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 246, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'PRO-�������', '', 247, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 24, '����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 248, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 249, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'fashion', '', 250, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� �������', '', 251, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 252, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 253, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� ������', '', 254, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����������', '', 255, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 25, '����', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����;����', '�����;����', 256, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������� ���', '', 257, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 258, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 259, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, 'Top Gear', '', 260, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 26, '��������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 261, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� �������', '', 262, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 263, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���.', '', 264, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 265, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 27, '�������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 266, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 267, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 268, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������������ ����� ���������', '', 269, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '�����', 270, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�� ����������� ����������', '', 271, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ �����', '', 272, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 273, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������� �����', '', 274, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 275, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��������', '', 276, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 277, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ���� ����', '', 278, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 279, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 28, '�����������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���', '������' , 280, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������������ �����', '', 281, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 282, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '��������', 283, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 29, '�������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 284, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 285, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����������', '', 286, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 30, '�������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�� ����� �������', '', 287, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� ����', '', 288, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �������', '', 289, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '��� ������', '', 290, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����;����', '', 291, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 31, '���������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 292, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �������', '', 293, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �������', '', 294, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 295, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������', '', 296, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 297, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 298, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������� �������', '', 299, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 300, NULL);
	
	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 32, '�������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '�����', 301, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������ ��������', '', 302, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '���������� ������', '', 303, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������� �����', '', 304, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�������', '', 305, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 33, '��������� � ������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 306, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 307, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����������', '', 308, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '������', '', 309, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '�����', '', 310, NULL);

	insert dbo.Genres (UID, IconID, GenreName, Visible) values (@UID, 34, '�������', 1);
	set @GID = SCOPE_IDENTITY(); 
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 311, NULL);
	insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, OrderCol, DeleteAfterDate) values (@GID, @UID, '����', '', 312, NULL);
	end try
	begin catch
		set @ErrCode = 72;
	end catch 
	
	begin try    
		insert dbo.Ratings (uid, IconID, RatingName, Visible) 
		values (@UID, 37, '��� ��������', 1),
		(@UID, 38, '����� ����������', 1),
		(@UID, 39, '���������', 1),
		(@UID, 40, '����������',1),
		(@UID, 41, '�������', 1),
		(@UID, 42, '��������',1);
	end try
	begin catch
		set @ErrCode = 73;
	end catch 
end;
GO

-- ��������� UserID �� UserNanme
create function fnGetUserIDByUserName(@UserName nvarchar(70))
returns int
as
begin
	declare @UserID int;
	select @UserID = su.UserID 
	from dbo.SystemUsers su
	where su.UserName Like @UserName;
	return @UserID;
end;
go