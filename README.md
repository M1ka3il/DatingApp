# 🧠 Fullstack Web App with .NET 9 & Angular 20

![.NET 9](https://img.shields.io/badge/.NET-9.0-blueviolet?logo=dotnet&logoColor=white)
![Angular 20](https://img.shields.io/badge/Angular-20-red?logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-5.8-blue?logo=typescript&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-Real--Time-green?logo=signal&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework-Core%209.0-blue?logo=database&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind%20CSS-4%20%2B%20DaisyUI%205-38B2AC?logo=tailwindcss&logoColor=white)
![License: MIT](https://img.shields.io/badge/license-MIT-yellow)

---

> 🚧 **This project is a work in progress. Features are being implemented step by step as part of my self taught WebDev Journey**

## ✅ Project Checklist

- [x] Project setup (.NET & Angular CLI)
- [x] User registration and JWT authentication (API)
- [x] Basic user profile UI (member list & member detail)
- [x] API integration between Angular and .NET Core
- [x] AutoMapper configuration
- [x] Entity Framework Core setup with code-first migrations (SQLite)
- [x] Filtering, sorting, and pagination
- [x] Notifications in Angular (ngx-toastr)
- [x] Error handling for API and SPA
- [x] Environment switching (Angular `environment.ts` / `environment.development.ts`)
- [x] Integration of third-party UI components (Tailwind CSS, DaisyUI, ngx-toastr)
- [x] Unit tests for `AccountController` and `MembersController` (xUnit, EF Core InMemory)
- [x] Persist JWT in the client & send it with requests (localStorage + JWT interceptor)
- [x] Photo upload with drag & drop (local storage in `wwwroot/uploads`, set main photo, delete)
- [x] Private messaging system
- [x] Real-time presence and messaging with SignalR
- [ ] Registration form in the Angular client (API + `AccountService.register()` exist, UI missing)
- [ ] Route guards for authenticated pages
- [ ] Cloud storage for photos (e.g. Cloudinary – swap `LocalFileStorageService` via `IFileStorageService`)
- [ ] Deployment-ready configuration (production settings, CORS, serving the SPA)

---

## 🔧 Tech Stack

- **Frontend**: Angular 20 (standalone components, signals, zoneless change detection)
- **Styling**: Tailwind CSS 4 + DaisyUI 5
- **Backend**: ASP.NET Core (.NET 9) Web API
- **Database**: Entity Framework Core 9 (Code First) with SQLite
- **Real-time**: SignalR (presence & message hubs)
- **Auth**: JWT Bearer tokens, HMACSHA512 password hashing
- **Mapping**: AutoMapper
- **Testing**: xUnit + EF Core InMemory
- **IDE**: Visual Studio

---

## 📦 What's implemented

**API**
- `POST api/account/register` / `POST api/account/login` – returns user + JWT
- `GET api/members` – search by username, sort asc/desc, pagination (`Pagination` response header, max page size 50)
- `GET api/members/{id}` – single member
- `POST api/members/add-photo`, `PUT api/members/set-main-photo/{id}`, `DELETE api/members/delete-photo/{id}` – photo management (jpg/png/gif/webp, max 10 MB)
- `POST api/messages`, `GET api/messages` (Inbox / Outbox / Unread, paginated), `GET api/messages/thread/{userId}`, `DELETE api/messages/{id}`
- SignalR hubs: `hubs/presence` (online users) and `hubs/message` (live chat threads), JWT via `access_token` query string
- Global `ExceptionMiddleware` (stack trace only in Development)
- `api/buggy/*` endpoints to test the error-handling pipeline

**Client**
- Login / logout in the navbar, session persisted in localStorage, JWT attached by interceptor
- Member list with search, A–Z / Z–A sorting and pagination
- Member detail page with drag & drop photo upload, main-photo selection and deletion
- Messages page (inbox/outbox) and live chat thread
- Online presence indicator
- HTTP error interceptor: toasts for 400/401, redirect to `/not-found` (404) and `/server-error` (500)

---

## 🚀 Features (planned)

- 🔜 Registration form in the client
- 🔜 Route guards
- 🔜 Cloud storage for photos
- 🔜 Deployment-ready configuration

---
