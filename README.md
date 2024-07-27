# E-Commerce Platform

Welcome to the E-Commerce Platform, a robust and secure e-commerce application built with .NET 8 and Bootstrap 5. This platform provides a comprehensive admin panel and a seamless shopping experience for users.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Technologies Used](#technologies-used)
3. [Features](#features)
4. [Detailed Project Areas](#detailed-project-areas)
    - [Seller](#seller)
    - [Admin](#admin)
    - [Customer](#customer)
5. [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Configuration](#configuration)
6. [Running the Application](#running-the-application)
7. [Contributing](#contributing)
8. [License](#license)

## Project Overview

This project represents the culmination of three months of effort, encompassing business logic, code refactoring, bug fixing, deployment issues, and feature implementation. This is the initial release of the project, with plans for further enhancements.

## Technologies Used

- **.NET 8**: The main framework for backend development.
- **C# 12**: The programming language used for backend logic.
- **Microsoft SQL Server**: Database management system.
- **Entity Framework Core**: ORM for database interaction.
- **jQuery, Ajax, JavaScript, HTML, CSS, Bootstrap**: Frontend technologies.
- **Libraries**: Toastr, Owl Carousel, Font Awesome, Boxicons, TinyMCE.

## Features

- Localization based on Resources for multi-language support. 🌐
- Repository pattern.
- Dependency injection.
- Custom Middlewares.
- Custom Validation.
- Integration with the Stripe Payment API.
- Caching, session management, and cookie handling.
- Identity Model customization.
- Permission-based authorization.
- Configuration Manager for custom settings in the web application.
- Application of the DRY principle by using delegates for common functions in services.
- AJAX for async server communication.
- Searching functionality with sorting based on various criteria. 🔍

## Detailed Project Areas

### Seller

- Manage products, including various fields and discounts. 🛒
- Handle incoming orders with different states (InProgress, Rejected, Arrived, etc.). 📦
- Access a dashboard displaying essential statistics. 📊

### Admin

- Control permissions for each role. 🔐
- Manage users by banning or deleting them. 👥
- Manage products, including displaying/hiding products and marking them with a premium tag. 💼
- Manage banners on the home page. 🖼
- Manage categories and brands. 🏷
- Access a dashboard presenting website-wide statistics. 📈

### Customer

- Add products to the cart/wishlist. 🛍
- Checkout functionality for products in the cart with available quantities. 💳
- Order tracking to monitor order statuses. 📋
- Review and rate purchased products. ⭐️

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio or any other C# IDE

### Installation

1. Clone the repository:

    ```sh
    git clone https://github.com/yourusername/ecommerce-platform.git
    cd ecommerce-platform
    ```

2. Install the required packages:

    ```sh
    dotnet restore
    ```

### Configuration

1. Set up the database connection string in `appsettings.json`:

    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "Server=your_server;Database=ECommerceDb;User Id=your_username;Password=your_password;"
        }
    }
    ```

2. Apply database migrations:

    ```sh
    dotnet ef database update
    ```

## Running the Application

1. Start the application:

    ```sh
    dotnet run
    ```

2. Navigate to `https://localhost:5001` to view the application.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request for review.

## License

This project is licensed under the MIT License.

---

Thank you for using our E-Commerce Platform! If you have any questions or need further assistance, feel free to contact us.
