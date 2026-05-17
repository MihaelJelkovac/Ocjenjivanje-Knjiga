# Semantic Model - Lab 3

## Database Schema

Baza podataka `catalog.db` koristi SQLite s sljedećim tablicama i vezama.

### Tablice

| Tablica | Opis |
|---------|------|
| **Authors** | Autori knjiga s informacijama o biografiji, rođenju i nacionalnosti |
| **Publishers** | Izdavači knjiga s informacijama o gradu, države i kontaktu |
| **Genres** | Žanrovi/kategorije knjiga |
| **Books** | Knjige s informacijama o ISBN-u, stranicama, jeziku i statusu |
| **BookGenres** | Veza između knjiga i žanrova (N-N) |
| **Reviews** | Recenzije knjiga od korisnika s ocjenom i sentimentom |
| **Users** | Korisnici koji pišu recenzije |

### Glavna svojstva po tablici

#### Authors (Autori)
- `Id` (int) - Primarni ključ, auto-increment
- `FirstName` (string) - Ime autora
- `LastName` (string) - Prezime autora
- `Biography` (string) - Biografija
- `BirthDate` (datetime) - Datum rođenja
- `Nationality` (string) - Nacionalnost
- `Website` (string) - Websajt autora

#### Publishers (Izdavači)
- `Id` (int) - Primarni ključ, auto-increment
- `Name` (string) - Naziv izdavača
- `City` (string) - Grad sjedišta
- `Country` (string) - Država
- `FoundedOn` (datetime) - Datum osnivanja
- `Website` (string) - Websajt
- `ContactEmail` (string) - Kontakt email

#### Genres (Žanrovi)
- `Id` (int) - Primarni ključ, auto-increment
- `Name` (string) - Naziv žanra
- `Description` (string) - Opis
- `Audience` (string) - Ciljna publika

#### Books (Knjige)
- `Id` (int) - Primarni ključ, auto-increment
- `Title` (string) - Naslov knjige
- `Isbn` (string) - ISBN broj
- `Description` (string) - Opis
- `PublishedOn` (datetime) - Datum objavljivanja
- `PageCount` (int) - Broj stranica
- `Language` (string) - Jezik
- `Status` (enum) - Status (Available, Reserved, Archived)
- `AuthorId` (int) - FK → Authors
- `PublisherId` (int) - FK → Publishers

#### BookGenres (Veza Knjiga-Žanrovi)
- `BookId` (int) - FK → Books (dio composite PK)
- `GenreId` (int) - FK → Genres (dio composite PK)
- `AddedAt` (datetime) - Datum dodjele

#### Reviews (Recenzije)
- `Id` (int) - Primarni ključ, auto-increment
- `Score` (int) - Ocjena (1-5)
- `Title` (string) - Naslov recenzije
- `Comment` (string) - Komentar
- `ReviewedAt` (datetime) - Datum recenzije
- `IsRecommended` (bool) - Preporuka
- `Sentiment` (enum) - Sentimen (Critical, Neutral, Positive, Enthusiastic)
- `BookId` (int) - FK → Books
- `UserId` (int) - FK → Users

#### Users (Korisnici)
- `Id` (int) - Primarni ključ, auto-increment
- `Username` (string) - Korisničko ime
- `FullName` (string) - Puno ime
- `Email` (string) - Email
- `JoinedAt` (datetime) - Datum registracije
- `FavoriteGenre` (string) - Omiljeni žanr
- `ReputationPoints` (int) - Reputacijski bodovi
- `IsPremiumMember` (bool) - Premium član

### Veze između tablica

```
Authors (1) ──── (N) Books
                  │
                  ├─── (N) Reviews
                  │       │
                  │       └─── (N) Users
                  │
                  └─── (N) BookGenres ─── (N) Genres

Publishers (1) ──── (N) Books
```

## EF Core Konfiguracija

- **DbContext**: `CatalogDbContext` (Lab3.Data)
- **Provider**: SQLite
- **Connection String**: `Data Source=catalog.db`
- **Migrations**: `Migrations` folder
- **Initial Migration**: `20260503214646_Initial`

## Seeding podataka

Baza se automatski puni inicijalno s:
- 3 autora (J.K. Rowling, George R. R. Martin, J.R.R. Tolkien)
- 3 izdavača (Bloomsbury, Bantam Books, Allen & Unwin)
- 3 žanra (Fantasy, Science Fiction, Drama)
- 3 knjige (Harry Potter, Game of Thrones, Fellowship of the Ring)
- 3 korisnika (Alice, Bob, Carol)
- 3 recenzije s različitim ocjenama
