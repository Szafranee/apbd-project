# APBD Project

## Project Description
This project was created as part of the Database Applications (APBD) course at the Polish-Japanese Academy of Information Technology. It is a contract management system that allows businesses to manage clients, contracts, products, and payment processing.

### What This Application Does
The application provides a comprehensive contract management solution with the following core functionality:

- **Client Management**: Handles two types of clients - individual and corporate clients. Individual clients can be soft-deleted (anonymized) while corporate clients have different deletion rules.
  
- **Contract Management**: Creates and manages contracts between clients and products, with support for:
  - Multiple years of support options
  - Automatic price calculation based on product cost and support duration
  - Contract status tracking (e.g., Pending, Active, Completed)
  
- **Product Integration**: Products with yearly license costs are integrated into the contract system.

- **Payment Tracking**: Payments associated with contracts can be tracked with status information.

- **Discount System**: Implements a flexible discount system with:
  - Time-limited discounts (with start and end dates)
  - Percentage-based discount values
  - Automatic application of the highest available discount
  - Special 5% discount for returning customers

## Features
- Complete CRUD operations for clients, contracts, and discounts
- Automatic price calculation including applicable discounts
- Client type-specific business rules (different handling for individual vs. corporate clients)
- Time-bound discount management
- Soft deletion for individual clients (data anonymization)
- Contract status tracking

## Technologies
- Backend: ASP.NET Core / Entity Framework Core
- Database: SQL Server
- API: RESTful API endpoints

## System Requirements
- .NET 6.0 or newer
- SQL Server

## Installation

### Cloning the Repository
```bash
git clone https://github.com/yourusername/apbd-project.git
cd apbd-project
```

### Backend Configuration
```bash
dotnet restore
dotnet build
```

### Database Configuration
1. Create a database in SQL Server
2. Update the connection string in the `appsettings.json` file
3. Apply migrations:
```bash
dotnet ef database update
```

## Running the Application
```bash
dotnet run
```

## Project Structure
```
apbd-project/
├── Controllers/
│   ├── ClientsController.cs      # API endpoints for client management
│   ├── ContractsController.cs    # API endpoints for contract management
│   └── DiscountsController.cs    # API endpoints for discount management
│
├── Data/
│   └── DatabaseContext.cs        # EF Core database context
│
├── Domain/
│   ├── Clients/
│   │   ├── Clients.cs            # Client domain models
│   │   └── ClientType.cs         # Client type enumeration
│   ├── Contracts/
│   │   ├── Contract.cs           # Contract domain model
│   │   └── ContractStatus.cs     # Contract status enumeration
│   ├── Discounts/
│   │   └── Discount.cs           # Discount domain model
│   ├── Payments/
│   │   ├── Payment.cs            # Payment domain model
│   │   └── PaymentStatus.cs      # Payment status enumeration
│   └── Products/
│       └── Product.cs            # Product domain model
│
├── DTOs/
│   └── ClientDto.cs              # Data transfer objects
│
├── Exceptions/
│   └── NotFoundException.cs      # Custom exceptions
│
├── Interfaces/
│   ├── IClientService.cs         # Service interfaces
│   ├── IContractService.cs
│   └── IDiscountService.cs
│
├── Services/
│   ├── ClientService.cs          # Service implementations
│   ├── ContractService.cs
│   └── DiscountService.cs
│
├── Program.cs                    # Application entry point
└── README.md                     # Project documentation
```

## API Documentation
The application provides the following API endpoints:

### Clients
- `GET /api/clients` - Get list of clients
- `GET /api/clients/{id}` - Get client details
- `POST /api/clients` - Create a new client
- `PUT /api/clients/{id}` - Update a client
- `DELETE /api/clients/{id}` - Delete a client (soft delete for individual clients)

### Contracts
- `GET /api/contracts` - Get list of contracts
- `GET /api/contracts/{id}` - Get contract details
- `POST /api/contracts` - Create a new contract
- `PUT /api/contracts/{id}` - Update a contract
- `DELETE /api/contracts/{id}` - Delete a contract

### Discounts
- `GET /api/discounts` - Get list of discounts
- `GET /api/discounts/{id}` - Get discount details
- `POST /api/discounts` - Create a new discount
- `PUT /api/discounts/{id}` - Update a discount
- `DELETE /api/discounts/{id}` - Delete a discount

## Business Rules

### Client Management
- Individual clients can be soft-deleted (data anonymized)
- Corporate clients cannot be deleted (business rule restriction)

### Contract Pricing
- Base price comes from the product's yearly license cost
- Support years add additional cost (1000 per year)
- Active discounts are automatically applied (highest percentage used)
- Returning customers receive an additional 5% discount

## Testing
```bash
dotnet test
```

## Troubleshooting
- Database connection issues: Check the connection string in the `appsettings.json` file
- Migration errors: Delete the Migrations folder and generate migrations again

## Author
Kacper Szafrański - s28102@pjwstk.edu.pl

## License
This project is licensed under the MIT License. See the LICENSE file for details.
