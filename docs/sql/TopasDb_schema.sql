-- Схема базы данных TopasDb (рабочая БД приложения «Топаз»).
-- Выполнять при подключении к базе TopasDb:
--   psql -U postgres -h localhost -p 5433 -d TopasDb -f TopasDb_schema.sql

-- Таблицы без внешних ключей (порядок важен для зависимостей)
CREATE TABLE IF NOT EXISTS "Buyers" (
    "Id" text NOT NULL,
    "FIO" text NOT NULL,
    "PhoneNumber" text NOT NULL,
    "DiscountSize" double precision NOT NULL,
    CONSTRAINT "PK_Buyers" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Buyers_PhoneNumber" ON "Buyers" ("PhoneNumber");

CREATE TABLE IF NOT EXISTS "Manufacturers" (
    "Id" text NOT NULL,
    "ManufacturerName" text NOT NULL,
    "PrevManufacturerName" text,
    "PrevPrevManufacturerName" text,
    CONSTRAINT "PK_Manufacturers" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Manufacturers_ManufacturerName" ON "Manufacturers" ("ManufacturerName");

CREATE TABLE IF NOT EXISTS "Posts" (
    "Id" text NOT NULL,
    "PostId" text NOT NULL,
    "PostName" text NOT NULL,
    "PostType" integer NOT NULL,
    "Salary" double precision NOT NULL,
    "IsActual" boolean NOT NULL,
    "ChangeDate" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Posts" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Posts_PostName_IsActual" ON "Posts" ("PostName", "IsActual") WHERE "IsActual" = TRUE;
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Posts_PostId_IsActual" ON "Posts" ("PostId", "IsActual") WHERE "IsActual" = TRUE;

CREATE TABLE IF NOT EXISTS "Workers" (
    "Id" text NOT NULL,
    "FIO" text NOT NULL,
    "PostId" text NOT NULL,
    "BirthDate" timestamp with time zone NOT NULL,
    "EmploymentDate" timestamp with time zone NOT NULL,
    "IsDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Workers" PRIMARY KEY ("Id")
);

-- Таблицы с внешними ключами
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" text NOT NULL,
    "ManufacturerId" text NOT NULL,
    "ProductName" text NOT NULL,
    "ProductType" integer NOT NULL,
    "Price" double precision NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "PrevProductName" text,
    "PrevPrevProductName" text,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Products_Manufacturers_ManufacturerId" FOREIGN KEY ("ManufacturerId") REFERENCES "Manufacturers" ("Id") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Products_ProductName_IsDeleted" ON "Products" ("ProductName", "IsDeleted") WHERE "IsDeleted" = FALSE;

CREATE TABLE IF NOT EXISTS "ProductHistories" (
    "Id" text NOT NULL,
    "ProductId" text NOT NULL,
    "OldPrice" double precision NOT NULL,
    "ChangeDate" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ProductHistories" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProductHistories_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Salaries" (
    "Id" text NOT NULL,
    "WorkerId" text NOT NULL,
    "WorkerSalary" double precision NOT NULL,
    "SalaryDate" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Salaries" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Salaries_Workers_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Workers" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "Sales" (
    "Id" text NOT NULL,
    "WorkerId" text NOT NULL,
    "BuyerId" text,
    "SaleDate" timestamp with time zone NOT NULL,
    "Sum" double precision NOT NULL,
    "DiscountType" integer NOT NULL,
    "Discount" double precision NOT NULL,
    "IsCancel" boolean NOT NULL,
    CONSTRAINT "PK_Sales" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Sales_Workers_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Workers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Sales_Buyers_BuyerId" FOREIGN KEY ("BuyerId") REFERENCES "Buyers" ("Id") ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS "SaleProducts" (
    "SaleId" text NOT NULL,
    "ProductId" text NOT NULL,
    "Count" integer NOT NULL,
    "Price" double precision NOT NULL,
    CONSTRAINT "PK_SaleProducts" PRIMARY KEY ("SaleId", "ProductId"),
    CONSTRAINT "FK_SaleProducts_Sales_SaleId" FOREIGN KEY ("SaleId") REFERENCES "Sales" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SaleProducts_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);
