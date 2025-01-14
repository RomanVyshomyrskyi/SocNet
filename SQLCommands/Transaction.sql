BEGIN TRANSACTION;

-- Lock the table in a way that prevents modifications
SELECT * FROM dbo.Users WITH (TABLOCK, HOLDLOCK);

-- Perform your read-only operations here
-- For example, you can select data from the table
SELECT * FROM dbo.Users;

-- Attempt to insert a new record into the Users table
BEGIN TRY
    INSERT INTO dbo.Users (UserName, PasswordHash, Email, DateOfCreation, LastLogin, ImgBinary)
    VALUES ('testuser', 'hashedpassword', 'testuser@example.com', GETDATE(), GETDATE(), 'binarydata');
    PRINT 'Insert succeeded';
END TRY
BEGIN CATCH
    PRINT 'Insert failed: ' + ERROR_MESSAGE();
END CATCH;

-- Commit the transaction to release the lock
COMMIT TRANSACTION;

BEGIN TRANSACTION;

-- Lock the table in a way that prevents modifications
SELECT * FROM dbo.Users WITH (TABLOCK, HOLDLOCK);

-- Perform your read-only operations here
-- For example, you can select data from the table
SELECT * FROM dbo.Users;

-- Attempt to insert a new record into the Users table
BEGIN TRY
    INSERT INTO dbo.Users (UserName, PasswordHash, Email, DateOfCreation, LastLogin, ImgBinary)
    VALUES ('testuser', 'hashedpassword', 'testuser@example.com', GETDATE(), GETDATE(), 'binarydata');
    PRINT 'Insert succeeded';
END TRY
BEGIN CATCH
    PRINT 'Insert failed: ' + ERROR_MESSAGE();
END CATCH;

-- Roll back the transaction to release the lock
ROLLBACK TRANSACTION;

