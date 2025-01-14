-- Create a function to calculate the average number of likes for posts created by a specific user
CREATE FUNCTION dbo.fn_AverageLikesByUser (@UserId INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @AverageLikes FLOAT;

    SELECT @AverageLikes = AVG(Likes)
    FROM BasePosts
    WHERE CreatorID = @UserId;

    RETURN @AverageLikes;
END;