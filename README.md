# Cuppie Finance Manager

Микросервисное приложение для управления личными финансами с безопасной аутентификацией и учётом транзакций.

## Архитектура проекта

Проект построен на микросервисной архитектуре и состоит из трёх основных компонентов:

### Cuppie Auth Service
**Независимый микросервис аутентификации и авторизации**

Сервис предоставляет готовое решение для аутентификации пользователей, построенное на принципах Clean Architecture. Используется в финансовом менеджере, но может быть легко интегрирован в любое другое приложение.

**Особенности:**
- **JWT токены**: Access token (короткоживущий) и Refresh token (долгоживущий) для безопасной аутентификации
- **Хранение паролей**: Пароли никогда не хранятся в открытом виде
  - Генерация криптографически стойкой случайной соли (16 байт) для каждого пользователя
  - Хеширование пароля по алгоритму SHA-256: `hash = SHA256(password + salt)`
  - Сохранение в БД только хеша пароля и соли
- **Ротация токенов**: При каждом обновлении Access token автоматически отзывается старый Refresh token
- **HTTP-only cookies**: Токены передаются только через защищённые cookies, недоступные для JavaScript
- **Clean Architecture**: Разделение на слои API, Application, Infrastructure, Domain

**Технологии:**
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- xUnit для тестирования

### Finance Backend
**Основной backend для финансового менеджера**

Сервис управляет финансовыми данными пользователей: транзакциями, категориями и статистикой.

**Основной функционал:**
- Управление транзакциями (доходы/расходы)
- Управление категориями транзакций
- Фильтрация и группировка транзакций
- Интеграция с Auth Service через HTTP-клиент
- JWT авторизация для защищённых эндпоинтов

**Технологии:**
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- REST API

### Frontend
**Основной функционал:**
- Управление транзакциями (создание, редактирование, удаление)
- Управление категориями (CRUD операции)
- Визуализация данных (доходы/расходы через круговые диаграммы)
- Фильтрация транзакций по дате, типу, категориям
- Адаптивный дизайн для мобильных устройств

**Технологии:**
- React 18
- TypeScript
- Vite
- Tailwind CSS
- Shadcn/ui компоненты
- Recharts для графиков
- date-fns для работы с датами

## Запуск проекта

В корневой директории проекта выполните:

```bash
docker compose up --build -d
```

Это запустит все сервисы:
- Auth Service (порт 5295)
- Finance Backend (порт 5296)
- Frontend (порт 3000)
- Два экземпляра PostgreSQL

После запуска приложение будет доступно по адресам:
- **Frontend**: http://localhost:3000
- **Finance API Swagger**: http://localhost:5295/swagger
- **Auth API Swagger**: http://localhost:5001/swagger

## Настройки баз данных

### База данных Finance
- Порт: `5432`
- Пользователь: `cuppie`
- Пароль: `cupcup`
- База данных: `db`

### База данных Auth
- Порт: `5433`
- Пользователь: `cuppie`
- Пароль: `cupcup`
- База данных: `users_auth_db`

При первом запуске бд заполняется автоматически

## Безопасность аутентификации

### Хранение паролей

Пароли хранятся с использованием криптографического хеширования:

```csharp
// Генерация уникальной соли для каждого пользователя
byte[] salt = RandomNumberGenerator.GetBytes(16);

// Хеширование: SHA256(password + salt)
string passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt)));

// Сохранение в БД только хеша и соли
user.PasswordHash = passwordHash;
user.Salt = salt;
```

**Преимущества:**
- ✅ Пароли нельзя восстановить даже при компрометации БД
- ✅ Каждый пароль имеет уникальную соль (защита от rainbow tables)
- ✅ Криптографически стойкий алгоритм SHA-256

### JWT токены

**Access Token:**
- Короткое время жизни
- Содержит информацию о пользователе (ID, имя, email)
- Используется для авторизации API запросов

**Refresh Token:**
- Долгоживущий токен
- Используется только для обновления Access Token
- Хранится в БД с возможностью отзыва
- Автоматическая ротация при каждом обновлении

### Cookies

Все токены передаются через HTTP-only cookies с параметрами:
- `HttpOnly: true` - недоступны для JavaScript (защита от XSS)
- `Secure: false` - (для локальной развертки)

## Структура проекта

```
cuppie-finance-manager/
├── cuppie-auth-service/          # Микросервис аутентификации
│   ├── src/
│   │   ├── Cuppie.Api/          # API слой
│   │   ├── Cuppie.Application/  # Бизнес-логика
│   │   ├── Cuppie.Infrastructure/ # Реализация сервисов, БД
│   │   └── Cuppie.Domain/       # Доменные сущности
│   └── UnitTests/               # Тесты
├── cuppie-finance-manager-backend/  # Backend финансового менеджера
│   ├── Application/             # Бизнес-логика
│   ├── Domain/                  # Доменные модели
│   ├── Infrastructure/          # Репозитории, клиенты
│   └── CFinanceManager/         # API
├── cuppie-finance-manager-frontend/ # Frontend приложение
│   ├── src/
│   │   ├── components/          # React компоненты
│   │   ├── context/             # Context API
│   │   ├── Pages/               # Страницы приложения
│   │   └── lib/                 # Утилиты
│   └── public/
└── docker-compose.yaml          # Оркестрация контейнеров
```

## Разработка

### Стек

- .NET SDK 9
- Node.js 20+ (LTS)
- React + Vite TS
- Docker
- PostgreSQL

### Frontend разработка

```bash
cd cuppie-finance-manager-frontend
npm install
npm run dev
```

### Backend разработка

Каждый сервис можно запускать отдельно через IDE или командами:

```bash
# Auth Service
cd cuppie-auth-service/src/Cuppie.Api
dotnet run

# Finance Backend
cd cuppie-finance-manager-backend/CFinanceManager
dotnet run
```

## API документация

После запуска Swagger доступен по адресам:
- Finance API: http://localhost:5296/swagger
- Auth API: http://localhost:5295/api/swagger
