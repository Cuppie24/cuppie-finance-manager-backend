# Cuppie Finance Manager

A microservices-based personal finance management system with authentication, transaction tracking, and financial analytics.

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- (Optional) .NET 9 SDK for local development
- (Optional) Node.js 18+ for frontend local development

### Run Everything with One Command

From the root directory:

```bash
docker-compose up --build
```

This starts all services:
- **Auth Service** - Port 5001
- **Finance Backend** - Port 5295
- **Frontend** - Port 3000
- **PostgreSQL (Auth)** - Port 5433
- **PostgreSQL (Finance)** - Port 5432

### Access the Application

Open your browser and navigate to:
```
http://localhost:3000
```

### Create Test Users

After starting the services, create test users with the provided script:

**Unix/Linux/Mac:**
```bash
cd cuppie-auth-service
./create-test-users.sh
```

**Windows:**
```bash
cd cuppie-auth-service
create-test-users.bat
```

**Test Credentials:**
- Username: `admin` | Password: `password123`
- Username: `john_doe` | Password: `password123`
- Username: `jane_smith` | Password: `password123`
- Username: `test_user` | Password: `password123`

### Stop All Services

```bash
docker-compose down
```

To remove databases and start fresh:
```bash
docker-compose down -v
```

## 📦 Architecture

This is a monorepo containing three independent microservices:

```
cuppie-finance-manager/
├── cuppie-auth-service/              # Authentication service (.NET 9)
├── cuppie-finance-manager-backend/   # Finance API (.NET 9)
├── cuppie-finance-manager-frontend/  # React SPA (Vite + TypeScript)
└── docker-compose.yaml               # Orchestrate all services
```

## 🛠️ Tech Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Auth Backend | .NET Core | 9.0 |
| Finance Backend | .NET Core | 9.0 |
| Frontend | React + TypeScript | 18.2.0 + 5.0.2 |
| Build Tool | Vite | 4.3.2 |
| Database (Auth) | PostgreSQL | 16 |
| Database (Finance) | PostgreSQL | 17 |
| ORM | Entity Framework Core | 9.0.x |
| UI Library | Shadcn/ui + Radix UI | Latest |
| Styling | Tailwind CSS | 3.4.17 |

## 🔑 Features

### Authentication Service
- JWT-based authentication with refresh tokens
- Secure password hashing (SHA-256 + salt)
- Token rotation with automatic revocation
- Clean Architecture implementation

### Finance Manager
- Transaction management (income/expense)
- Category management with type filtering
- Date range filtering and quick filters
- Financial analytics with pie charts
- Multi-user data isolation
- Real-time balance calculation

### Frontend
- Modern React 18 with TypeScript
- Shadcn/ui component library
- Responsive design with Tailwind CSS
- Automatic token refresh
- Protected routes with auth guards

## 📚 Documentation

For detailed documentation on architecture, development workflows, and API endpoints, see [CLAUDE.md](CLAUDE.md).

## 🔧 Development

### Database Migrations

**Auth Service:**
```bash
cd cuppie-auth-service
dotnet ef migrations add MigrationName \
  --project ./src/Cuppie.Infrastructure \
  --startup-project ./src/Cuppie.Api \
  --context CuppieDbContext \
  --output-dir Data/Migrations

dotnet ef database update \
  --project ./src/Cuppie.Infrastructure \
  --startup-project ./src/Cuppie.Api \
  --context CuppieDbContext
```

**Finance Backend:**
```bash
cd cuppie-finance-manager-backend
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Local Development (without Docker)

**Backend Services:**
```bash
# Terminal 1: Auth service
cd cuppie-auth-service
docker-compose up -d  # Only database
dotnet run --project ./src/Cuppie.Api

# Terminal 2: Finance backend
cd cuppie-finance-manager-backend
docker-compose up -d  # Only database
dotnet run --project CFinanceManager
```

**Frontend:**
```bash
cd cuppie-finance-manager-frontend
npm install
npm run dev
```

## 🔐 Security

- JWT access tokens: 15 minutes expiration
- Refresh tokens: 24 hours expiration
- HttpOnly cookies for token storage
- SHA-256 password hashing with per-user salt
- CORS restrictions to specific origins
- User data isolation at database query level

## 📝 License

MIT License - see LICENSE file for details

## 🤝 Contributing

This is a personal project, but suggestions and feedback are welcome!
