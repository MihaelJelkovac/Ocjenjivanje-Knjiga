# CRUD Repository Skill — Lab-4 Repository Extension

**Svrha**: Proširiti sve repository interfacee i implementacije s CRUD metodama (Create, Update, Delete, Search) prema Lab4.md zahtjevima za soft delete i AJAX pretragu.

## Input Parametri

- `entityName` — Naziv entiteta (Author, Book, Genre, Publisher, Review, User)
- `repositoryPath` — Puna putanja do repozitorija (npr. `Services/AuthorRepository.cs`)
- `interfacePath` — Puna putanja do interfacea (npr. `Services/IAuthorRepository.cs`)
- `includeSearch` — `true` samo za Authors, Publishers, Genres; `false` za ostale

## Output — Interfejs

Dodati tri nove metode u svaki `I*Repository` interfejs:

```csharp
// Dodati nakon GetByIdAsync
Task<int> CreateAsync(T entity);

Task<bool> UpdateAsync(T entity);

Task<bool> DeleteAsync(int id);

// Samo za Authors, Publishers, Genres:
Task<IReadOnlyList<T>> SearchAsync(string query);
```

## Output — Implementacija

Dodati u svaki `*Repository` class:

### CreateAsync
```csharp
public async Task<int> CreateAsync(T entity)
{
    // Refleksija da postavi DeletedAt = null ako postoji
    var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
    if (deletedAtProperty != null && deletedAtProperty.CanWrite)
    {
        deletedAtProperty.SetValue(entity, null);
    }
    
    _context.Set<T>().Add(entity);
    await _context.SaveChangesAsync();
    
    return (int)typeof(T).GetProperty("Id")?.GetValue(entity)!;
}
```

### UpdateAsync
```csharp
public async Task<bool> UpdateAsync(T entity)
{
    try
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    catch
    {
        return false;
    }
}
```

### DeleteAsync (Soft Delete)
```csharp
public async Task<bool> DeleteAsync(int id)
{
    try
    {
        var entity = await _context.Set<T>().FirstOrDefaultAsync(e => 
            EF.Property<int>(e, "Id") == id && 
            EF.Property<DateTime?>(e, "DeletedAt") == null);
        
        if (entity == null)
            return false;
        
        var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedAtProperty != null && deletedAtProperty.CanWrite)
        {
            deletedAtProperty.SetValue(entity, DateTime.UtcNow);
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }
    catch
    {
        return false;
    }
}
```

### SearchAsync (Samo za Authors, Publishers, Genres)

**Za Author:**
```csharp
public async Task<IReadOnlyList<Author>> SearchAsync(string query)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return await GetAllAsync();
    }
    
    var searchQuery = query.ToLower();
    
    return await _context.Authors
        .Where(a => a.DeletedAt == null &&
                   (a.FirstName.ToLower().Contains(searchQuery) ||
                    a.LastName.ToLower().Contains(searchQuery)))
        .OrderBy(a => a.LastName)
        .ThenBy(a => a.FirstName)
        .Take(20)
        .ToListAsync();
}
```

**Za Publisher:**
```csharp
public async Task<IReadOnlyList<Publisher>> SearchAsync(string query)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return await GetAllAsync();
    }
    
    var searchQuery = query.ToLower();
    
    return await _context.Publishers
        .Where(p => p.DeletedAt == null &&
                   p.Name.ToLower().Contains(searchQuery))
        .OrderBy(p => p.Name)
        .Take(20)
        .ToListAsync();
}
```

**Za Genre:**
```csharp
public async Task<IReadOnlyList<Genre>> SearchAsync(string query)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return await GetAllAsync();
    }
    
    var searchQuery = query.ToLower();
    
    return await _context.Genres
        .Where(g => g.DeletedAt == null &&
                   g.Name.ToLower().Contains(searchQuery))
        .OrderBy(g => g.Name)
        .Take(20)
        .ToListAsync();
}
```

## Ažuriranje GetAllAsync Metode

**VAŽNO**: Sve `GetAllAsync` metode MORAJU biti ažurirane da filtriraju obrisane zapise:

```csharp
public async Task<IReadOnlyList<T>> GetAllAsync()
{
    return await _context.Set<T>()
        .Where(e => EF.Property<DateTime?>(e, "DeletedAt") == null)
        // ... ostatak koda (OrderBy, Include, itd.)
        .ToListAsync();
}
```

## Ažuriranje GetByIdAsync Metode

```csharp
public async Task<T?> GetByIdAsync(int id)
{
    return await _context.Set<T>()
        .Where(e => EF.Property<int>(e, "Id") == id &&
                   EF.Property<DateTime?>(e, "DeletedAt") == null)
        // ... ostatak koda (Include, itd.)
        .FirstOrDefaultAsync();
}
```

## Interfejs Primjer (IAuthorRepository)

```csharp
namespace Lab3.Services;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync();

    Task<Author?> GetByIdAsync(int id);

    Task<int> CreateAsync(Author entity);

    Task<bool> UpdateAsync(Author entity);

    Task<bool> DeleteAsync(int id);

    Task<IReadOnlyList<Author>> SearchAsync(string query);
}
```

## Implementacija Primjer (AuthorRepository)

```csharp
namespace Lab3.Services;

public class AuthorRepository : IAuthorRepository
{
    private readonly CatalogDbContext _context;

    public AuthorRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Author>> GetAllAsync()
    {
        return await _context.Authors
            .Where(a => a.DeletedAt == null)
            .Include(a => a.Books)
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .Where(a => a.DeletedAt == null)
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<int> CreateAsync(Author entity)
    {
        entity.DeletedAt = null;
        _context.Authors.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Author entity)
    {
        try
        {
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var author = await GetByIdAsync(id);
            if (author == null)
                return false;

            author.DeletedAt = DateTime.UtcNow;
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IReadOnlyList<Author>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllAsync();
        }

        var searchQuery = query.ToLower();

        return await _context.Authors
            .Where(a => a.DeletedAt == null &&
                       (a.FirstName.ToLower().Contains(searchQuery) ||
                        a.LastName.ToLower().Contains(searchQuery)))
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Take(20)
            .ToListAsync();
    }
}
```

## Važne Napomene

1. **Soft Delete**: DeleteAsync NIKADA ne poziva `Remove()`, već postavlja `DeletedAt = DateTime.UtcNow`
2. **Filtriranje**: Sve query metode MORAJU filtrirati gdje je `DeletedAt == null`
3. **SearchAsync**: Trebam ga dodati SAMO za Authors, Publishers, Genres (za AJAX pretragu)
4. **Redoslijed**: SearchAsync vraća max 20 rezultata za performansu
5. **Include relacije**: Trebam zadržati sve postojeće Include pozive

## Verifikacija

- [ ] Svaki interfejs ima CreateAsync, UpdateAsync, DeleteAsync
- [ ] Authors, Publishers, Genres imaju SearchAsync
- [ ] Sve GetAllAsync metode filtriraju DeletedAt == null
- [ ] Sve GetByIdAsync metode filtriraju DeletedAt == null
- [ ] DeleteAsync postavlja DeletedAt = DateTime.UtcNow
- [ ] Kod kompajlira bez grešaka
