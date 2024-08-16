

INSERT INTO Categories (CategoryName, Description, ParentCategoryId, Status)
VALUES 
('Programming', 'Courses related to programming languages and development.', NULL, 1),
('Data Science', 'Courses covering data analysis, statistics, and machine learning.', NULL, 1),
('Web Development', 'Courses on building websites and web applications.', 1, 1),
('Mobile Development', 'Courses on developing mobile applications for Android and iOS.', 1, 1),
('Database Management', 'Courses on database design, SQL, and database administration.', NULL, 1),
('Cloud Computing', 'Courses on cloud services, architecture, and deployment.', NULL, 1),
('Cybersecurity', 'Courses on network security, encryption, and ethical hacking.', NULL, 1),
('DevOps', 'Courses on development operations, CI/CD, and automation.', NULL, 1),
('Game Development', 'Courses on creating video games using various platforms and engines.', 1, 1),
('AI & Machine Learning', 'Courses on artificial intelligence and machine learning technologies.', 2, 1);



INSERT INTO Courses (Name, ShortSummary, Description, Thumbnail, Price, OldPrice, Status, Version, Point, Reason, AllowComments, AdminModified, InstructorId)
VALUES 
('Introduction to Python', 'Learn the basics of Python programming.', 'This course covers fundamental concepts of Python programming, including syntax, data types, and basic algorithms.', 'thumbnail1.jpg', 49.99, 59.99, 'Active', '1.0', 10, NULL, 1, 0, 3),
('Data Science with R', 'Master data science with R.', 'This course teaches data manipulation, visualization, and analysis using the R programming language.', 'thumbnail2.jpg', 79.99, 79.99, 'Active', '1.0', 20, NULL, 1, 0, 3),  -- Set OldPrice to same as Price
('Web Development with JavaScript', 'Build dynamic websites using JavaScript.', 'This course covers JavaScript fundamentals, DOM manipulation, and AJAX to create dynamic web applications.', 'thumbnail3.jpg', 99.99, 119.99, 'Active', '1.0', 15, NULL, 1, 0, 3),
('Mobile App Development with Flutter', 'Create beautiful mobile apps with Flutter.', 'Learn how to build cross-platform mobile applications using Flutter and Dart.', 'thumbnail4.jpg', 119.99, 149.99, 'Active', '1.0', 25, NULL, 1, 0, 3),
('Database Design with SQL', 'Design and manage databases using SQL.', 'This course covers relational database design, normalization, and SQL queries.', 'thumbnail5.jpg', 59.99, 69.99, 'Active', '1.0', 15, NULL, 1, 0, 3),
('Machine Learning with TensorFlow', 'Implement machine learning models using TensorFlow.', 'Learn how to build and train machine learning models using the TensorFlow framework.', 'thumbnail6.jpg', 149.99, 199.99, 'Active', '1.0', 30, NULL, 1, 0, 3),
('Advanced Java Programming', 'Deep dive into advanced Java concepts.', 'This course covers advanced Java topics such as concurrency, streams, and design patterns.', 'thumbnail7.jpg', 89.99, 109.99, 'Active', '1.0', 20, NULL, 1, 0, 3),
('Cybersecurity Fundamentals', 'Learn the basics of cybersecurity.', 'Understand the principles of cybersecurity, including threat analysis, cryptography, and network security.', 'thumbnail8.jpg', 69.99, 89.99, 'Active', '1.0', 15, NULL, 1, 0, 3),
('Cloud Computing with AWS', 'Master cloud services with AWS.', 'This course covers AWS services, cloud architecture, and best practices for deploying applications in the cloud.', 'thumbnail9.jpg', 129.99, 159.99, 'Active', '1.0', 25, NULL, 1, 0, 3),
('Introduction to DevOps', 'Learn the fundamentals of DevOps.', 'Understand DevOps principles and practices, including CI/CD, automation, and collaboration.', 'thumbnail10.jpg', 79.99, 99.99, 'Active', '1.0', 20, NULL, 1, 0, 3),
('Artificial Intelligence with Python', 'Build AI applications using Python.', 'This course covers AI concepts, including neural networks, natural language processing, and computer vision, using Python.', 'thumbnail11.jpg', 159.99, 199.99, 'Active', '1.0', 30, NULL, 1, 0, 3),
('Blockchain Technology', 'Understand the fundamentals of blockchain.', 'Learn about blockchain principles, cryptocurrencies, and how to develop blockchain applications.', 'thumbnail12.jpg', 99.99, 129.99, 'Active', '1.0', 25, NULL, 1, 0, 3),
('Big Data Analytics', 'Analyze large datasets using big data technologies.', 'This course covers big data tools and techniques, including Hadoop, Spark, and data visualization.', 'thumbnail13.jpg', 139.99, 179.99, 'Active', '1.0', 30, NULL, 1, 0, 3);



