# 🛒 E-Commerce Platform — Moroccan Cooperatives

> A full-featured ASP.NET Web Forms e-commerce application designed for Moroccan cooperatives to sell agricultural, handicraft, and terroir products, both domestically and internationally.

---

## 📸 Overview

This platform empowers local Moroccan cooperatives with a complete online storefront — from product browsing and cart management to order tracking and admin analytics. It includes an AI-powered chatbot, Google OAuth authentication, PDF invoice generation, and a promo code system.

---

## 🚀 Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | ASP.NET Web Forms (.NET Framework 4.7.2) |
| **Language** | C# |
| **Database** | SQL Server |
| **Frontend** | HTML5, CSS3, JavaScript, Bootstrap |
| **Authentication** | Session-based + Google OAuth 2.0 |
| **Email** | SMTP via Gmail |
| **AI Chatbot** | Groq API (Llama 3.3 70B) |
| **PDF Generation** | iTextSharp 5.5.13.4 |
| **Cryptography** | BouncyCastle 2.6.2 |

---

## ✨ Features

### 👤 Customer Features

- **Authentication**: Registration with email verification, login/logout, forgot/reset password, Google OAuth
- **Product Catalog**: Listing, search, filtering by price/category, product details with image gallery & variants
- **Shopping Cart**: Guest & logged-in cart with session persistence, cart merging on login
- **Wishlist**: Save products for later
- **Checkout**: Full checkout flow with address management, promo code support, and order confirmation
- **Orders**: Order history, order tracking, detailed order view, and PDF invoice download
- **Profile**: Account management, address book, complaint submission
- **AI Chatbot**: Integrated chatbot powered by Groq/Llama for customer support

### 🛠️ Admin Features

- **Dashboard**: Sales statistics, order metrics, product analytics
- **Product Management**: Full CRUD — products, categories, images, variants, stock
- **Order Management**: View/filter orders, update statuses, track history
- **User Management**: List users, activate/deactivate accounts
- **Promo Codes**: Create, edit, delete coupon codes with rules (min order, max discount, usage limits)
- **Complaint Management**: View, respond to, and manage customer complaints with priority assignment

---

## 📁 Project Structure

```
E-commerce/
├── App_Code/                   # Core business logic
│   ├── DbContext.cs            # Database access layer
│   ├── SecurityHelper.cs       # Auth & security utilities
│   ├── CartHelper.cs           # Shopping cart management
│   ├── CouponHelper.cs         # Promo code validation
│   └── ChatbotLogic.cs         # AI chatbot integration
├── Pages/
│   ├── Public/                 # 19 customer-facing pages
│   │   ├── Login.aspx
│   │   ├── Register.aspx
│   │   ├── Shop.aspx
│   │   ├── ProductDetails.aspx
│   │   ├── Cart.aspx
│   │   ├── Checkout.aspx
│   │   ├── OrderTracking.aspx
│   │   ├── Profile.aspx
│   │   └── ...
│   └── Admin/                  # 8 admin dashboard pages
│       ├── Dashboard.aspx
│       ├── Products.aspx
│       ├── Orders.aspx
│       ├── Users.aspx
│       ├── PromoCodes.aspx
│       ├── Categories.aspx
│       ├── ComplaintsManagement.aspx
│       └── OrderHistory.aspx
├── Assets/
│   ├── Styles/                 # CSS files
│   ├── Images/                 # Product & category images
│   └── Js/                     # JavaScript files
├── Database/                   # SQL setup & migration scripts
│   ├── Setup.sql
│   ├── Migration_v2.0.sql
│   ├── Migration_v3.0.sql
│   └── Migration_PromoCodes.sql
├── Utils/
│   ├── EmailTemplates.cs       # HTML email templates
│   └── InvoiceHelper.cs        # PDF invoice generation
├── auth/google/                # Google OAuth callback handler
├── ChatbotService.asmx         # Chatbot web service
├── Site.Master                 # Customer-facing master page
├── Admin.Master                # Admin master page
└── Web.config                  # Application configuration
```

