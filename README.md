# User Management Web Application

This project is a simple, business-oriented user management web application built with **ASP.NET Core Razor Pages** and **Entity Framework Core**.

The application is developed strictly according to the task requirements and focuses on correctness, consistency, and storage-level data integrity.

---

## 🚀 Technology Stack

- **.NET / ASP.NET Core (Razor Pages)**
- **Entity Framework Core**
- **SQL Server**
- **Bootstrap 5**
- **MailKit & MimeKit** (asynchronous email sending)

---

## 📌 Key Features

- User registration and authentication
- Cookie-based authentication
- Email confirmation (sent asynchronously)
- User management table with:
  - Multiple selection using checkboxes
  - Sorting by last login / activity time
  - Status indicators (Unverified / Active / Blocked)
- Toolbar actions:
  - Block users
  - Unblock users
  - Delete users
  - Delete unverified users
- No row-level action buttons (toolbar only)
- Responsive and business-oriented UI
- Works correctly on desktop and mobile resolutions

---

## 🔐 Authentication & Authorization

- Non-authenticated users can access **only**:
  - Login page
  - Registration page
- Authenticated users can access the user management page
- Before each request (except login/registration), the server:
  - Checks whether the user exists
  - Checks whether the user is not blocked
- Blocked or deleted users are redirected to the login page

Authentication is implemented using **cookie-based authentication**, which is fully aligned with the task requirements.

---

## 🗄 Database Design

- Relational database (SQL Server)
- Users are stored in a `Users` table
- **E-mail uniqueness is guaranteed at the database level**

### ⚠ Important Note (Task Requirement)

E-mail uniqueness is enforced **only by a UNIQUE INDEX in the database**,  
not by checking existence in the application code.

This guarantees correctness even under concurrent access.

---

## 🧱 User Entity Fields

- Id (Primary Key)
- Name
- Email (Unique Index)
- Status (Unverified / Active / Blocked)
- LastLoginTime
- RegistrationTime (optional)

Deleted users are physically removed from the database (not soft-deleted).

---

## ✉ Email Confirmation

- Confirmation e-mails are sent asynchronously
- Clicking the confirmation link:
  - Changes status from **Unverified** to **Active**
  - Blocked users remain blocked
- Any SMTP provider can be used (Gmail is supported for testing)

---

## 🎨 UI Requirements Compliance

- Bootstrap CSS framework is used
- No animations
- No wallpapers under the table
- No browser alert dialogs
- No buttons inside table rows
- Toolbar is always visible (enabled/disabled based on selection)
- Clean, consistent, and professional appearance

---

## 🛠 How to Run the Project

1. Configure the database connection string in `appsettings.json`
2. Apply migrations:
   ```bash
   dotnet ef database update
