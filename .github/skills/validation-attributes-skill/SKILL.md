# Validation Attributes Skill — Lab-4 CRUD Modeli

**Svrha**: Dodati sve potrebne validacijske atribute u modele prema Lab4.md zahtjevima za client-side i server-side validaciju.

## Input Parametri

- `entityName` — Naziv entiteta (Author, Book, Genre, Publisher, Review, User)
- `entityFilePath` — Puna putanja do modela (npr. `Models/Author.cs`)

## Output

Ažurirani model s dodanim:
- `using System.ComponentModel.DataAnnotations;` namespace
- `[Required]` atributu na svim obaveznim poljima
- `[StringLength(max, MinimumLength = min)]` atributu za string polja
- `[Range(min, max)]` atributu za numerička polja
- `[EmailAddress]` atributu za email polja
- Sve postojeće anotacije (`[Key]`, `[ForeignKey]`, itd.) ostaju netaknute
- **Novo**: `public DateTime? DeletedAt { get; set; }` polje na kraju klase (prije zatvorene zagrade)

## Validacijska Pravila po Entitetu

### Author
```csharp
[Required(ErrorMessage = "Ime autora je obavezno")]
[StringLength(100, MinimumLength = 2)]
public string FirstName { get; set; }

[Required(ErrorMessage = "Prezime autora je obavezno")]
[StringLength(100, MinimumLength = 2)]
public string LastName { get; set; }

[StringLength(1000)]
public string Biography { get; set; }

[StringLength(100)]
public string Nationality { get; set; }

[StringLength(500)]
[Url]
public string Website { get; set; }

// Novo: Soft Delete
public DateTime? DeletedAt { get; set; }
```

### Book
```csharp
[Required(ErrorMessage = "Naslov knjige je obavezan")]
[StringLength(500, MinimumLength = 3)]
public string Title { get; set; }

[Required(ErrorMessage = "ISBN je obavezan")]
[StringLength(20, MinimumLength = 10)]
[RegularExpression(@"^\d{10}(\d{3})?$|^97[89]\d{10}$")]
public string Isbn { get; set; }

[StringLength(2000)]
public string Description { get; set; }

[Required]
[Range(1, 10000, ErrorMessage = "Broj stranica mora biti između 1 i 10000")]
public int PageCount { get; set; }

[StringLength(50)]
public string Language { get; set; }

// Novo: Soft Delete
public DateTime? DeletedAt { get; set; }
```

### Genre
```csharp
[Required(ErrorMessage = "Naziv žanra je obavezan")]
[StringLength(100, MinimumLength = 2)]
public string Name { get; set; }

[StringLength(500)]
public string Description { get; set; }

[StringLength(100)]
public string Audience { get; set; }

// Novo: Soft Delete
public DateTime? DeletedAt { get; set; }
```

### Publisher
```csharp
[Required(ErrorMessage = "Naziv izdavača je obavezan")]
[StringLength(200, MinimumLength = 2)]
public string Name { get; set; }

[Required(ErrorMessage = "Grad je obavezan")]
[StringLength(100, MinimumLength = 2)]
public string City { get; set; }

[Required(ErrorMessage = "Država je obavezna")]
[StringLength(100, MinimumLength = 2)]
public string Country { get; set; }

[StringLength(500)]
[Url]
public string Website { get; set; }

[StringLength(150)]
[EmailAddress]
public string ContactEmail { get; set; }

// Novo: Soft Delete
public DateTime? DeletedAt { get; set; }
```

### Review
```csharp
[Required(ErrorMessage = "Naslov recenzije je obavezan")]
[StringLength(300, MinimumLength = 3)]
public string Title { get; set; }

[Required(ErrorMessage = "Ocjena je obavezna")]
[Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5")]
public int Score { get; set; }

[StringLength(2000)]
public string Comment { get; set; }

[StringLength(500)]
public string? Comment { get; set; }

[Required]
public bool IsRecommended { get; set; }

// Novo: Soft Delete
public DateTime? DeletedAt { get; set; }
```

### User
```csharp
[Required(ErrorMessage = "Korisničko ime je obavezno")]
[StringLength(100, MinimumLength = 3)]
[RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Korisničko ime može sadržavati samo slova, brojeve i _")]
public string Username { get; set; }

[Required(ErrorMessage = "Puno ime je obavezno")]
[StringLength(200, MinimumLength = 3)]
public string FullName { get; set; }

[Required(ErrorMessage = "Email je obavezan")]
[EmailAddress(ErrorMessage = "Email nije u ispravnom formatu")]
[StringLength(200)]
public string Email { get; set; }

[StringLength(100)]
public string FavoriteGenre { get; set; }

[Range(0, 10000, ErrorMessage = "Reputacijski bodovi moraju biti između 0 i 10000")]
public int ReputationPoints { get; set; }

// Novo: Soft Delete
public DateTime? DeletedAt { get; set; }
```

## Primjeri Korištenja

### Primjer 1: Author
```
Entitet: Author
Akcija: Dodaj validacijske atribute
Očekivani rezultat: FirstName [Required], LastName [Required], DeletedAt? polje
```

### Primjer 2: Book
```
Entitet: Book
Akcija: Dodaj validacijske atribute
Očekivani rezultat: Title [Required], ISBN [Required], PageCount [Range(1,10000)], DeletedAt? polje
```

## Važne Napomene

1. **DeletedAt polje**: MORA biti `DateTime?` (nullable) na kraju klase, prije zatvorene zagrade
2. **Using statement**: `using System.ComponentModel.DataAnnotations;` MORA biti na početku datoteke
3. **Url atribut**: Ako ne postoji, trebam dodati `using System.ComponentModel.DataAnnotations;` import
4. **Greške sa ErrorMessage**: Sve poruke trebaju biti na hrvatskom jeziku
5. **Redoslijed**: Atributi trebaju biti u redoslijedu [Key], [ForeignKey], zatim [Required], [StringLength], [Range], [EmailAddress], [Url], [RegularExpression]
6. **Postojeće relacije**: Sve postojeće `[ForeignKey]` i navigacijske properties ostaju netaknute

## Verifikacija

- [ ] Svi string atributi imaju [StringLength]
- [ ] Svi obavezni atributi imaju [Required]
- [ ] Svi numerički imaju [Range]
- [ ] Email polja imaju [EmailAddress]
- [ ] DeletedAt je `DateTime?`
- [ ] Model se kompajlira bez grešaka
- [ ] Nema dodanog `using` koji nije potreban