---

## 🗄️ Database Schema

The application uses **19 SQL Server tables**:

| Table | Description |
|---|---|
| `Users` | Customer, admin, and cooperative accounts |
| `Addresses` | User shipping addresses |
| `Categories` | Hierarchical product categories |
| `Products` | Product catalog |
| `ProductImages` | Additional product images |
| `ProductVariants` | Size, color, and other variants |
| `Orders` | Order records |
| `OrderItems` | Line items per order |
| `OrderHistory` | Status change history |
| `ShoppingCart` | Guest & user carts |
| `Wishlist` | Saved products |
| `Reviews` | Product reviews *(structure ready)* |
| `Coupons` | Promo codes |
| `CouponUsage` | Per-user coupon usage tracking |
| `Complaints` | Customer complaint tickets |
| `Notifications` | In-app user notifications |
| `PaymentMethods` | Payment method configuration |
| `ShippingMethods` | Shipping option configuration |
| `Cooperatives` | Multi-vendor cooperative support *(structure ready)* |

---

## ⚙️ Setup & Installation

### Prerequisites

- Visual Studio 2019 or later
- SQL Server (2017+ recommended) or SQL Server Express
- .NET Framework 4.7.2
- SQL Server Management Studio (SSMS) *(optional)*

### 1. Clone the Repository

```bash
git clone https://github.com/Houssam265/Application-E-commerce--DotNet-.git
cd Application-E-commerce--DotNet-
```

### 2. Configure the Database

1. Open SQL Server Management Studio.
2. Create a new database (e.g., `EcommerceDB`).
3. Run the SQL scripts in order:
   ```
   Database/Setup.sql
   Database/Migration_v2.0.sql
   Database/Migration_v3.0.sql
   Database/Migration_PromoCodes.sql
   ```

### 3. Configure `Web.config`

Update the connection string and external service credentials in `E-commerce/Web.config`:

```xml
<!-- Database Connection -->
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Server=YOUR_SERVER;Database=EcommerceDB;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>

<appSettings>
  <!-- Google OAuth -->
  <add key="GoogleClientId" value="YOUR_GOOGLE_CLIENT_ID" />
  <add key="GoogleClientSecret" value="YOUR_GOOGLE_CLIENT_SECRET" />

  <!-- Gmail SMTP -->
  <add key="SmtpEmail" value="your-email@gmail.com" />
  <add key="SmtpPassword" value="YOUR_APP_PASSWORD" />

  <!-- Groq AI Chatbot -->
  <add key="GroqApiKey" value="YOUR_GROQ_API_KEY" />
</appSettings>
```

> ⚠️ **Security Note**: Never commit real credentials to source control. Use environment variables or a secret manager in production.

### 4. Run the Application

1. Open `E-commerce.slnx` in Visual Studio.
2. Build the solution (`Ctrl+Shift+B`).
3. Press `F5` to launch in IIS Express.

---

## 🔒 Security

| Measure | Status |
|---|---|
| SQL Injection Prevention | ✅ Parameterized queries throughout |
| XSS Protection | ✅ HtmlEncode on all output |
| Input Validation | ✅ Server-side + client-side |
| Session-based Auth | ✅ Role checking on every protected page |
| Email Verification | ✅ Required at registration |
| Password Reset | ✅ Token-based reset flow |
| Password Hashing | ⚠️ SHA-256 (consider upgrading to bcrypt/Argon2) |
| CSRF Protection | ❌ Not yet implemented |
| Rate Limiting | ❌ Not yet implemented |

---

## 🧩 NuGet Dependencies

| Package | Version | Purpose |
|---|---|---|
| `iTextSharp` | 5.5.13.4 | PDF invoice generation |
| `BouncyCastle.Cryptography` | 2.6.2 | Cryptographic operations |
| `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` | 4.1.0 | Roslyn C# compiler |
