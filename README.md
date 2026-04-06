# 🚀 StaffTrack - Employee Management System

StaffTrack is a production-level **Microservices-based Employee Management System** built using .NET. It helps organizations efficiently manage employees, authentication, leaves, timesheets, and notifications.

---

## 📌 Features

* 🔐 Secure Authentication & Authorization (JWT-based)
* 👨‍💼 Employee Management System
* 📝 Leave Management (Apply / Approve / Reject)
* ⏱️ Timesheet Tracking
* 🔔 Email Notification Service
* 🌐 API Gateway (Centralized Routing)
* 🧪 Unit Testing using NUnit
* 🧩 Scalable Microservices Architecture

---

## 🏗️ Architecture Overview

The system follows **Clean Architecture + Microservices Pattern**.

### 🔹 Services Included

* **AuthService**
  Handles user registration, login, JWT token generation, and role management.

* **EmployeeService**
  Manages employee records and profiles.

* **LeaveService**
  Handles leave requests, approvals, and tracking.

* **TimeSheetService**
  Tracks employee working hours and productivity.

* **NotificationService**
  Sends email notifications (login credentials, alerts, updates).

* **API Gateway**
  Acts as a single entry point for all services.

---

## 🛠️ Tech Stack

| Category       | Technology Used       |
| -------------- | --------------------- |
| Backend        | .NET 8 Web API        |
| Database       | SQL Server            |
| ORM            | Entity Framework Core |
| Authentication | JWT                   |
| Testing        | NUnit                 |
| Gateway        | Ocelot / YARP         |
| Email Service  | SMTP / Mailtrap       |

---

## 📂 Project Structure

```
StaffTrack/
│
├── AuthService/
├── EmployeeService/
├── LeaveService/
├── TimeSheetService/
├── NotificationService/
├── API.Gateway/
```

---

## ⚙️ Setup Instructions

### 1️⃣ Clone Repository

```bash
git clone https://github.com/khushi-0206/stafftrack.git
cd stafftrack
```

---

### 2️⃣ Configure Database

Update `appsettings.json` in each service:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=StaffTrackDB;Trusted_Connection=True;"
}
```

---

### 3️⃣ Configure Email (Mailtrap / SMTP)

```json
"Email": {
  "From": "your-email@gmail.com",
  "User": "your-email@gmail.com",
  "Pass": "your-password",
  "Host": "smtp.mailtrap.io",
  "Port": 2525
}
```

---

### 4️⃣ Run Database Migrations

```bash
dotnet ef database update
```

---

### 5️⃣ Run the Application

Run each service individually:

```bash
dotnet run
```

OR use **Visual Studio → Multiple Startup Projects**

---

## 🔐 Authentication Flow

1. User registers (Employee / Manager / HR)
2. System sends email with credentials
3. User logs in
4. JWT Token is generated
5. Token is used for secure API access

---

## 🧪 Running Tests

```bash
dotnet test
```

---

## 📬 Sample API Endpoints

### 🔹 Auth Service

* `POST /api/auth/register`
* `POST /api/auth/login`

### 🔹 Employee Service

* `GET /api/employees`
* `POST /api/employees`
* `PUT /api/employees/{id}`
* `DELETE /api/employees/{id}`

### 🔹 Leave Service

* `POST /api/leaves/apply`
* `PUT /api/leaves/approve`
* `PUT /api/leaves/reject`

### 🔹 TimeSheet Service

* `POST /api/timesheet`
* `GET /api/timesheet/{employeeId}`

---

## 🚀 Future Enhancements

* 🌐 Frontend (Angular / React)
* 🐳 Docker & Kubernetes Deployment
* 📊 Dashboard & Analytics
* 🔔 Real-time Notifications
* 📱 Mobile App Integration

---

## 👩‍💻 Author

**Khushi Mehta**

* 🔗 GitHub: https://github.com/khushi-0206

---

## ⭐ Support

If you like this project, give it a ⭐ on GitHub!

---
