CREATE DATABASE RaceDay_Part_2_Restful_API; 
USE RaceDay_Part_2_Restful_API; 

CREATE TABLE dbo.[Users] (
user_id INT IDENTITY(1,1) PRIMARY KEY, 
first_name NVARCHAR(50) NOT NULL, 
last_name NVARCHAR(50) NOT NULL, 
email NVARCHAR (100) NOT NULL UNIQUE, 
user_password NVARCHAR(255) NOT NULL, 
[user_roler] NVARCHAR(20) NOT NULL CHECK ([user_role] IN ('organiser', 'participant')), 
created_at DATETIME DEFAULT GETDATE(), 
updated_at DATETIME DEFAULT GETDATE()

); 

CREATE TABLE dbo.[Events] (
event_id INT IDENTITY (1,1) PRIMARY KEY, 
user_id INT NOT NULL, 
event_title NVARCHAR (100) NOT NULL, 
event_description NVARCHAR(MAX) NULL, 
event_date DATE NOT NULL, 
event_location NVARCHAR(200) NOT NULL, 
created_at DATETIME DEFAULT GETDATE(), 
updated_at DATETIME DEFAULT GETDATE(), 
CONSTRAINT FK_Event_Organiser FOREIGN KEY(user_id) REFERENCES dbo.[Users] (user_id) ON DELETE CASCADE 
); 

CREATE TABLE dbo.[Routes] (
route_id INT IDENTITY (1,1) PRIMARY KEY, 
event_id INT NOT NULL UNIQUE, 
route_description NVARCHAR(MAX) NULL, 
distance_km DECIMAL (8, 2) NOT NULL, 
elevation_m INT NULL, 
CONSTRAINT FK_Route_Event FOREIGN KEY (event_id) REFERENCES dbo.Events(event_id) ON DELETE CASCADE 
);

CREATE TABLE dbo.[Weather] (
weather_id INT IDENTITY(1,1) PRIMARY KEY, 
event_id INT NOT NULL, 
forecast_date DATE NOT NULL, 
temperature DECIMAL (5,2) NULL, 
rain_chance DECIMAL (5,2) NULL, 
wind_speed DECIMAL (5,2) NULL, 
updated_at DATETIME DEFAULT GETDATE(), 
CONSTRAINT FK_Weather_Event FOREIGN KEY (event_id) REFERENCES dbo.[Events] (event_id) ON DELETE CASCADE 
);

CREATE TABLE dbo.[Categories] (
category_id INT IDENTITY (1,1) PRIMARY KEY, 
category_name NVARCHAR (50) NOT NULL, 
category_description NVARCHAR(255) NULL, 
category_type NVARCHAR(20) NOT NULL CHECK (category_type IN ('run', 'walk', 'cycle')), 
);

CREATE TABLE dbo.[EventCategories] (
event_category_id INT IDENTITY (1,1) PRIMARY KEY,
event_id INT NOT NULL, 
category_id INT NOT NULL, 
start_time TIME NULL, 
entry_fee DECIMAL (10, 2),
created_at DATETIME DEFAULT GETDATE(), 
CONSTRAINT Fk_EventCategories_Event FOREIGN KEY (event_id) REFERENCES dbo.Events(event_id) ON DELETE CASCADE,
CONSTRAINT FK_EventCategories_Categories FOREIGN KEY (category_id) REFERENCES dbo.Catgories(category_id) ON DELETE CASCADE, 
CONSTRAINT UQ_EventCategories_EventCategories UNIQUE (event_id, category_id)
);

CREATE TABLE dbo.[Registration] (
registration_id INT IDENTITY (1,1) PRIMARY KEY, 
user_id INT NOT NULL, 
event_category_id INT NOT NULL, 
registration_date DATETIME DEFAULT GETDATE(), 
[registration_status] NVARCHAR(20) DEFAULT 'pending' CHECK ([registration_status] IN ('pending', 'confirmed', 'cancelled', 'withdrawn')), 
created_at DATETIME DEFAULT GETDATE(), 
updated_at DATETIME DEFAULT GETDATE(), 
CONSTRAINT FK_Registration_Users FOREIGN KEY (user_id) REFERENCES dbo.[Users] ON DELETE CASCADE, 
CONSTRAINT FK_Registration_EventCategories FOREIGN KEY (event_vategory_id) REFERENCES dbo.EventCategories(event_category_id) ON DELETE CASCADE
);

CREATE TABLE dbo.[Results] (
result_id INT IDENTITY(1,1) PRIMARY KEY, 
registration_id INT NOT NULL UNIQUE, 
finish_time TIME NULL, 
overall_rank INT NULL,
category_rank INT NULL, 
[result_status] NVARCHAR (10) DEFAULT 'OK' CHECK ([status] IN ('OK', 'DNF', 'DQ', 'DNS')), 
created_at DATETIME DEFAULT GETDATE(), 
updated_at DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Results_Registration FOREIGN KEY (registration_id) REFERENCES dbo.Registration(registration_id) ON DELETE CASCADE
);