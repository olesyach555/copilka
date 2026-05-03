# Инструкции для ИИ (Onboarding Prompt)

Этот файл содержит контекст и технические правила проекта для ИИ-помощников.

## Контекст проекта
**Kopilka** — настольное WPF-приложение (.NET 8) для семейного учета финансов.

## Технологический стек
* **Язык:** C# 12 / .NET 8
* **UI:** WPF (XAML)
* **БД:** SQLite + EF Core
* **Архитектура:** Layered (Shared, DataAccess, BusinessLogic, FinanceManager)
* **MVVM:** CommunityToolkit.Mvvm

## Важные правила и "Подводные камни"

### 1. SQLite и Decimal Sum
SQLite в EF Core не умеет в `SumAsync` для `decimal`.
**Правило:** Кастовать в `double` внутри запроса:
```csharp
.SumAsync(a => (double)a.Balance)
```

### 2. XAML Стили
В `ResourceDictionary` нельзя присваивать `Brush` свойству `Color`.
**Ошибка:** `<SolidColorBrush Color="{StaticResource SomeBrush}" />` — вызовет исключение.
**Правильно:** Использовать HEX или `StaticResource` указывающий на `Color`.

### 3. Dependency Injection
В проекте **ручной DI**. Сервисы создаются в `App.xaml.cs` и пробрасываются через конструкторы. Не используйте автоматические DI-контейнеры без явной просьбы.

### 4. Nullable Reference Types (NRT)
Поля в UI-классах должны быть nullable (например, `User? _user`), если они не инициализируются в конструкторе. Всегда проверяйте на `null` перед использованием.

### 5. Имена полей в моделях
* `User.Login` (не Username)
* `Transaction.Date` (не DateTime)
* `Transaction.Comment` (не Description)

## Структура проекта
* `src/Kopilka.Shared` — Модели
* `src/Kopilka.DataAccess` — Context и Миграции
* `src/Kopilka.BusinessLogic` — Сервисы и ViewModels
* `src/Kopilka.FinanceManager` — WPF UI
