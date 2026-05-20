using Xunit;
using Lab4.Models;
using Lab4.Services;
using Lab4.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Tests
{
    /// <summary>
    /// Testira sve CRUD operacije za Books entitet
    /// Primjer: Za ostale entitete (Author, Genre, Publisher, Review) kopiraj ovaj pattern
    /// </summary>
    public class BookRepositoryTests
    {
        // Metoda koja kreira testni DbContext s InMemory bazom
        private CatalogDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CatalogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Jedinstvena baza za svaki test
                .Options;

            var context = new CatalogDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Test: Kreiraj novu knjižu i provjeri da se pojavljuje u bazi
        /// 
        /// Scenarij: Administrator dodaje novu knjižu
        /// 1. Kreira novu Book entitetu s valjanim podacima
        /// 2. Sprema ju u bazu kroz repository
        /// 3. Provjerava da se knjižu može učitati po ID-u
        /// 4. Provjerava da su svi podaci točni
        /// </summary>
        [Fact]
        public async Task CreateBook_WithValidData_ShouldSaveToDatabase()
        {
            // ARRANGE - Pripremite testni scenarij
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var newBook = new Book
            {
                Title = "Gospodar Prstenova",
                Isbn = "978-0547928227",
                PublishedOn = new DateTime(1954, 7, 29),
                PageCount = 569,
                AuthorId = 1,
                PublisherId = 1
            };

            // ACT - Izvedite akciju koju testirate
            await repository.CreateAsync(newBook);
            await context.SaveChangesAsync();

            // ASSERT - Provjerite rezultat
            var savedBook = await context.Books
                .FirstOrDefaultAsync(b => b.Title == "Gospodar Prstenova");

            Assert.NotNull(savedBook);
            Assert.Equal("978-0547928227", savedBook.Isbn);
            Assert.Equal(569, savedBook.PageCount);
            Assert.Null(savedBook.DeletedAt);  // Ne smije biti obrisana
        }

        /// <summary>
        /// Test: Ažuriraj postojeću knjižu i provjeri da se promjene spravljaju
        /// 
        /// Scenarij: Administrator mijenja broj stranica knjižu
        /// 1. Učita postojeću knjižu iz baze
        /// 2. Mijenja Pages svojstvo
        /// 3. Sprema promjene
        /// 4. Provjerava da se promjena vidi u bazi
        /// </summary>
        [Fact]
        public async Task UpdateBook_WithNewPageCount_ShouldUpdateDatabase()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            // Dodaj test knjižu
            var book = new Book
            {
                Title = "Hobbit",
                Isbn = "978-0547928186",
                PageCount = 310,
                PublishedOn = new DateTime(1937, 9, 21)
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var bookId = book.Id;

            // ACT - Ažuriraj broj stranica
            var bookToUpdate = await context.Books.FindAsync(bookId);
            bookToUpdate.PageCount = 320;  // Promijenjena vrijednost
            context.Books.Update(bookToUpdate);
            await context.SaveChangesAsync();

            // ASSERT - Provjeri da se promjena vidi
            var updatedBook = await context.Books.FindAsync(bookId);
            Assert.Equal(320, updatedBook.PageCount);
        }

        /// <summary>
        /// Test: Obriši knjižu (soft delete) i provjeri da DeletedAt bude postavljen
        /// 
        /// Scenarij: Administrator briše knjižu iz kataloga
        /// 1. Učita knjižu iz baze
        /// 2. Postavi DeletedAt na trenutno vrijeme
        /// 3. Sprema promjene
        /// 4. Provjerava da obrisana knjižu ne pojavljuje se u listama
        /// </summary>
        [Fact]
        public async Task DeleteBook_SoftDelete_ShouldSetDeletedAtNotNull()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            // Dodaj test knjižu
            var book = new Book
            {
                Title = "1984",
                Isbn = "978-0451524935",
                PageCount = 328
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var bookId = book.Id;

            // ACT - Soft delete (postavi DeletedAt)
            var bookToDelete = await context.Books.FindAsync(bookId);
            bookToDelete.DeletedAt = DateTime.UtcNow;
            context.Books.Update(bookToDelete);
            await context.SaveChangesAsync();

            // ASSERT - Provjeri da se obrisana knjižu ne pojavljuje
            var activeBooks = await context.Books
                .Where(b => b.DeletedAt == null)
                .ToListAsync();

            Assert.DoesNotContain(activeBooks, b => b.Id == bookId);

            // Provjeri da DeletedAt nije null
            var deletedBook = await context.Books.FindAsync(bookId);
            Assert.NotNull(deletedBook.DeletedAt);
        }

        /// <summary>
        /// Test: Validacija obaveznog polja (Title)
        /// 
        /// Scenarij: Korisnik pokušava kreiranu knjižu bez naslova
        /// 1. Kreira entitetu bez Title svojstva
        /// 2. Validacija bi trebala detektirati grešku
        /// 3. Podaci se ne spravljaju u bazu
        /// </summary>
        [Fact]
        public async Task CreateBook_WithoutTitle_ShouldFailValidation()
        {
            // ARRANGE
            var context = GetInMemoryDbContext();
            var book = new Book
            {
                Title = null,  // Nedostaje obavezno polje
                Isbn = "978-0451524935",
                PageCount = 328
            };

            // ACT & ASSERT - Validation će biti provjeravana na model level
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(book);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(book, validationContext, results, true);

            Assert.False(isValid);
            Assert.NotEmpty(results);
        }
    }
}
