insert dbo.SystemUsers (UserName, PassHash, LastName, FirstName, MiddleName, BirthDate, Gender, Email,
MobPhoneNumber,
OtherPhoneNumber1,
OtherPhoneNumber2,
Address,
GmtZone,
Status)
values ('Root', HASHBYTES('sha1', ''), '���������������', '', '', cast('2014-10-17' as datetime), 1,
'tvprogviewer@gmail.com', '', '', '', '', '+03:00', 1);

insert dbo.SystemUsers (UserName, PassHash, LastName, FirstName, MiddleName, BirthDate, Gender, Email,
MobPhoneNumber,
OtherPhoneNumber1,
OtherPhoneNumber2,
Address,
GmtZone,
Status)
values ('edkarpeshin', HASHBYTES('sha1', ''), 'Ka', 'Ed', 'Nik', cast('1900-01-01' as datetime), 1,
'ed@gmail.com', '', '', '', '', '+03:00', 1);
go


/*insert dbo.ExtUserSettings (UID, TVProgProviderID,  UncheckedChannels)
values (2, 1, 1)
go*/
insert dbo.TVProgProviders (ProviderName, ProviderWebSite, ContactName, ContactEmail, Rss)
values ('�������.����', 'http://www.teleguide.info', '', '', 'https://www.teleguide.info/news.xml');
insert dbo.TVProgProviders (ProviderName, ProviderWebSite, ContactName, ContactEmail, Rss)
values ('������������� �����', 'http://tele.perm.ru', '', '', '');

insert dbo.TypeProg (TVProgProviderID, TypeName, FileFormat) values (1, '������ XMLTV', 'xml');
insert dbo.TypeProg (TVProgProviderID, TypeName, FileFormat) values (1, '������ �����-��', 'txt');
insert dbo.TypeProg (TVProgProviderID, TypeName, FileFormat) values (2, '������ XMLTV', 'xml');
go

insert dbo.WebResources(TPID, FileName, ResourceName, ResourceUrl)
values (1, 'TGXmlTvOld.xml', '������ teleguide.info ������ ������������� (XMLTV)', 'http://www.teleguide.info/download/old/xmltv.xml.gz');
insert dbo.WebResources(TPID, FileName, ResourceName, ResourceUrl)
values (1, 'TGXmlTvNew.xml', '������ teleguide.info ����� ������������� (XMLTV)', 'http://www.teleguide.info/download/new3/xmltv.xml.gz');
insert dbo.WebResources(TPID, FileName, ResourceName, ResourceUrl)
values (2, 'TGInterTVOld.txt', '������ teleguide.info ������ ������������� (�����-��)', 'http://www.teleguide.info/download/old/inter-tv.zip');
insert dbo.WebResources(TPID, FileName, ResourceName, ResourceUrl)
values (2, 'TGInterTVNew.txt', '������ teleguide.info ����� ������������� (�����-��)', 'http://www.teleguide.info/download/new3/inter-tv.zip');
insert dbo.WebResources(TPID, FileName, ResourceName, ResourceUrl)
values (3, 'TPXmlTvNew.xml', '������ tele.perm.ru (XML TV)', 'http://tele.perm.ru/download/tvguide.xml.gz');
go

insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'untype.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\untype.png'    , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'hudFilm.gif'     , 'image/gif', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\hudFilm.gif'       , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'teleserial.png'  , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\teleserial.png'    , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'balloons.gif'    , 'image/gif', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\balloons.gif'  , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'sport.png' 	   , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\sport.png' 	  , SINGLE_BLOB)      as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'docs.png'        , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\docs.png'          , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'info.png'        , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\info.png'          , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'humor.png'       , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\humor.png'         , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'teleshow.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\teleshow.png'      , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'telegames.png'   , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\telegames.png' , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'theater.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\theater.png'       , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'multfilm.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\multfilm.png'      , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'criminal.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\criminal.png'  , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'music.png'       , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\music.png'         , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'web.png'         , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\web.png'       , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'animals.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\animals.png'       , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'weather.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\weather.png'       , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'art.png'         , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\art.png'       , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'politic.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\politic.png'   , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'science.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\science.png'     , SINGLE_BLOB)   as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'travel.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\travel.png'      , SINGLE_BLOB)   as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'people.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\people.png'        , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'tech.png'        , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\tech.png'          , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'fashion.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\fashion.png'   , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'auto.png'	       , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\auto.png'	      , SINGLE_BLOB)  as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'medicine.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\medicine.png'  , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'religion.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\religion.png'      , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'gardening.png'   , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\gardening.png'     , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'advertize.png'   , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\advertize.png'     , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'female.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\female.png'        , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'cooking.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\cooking.png'   , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'extrime.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\extrime.png'       , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'finance.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\finance.png'   , SINGLE_BLOB)     as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'erotic.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Genres\erotic.png'        , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'GreenAnons.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Media\GreenAnons.png'        , SINGLE_BLOB) as Import(x);
insert dbo.MediaPic (FileName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'satellite_25.png'      , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Media\satellite_25.png'        , SINGLE_BLOB) as Import(x);
go                                                                                                                               
                                                                                                                                   
insert dbo.MediaPic (FIleName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'favempty.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Favorites\favempty.png', SINGLE_BLOB)    as ImportFav(x);
insert dbo.MediaPic (FIleName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'favblue.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Favorites\favblue.png', SINGLE_BLOB)     as ImportFav(x);
insert dbo.MediaPic (FIleName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'favpurple.png'   , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Favorites\favpurple.png', SINGLE_BLOB)   as ImportFav(x);
insert dbo.MediaPic (FIleName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'favgreen.png'    , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Favorites\favgreen.png', SINGLE_BLOB)    as ImportFav(x);
insert dbo.MediaPic (FIleName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'favyellow.png'   , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Favorites\favyellow.png', SINGLE_BLOB)   as ImportFav(x);
insert dbo.MediaPic (FIleName, ContentType, ContentCoding, ContentOrig, Length, IsSystem) select 'favpink.png'     , 'image/png', 'gzip', x, LEN(x), 1 from OPENROWSET(BULK N'H:\Projects\TVProgViewer\src\TVProgViewer\Dialogs\Media\Favorites\favpink.png', SINGLE_BLOB)     as ImportFav(x);
go
		   
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '�����', '�����;���;�/�', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '�.�', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '�. �', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '�. �', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '�/�', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '�\\�', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (2, NULL, '��������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (3, NULL, '�/�', '�/�', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (3, NULL, '�����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (3, NULL, '����', '�/;�����;������', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '��������� ����, ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '�� 16', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (4, NULL, '���������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '����-���','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '����������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '��� ��� ������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '��������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '���','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '��������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '���','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '����','�����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '��������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '���������','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '������ �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (5, NULL, '�����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '���.', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '�. �', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '�. �.', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '�/�', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (6, NULL, '�/�', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '��������', '�����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������', '� ��������', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�����', '���;������', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�����', '������', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�������� ���������', '', NULL)
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, 'News', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������ ����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '���� �� ����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '24', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������ �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '������ � ���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (7, NULL, '����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '���� ������� � ����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '�������;�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '�. �. �.', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (8, NULL, '�����', '', NULL);
                                                                          
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '��� ���� ��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '������ ��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '����� ���������!', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '����� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '��� 2', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '��� ����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (9, NULL, '��������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '����', '�������;�����;�������', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '���� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '����', '�����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '������ � ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '������ ���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '��� ����� ����� �����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '���? ���? �����?', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (10, NULL, '���� ����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '�����', '������;������', NULL)
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, 'Stop! �����!', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '������ ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '��.', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (11, NULL, '���������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '�/', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '�. �', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '�/ ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (12, NULL, 'Fox Kids', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '������������ ������������',  '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�������������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (13, NULL, '�������������� ���������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '���', '����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '����', '�����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '��� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '������� 10-��', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '��������� 10-��', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������!', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, 'Extra', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, 'Impulse', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '�������� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (14, NULL, '10 �����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (15, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (15, NULL, '.Ru', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (15, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (15, NULL, '��������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (15, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (15, NULL, 'www', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '� � ��� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '������� � �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '���� � �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (16, NULL, '������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (17, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (17, NULL, '��������� �������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '��������', '����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '�����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '��������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '������� ����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (18, NULL, '�������', '', NULL);
 
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (19, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (19, NULL, '����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (19, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (19, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (19, NULL, '�������������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (19, NULL, '�������', '��������', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (20, NULL, 'X������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (20, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (20, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (20, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (20, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (20, NULL, '�����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '�����','', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '��������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '���� �� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '���������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '������ �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '���� ��� ��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '����� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '���� � ���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '���� ��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '�� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (21, NULL, '��������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '������������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '���� ��� ����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '������� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '������� �', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '����� ������������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '������� � �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '��� ���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (22, NULL, '������ ������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (23, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (23, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (23, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (23, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (23, NULL, 'PRO-�������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, 'fashion', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '��� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '�������� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (24, NULL, '����������', '', NULL);
 
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (25, NULL, '����;����', '�����;����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (25, NULL, '��������� ���', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (25, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (25, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (25, NULL, 'Top Gear', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (26, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (26, NULL, '��� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (26, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (26, NULL, '���.', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (26, NULL, '�����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '������������ ����� ���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '����', '�����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '�� ����������� ����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '������ �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '���������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (27, NULL, '��� ���� ����', '', NULL);
INSERT INTO GenreClassificator (GID, UID, ContainPhrases, NonContainPhrases, OrderCol)  VALUES (27, NULL, 'C�����', '', 312)


insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (28, NULL, '���', '������' , NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (28, NULL, '������������ �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (28, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (28, NULL, '����', '��������', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (29, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (29, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (29, NULL, '�����������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (30, NULL, '�� ����� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (30, NULL, '������� ����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (30, NULL, '������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (30, NULL, '��� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (30, NULL, '����;����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '�����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '���������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '�������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '������� �������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (31, NULL, '����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (32, NULL, '�����', '�����', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (32, NULL, '������ ��������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (32, NULL, '���������� ������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (32, NULL, '�������� �����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (32, NULL, '�������', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (33, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (33, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (33, NULL, '����������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (33, NULL, '������', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (33, NULL, '�����', '', NULL);

insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (34, NULL, '����', '', NULL);
insert dbo.GenreClassificator(gid, uid, ContainPhrases, NonContainPhrases, DeleteAfterDate) values (34, NULL, '����', '', NULL);
go
    
