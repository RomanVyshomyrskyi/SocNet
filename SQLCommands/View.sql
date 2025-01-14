-- dbo.UserPostComments source

CREATE VIEW UserPostComments AS
SELECT 
    u.UserName,
    u.Email,
    p.Text AS PostText,
    p.DateOfCreation AS PostDate,
    c.Text AS CommentText,
    c.DateOfCreation AS CommentDate
FROM 
    Users u
    INNER JOIN BasePosts p ON u.Id = p.CreatorID
    LEFT JOIN Comments c ON p.ID = c.PostID;