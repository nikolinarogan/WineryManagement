# Frontend Setup

## 🚀 Kako pokrenuti

### 1. Instaliraj dependencies
```bash
npm install
```

### 2. Kreiraj .env fajl
```bash
cp .env.example .env
```

Ili kreiraj `.env` fajl sa:
```env
VITE_API_BASE_URL=http://localhost:5115/api
```

### 3. Pokreni development server
```bash
npm run dev
```

Aplikacija će biti dostupna na: **http://localhost:5173**

---

## 🔐 Login Kredencijali

**Default Menadžer:**
- Email: `admin@vinarija.rs`
- Lozinka: `Admin123!`

---

## 📁 Struktura

```
src/
├── assets/          # Slike, CSS
├── components/      # Vue komponente
├── router/          # Rute + guards
├── services/        # API servisi
│   ├── api.ts          # Axios konfiguracija
│   └── authService.ts  # Auth API calls
├── stores/          # Pinia stores
│   └── auth.ts         # Auth state management
├── types/           # TypeScript tipovi
│   └── auth.ts         # Auth interface-i
├── views/           # Stranice
│   ├── LoginView.vue
│   ├── RegisterView.vue
│   ├── HomeView.vue
│   └── AboutView.vue
├── App.vue          # Root komponenta
└── main.ts          # Entry point
```

---

## 🎨 Dizajn

- **Boje:** Crno-bela šema
- **Font:** System font stack
- **Stil:** Minimalistički, clean

---

## 🔒 Autentifikacija

- JWT token se čuva u `localStorage`
- Automatski se dodaje u svaki API request (axios interceptor)
- Pri 401 Unauthorized → automatski logout i redirect na login
- Router guards štite rute koje zahtevaju autentifikaciju

### Zaštićene rute:
- `/` - Početna (zahteva auth)
- `/about` - O nama (zahteva auth)
- `/register` - Dodaj zaposlenog (samo Menadžer)

### Javne rute:
- `/login` - Login stranica

---

## 📋 Funkcionalnosti

### Za sve korisnike:
- ✅ Login
- ✅ Logout
- ✅ Pregled profila
- ✅ Promena lozinke (u pripremi)

### Za Menadžera:
- ✅ Dodavanje novih zaposlenih
- ✅ Pristup svim funkcijama

---

## 🛠️ Build za produkciju

```bash
npm run build
```

Build fajlovi će biti u `dist/` folderu.

