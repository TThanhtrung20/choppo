# Fraud Verification & Response Demo

**ASP.NET Core 10 Razor Pages Application**  
**MVP Demo - 10-Day Development Sprint**

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-blue.svg)](https://docs.microsoft.com/aspnet/core/)
[![License](https://img.shields.io/badge/license-Demo-orange.svg)](LICENSE)

## 📋 Tổng Quan / Overview

Ứng dụng demo **Fraud Verification & Response** (Phòng Chống Lừa Đảo) được xây dựng trong 10 ngày, giúp người dùng:
- ✅ Xác minh tình huống khẩn cấp (thanh toán, cung cấp thông tin, sau sự cố)
- ✅ Quản lý baseline cá nhân (quy tắc bảo vệ, người liên hệ tin cậy, phương thức thanh toán)
- ✅ Tạm dừng - So sánh - Xác minh trước khi hành động

**Triết lý**: Không dùng AI dán nhãn "lừa đảo" mà giúp người dùng tự nhận biết điểm bất thường.

---

## ✨ Tính Năng Chính / Key Features

### 🚨 1. Emergency Help (Không cần đăng nhập)
- **Pay Page** - Hướng dẫn khi bị yêu cầu thanh toán khẩn
- **Information Page** - Hướng dẫn khi bị yêu cầu cung cấp thông tin
- **After Incident** - Hướng dẫn sau khi bị lừa đảo

### 🔐 2. Authentication System
- Register/Login/Logout
- Delete Personal Data (GDPR compliant)

### 📊 3. Personal Baseline Management
- **Protection Rules** - Quản lý các quy tắc bảo vệ cá nhân
- **Trusted Contacts** - Danh sách người liên hệ tin cậy
- **Payment Methods** - Phương thức thanh toán thường dùng

### 🎨 4. Modern UI Enhancement
- Soft gradient design
- Smooth animations & transitions
- Rounded corners & shadows
- Responsive mobile-first layout

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 10 Razor Pages
- **Database**: SQLite (local demo)
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5 + Custom CSS
- **Language**: C# 12 (.NET 10)

---

## 🚀 Cài Đặt / Installation

### Yêu Cầu / Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (10.0.301 or later)
- Git

### Các Bước / Steps

1. **Clone repository**
   ```bash
   git clone https://github.com/TThanhtrung20/choppo.git
   cd choppo
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run application**
   ```bash
   dotnet run
   ```

5. **Open browser**
   ```
   http://localhost:5016
   ```

---

## 📁 Project Structure

```
FraudVerificationDemo/
├── Data/
│   └── ApplicationDbContext.cs       # EF Core DbContext
├── Models/
│   ├── ApplicationUser.cs            # Identity User model
│   ├── ProtectionRule.cs             # Protection rules entity
│   ├── TrustedContact.cs             # Trusted contacts entity
│   └── UsualPaymentMethod.cs         # Payment methods entity
├── Pages/
│   ├── Account/                      # Authentication pages
│   ├── Baseline/                     # Personal baseline management
│   │   ├── Index.cshtml              # Dashboard
│   │   ├── Rules.cshtml              # Protection Rules CRUD
│   │   ├── Contacts.cshtml           # Trusted Contacts CRUD
│   │   └── Payments.cshtml           # Payment Methods CRUD
│   ├── Help/                         # Emergency help pages
│   │   ├── Select.cshtml             # 11 scenarios selection
│   │   ├── Pay.cshtml                # Payment verification
│   │   ├── Information.cshtml        # Information verification
│   │   └── AfterIncident.cshtml      # Post-incident guide
│   └── Shared/
│       ├── _Layout.cshtml            # Master layout
│       └── Components/               # Reusable components
├── Modules/                          # Future feature modules
│   └── Chat/                         # Chat module (placeholder)
├── Migrations/                       # EF Core migrations
├── wwwroot/
│   └── css/
│       └── site.css                  # Custom modern styling
└── Program.cs                        # Application entry point
```

---

## 📖 Hướng Dẫn Sử Dụng / Usage Guide

### Cho Người Dùng Mới / For New Users

1. **Không cần đăng nhập** - Truy cập ngay các trang Help:
   - `/Help/Select` - Chọn tình huống
   - `/Help/Pay` - Hướng dẫn khi bị yêu cầu thanh toán
   - `/Help/Information` - Hướng dẫn khi bị yêu cầu thông tin

2. **Muốn lưu baseline cá nhân**:
   - Register tài khoản tại `/Account/Register`
   - Login tại `/Account/Login`
   - Quản lý baseline tại `/Baseline/Index`

### Quản Lý Baseline / Baseline Management

1. **Protection Rules** (`/Baseline/Rules`)
   - Thêm quy tắc như: "Ngân hàng không bao giờ gọi yêu cầu mật khẩu"
   - Xem lại khi gặp tình huống đáng ngờ

2. **Trusted Contacts** (`/Baseline/Contacts`)
   - Thêm số điện thoại người thân, bạn bè tin cậy
   - So sánh với số lạ đang gọi đến

3. **Payment Methods** (`/Baseline/Payments`)
   - Ghi lại các khoản thanh toán thường xuyên
   - Nhận biết yêu cầu thanh toán bất thường

---

## 🎨 UI Enhancement Details

### Design System
- **Colors**: Indigo primary palette (#4F46E5)
- **Shadows**: 4-level shadow system (sm/md/lg/xl)
- **Radius**: 0.75rem - 2rem rounded corners
- **Transitions**: 0.2s - 0.4s smooth animations

### Key Features
- Gradient backgrounds for all major elements
- Hover lift effects on cards & buttons
- Smooth focus states for accessibility
- Custom scrollbar styling
- Mobile-responsive design

---

## 📊 Development Timeline

**Total**: 10 days (~7.5 hours actual work time)

- **Day 1**: Database models + migrations
- **Day 2**: Layout + navigation
- **Day 3**: Emergency help pages
- **Day 4**: Reusable components
- **Day 5**: Scenario selection routing
- **Day 6**: Authentication system
- **Day 7**: Protection Rules CRUD
- **Day 8**: Trusted Contacts CRUD
- **Day 9**: Payment Methods CRUD
- **Day 10**: Dashboard enhancements
- **UI Enhancement**: Modern soft design upgrade

---

## 🔒 Security Notes

⚠️ **This is a DEMO application**:
- SQLite database (not for production)
- Known package vulnerabilities (acceptable for demo)
- No real payment processing
- Synthetic data only

For production use:
- Migrate to PostgreSQL/SQL Server
- Update vulnerable packages
- Implement proper security hardening
- Add rate limiting & CAPTCHA

---

## 📝 Database Schema

```sql
ApplicationUsers (Identity tables)
├── Id (string, PK)
├── UserName
├── Email
└── ... (standard Identity fields)

ProtectionRules
├── Id (int, PK)
├── UserId (string, FK)
├── Content (string)
└── CreatedAt (DateTime)

TrustedContacts
├── Id (int, PK)
├── UserId (string, FK)
├── Name (string)
├── PhoneNumber (string)
├── Relationship (string)
└── CreatedAt (DateTime)

UsualPaymentMethods
├── Id (int, PK)
├── UserId (string, FK)
├── RecipientName (string)
├── Method (enum: BankTransfer/Cash/EWallet/Card)
├── UsualAmount (decimal?)
└── CreatedAt (DateTime)
```

---

## 🧪 Testing

```bash
# Build
dotnet build

# Run tests (if added)
dotnet test

# Check warnings
dotnet build --no-restore
```

**Build Status**: ✅ Success (6 warnings - package vulnerabilities)

---

## 🌐 Deployment

### Local Development
```bash
dotnet run --environment Development
```

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Docker (Optional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY ./publish /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "FraudVerificationDemo.dll"]
```

---

## 📚 Documentation

- [PROJECT-COMPLETE.md](PROJECT-COMPLETE.md) - Full project completion report
- [FINAL-SUMMARY.md](FINAL-SUMMARY.md) - Final summary
- [UI-ENHANCEMENT.md](UI-ENHANCEMENT.md) - UI upgrade details
- [PROGRESS-TRACKER.md](PROGRESS-TRACKER.md) - Daily progress tracking
- [DAY-1-COMPLETE.md](DAY-1-COMPLETE.md) through [DAY-10-COMPLETE.md](DAY-10-COMPLETE.md) - Daily logs

---

## 🤝 Contributing

This is a demo project. For improvements:
1. Fork the repository
2. Create a feature branch
3. Commit changes
4. Push to the branch
5. Open a Pull Request

---

## 📄 License

**Demo License** - For educational and demonstration purposes only.

---

## 👤 Author

**Trung Tran**  
GitHub: [@TThanhtrung20](https://github.com/TThanhtrung20)

---

## 🙏 Acknowledgments

- ASP.NET Core Team for excellent documentation
- Bootstrap for responsive framework
- Entity Framework Core for ORM
- SQLite for lightweight database

---

## 📞 Support

For questions or issues:
- Open an issue on GitHub
- Check documentation files in the repository

---

## 🗺️ Roadmap (Future Enhancements)

- [ ] Chat module with AI guidance
- [ ] Voice input for emergency situations
- [ ] Credit/reward system
- [ ] Multi-language support (EN/VI)
- [ ] Dark mode theme
- [ ] Mobile app (MAUI)
- [ ] SMS verification alerts
- [ ] Integration with real anti-fraud databases

---

**Built with ❤️ in 10 days**
