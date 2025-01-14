
-- Create a new user account
CREATE LOGIN NewUser WITH PASSWORD = 'Password123';
CREATE USER NewUser FOR LOGIN NewUser;

-- Verify the user account creation
SELECT * FROM sys.database_principals WHERE name = 'NewUser';


-- Create a new role
CREATE ROLE NewRole;

-- Verify the role creation
SELECT * FROM sys.database_principals WHERE type = 'R' AND name = 'NewRole';


-- Grant SELECT permission on a specific table to the new role
GRANT SELECT ON dbo.Users TO NewRole;

-- Add the new user to the new role
ALTER ROLE NewRole ADD MEMBER NewUser;

-- Verify the permissions
SELECT dp.name AS PrincipalName, dp.type_desc AS PrincipalType, 
       o.name AS ObjectName, p.permission_name, p.state_desc AS PermissionState
FROM sys.database_permissions p
JOIN sys.objects o ON p.major_id = o.object_id
JOIN sys.database_principals dp ON p.grantee_principal_id = dp.principal_id
WHERE dp.name = 'NewRole';


-- Revoke SELECT permission on a specific table from the new role
REVOKE SELECT ON dbo.Users FROM NewRole;

-- Remove the new user from the new role
ALTER ROLE NewRole DROP MEMBER NewUser;

-- Verify the permissions
SELECT dp.name AS PrincipalName, dp.type_desc AS PrincipalType, 
       o.name AS ObjectName, p.permission_name, p.state_desc AS PermissionState
FROM sys.database_permissions p
JOIN sys.objects o ON p.major_id = o.object_id
JOIN sys.database_principals dp ON p.grantee_principal_id = dp.principal_id
WHERE dp.name = 'NewRole';

-- Drop the role
DROP ROLE NewRole;

-- Verify the role deletion
SELECT * FROM sys.database_principals WHERE type = 'R' AND name = 'NewRole';


-- Drop the user account
DROP USER NewUser;
DROP LOGIN NewUser;

-- Verify the user account deletion
SELECT * FROM sys.database_principals WHERE name = 'NewUser';