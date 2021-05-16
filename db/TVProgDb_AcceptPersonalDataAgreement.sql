INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Account.Fields.AcceptPersonalDataAgreement.Required'), '��� ����������� ���������� ����������� � ��������� ��������� ������������ ������', 2); 
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Account.Fields.AcceptPersonalDataAgreement'), '� ��������(��������) �� ��������� ���� ������������ ������', 2);
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Admin.Users.Users.Fields.AcceptPersonalDataAgreement'), '�������� �� ��������� ������������ ������', 2); 
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Account.Field.AcceptPersonalDataAgreementDescription'),'� ������������ �� ������� 9 ��-152 �� 27 ���� 2006 ���� �� ������������ �������, ��� �������� �� ��������� � �������������� �/��� ����������� ����� ���� ������������ ������ ����� TVProgViewer.Ru', 2);
GO

INSERT INTO [dbo].[LocaleStringResource] (ResourceName, ResourceValue, LanguageId)
VALUES (LOWER('Admin.Configuration.Settings.UserUser.AcceptPersonalDataAgreementEnabled'), '''�������� �� ��������� ������������ ������'' ��������', 2);
GO

UPDATE LocaleStringResource SET ResourceName = replace(ResourceName, 'customer', 'user'); 
GO

INSERT INTO [dbo].[Setting] (Name, Value, StoreId)
VALUES (LOWER('UserSettings.AcceptPersonalDataAgreementEnabled'), 'True', 0);
GO

INSERT INTO [dbo].[Setting] (Name, Value, StoreId)
VALUES (LOWER('UserSettings.AcceptPersonalDataAgreementRequired'), 'True', 0);
GO

SELECT * FROM [dbo].[Setting]
WHERE Name IN ('UserSettings.AcceptPersonalDataAgreementEnabled', 'UserSettings.AcceptPersonalDataAgreementRequired')

SELECT * FROM [dbo].[LocaleStringResource]
WHERE ResourceName  IN ('Account.Fields.AcceptPersonalDataAgreement.Required'
, 'Account.Fields.AcceptPersonalDataAgreement'
, 'Admin.Users.Users.Fields.AcceptPersonalDataAgreement'
, 'Account.Field.AcceptPersonalDataAgreementDescription'
, 'Admin.Configuration.Settings.UserUser.AcceptPersonalDataAgreementEnabled')