-- Insert Chapter entries
INSERT INTO Chapters (Content, Thumbnail, [Order], Duration, [Type], CourseId)
VALUES 
('Introduction to Python - Basics', 'python_intro.jpg', 1, '00:10:00', 0, 1),  -- Assuming CourseId 1 is for 'Introduction to Python'
('Python Data Structures', 'python_data_structures.jpg', 2, '00:20:00', 0, 1),
('Python Functions and Modules', 'python_functions.jpg', 3, '00:15:00', 0, 1),
('Data Science with R - Overview', 'r_intro.jpg', 1, '00:08:00', 0, 2),  -- Assuming CourseId 2 is for 'Data Science with R'
('Data Manipulation in R', 'r_data_manipulation.jpg', 2, '00:25:00', 0, 2),
('Data Visualization in R', 'r_data_visualization.jpg', 3, '00:18:00', 0, 2),
('Web Development with JavaScript - Introduction', 'js_intro.jpg', 1, '00:12:00', 1, 3),  -- Assuming CourseId 3 is for 'Web Development with JavaScript'
('JavaScript Basics', 'js_basics.jpg', 2, '00:22:00', 1, 3),
('Advanced JavaScript', 'js_advanced.jpg', 3, '00:30:00', 1, 3),
('Flutter App Development - Getting Started', 'flutter_intro.jpg', 1, '00:09:00', 1, 4),  -- Assuming CourseId 4 is for 'Mobile App Development with Flutter'
('Building Layouts in Flutter', 'flutter_layouts.jpg', 2, '00:35:00', 1, 4),
('State Management in Flutter', 'flutter_state_management.jpg', 3, '00:27:00', 1, 4),
('Database Design with SQL - Basics', 'sql_intro.jpg', 1, '00:11:00', 0, 5),  -- Assuming CourseId 5 is for 'Database Design with SQL'
('Advanced SQL Queries', 'sql_advanced.jpg', 2, '00:32:00', 0, 5),
('Database Normalization', 'sql_normalization.jpg', 3, '00:19:00', 0, 5),
('Machine Learning with TensorFlow - Introduction', 'tensorflow_intro.jpg', 1, '00:13:00', 1, 6),  -- Assuming CourseId 6 is for 'Machine Learning with TensorFlow'
('Building Neural Networks with TensorFlow', 'tensorflow_neural_networks.jpg', 2, '00:45:00', 1, 6),
('Training and Evaluating Models in TensorFlow', 'tensorflow_training.jpg', 3, '00:29:00', 1, 6);


INSERT INTO StudentInCourses (CourseId, UserId, InstructorId, Rating, Progress, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy, DeletedDate, DeletedBy, IsDelete)
VALUES 
-- Student 1 in various courses
(2, 2, 3, 5, 0.75, GETDATE(), 'System', GETDATE(), 'System', NULL, NULL, 0),  -- UserId 2, CourseId 2, 75% Progress
(3, 2, 3, 4, 0.60, GETDATE(), 'System', GETDATE(), 'System', NULL, NULL, 0),  -- UserId 2, CourseId 3, 60% Progress
(4, 2, 3, 5, 0.90, GETDATE(), 'System', GETDATE(), 'System', NULL, NULL, 0),  -- UserId 2, CourseId 4, 90% Progress

-- Student 2 in various courses (now UserId 3)
(2, 3, 3, 5, 0.50, GETDATE(), 'System', GETDATE(), 'System', NULL, NULL, 0),  -- UserId 3, CourseId 2, 50% Progress
(5, 3, 3, 3, 0.40, GETDATE(), 'System', GETDATE(), 'System', NULL, NULL, 0),  -- UserId 3, CourseId 5, 40% Progress
(6, 3, 3, 4, 0.80, GETDATE(), 'System', GETDATE(), 'System', NULL, NULL, 0);  -- UserId 3, CourseId 6, 80% Progress



