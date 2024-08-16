# 🌟 Curus Project



## 🚀 Overview
Curus is a state-of-the-art platform designed for seamless course management and sales. Built with the power of **.NET 7.0**, Curus employs a robust **N-layer architecture** that ensures clarity, scalability, and maintainability.

## 📂 Project Structure
Here's a glance at the project's structure:
├── Curus.API/ # 🌐 The API layer providing RESTful endpoints
├── Curus.Repository/ # 🗄️ Data access layer managing database operations
├── Curus.Service/ # ⚙️ Business logic layer with core functionalities
├── DBScript/ # 💾 Database scripts for setup and migration
├── .gitignore # 📜 Specifies files and directories to be ignored by Git
├── Curus.sln # 🔧 Solution file for the project


## ✨ Special Features

### 1. 🏗️ N-layer Architecture
- **Curus.API**: Exposes RESTful APIs, facilitating interaction with the system.
- **Curus.Repository**: Handles all data access and database interactions.
- **Curus.Service**: Implements business logic, enforcing service rules and operations.

### 2. 🎓 InstructorData Subclass
- **InstructorData**: A specialized subclass of `User`, enriched with additional fields like `TaxNumber`, `CardNumber`, `Certification`, and more, tailored specifically for instructors.

### 3. 💳 VNPAY Integration
- Seamlessly integrated with **VNPAY** for secure payment processing.
- Supports both **payment tokenization** and **gateway transactions**, ensuring a robust payment solution.

### 4. 🛠️ Database Management
- The **DBScript** directory contains scripts for database setup and migration, ensuring the system is always up to date with the latest schema.

## 🛠️ Getting Started
Follow these steps to get the Curus project up and running:

1. **Clone the repository**:

   git clone https://github.com/phuctran362003/course-nexus.git

Open the solution:
Open Curus.sln in Visual Studio.
Restore NuGet packages:
Right-click on the solution in Solution Explorer and select Restore NuGet Packages.
Build and Run:
Build the solution and run the Curus.API project to start the web API.
🤝 Contributing
We welcome contributions! Please read our Contributing Guidelines to get started. Every bit of help is appreciated!

📜 License
This project is licensed under the MIT License. See the LICENSE file for more details.

📬 Contact
For any inquiries, please reach out to the project maintainers at:

Email: phuctgse172360@fpt.edu.vn
Developed with ❤️ by the Curus Team


