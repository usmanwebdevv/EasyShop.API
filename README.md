# EasyShop.API

A RESTful e-commerce backend built with ASP.NET Core Web API, C#, Entity Framework Core, and SQL Server.

## Features

- User registration and login with JWT authentication
- Password hashing with BCrypt
- Role-based authorization (Admin and User)
- Product and category management
- Product search, filtering, sorting, and pagination
- Shopping cart management
- Checkout and order creation
- Stock validation and stock deduction
- Order cancellation with stock restoration
- Order status management
- Database transactions
- Global exception handling

## Tech Stack

- C# / .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- BCrypt
- Repository Pattern
- Service Layer

## Project Architecture

Controllers
    ↓
Services
    ↓
Repositories
    ↓
Entity Framework Core
    ↓
SQL Server

## Main API Endpoints

### Authentication
POST /api/auth/register
POST /api/auth/login

### Products
GET /api/products
POST /api/products
PUT /api/products/{id}
DELETE /api/products/{id}

### Categories
GET /api/categories
POST /api/categories
PUT /api/categories/{id}
DELETE /api/categories/{id}

### Cart
GET /api/cart
POST /api/cart/items
PUT /api/cart/items/{productId}
DELETE /api/cart/items/{productId}
POST /api/cart/checkout

### Orders
POST /api/orders
GET /api/orders/my
GET /api/orders/{id}
GET /api/orders
PUT /api/orders/{id}/status
PUT /api/orders/{id}/cancel

Note: Some endpoints require authentication or Admin authorization.

## Local Setup

1. Clone the repository.
2. Install the .NET 9 SDK and SQL Server.
3. Configure the DefaultConnection connection string.
4. Initialize .NET User Secrets:

   dotnet user-secrets init

5. Set a strong JWT signing key:

   dotnet user-secrets set "Jwt:Key" "YOUR_OWN_SECURE_RANDOM_KEY"

6. Apply database migrations:

   dotnet ef database update

7. Run the API:

   dotnet run

Replace the example JWT key with your own securely generated secret. Never commit real secrets.

## Project Status

Backend core features implemented and manually tested using Postman.

Automated tests, concurrency protection, production deployment, and frontend integration are planned.

## Author

Usman Ali