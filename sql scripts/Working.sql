--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'CaseNumber', 10,10,27)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PersonNumber', 13,2,47)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'BatchNumber', 0,4,52)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'ActionCode', 23,1,59)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Office', 27,4,40)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'ProviderWarning', 0,1,70)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'CaseName', 34,22,74)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address', 39,20,63)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address2', 36,20,86)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address3', 41,22,0)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Address4', 44,22,0)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'City', 47,18,109)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'State', 50,2,130)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Zip', 53,5,135)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'LastName', 60,12,147)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'FirstName', 62,7,162)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Middle', 64,1,174)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'DateOfBirth', 66,8,178)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'SocialSecurity', 69,9,189)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Sex', 71,1,201)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'MaritalStatus', 73,1,205)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Race', 75,1,209)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PriorCase', 77,10,213)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PriorPersonNumber', 79,2,226)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'AlienType', 88,1,231)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'TempDate', 90,8,270)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'EffectiveDate', 116,8,277) --64
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'TermDate', 117,8,288) 
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'AddCode', 119,2,299)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'TRMCode', 121,2,304)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PGM', 122,3,309)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'SUPV', 124,3,315)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'RES', 126,2,321)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'ExtType', 128,1,326)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PrenancyDueDate', 130,8,330)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'AddressAction', 31,1,59)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'PersonAction', 55,1,143)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'EligSeg', 98,1,265)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Supervisor', 93,2,246)
--INSERT INTO Phoenix.MedicaidFields VALUES (1, 'Worker61', 96,2,251)

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