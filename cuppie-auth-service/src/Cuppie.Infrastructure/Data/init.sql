-- Создание таблицы пользователей
CREATE TABLE IF NOT EXISTS "User"
(
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(30) NOT NULL CHECK ("Username" ~ '^[a-zA-Z0-9_-]+$'),
    "Email" VARCHAR(100) NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Salt" BYTEA NOT NULL
    );

-- Создание таблицы refresh-токенов
CREATE TABLE IF NOT EXISTS "RefreshToken"
(
    "Id" SERIAL PRIMARY KEY,
    "Token" TEXT NOT NULL,
    "UserId" INT NOT NULL REFERENCES "User"("Id") ON DELETE CASCADE,
    "CreatedByIp" VARCHAR(45) NOT NULL,
    "RevokedByIp" VARCHAR(45),
    "IsRevoked" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "RevokedAt" TIMESTAMPTZ,
    "Expires" TIMESTAMPTZ NOT NULL
    );
