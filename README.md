# Pexita E-commerce Backend

A robust, event-driven e-commerce backend system built with .NET 8, implementing modern architecture patterns and best practices for scalable online retail operations.

## 🚀 Features

### Core Functionality
- **Brand Management**
  - Brands can Sign up as brand role
  - Brands can create and share their own products under their name
  - Complete support for Order Management
  - Strict Validation on Order Creation

- **Product Management**
  - Complete CRUD operations for products
  - Support for product images with validation
  - Product categorization with tags
  - Rating and comment system
  - Brand association

- **User Management**
  - Secure user authentication using JWT
  - Comprehensive user profiles including:
    - Personal information (name, email, phone)
    - Multiple delivery addresses
    - Profile pictures
    - Shopping history
    - Newsletter subscriptions

### Shopping Experience
- Shopping cart management
- Order processing
- Payment information handling
- Order status tracking
- Delivery status updates

### Event-Driven Architecture
- Custom event dispatcher system handling:
  - Product availability notifications
  - Brand release events
  - Newsletter distributions
  - Event-handler mapping and automatic dispatch

### Location Validation
- Integration with IranAPI for address validation
- Comprehensive database of Iranian cities and provinces
- Bilingual support (English/Persian)
- Real address verification

## 🛠 Technical Stack

### Core Technologies
- **.NET 8**: Latest framework version for optimal performance
- **MS SQL Server**: Primary database
- **Entity Framework Core**: ORM for database operations

### Authentication & Security
- **JWT**: Token-based authentication
- **BCrypt**: Secure password hashing
- **Local Storage**: JWT token management

### Data Processing
- **AutoMapper**: Efficient object-to-object mapping
- **FluentValidation**: Request validation
- **Custom File Processing**: Image validation and storage

## 📦 Key Components

### Data Models
- Brands
- Products
- Comments
- Events
- Newsletters (Brand & Product)
- Orders
- Payments
- Shopping Carts
- Tags
- Users

### Event System
The system implements an event-driven architecture through:
- Singleton Event Dispatcher
- Event-Handler Registration
- Automatic Event Processing
- Newsletter Email Notifications

## 🔒 Security Features

- Secure password hashing with BCrypt
- JWT-based authentication
- Input validation using FluentValidation
- Image content and format validation
- Secure file storage with unique naming

## 🏗 Architecture Highlights

- Clean Architecture
- Event-driven design for loose coupling
- Repository pattern for data access
- Service layer architecture
- DTO pattern with AutoMapper
- Validator configurations using FluentValidation
- Clean separation of concerns

## 📝 License

This project is licensed under the GPLv3 License - see the [LICENSE.md](LICENSE) file for details.
