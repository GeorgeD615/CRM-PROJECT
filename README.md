# CRM-PROJECT

## DirectoryService

Сервис оргструктуры: локации, подразделения, должности.

### Публичный API

Реализован публичный REST-контракт для трёх сущностей — **Location**, **Department**, **Position**. Бизнес-логики и хранения пока нет: контроллеры возвращают заглушки, контракт зафиксирован, чтобы фронтенд мог писать клиент параллельно с бэком.

У всех трёх сущностей одинаковая схема endpoint-ов (`{entity}` — `locations`, `departments`, `positions`):

| Метод | URL | Описание | Ответ |
|---|---|---|---|
| POST | `/api/{entity}` | Создание | 201 Created + объект |
| GET | `/api/{entity}` | Список | 200 OK + массив |
| GET | `/api/{entity}/{id}` | Получение по id | 200 OK / 404 Not Found |
| PUT | `/api/{entity}/{id}` | Обновление | 200 OK + объект / 404 Not Found |
| DELETE | `/api/{entity}/{id}` | Удаление | 204 No Content / 404 Not Found |

Также доступен health-check: `GET /api/health`.

### Контракты

DTO лежат в отдельном проекте `DirectoryService.Contracts` и не зависят от Domain и Infrastructure — на каждую сущность свой запрос на создание, запрос на обновление и ответ:

- **Locations** — `CreateLocationRequest`, `UpdateLocationRequest`, `LocationResponse` (Id, Name, Address); адрес — вложенный `AddressDto` (City, Street, House)
- **Departments** — `CreateDepartmentRequest` (Name, Slug, ParentId?), `UpdateDepartmentRequest` (Name; slug неизменяем), `DepartmentResponse` (Id, Name, Slug, Path, ParentId)
- **Positions** — `CreatePositionRequest`, `UpdatePositionRequest`, `PositionResponse` (Id, Name)

### Запуск

```bash
dotnet run --project backend/DirectoryService/src/DirectoryService.Web --launch-profile http
```

Документация API (Scalar): http://localhost:5121/scalar — все endpoint-ы с типизированными request/response.
