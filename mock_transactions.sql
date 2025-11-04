DO
$$
    DECLARE
        i INT;
        random_income BOOLEAN;
        random_category_id INT;
        random_amount NUMERIC;
        random_comment TEXT;
        random_date TIMESTAMP;

    BEGIN
        FOR i IN 1..200 LOOP
                -- Примерно 30% транзакций — доходы
                random_income := (random() < 0.3);

                IF random_income THEN
                    -- Доходы: 70% зарплата, 30% иные доходы
                    IF random() < 0.7 THEN
                        random_category_id := 2; -- Заработная плата
                    ELSE
                        random_category_id := 3; -- Иные доходы
                    END IF;

                    -- Сумма: от 500 000 до 4 500 000
                    random_amount := round((random() * 4000000 + 500000)::numeric, 2);
                ELSE
                    -- Расходы с разными шансами
                    random_category_id := CASE
                                              WHEN random() < 0.03 THEN 1  -- Без категории (редко)
                                              WHEN random() < 0.28 THEN 4  -- Продукты питания (часто)
                                              WHEN random() < 0.43 THEN 5  -- Транспорт
                                              WHEN random() < 0.58 THEN 6  -- Мобильная связь
                                              WHEN random() < 0.73 THEN 7  -- Интернет
                                              ELSE 8                       -- Развлечения
                        END;

                    -- Сумма: от 1 000 до 300 000
                    random_amount := round((random() * 300000 + 1000)::numeric, 2);
                END IF;

                -- Дата за последние 90 дней
                random_date := now() - (random() * interval '90 days');

                -- Комментарии
                random_comment := CASE floor(random() * 8)
                                      WHEN 0 THEN 'Покупка'
                                      WHEN 1 THEN 'Перевод'
                                      WHEN 2 THEN 'Оплата счёта'
                                      WHEN 3 THEN 'Зарплата'
                                      WHEN 4 THEN 'Подарок'
                                      WHEN 5 THEN 'Еда на вынос'
                                      WHEN 6 THEN 'Такси'
                                      ELSE NULL
                    END;

                -- Вставка записи
                INSERT INTO "Transactions" ("CreatedAt", "Amount", "Income", "Comment", "CategoryId", "UserId")
                VALUES (random_date, random_amount, random_income, random_comment, random_category_id, 1);
            END LOOP;
    END
$$;
