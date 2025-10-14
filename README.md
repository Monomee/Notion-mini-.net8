Dưới đây là phiên bản **README.md** được chỉnh sửa lại để **đẹp, rõ ràng, dễ đọc**, và **chuyên nghiệp hơn**, có phân chia mục, highlight code, và format chuẩn Markdown cho GitHub hoặc GitLab:

---

````markdown
# 🧠 Notion-mini (.NET 8)

> Một ứng dụng WPF mô phỏng lại **Notion** — giúp quản lý ghi chú, workspace, và tag với cơ sở dữ liệu SQL Server.  
> Dự án sử dụng **Entity Framework Core 8 (EF Core)** để kết nối dữ liệu.

---

## 🗄️ 1. Cấu hình Cơ sở dữ liệu (Database Setup)

### 🧩 Tên Database
**`NoteHubDB`**

### ⚙️ Script tạo Database
```sql
-- Tạo database
CREATE DATABASE NoteHubDB;
GO
USE NoteHubDB;
GO

-- Bảng User
CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Workspace
CREATE TABLE Workspace (
    WorkspaceId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
        ON DELETE CASCADE ON UPDATE CASCADE
);
GO

-- Bảng Page
CREATE TABLE Page (
    PageId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    IsPinned BIT DEFAULT 0,
    WorkspaceId INT NOT NULL,
    FOREIGN KEY (WorkspaceId) REFERENCES Workspace(WorkspaceId)
        ON DELETE CASCADE ON UPDATE CASCADE
);
GO

-- Bảng Tag
CREATE TABLE Tag (
    TagId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Bảng trung gian PageTag (many-to-many)
CREATE TABLE PageTag (
    PageId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (PageId, TagId),
    FOREIGN KEY (PageId) REFERENCES Page(PageId) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tag(TagId) ON DELETE CASCADE
);
GO
````

### 🧪 Dữ liệu mẫu

```sql
INSERT INTO [User] (Username, PasswordHash)
VALUES 
('hoang', '123456'),
('admin', 'admin123');

INSERT INTO Workspace (Name, UserId)
VALUES 
('Học tập', 1),
('Dự án', 1);

INSERT INTO Page (Title, Content, WorkspaceId)
VALUES 
('Ghi chú đầu tiên', N'Xin chào NoteHub!', 1),
('Kế hoạch tuần này', N'- Làm báo cáo\n- Học EF Core', 1);

INSERT INTO Tag (Name)
VALUES ('work'), ('study'), ('idea');

INSERT INTO PageTag (PageId, TagId)
VALUES (1, 3), (2, 2);
```

### 🔍 Kiểm tra mối quan hệ

```sql
SELECT p.Title, w.Name AS Workspace, u.Username
FROM Page p
JOIN Workspace w ON p.WorkspaceId = w.WorkspaceId
JOIN [User] u ON w.UserId = u.UserId;
```

**Kết quả:**

| Title             | Workspace | Username |
| ----------------- | --------- | -------- |
| Ghi chú đầu tiên  | Học tập   | hoang    |
| Kế hoạch tuần này | Học tập   | hoang    |

---

## ⚙️ 2. Cấu hình Dự án (Project Setup)

> ⚠️ Nếu đã cài rồi, có thể bỏ qua phần này.

### 🧩 Cài đặt `dotnet-ef`

```bash
dotnet tool uninstall --global dotnet-ef      # Gỡ bản cũ (nếu có)
dotnet tool install --global dotnet-ef --version 8.0.20
dotnet tool list --global                     # Kiểm tra lại
```

### 📦 Cài đặt các Package cần thiết

| Package                                 | Version |
| --------------------------------------- | ------- |
| Microsoft.EntityFrameworkCore.Design    | 8.0.20  |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.20  |
| Microsoft.EntityFrameworkCore.Tools     | 8.0.20  |
| Microsoft.Extensions.Configuration.Json | 8.0.1   |

Kiểm tra:

```bash
dotnet list package
```

---

## 🏗️ 3. Kết nối Database và Sinh Model

### 🧬 Scaffold Entity

```bash
dotnet ef dbcontext scaffold "Data Source=localhost\SQLEXPRESS;Initial Catalog=NoteHubDB; Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -o Models -f
```

---

## ⚙️ 4. Cấu hình `appsettings.json`

```json
{
  "exclude": [
    "**/bin",
    "**/bower_components",
    "**/jspm_packages",
    "**/node_modules",
    "**/obj",
    "**/platforms"
  ],
  "ConnectionStrings": {
    "NoteHubDB": "Data Source=YOUR_SERVER_NAME;Initial Catalog=NoteHubDB;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true"
  }
}
```

> 💡 Thay `YOUR_SERVER_NAME` bằng tên server thật của bạn (ví dụ: `localhost\\SQLEXPRESS`).

---

## 🧾 Ghi chú thêm

* Dự án dùng **.NET 8**, **WPF**, **Entity Framework Core**, và **SQL Server**.
* Khi build, kiểm tra lại `appsettings.json` đã được copy vào thư mục `bin/Debug/net8.0-windows`.
* Nếu thiếu, thêm **Copy always** trong file property.

---

## 💬 Liên hệ & Đóng góp

Nếu bạn muốn đóng góp, tạo pull request hoặc liên hệ qua GitHub để trao đổi thêm.

---

✨ *NoteHub — Ghi chú thông minh, quản lý dễ dàng.*

```

---
