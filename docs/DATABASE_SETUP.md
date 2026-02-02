# Инструкция по созданию базы данных PostgreSQL для проекта «Топаз»

## 1. Установка PostgreSQL

### 1.1. Скачивание и установка

1. Перейдите на официальный сайт: https://www.postgresql.org/download/windows/
2. Скачайте установщик с сайта PostgreSQL или через EDB: https://www.enterprisedb.com/downloads/postgres-postgresql-downloads
3. Запустите установщик и пройдите шаги мастера.
4. На шаге **Password** задайте пароль пользователя `postgres` (запомните его — он понадобится для строки подключения).
5. Порт по умолчанию оставьте **5432**, если не планируете другой.
6. Завершите установку.

### 1.2. Проверка установки

Откройте командную строку или PowerShell и выполните:

```bash
psql -U postgres -h localhost -p 5433 -c "SELECT version();"
```

Если команда не найдена, добавьте каталог PostgreSQL в переменную PATH (например, `C:\Program Files\PostgreSQL\16\bin`).

---

## 2. Создание баз данных

Для проекта предполагаются две базы:

- **Рабочая (продакшен/разработка)** — например, `TopasDb`
- **Тестовая** — например, `TopasTest` (для автоматических тестов хранилища)

### 2.1. Через графический клиент (pgAdmin)

1. Запустите **pgAdmin** (устанавливается вместе с PostgreSQL).
2. Подключитесь к серверу (двойной щелчок, введите пароль `postgres`).
3. Правый щелчок по **Databases** → **Create** → **Database**.
4. Имя: `TopasDb` → **Save**.
5. Повторите для тестовой БД: имя `TopasTest` → **Save**.

### 2.2. Через командную строку (psql)

Подключитесь к серверу:

```bash
psql -U postgres -h localhost -p 5433
```

В интерактивной консоли выполните:

```sql
-- Рабочая база для приложения «Топаз»
CREATE DATABASE "TopasDb"
    WITH ENCODING = 'UTF8'
    LC_COLLATE = 'Russian_Russia.1251'
    LC_CTYPE = 'Russian_Russia.1251'
    TEMPLATE = template0;

-- Тестовая база для тестов хранилища
CREATE DATABASE "TopasTest"
    WITH ENCODING = 'UTF8'
    LC_COLLATE = 'Russian_Russia.1251'
    LC_CTYPE = 'Russian_Russia.1251'
    TEMPLATE = template0;
```

Если кодировка по умолчанию уже UTF8, можно упростить:

```sql
CREATE DATABASE "TopasDb";
CREATE DATABASE "TopasTest";
```

Выход из psql: `\q`.

### 2.3. Одной командой из PowerShell/CMD (без входа в psql)

```bash
psql -U postgres -h localhost -p 5433 -c "CREATE DATABASE \"TopasDb\";"
psql -U postgres -h localhost -p 5433 -c "CREATE DATABASE \"TopasTest\";"
```

Пароль запросится после каждой команды.

---

## 3. Строка подключения

Формат для Npgsql / Entity Framework Core:

```
Host=localhost;Port=5433;Database=TopasDb;Username=postgres;Password=Filosof;
```

Подставьте свои значения:

| Параметр    | Описание              | Пример      |
|------------|------------------------|-------------|
| Host       | Адрес сервера          | `localhost` |
| Port       | Порт PostgreSQL        | `5433`      |
| Database   | Имя базы               | `TopasDb` или `TopasTest` |
| Username   | Пользователь           | `postgres`  |
| Password   | Пароль пользователя    | Filosof     |

Примеры:

- Рабочая БД:  
  `Host=localhost;Port=5433;Database=TopasDb;Username=postgres;Password=Filosof;`
- Тестовая БД:  
  `Host=localhost;Port=5433;Database=TopasTest;Username=postgres;Password=Filosof;`

Храните строки подключения в конфигурации приложения (appsettings.json, переменные окружения, User Secrets), не коммитьте пароли в репозиторий.

---

## 4. Проектирование: сущности и ограничения

Классы-сущности соответствуют классам-моделям (DataModels), разработанным на первом этапе. Исключение — сущность **Должность** (историчность типа 2). Ограничения по уникальности и связи задаются в `OnModelCreating` наследника `DbContext`.

### 4.1. Таблицы и поля

| Сущность        | Таблица       | Основные поля |
|-----------------|---------------|----------------|
| Покупатель      | Buyers        | Id, FIO, PhoneNumber, DiscountSize |
| Производитель   | Manufacturers | Id, ManufacturerName, PrevManufacturerName, PrevPrevManufacturerName |
| Товар           | Products      | Id, ManufacturerId, ProductName, ProductType, Price, IsDeleted, PrevProductName, PrevPrevProductName |
| История товара  | ProductHistories | Id, ProductId, OldPrice, ChangeDate |
| Должность       | Posts         | **Id** (уникален для каждой строки), **PostId** (идентификатор должности из модели), PostName, PostType, Salary, **IsActual**, **ChangeDate** |
| Работник        | Workers       | Id, FIO, PostId, BirthDate, EmploymentDate, IsDeleted |
| Зарплата        | Salaries      | Id, WorkerId, WorkerSalary, SalaryDate |
| Продажа         | Sales         | Id, WorkerId, BuyerId, SaleDate, Sum, DiscountType, Discount, IsCancel |
| Продажа–Товар   | SaleProducts  | **SaleId, ProductId** (составной первичный ключ), Count, Price |

