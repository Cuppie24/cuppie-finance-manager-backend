CREATE TABLE IF NOT EXISTS "Categories"
(
    "Id"
             SERIAL
        PRIMARY
            KEY,
    "Name"
             VARCHAR(20) NOT NULL,
    "Income" BOOLEAN     NOT NULL
);  

CREATE TABLE IF NOT EXISTS "Transactions"
(
    "Id"
                 BIGSERIAL
        PRIMARY
            KEY,
    "CreatedAt"
                 TIMESTAMP
                         NOT
                             NULL,
    "Amount"
                 NUMERIC(18,
                     2)  NOT NULL,
    "Income"     BOOLEAN NOT NULL,
    "Comment"    VARCHAR(50),
    "CategoryId" BIGINT  NOT NULL REFERENCES "Categories"
        (
         "Id"
            ) ON DELETE SET DEFAULT,
    "UserId"     INT     NOT NULL
);

INSERT INTO "Categories" ("Name", "Income")
VALUES ('Без категории', FALSE)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO "Categories" ("Name", "Income")
VALUES ('Заработная плата', TRUE),
       ('Иные доходы', TRUE),
       ('Продукты питания', FALSE),
       ('Транспорт', FALSE),
       ('Мобильная связь', FALSE),
       ('Интернет', FALSE),
       ('Развлечения', FALSE);
