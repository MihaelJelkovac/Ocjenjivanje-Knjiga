# Sitemap - Lab 3 URL Routing Map

## Custom Routing Overview

Lab 3 koristi **Attribute Routing** za fleksibilnije i intuitivnije URL-e s lokalnim nazivima za kontrolere (npr. `/zanrovi` umjesto `/Genres`).

## Route struktura

### Default Route
```
Pattern: {controller=Home}/{action=Index}/{id?}
Primjer: /Home/Index, /Books/Details/1
```

### Attribute Routes (prilagođeni URL-i)

---

## URL Mapa

### Home Controller
| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/` | Home | Index | Home/Index.cshtml | Početna stranica s dashboard-om |
| `/home` | Home | Index | Home/Index.cshtml | Početna stranica (alternativno) |
| `/home/index` | Home | Index | Home/Index.cshtml | Početna stranica (eksplicitno) |
| `/arena` | Home | RatingArena | Home/RatingArena.cshtml | Stranu sa žanrovskim statistikama |
| `/rating-arena` | Home | RatingArena | Home/RatingArena.cshtml | Stranu sa žanrovskim statistikama (alternativno) |
| `/home/privacy` | Home | Privacy | Home/Privacy.cshtml | Stranica o privatnosti |
| `/home/error` | Home | Error | Shared/Error.cshtml | Stranica greške |

---

### Books Controller
**Attribute Routing**: `[Route("[controller]")]` i `[Route("knjige")]`

| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/books` | Books | Index | Books/Index.cshtml | Popis svih knjiga |
| `/books/` | Books | Index | Books/Index.cshtml | Popis svih knjiga (trailing slash) |
| `/books/index` | Books | Index | Books/Index.cshtml | Popis svih knjiga (eksplicitno) |
| `/books/1` | Books | Details | Books/Details.cshtml | Detalji knjige s ID-om 1 |
| `/books/detalji/1` | Books | Details | Books/Details.cshtml | Detalji knjige (prilagođeni URL) |
| `/knjige` | Books | Index | Books/Index.cshtml | Popis svih knjiga (lokalizirani URL) |
| `/knjige/` | Books | Index | Books/Index.cshtml | Popis svih knjiga (lokalizirani URL s trailing slash) |
| `/knjige/1` | Books | Details | Books/Details.cshtml | Detalji knjige s ID-om 1 (lokalizirani) |
| `/knjige/detalji/1` | Books | Details | Books/Details.cshtml | Detalji knjige (lokalizirani prilagođeni) |

---

### Authors Controller
**Attribute Routing**: `[Route("[controller]")]` i `[Route("autori")]`

| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/authors` | Authors | Index | Authors/Index.cshtml | Popis svih autora |
| `/authors/index` | Authors | Index | Authors/Index.cshtml | Popis svih autora (eksplicitno) |
| `/authors/1` | Authors | Details | Authors/Details.cshtml | Detalji autora s ID-om 1 |
| `/authors/profil/1` | Authors | Details | Authors/Details.cshtml | Profil autora (prilagođeni URL) |
| `/autori` | Authors | Index | Authors/Index.cshtml | Popis svih autora (lokalizirani URL) |
| `/autori/1` | Authors | Details | Authors/Details.cshtml | Detalji autora (lokalizirani) |
| `/autori/profil/1` | Authors | Details | Authors/Details.cshtml | Profil autora (lokalizirani prilagođeni) |

---

### Genres Controller
**Attribute Routing**: `[Route("[controller]")]` i `[Route("zanrovi")]`

| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/genres` | Genres | Index | Genres/Index.cshtml | Popis svih žanrova |
| `/genres/index` | Genres | Index | Genres/Index.cshtml | Popis svih žanrova (eksplicitno) |
| `/genres/1` | Genres | Details | Genres/Details.cshtml | Detalji žanra s ID-om 1 |
| `/genres/popis/1` | Genres | Details | Genres/Details.cshtml | Popis knjiga po žanru (prilagođeni URL) |
| `/zanrovi` | Genres | Index | Genres/Index.cshtml | Popis svih žanrova (lokalizirani URL) |
| `/zanrovi/1` | Genres | Details | Genres/Details.cshtml | Detalji žanra (lokalizirani) |
| `/zanrovi/popis/1` | Genres | Details | Genres/Details.cshtml | Popis knjiga po žanru (lokalizirani prilagođeni) |

---

### Publishers Controller
**Attribute Routing**: `[Route("[controller]")]` i `[Route("izdavaci")]`

| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/publishers` | Publishers | Index | Publishers/Index.cshtml | Popis svih izdavača |
| `/publishers/index` | Publishers | Index | Publishers/Index.cshtml | Popis svih izdavača (eksplicitno) |
| `/publishers/1` | Publishers | Details | Publishers/Details.cshtml | Detalji izdavača s ID-om 1 |
| `/publishers/detalji/1` | Publishers | Details | Publishers/Details.cshtml | Detalji izdavača (prilagođeni URL) |
| `/izdavaci` | Publishers | Index | Publishers/Index.cshtml | Popis svih izdavača (lokalizirani URL) |
| `/izdavaci/1` | Publishers | Details | Publishers/Details.cshtml | Detalji izdavača (lokalizirani) |
| `/izdavaci/detalji/1` | Publishers | Details | Publishers/Details.cshtml | Detalji izdavača (lokalizirani prilagođeni) |

---

### Reviews Controller
**Attribute Routing**: `[Route("[controller]")]` i `[Route("recenzije")]`

| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/reviews` | Reviews | Index | Reviews/Index.cshtml | Popis svih recenzija |
| `/reviews/index` | Reviews | Index | Reviews/Index.cshtml | Popis svih recenzija (eksplicitno) |
| `/reviews/1` | Reviews | Details | Reviews/Details.cshtml | Detalji recenzije s ID-om 1 |
| `/reviews/prikaz/1` | Reviews | Details | Reviews/Details.cshtml | Prikaz recenzije (prilagođeni URL) |
| `/recenzije` | Reviews | Index | Reviews/Index.cshtml | Popis svih recenzija (lokalizirani URL) |
| `/recenzije/1` | Reviews | Details | Reviews/Details.cshtml | Detalji recenzije (lokalizirani) |
| `/recenzije/prikaz/1` | Reviews | Details | Reviews/Details.cshtml | Prikaz recenzije (lokalizirani prilagođeni) |

---

### Users Controller
**Attribute Routing**: `[Route("[controller]")]` i `[Route("korisnici")]`

| URL | Controller | Action | View | Opis |
|-----|-----------|--------|------|------|
| `/users` | Users | Index | Users/Index.cshtml | Popis svih korisnika |
| `/users/index` | Users | Index | Users/Index.cshtml | Popis svih korisnika (eksplicitno) |
| `/users/1` | Users | Details | Users/Details.cshtml | Detalji korisnika s ID-om 1 |
| `/users/profil/1` | Users | Details | Users/Details.cshtml | Profil korisnika (prilagođeni URL) |
| `/korisnici` | Users | Index | Users/Index.cshtml | Popis svih korisnika (lokalizirani URL) |
| `/korisnici/1` | Users | Details | Users/Details.cshtml | Detalji korisnika (lokalizirani) |
| `/korisnici/profil/1` | Users | Details | Users/Details.cshtml | Profil korisnika (lokalizirani prilagođeni) |

---

## Custom Routes - Opis

Svaki kontroler ima sljedeće custom route konfiguracije:

1. **Prilagođeni URL-i** - Brojne alternative za iste akcije
2. **Lokalizirani URL-i** - Nazivi na hrvatskom jeziku (np. `/zanrovi` umjesto `/Genres`)
3. **Semantički URL-i** - Više ispravnih URL-a za istu akciju (npr. `/books/1` i `/books/detalji/1`)

## Route Constraints

- `{id:int}` - ID mora biti cijeli broj

## Redoslijed skeniranja ruta

1. Attribute routes se procesiraju prvi
2. Ako attribute route odgovara, koristi se
3. Ako ne postoji attribute route match, koristi se default route pattern

---

## Primjeri korištenja

### Pristup knjigama
- Engleski nazivi: `/books`, `/books/1`
- Hrvatski nazivi: `/knjige`, `/knjige/1`
- Semantički URL-i: `/books/detalji/1`, `/knjige/detalji/1`

### Pristup autorima
- Engleski nazivi: `/authors`, `/authors/1`
- Hrvatski nazivi: `/autori`, `/autori/1`
- Profil URL-i: `/authors/profil/1`, `/autori/profil/1`

---

## Ključne karakteristike routinga

✓ Attribute routing za preciznu kontrolu
✓ Lokalizirani URL-i na hrvatskom
✓ Alternativni URL-i za istu akciju
✓ Type-safe routing s `{id:int}` constraint-ima
✓ Default fallback route za kompatibilnost
