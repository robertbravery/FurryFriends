INSERT INTO [dbo].[PetWalkers] (
    [Id], [FirstName], [LastName], [Gender], [Biography], [DateOfBirth], [HourlyRate], 
    [Currency], [IsActive], [IsVerified], [YearsOfExperience], [HasInsurance], 
    [HasFirstAidCertificatation], [DailyPetWalkLimit], [CreatedAt], [UpdatedAt], 
    [Email], [PhoneCountryCode], [PhoneNumber], [Street], [City], [StateProvinceRegion], 
    [ZipCode], [Country]
) VALUES 
(
    NEWID(), N'John', N'Doe', 1, N'Passionate pet lover with years of experience.', 
    '1985-08-10T00:00:00', 15.00, N'USD', 1, 1, 10, 1, 1, 5, 
    GETDATE(), GETDATE(), N'john.doe@example.com', N'+27', N'1234567890', N'123 Pet Lane', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Jane', N'Smith', 2, N'Dedicated to ensuring pets have a great time.', 
    '1990-02-20T00:00:00', 18.50, N'USD', 1, 1, 8, 1, 1, 6, 
    GETDATE(), GETDATE(), N'jane.smith@example.com', N'+27', N'0987654321', N'456 Furry Road', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Michael', N'Johnson', 1, N'Experienced in handling all types of pets.', 
    '1978-11-15T00:00:00', 20.00, N'USD', 1, 1, 12, 1, 1, 7, 
    GETDATE(), GETDATE(), N'michael.johnson@example.com', N'+27', N'1122334455', N'789 Animal Avenue', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Emily', N'Davis', 2, N'Lifelong pet enthusiast with a passion for walking.', 
    '1995-05-25T00:00:00', 22.75, N'USD', 1, 1, 5, 1, 1, 8, 
    GETDATE(), GETDATE(), N'emily.davis@example.com', N'+27', N'5544332211', N'321 Paws Path', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Daniel', N'Wilson', 1, N'Reliable and caring pet walker.', 
    '1983-12-05T00:00:00', 19.00, N'USD', 1, 1, 11, 1, 1, 7, 
    GETDATE(), GETDATE(), N'daniel.wilson@example.com', N'+27', N'6677889900', N'654 Canine Crescent', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Alice', N'Brown', 2, N'Experienced with a variety of pets.', 
    '1988-03-30T00:00:00', 21.50, N'USD', 1, 1, 9, 1, 1, 6, 
    GETDATE(), GETDATE(), N'alice.brown@example.com', N'+27', N'3344556677', N'987 Feline Street', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Joshua', N'Miller', 1, N'Dedicated to providing the best care.', 
    '1992-07-22T00:00:00', 17.00, N'USD', 1, 1, 7, 1, 1, 5, 
    GETDATE(), GETDATE(), N'joshua.miller@example.com', N'+27', N'2233445566', N'321 Pet Lane', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Sarah', N'Taylor', 2, N'Passionate about pets and their well-being.', 
    '1980-09-17T00:00:00', 16.50, N'USD', 1, 1, 15, 1, 1, 8, 
    GETDATE(), GETDATE(), N'sarah.taylor@example.com', N'+27', N'5566778899', N'654 Animal Avenue', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Andrew', N'Moore', 1, N'Experienced pet walker and trainer.', 
    '1977-01-10T00:00:00', 24.00, N'USD', 1, 1, 20, 1, 1, 10, 
    GETDATE(), GETDATE(), N'andrew.moore@example.com', N'+27', N'4455667788', N'123 Furry Road', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Linda', N'Martin', 2, N'Lifelong animal lover with years of experience.', 
    '1996-06-10T00:00:00', 18.75, N'USD', 1, 1, 4, 1, 1, 6, 
    GETDATE(), GETDATE(), N'linda.martin@example.com', N'+27', N'6677889901', N'789 Paws Path', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Chris', N'Jackson', 1, N'Providing top-notch care for pets.', 
    '1984-10-05T00:00:00', 23.00, N'USD', 1, 1, 14, 1, 1, 8, 
    GETDATE(), GETDATE(), N'chris.jackson@example.com', N'+27', N'1122334466', N'456 Canine Crescent', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Karen', N'Lewis', 2, N'Devoted pet walker with a love for animals.', 
    '1991-04-22T00:00:00', 20.50, N'USD', 1, 1, 6, 1, 1, 7, 
    GETDATE(), GETDATE(), N'karen.lewis@example.com', N'+27', N'3344556678', N'987 Furry Road', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Paul', N'White', 1, N'Committed to providing excellent pet care.', 
    '1979-12-30T00:00:00', 19.75, N'USD', 1, 1, 10, 1, 1, 5, 
    GETDATE(), GETDATE(), N'paul.white@example.com', N'+27', N'2233445577', N'321 Pet Lane', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Susan', N'Harris', 2, N'Experienced and caring pet walker.', 
    '1987-08-05T00:00:00', 22.00, N'USD', 1, 1, 9, 1, 1, 6, 
    GETDATE(), GETDATE(), N'susan.harris@example.com', N'+27', N'5566778890', N'654 Animal Avenue', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),

(
    NEWID(), N'Laura', N'Walker', 2, N'Experienced pet walker with a love for all animals.', 
    '1986-03-15T00:00:00', 19.25, N'USD', 1, 1, 13, 1, 1, 9, 
    GETDATE(), GETDATE(), N'laura.walker@example.com', N'+27', N'8899776655', N'963 Pet Park', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Kevin', N'Green', 1, N'Passionate about pets, providing excellent care.', 
    '1981-07-30T00:00:00', 21.75, N'USD', 1, 1, 16, 1, 1, 7, 
    GETDATE(), GETDATE(), N'kevin.green@example.com', N'+27', N'9988776655', N'951 Animal Street', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Rebecca', N'Clarkson', 2, N'Lifelong animal lover and experienced walker.', 
    '1993-11-12T00:00:00', 20.00, N'USD', 1, 1, 7, 1, 1, 8, 
    GETDATE(), GETDATE(), N'rebecca.clarkson@example.com', N'+27', N'6677889955', N'741 Fur Lane', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'James', N'Robinson', 1, N'Committed to ensuring pets have the best time.', 
    '1998-02-18T00:00:00', 18.00, N'USD', 1, 1, 5, 1, 1, 6, 
    GETDATE(), GETDATE(), N'james.robinson@example.com', N'+27', N'2233554477', N'135 Paw Path', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'Samantha', N'Thomas', 2, N'Dedicated pet walker with a heart for animals.', 
    '1994-09-25T00:00:00', 23.50, N'USD', 1, 1, 6, 1, 1, 7, 
    GETDATE(), GETDATE(), N'samantha.thomas@example.com', N'+27', N'3344556688', N'753 Tail Avenue', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
),
(
    NEWID(), N'David', N'King', 1, N'Experienced and reliable pet care provider.', 
    '1989-05-05T00:00:00', 22.00, N'USD', 1, 1, 9, 1, 1, 8, 
    GETDATE(), GETDATE(), N'david.king@example.com', N'+27', N'4455667799', N'147 Pet Walk', 
    N'Brakpan', N'Gauteng', N'1541', N'South Africa'
);




delete petwalkers