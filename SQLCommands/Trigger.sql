CREATE TRIGGER trg_DeletePostsOnThreshold
ON BasePosts
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeletedPostCount INT;

    -- Calculate the number of deleted posts
    SELECT @DeletedPostCount = COUNT(*)
    FROM BasePosts
    WHERE IsDeleted = 1;

    -- Check if the count of deleted posts is 10 or more
    IF @DeletedPostCount >= 10
    BEGIN
        -- Delete related images first to maintain referential integrity
        DELETE FROM PostImages
        WHERE PostID IN (SELECT PostID FROM BasePosts WHERE IsDeleted = 1);

        -- Delete related comments
        DELETE FROM Comments
        WHERE PostID IN (SELECT PostID FROM BasePosts WHERE IsDeleted = 1);

        -- Delete all posts that are marked as deleted
        DELETE FROM BasePosts
        WHERE IsDeleted = 1;
    END
END;