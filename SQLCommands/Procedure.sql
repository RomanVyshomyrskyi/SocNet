CREATE PROCEDURE dbo.AssignUserBadges
AS
BEGIN
    DECLARE @UserId INT;
    DECLARE @UserName NVARCHAR(50);
    DECLARE @PostCount INT;
    DECLARE @Badge NVARCHAR(50);

    -- Create a temporary table to store the badges
    CREATE TABLE #TempUserBadges (
        UserId INT,
        UserName NVARCHAR(50),
        Badge NVARCHAR(50)
    );

    -- Start a TRY block for error handling
    BEGIN TRY
        -- Start a transaction
        BEGIN TRANSACTION;

        -- Declare a cursor to iterate through users
        DECLARE user_cursor CURSOR FOR
        SELECT u.Id, u.UserName, ISNULL(COUNT(p.ID), 0) AS PostCount
        FROM Users u
        LEFT JOIN BasePosts p ON u.Id = p.CreatorID
        GROUP BY u.Id, u.UserName;

        -- Open the cursor
        OPEN user_cursor;

        -- Fetch the first row
        FETCH NEXT FROM user_cursor INTO @UserId, @UserName, @PostCount;

        -- Loop through the cursor
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Assign badges based on post count
            IF @PostCount >= 100
            BEGIN
                SET @Badge = 'Gold';
            END
            ELSE IF @PostCount >= 50
            BEGIN
                SET @Badge = 'Silver';
            END
            ELSE IF @PostCount >= 10
            BEGIN
                SET @Badge = 'Bronze';
            END
            ELSE
            BEGIN
                SET @Badge = 'Newbie';
            END

            -- Insert the badge into the temporary table
            INSERT INTO #TempUserBadges (UserId, UserName, Badge)
            VALUES (@UserId, @UserName, @Badge);

            -- Fetch the next row
            FETCH NEXT FROM user_cursor INTO @UserId, @UserName, @PostCount;
        END

        -- Close and deallocate the cursor
        CLOSE user_cursor;
        DEALLOCATE user_cursor;

        -- Insert or update the badges in the UserBadges table
        MERGE UserBadges AS target
        USING #TempUserBadges AS source
        ON target.UserId = source.UserId
        WHEN MATCHED THEN
            UPDATE SET target.Badge = source.Badge
        WHEN NOT MATCHED THEN
            INSERT (UserId, UserName, Badge)
            VALUES (source.UserId, source.UserName, source.Badge);

        -- Commit the transaction if all operations succeed
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Handle any errors that occur
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        -- Rollback the transaction in case of an error
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END

        -- Print the error message
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;

    -- Select all badges from the temporary table
    SELECT * FROM #TempUserBadges;

    -- Drop the temporary table
    DROP TABLE #TempUserBadges;
END