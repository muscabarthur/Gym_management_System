CREATE DATABASE GymManagementDB;

USE GymManagementDB;
GO

CREATE TABLE Members (
    MemberID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Gender VARCHAR(10) NULL,
    Phone VARCHAR(20) NULL,
    Address VARCHAR(100) NULL
);
GO

CREATE TABLE Trainers (
    TrainerID INT PRIMARY KEY IDENTITY(1,1),
    TrainerName VARCHAR(100) NOT NULL,
    Specialty VARCHAR(50) NULL,
    Phone VARCHAR(20) NULL
);
GO

CREATE TABLE MembershipPlans (
    PlanID INT PRIMARY KEY IDENTITY(1,1),
    PlanName VARCHAR(50) NOT NULL,
    DurationMonths INT NULL,
    Price DECIMAL(10,2) NULL
);
GO

CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    MemberID INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentDate DATE NOT NULL,
    CONSTRAINT FK_Payments_Members FOREIGN KEY (MemberID) REFERENCES Members(MemberID)
);
GO

CREATE TABLE Equipment (
    EquipmentID INT PRIMARY KEY IDENTITY(1,1),
    EquipmentName VARCHAR(100) NOT NULL,
    Quantity INT NOT NULL,
    Status VARCHAR(20) NOT NULL
);
GO

CREATE TABLE WorkoutPrograms (
    ProgramID INT PRIMARY KEY IDENTITY(1,1),
    ProgramName VARCHAR(100) NOT NULL,
    TrainerID INT NOT NULL,
    CONSTRAINT FK_WorkoutPrograms_Trainers FOREIGN KEY (TrainerID) REFERENCES Trainers(TrainerID)
);
GO

CREATE TABLE MemberPrograms (
    MemberProgramID INT PRIMARY KEY IDENTITY(1,1),
    MemberID INT NOT NULL,
    ProgramID INT NOT NULL,
    StartDate DATE NOT NULL,
    CONSTRAINT FK_MemberPrograms_Members FOREIGN KEY (MemberID) REFERENCES Members(MemberID),
    CONSTRAINT FK_MemberPrograms_Programs FOREIGN KEY (ProgramID) REFERENCES WorkoutPrograms(ProgramID),
    CONSTRAINT UQ_MemberPrograms UNIQUE (MemberID, ProgramID)
);
GO

INSERT INTO Members (FullName, Gender, Phone, Address) VALUES
('Ahmed Mohamed Ali', 'Male', '+252610000101', 'Hodan, Mogadishu'),
('Amina Hassan Noor', 'Female', '+252610000102', 'Waberi, Mogadishu'),
('Abdi Omar Yusuf', 'Male', '+252610000103', 'Wadajir, Mogadishu'),
('Fadumo Ali Ahmed', 'Female', '+252610000104', 'Karaan, Mogadishu');
GO

INSERT INTO Trainers (TrainerName, Specialty, Phone) VALUES
('Hassan Ibrahim', 'Strength Training', '+252610000201'),
('Maryan Abdullahi', 'Cardio and Weight Loss', '+252610000202'),
('Yusuf Mohamed', 'Bodybuilding', '+252610000203');
GO

INSERT INTO MembershipPlans (PlanName, DurationMonths, Price) VALUES
('Monthly Plan', 1, 30.00),
('Quarterly Plan', 3, 80.00),
('Six Month Plan', 6, 150.00),
('Annual Plan', 12, 280.00);
GO

INSERT INTO Equipment (EquipmentName, Quantity, Status) VALUES
('Treadmill', 5, 'Available'),
('Exercise Bike', 4, 'Available'),
('Bench Press', 3, 'Available'),
('Dumbbell Set', 10, 'Available'),
('Cable Machine', 1, 'Maintenance');
GO

INSERT INTO WorkoutPrograms (ProgramName, TrainerID) VALUES
('Beginner Strength Program', 1),
('Fat Loss Cardio Program', 2),
('Advanced Bodybuilding Program', 3),
('General Fitness Program', 1);
GO

INSERT INTO Payments (MemberID, Amount, PaymentDate) VALUES
(1, 30.00, '2026-06-01'),
(2, 80.00, '2026-06-03'),
(3, 30.00, '2026-06-05'),
(4, 150.00, '2026-06-10');
GO

INSERT INTO MemberPrograms (MemberID, ProgramID, StartDate) VALUES
(1, 1, '2026-06-01'),
(2, 2, '2026-06-03'),
(3, 3, '2026-06-05'),
(4, 4, '2026-06-10');
GO

SELECT * FROM Members;
SELECT * FROM Trainers;
SELECT * FROM MembershipPlans;
SELECT * FROM Payments;
SELECT * FROM Equipment;
SELECT * FROM WorkoutPrograms;
SELECT * FROM MemberPrograms;
GO
