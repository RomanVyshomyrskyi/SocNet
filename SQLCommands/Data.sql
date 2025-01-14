-- Insert data into Users table
INSERT INTO Users (UserName, PasswordHash, Email, DateOfCreation, LastLogin, ImgBinary)
VALUES 
('john_doe', 'password123', 'john@gmail.com', '2023-01-01', '2023-01-10', 'img1'),
('jane_smith', 'securePass!@#', 'jane@gmail.com', '2023-01-02', '2023-01-11', 'img2'),
('alice_jones', 'alice2023$', 'alice@example.com', '2023-01-03', '2023-01-12', 'img3'),
('bob_brown', 'bobSecure#1', 'bob@example.com', '2023-01-04', '2023-01-13', 'img4'),
('charlie_davis', 'charlie!@#$', 'charlie@gmail.com', '2023-01-05', '2023-01-14', 'img5'),
('david_evans', 'david2023!', 'david@example.com', '2023-01-06', '2023-01-15', 'img6'),
('eve_frank', 'eveSecure123', 'eve@gmail.com', '2023-01-07', '2023-01-16', 'img7'),
('frank_green', 'frank!@#$', 'frank@example.com', '2023-01-08', '2023-01-17', 'img8'),
('grace_hill', 'grace2023$', 'grace@gmail.com', '2023-01-09', '2023-01-18', 'img9'),
('hank_ivan', 'hankSecure#1', 'hank@example.com', '2023-01-10', '2023-01-19', 'img10'),
('ian_jack', 'ian!@#$', 'ian@gmail.com', '2023-01-11', '2023-01-20', 'img11'),
('jill_king', 'jill2023!', 'jill@example.com', '2023-01-12', '2023-01-21', 'img12'),
('kyle_law', 'kyleSecure123', 'kyle@gmail.com', '2023-01-13', '2023-01-22', 'img13'),
('laura_moon', 'laura!@#$', 'laura@example.com', '2023-01-14', '2023-01-23', 'img14'),
('mike_nash', 'mike2023$', 'mike@gmail.com', '2023-01-15', '2023-01-24', 'img15'),
('nina_owen', 'ninaSecure#1', 'nina@example.com', '2023-01-16', '2023-01-25', 'img16'),
('oscar_paul', 'oscar!@#$', 'oscar@gmail.com', '2023-01-17', '2023-01-26', 'img17'),
('paula_quinn', 'paula2023!', 'paula@example.com', '2023-01-18', '2023-01-27', 'img18'),
('quincy_ryan', 'quincySecure123', 'quincy@gmail.com', '2023-01-19', '2023-01-28', 'img19'),
('rachel_smith', 'rachel!@#$', 'rachel@example.com', '2023-01-20', '2023-01-29', 'img20');

-- Insert data into Roles table
INSERT INTO Roles (Role)
VALUES 
('Admin'),
('User'),
('Moderator'),
('Guest'),
('Editor'),
('Contributor'),
('Subscriber'),
('Manager'),
('Analyst'),
('Developer'),
('Designer'),
('Tester'),
('Support'),
('Sales'),
('Marketing'),
('HR'),
('Finance'),
('IT'),
('Operations'),
('Legal');

-- Insert data into UserRoles table
INSERT INTO UserRoles (UserId, RoleId)
VALUES 
(1, 2),
(2, 2),
(3, 2),
(4, 2),
(5, 2),
(6, 2),
(7, 2),
(8, 2),
(9, 2),
(10, 2),
(11, 2),
(12, 2),
(13, 2),
(14, 2),
(15, 2),
(16, 2),
(17, 2),
(18, 2),
(19, 2),
(20, 2);

-- Insert data into UserFriends table
INSERT INTO UserFriends (UserId, FriendId)
VALUES 
(1, 2),
(1, 3),
(2, 4),
(2, 5),
(3, 6),
(3, 7),
(4, 8),
(4, 9),
(5, 10),
(5, 11),
(6, 12),
(6, 13),
(7, 14),
(7, 15),
(8, 16),
(8, 17),
(9, 18),
(9, 19),
(10, 20),
(10, 1);

-- Insert data into BasePosts table
INSERT INTO BasePosts (CreatorID, Text, DateOfCreation)
VALUES 
(1, 'Just finished reading a great book!', '2023-01-01'),
(2, 'Had an amazing time at the beach today.', '2023-01-02'),
(3, 'Cooked a delicious meal for dinner.', '2023-01-03'),
(4, 'Went hiking in the mountains.', '2023-01-04'),
(5, 'Started learning a new programming language.', '2023-01-05'),
(6, 'Watched a fantastic movie last night.', '2023-01-06'),
(7, 'Visited a beautiful art gallery.', '2023-01-07'),
(8, 'Had a productive day at work.', '2023-01-08'),
(9, 'Enjoyed a relaxing weekend.', '2023-01-09'),
(10, 'Attended a fun concert.', '2023-01-10'),
(11, 'Went for a long bike ride.', '2023-01-11'),
(12, 'Had a great workout session.', '2023-01-12'),
(13, 'Spent time with family and friends.', '2023-01-13'),
(14, 'Read an interesting article.', '2023-01-14'),
(15, 'Tried a new restaurant.', '2023-01-15'),
(16, 'Worked on a personal project.', '2023-01-16'),
(17, 'Had a relaxing day at the spa.', '2023-01-17'),
(18, 'Went shopping for new clothes.', '2023-01-18'),
(19, 'Enjoyed a sunny day at the park.', '2023-01-19'),
(20, 'Had a fun game night with friends.', '2023-01-20');

-- Insert data into PostImages table
INSERT INTO PostImages (PostID, Image)
VALUES 
(1, 0x123456),
(2, 0x234567),
(3, 0x345678),
(4, 0x456789),
(5, 0x567890),
(6, 0x678901),
(7, 0x789012),
(8, 0x890123),
(9, 0x901234),
(10, 0x012345),
(11, 0x123456),
(12, 0x234567),
(13, 0x345678),
(14, 0x456789),
(15, 0x567890),
(16, 0x678901),
(17, 0x789012),
(18, 0x890123),
(19, 0x901234),
(20, 0x012345);

-- Insert data into Comments table
INSERT INTO Comments (PostID, CreatorID, Text, DateOfCreation)
VALUES 
(1, 2, 'Nice post!', '2023-01-01'),
(2, 3, 'Interesting.', '2023-01-02'),
(3, 4, 'I agree.', '2023-01-03'),
(4, 5, 'Well said.', '2023-01-04'),
(5, 6, 'Good point.', '2023-01-05'),
(6, 7, 'Thanks for sharing.', '2023-01-06'),
(7, 8, 'Great post.', '2023-01-07'),
(8, 9, 'Very informative.', '2023-01-08'),
(9, 10, 'I learned a lot.', '2023-01-09'),
(10, 11, 'Thanks!', '2023-01-10'),
(11, 12, 'Nice work.', '2023-01-11'),
(12, 13, 'Keep it up.', '2023-01-12'),
(13, 14, 'Well done.', '2023-01-13'),
(14, 15, 'Good job.', '2023-01-14'),
(15, 16, 'Excellent.', '2023-01-15'),
(16, 17, 'Very good.', '2023-01-16'),
(17, 18, 'Awesome.', '2023-01-17'),
(18, 19, 'Fantastic.', '2023-01-18'),
(19, 20, 'Amazing.', '2023-01-19'),
(20, 1, 'Superb.', '2023-01-20');