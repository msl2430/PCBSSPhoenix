--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'CaseNumber', 10,10,28)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PersonNumber', 13,2,48)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'BatchNumber', 0,4,53)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'ActionCode', 23,1,60)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Office', 27,4,41)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'ProviderWarning', 0,1,71)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'CaseName', 34,22,75)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address', 39,20,64)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address2', 36,20,87)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address3', 41,22,0)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address4', 44,22,0)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'City', 47,18,110)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'State', 50,2,131)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Zip', 53,5,136)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'LastName', 60,12,148)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'FirstName', 62,7,163)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Middle', 64,1,175)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'DateOfBirth', 66,8,179)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'SocialSecurity', 69,9,190)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Sex', 71,1,202)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'MaritalStatus', 73,1,206)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Race', 75,1,210)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PriorCase', 77,10,214)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PriorPersonNumber', 79,2,227)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'AlienType', 88,1,232)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'TempDate', 90,8,271)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'EffectiveDate', 116,8,278) --64
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'TermDate', 117,8,289) 
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'AddCode', 119,2,300)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'TRMCode', 121,2,305)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PGM', 122,3,310)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'SUPV', 124,3,316)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'RES', 126,2,322)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'ExtType', 128,1,327)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PrenancyDueDate', 130,8,331)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'AddressAction', 31,1,60)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PersonAction', 55,1,144)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'EligSeg', 98,1,266)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Supervisor', 93,2,247)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Worker61', 96,2,252)

--truncate table select * from Phoenix.MedicaidFields

UPDATE Phoenix.MedicaidFields
SET StartIndex = StartIndex - 1
WHERE MedicaidFormId = 1

UPDATE Phoenix.MedicaidFields
SET FieldName = 'Worker'
WHERE MedicaidFormId = 1 and MedicaidFieldId = 40

UPDATE Phoenix.MedicaidFields
SET StartIndex = 235
WHERE MedicaidFormId = 1 and MedicaidFieldId = 26