### 4.2. Ограничения по сущностям

- **Покупатель**: уникальность по полю **PhoneNumber** (не допускаются два покупателя с одним номером телефона). По ФИО уникальность не задаётся.
- **Производитель**: уникальность по полю **ManufacturerName**. Дополнительно — **запрет на удаление** производителя при наличии у него товаров (`OnDelete(DeleteBehavior.Restrict)` для связи Manufacturer → Products).
- **Товар**: уникальность по **ProductName** только среди **неудалённых** записей (`IsDeleted = false`). Удалённый товар может иметь то же название, что и активный; среди активных дубликатов названия быть не должно.
- **Должность** (историчная сущность):
  - У одной должности может быть несколько строк в таблице (один **PostId**, разные **Id**).
  - **PostId** — идентификатор должности из модели; **Id** — уникальный идентификатор каждой записи.
  - **IsActual** — признак актуальной записи по данному PostId; **ChangeDate** — дата добавления записи (для хронологии).
  - Уникальность **названия должности** только среди активных записей (`IsActual = true`).
  - Уникальность **PostId** только среди активных записей: не должно быть двух активных записей с одним PostId.
- **Работник, Зарплата, Продажа**: ограничений на уникальность полей нет.
- **Продажа–Товар (SaleProduct)**: отдельного поля-идентификатора нет; **первичный ключ — составной (SaleId, ProductId)**. Один и тот же товар в одной продаже не может фигурировать более одного раза.

### 4.3. Связи (кратко)

- Покупатель ↔ Продажа (один ко многим).
- Производитель ↔ Товар (один ко многим).
- Товар ↔ ProductHistory, Товар ↔ SaleProduct (один ко многим).
- Работник ↔ Продажа, Работник ↔ Зарплата (один ко многим).
- Продажа ↔ SaleProduct (один ко многим).
- Должность с Работником напрямую в EF не связывается навигацией (чтобы не делать PostId уникальным в таблице Posts).

### 4.4. Конфигурация в коде

- Строка подключения передаётся в DbContext через интерфейс, например `IConfigurationDatabase` с свойством `ConnectionString`.
- В `OnConfiguring` используется `UseNpgsql(connectionString)` (и при необходимости `SetPostgresVersion`).
- В `OnModelCreating` задаются: уникальные индексы (в т.ч. с фильтром по `IsActual` / `IsDeleted`), составной ключ для `SaleProduct`, `HasOne/WithMany` для Product → Manufacturer с `OnDelete(DeleteBehavior.Restrict)`.

Для 2–3 сущностей по заданию маппинг можно настроить через атрибуты на классах-сущностях; остальные — через Fluent API в `OnModelCreating`.

---

## 5. Создание таблиц (через Entity Framework Core)

После добавления в проект слоя с EF Core и DbContext:

1. В коде при старте приложения вызываются, например:
   - `context.Database.EnsureCreated()` — создаёт БД и таблицы по текущей модели, если их ещё нет,  
   или
   - миграции: `context.Database.Migrate()`.
2. При использовании **EnsureCreated()** отдельно создавать таблицы вручную не нужно — они создадутся при первом запуске приложения/тестов с подключением к соответствующей БД.

Убедитесь, что строка подключения в конфигурации указывает на нужную базу (`TopasDb` или `TopasTest`).

### 5.1. SQL-скрипты схемы (ручное создание таблиц)

В папке **docs/sql/** лежат готовые SQL-скрипты со схемой для каждой базы:

- **TopasDb_schema.sql** — таблицы и ограничения для рабочей БД.
- **TopasTest_schema.sql** — таблицы и ограничения для тестовой БД.

Их можно выполнить вручную, если нужно создать схему без EF (например, для проверки или восстановления):

```bash
# из корня репозитория, после создания пустых баз TopasDb и TopasTest:
psql -U postgres -h localhost -p 5433 -d TopasDb -f docs/sql/TopasDb_schema.sql
psql -U postgres -h localhost -p 5433 -d TopasTest -f docs/sql/TopasTest_schema.sql
```

В скриптах создаются таблицы: Buyers, Manufacturers, Posts, Workers, Products, ProductHistories, Salaries, Sales, SaleProducts — с первичными ключами, уникальными индексами и внешними ключами по заданию.

---

## 6. Типичные проблемы

**Ошибка подключения «Connection refused»**

- Убедитесь, что служба PostgreSQL запущена (служба Windows «postgresql-x64-16» или аналог).
- Проверьте, что в строке подключения указаны правильные Host и Port (в примере: localhost, 5433).

**Ошибка «password authentication failed»**

- Проверьте имя пользователя и пароль в строке подключения.
- При установке пароль задаётся для пользователя `postgres`.

**Кодировка и русский язык**

- При создании БД через `CREATE DATABASE` можно явно указать `ENCODING = 'UTF8'` и при необходимости нужные LC_COLLATE/LC_CTYPE для Windows.

---

## 7. Краткий чеклист (по созданию БД)

- [ ] Установлен PostgreSQL.
- [ ] Создана база `TopasDb` (рабочая).
- [ ] Создана база `TopasTest` (для тестов).
- [ ] Строка подключения прописана в конфигурации приложения (и при необходимости для тестов).
- [ ] При первом запуске приложения/тестов с EF таблицы созданы (EnsureCreated или миграции).

После этого можно подключать приложение «Топаз» к PostgreSQL и при необходимости дорабатывать конфигурацию DbContext и миграции.
