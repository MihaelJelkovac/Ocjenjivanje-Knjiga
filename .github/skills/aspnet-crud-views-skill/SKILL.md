# ASP.NET CRUD Views Skill — Lab-4 Create/Edit Forms

**Svrha**: Generirati sve Create/Edit view datoteke s validacijom, datumskom kontrolom kao partial view, AJAX autocomplete dropdownima, statičkim enum dropdownima i AJAX pretragom u Index viewima prema Lab4.md zahtjevima.

## Input Parametri

- `entityName` — Naziv entiteta (Author, Book, Genre, Publisher, Review, User)
- `properties` — Lista svojstava s tipovima i validacijama
- `foreignKeys` — Lista FK polja (npr. AuthorId, PublisherId)
- `enumFields` — Lista enum polja (npr. Status, Sentiment)
- `dateTimeFields` — Lista DateTime polja koja trebaju datumsku kontrolu

## Output 1 — Partial Views (Shared)

### `_AutocompleteDropdown.cshtml`

```html
@{
    string fieldName = ViewBag.FieldName ?? "id";
    string displayText = ViewBag.DisplayText ?? "Display";
    string placeholder = ViewBag.Placeholder ?? "Počni pisati...";
    string searchUrl = ViewBag.SearchUrl ?? "/api/search";
}

<div class="autocomplete-wrapper">
    <input type="text" 
           id="@fieldName-search" 
           class="form-control autocomplete-input"
           placeholder="@placeholder"
           data-search-url="@searchUrl"
           data-field-name="@fieldName"
           data-display-text="@displayText" />
    
    <input type="hidden" id="@fieldName-id" />
    
    <ul class="autocomplete-results" id="@fieldName-results" style="display: none;">
        <!-- Rezultati će biti dinamički dodani JavaScript-om -->
    </ul>
</div>

@section Scripts {
    <script type="text/javascript">
        $(document).ready(function () {
            const fieldName = "@fieldName";
            const searchUrl = "@searchUrl";
            
            const $input = $(`#${fieldName}-search`);
            const $resultsList = $(`#${fieldName}-results`);
            const $hiddenInput = $(`#${fieldName}-id`);
            
            let searchTimeout;
            
            $input.on("keyup", function () {
                const query = $(this).val().trim();
                
                clearTimeout(searchTimeout);
                
                if (query.length < 2) {
                    $resultsList.hide();
                    return;
                }
                
                searchTimeout = setTimeout(function () {
                    $.ajax({
                        url: searchUrl,
                        type: "GET",
                        data: { query: query },
                        success: function (data) {
                            $resultsList.empty();
                            
                            if (!data || data.length === 0) {
                                $resultsList.append('<li class="no-results">Nema rezultata</li>');
                                $resultsList.show();
                                return;
                            }
                            
                            data.forEach(function (item) {
                                const $li = $(`<li data-id="${item.id}" class="autocomplete-item">${item.text}</li>`);
                                $li.on("click", function () {
                                    $input.val(item.text);
                                    $hiddenInput.val(item.id);
                                    $resultsList.hide();
                                });
                                $resultsList.append($li);
                            });
                            
                            $resultsList.show();
                        },
                        error: function () {
                            $resultsList.empty().append('<li class="error">Greška pri pretrazi</li>').show();
                        }
                    });
                }, 300);
            });
            
            $(document).on("click", function (e) {
                if (!$(e.target).closest(".autocomplete-wrapper").length) {
                    $resultsList.hide();
                }
            });
        });
    </script>
}

<style>
    .autocomplete-wrapper {
        position: relative;
        width: 100%;
    }
    
    .autocomplete-input {
        width: 100%;
    }
    
    .autocomplete-results {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
        list-style: none;
        margin: 0;
        padding: 0;
        border: 1px solid #ddd;
        border-top: none;
        background: white;
        max-height: 300px;
        overflow-y: auto;
        z-index: 1000;
    }
    
    .autocomplete-item {
        padding: 10px;
        cursor: pointer;
        transition: background-color 0.2s;
    }
    
    .autocomplete-item:hover {
        background-color: #f5f5f5;
    }
    
    .no-results,
    .error {
        padding: 10px;
        color: #999;
        font-style: italic;
    }
</style>
```

### `_DateTimeControl.cshtml`

```html
@{
    string fieldName = ViewBag.FieldName ?? "dateTime";
    DateTime? value = ViewBag.Value as DateTime?;
    string dateFormat = System.Globalization.CultureInfo.CurrentCulture.Name == "hr" ? "dd.MM.yyyy" : "MM/dd/yyyy";
    string timeFormat = "HH:mm";
}

<div class="datetime-control">
    <div class="row g-2">
        <div class="col-md-6">
            <input type="text" 
                   id="@fieldName-date" 
                   class="form-control datetime-input"
                   placeholder="@dateFormat"
                   value="@(value?.ToString(dateFormat) ?? "")"
                   data-date-format="@dateFormat"
                   data-field-name="@fieldName" />
        </div>
        <div class="col-md-6">
            <input type="text" 
                   id="@fieldName-time" 
                   class="form-control datetime-input"
                   placeholder="@timeFormat"
                   value="@(value?.ToString(timeFormat) ?? "")"
                   data-time-format="@timeFormat"
                   data-field-name="@fieldName" />
        </div>
    </div>
    <input type="hidden" id="@fieldName-combined" name="@fieldName" />
</div>

@section Scripts {
    <script type="text/javascript">
        $(document).ready(function () {
            const fieldName = "@fieldName";
            const dateFormat = "@dateFormat";
            const timeFormat = "@timeFormat";
            
            const $dateInput = $(`#${fieldName}-date`);
            const $timeInput = $(`#${fieldName}-time`);
            const $combinedInput = $(`#${fieldName}-combined`);
            
            function updateCombinedValue() {
                const dateStr = $dateInput.val();
                const timeStr = $timeInput.val();
                
                if (dateStr && timeStr) {
                    // Konjugacija datuma i vremena
                    const dateObj = new Date(dateStr.split('.').reverse().join('-'));
                    if (!isNaN(dateObj)) {
                        const [hours, minutes] = timeStr.split(':');
                        dateObj.setHours(parseInt(hours), parseInt(minutes), 0);
                        $combinedInput.val(dateObj.toISOString());
                    }
                }
            }
            
            $dateInput.on("change", updateCombinedValue);
            $timeInput.on("change", updateCombinedValue);
            
            // Validacija datuma
            $dateInput.on("blur", function () {
                const value = $(this).val();
                if (value) {
                    const parts = value.split('.');
                    if (parts.length === 3) {
                        const day = parseInt(parts[0]);
                        const month = parseInt(parts[1]);
                        const year = parseInt(parts[2]);
                        
                        const date = new Date(year, month - 1, day);
                        if (isNaN(date.getTime()) || date.getDate() !== day) {
                            $(this).addClass("is-invalid").siblings(".invalid-feedback").text("Neispravna vrijednost datuma");
                            $combinedInput.val("");
                            return;
                        }
                        
                        $(this).removeClass("is-invalid");
                    }
                }
            });
        });
    </script>
}

<style>
    .datetime-control {
        width: 100%;
    }
    
    .datetime-input.is-invalid {
        border-color: #dc3545;
    }
</style>
```

## Output 2 — Create/Edit Forms (_CreateOrEdit Partial)

### `Authors/_CreateOrEdit.cshtml`

```html
@model Author

<div class="form-container">
    @if (Model?.Id > 0)
    {
        <h2>Ažuriranje Autora</h2>
    }
    else
    {
        <h2>Dodaj Novog Autora</h2>
    }

    <form asp-action="@(Model?.Id > 0 ? "Edit" : "Create")" 
          method="post" 
          class="needs-validation"
          novalidate>
        
        @if (Model?.Id > 0)
        {
            <input type="hidden" asp-for="Id" />
        }

        <div class="form-group mb-3">
            <label asp-for="FirstName" class="form-label"></label>
            <input asp-for="FirstName" class="form-control" />
            <span asp-validation-for="FirstName" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="LastName" class="form-label"></label>
            <input asp-for="LastName" class="form-control" />
            <span asp-validation-for="LastName" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Biography" class="form-label"></label>
            <textarea asp-for="Biography" class="form-control" rows="4"></textarea>
            <span asp-validation-for="Biography" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="BirthDate" class="form-label"></label>
            @{
                ViewBag.FieldName = nameof(Author.BirthDate);
                ViewBag.Value = Model?.BirthDate;
            }
            <partial name="_DateTimeControl" />
            <span asp-validation-for="BirthDate" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Nationality" class="form-label"></label>
            <input asp-for="Nationality" class="form-control" />
            <span asp-validation-for="Nationality" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Website" class="form-label"></label>
            <input asp-for="Website" class="form-control" />
            <span asp-validation-for="Website" class="text-danger small"></span>
        </div>

        <div class="form-group">
            <button type="submit" class="btn btn-primary">Spremi</button>
            <a asp-action="Index" class="btn btn-secondary">Odustani</a>
        </div>
    </form>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
    <script>
        $(document).ready(function () {
            // Client-side validacija
            const form = document.querySelector('.needs-validation');
            
            form.addEventListener('submit', function (e) {
                // Validacija se okida automatski kroz HTML5 atribute
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    </script>
}
```

### `Books/_CreateOrEdit.cshtml`

```html
@model Book

<div class="form-container">
    @if (Model?.Id > 0)
    {
        <h2>Ažuriranje Knjige</h2>
    }
    else
    {
        <h2>Dodaj Novu Knjigu</h2>
    }

    <form asp-action="@(Model?.Id > 0 ? "Edit" : "Create")" 
          method="post" 
          class="needs-validation"
          novalidate>
        
        @if (Model?.Id > 0)
        {
            <input type="hidden" asp-for="Id" />
        }

        <div class="form-group mb-3">
            <label asp-for="Title" class="form-label"></label>
            <input asp-for="Title" class="form-control" />
            <span asp-validation-for="Title" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Isbn" class="form-label"></label>
            <input asp-for="Isbn" class="form-control" />
            <span asp-validation-for="Isbn" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Description" class="form-label"></label>
            <textarea asp-for="Description" class="form-control" rows="4"></textarea>
            <span asp-validation-for="Description" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="PublishedOn" class="form-label">Datum Objavljivanja</label>
            @{
                ViewBag.FieldName = nameof(Book.PublishedOn);
                ViewBag.Value = Model?.PublishedOn;
            }
            <partial name="_DateTimeControl" />
            <span asp-validation-for="PublishedOn" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="PageCount" class="form-label"></label>
            <input asp-for="PageCount" class="form-control" type="number" />
            <span asp-validation-for="PageCount" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Language" class="form-label"></label>
            <input asp-for="Language" class="form-control" />
            <span asp-validation-for="Language" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label asp-for="Status" class="form-label"></label>
            <select asp-for="Status" asp-items="ViewBag.Statuses" class="form-select"></select>
            <span asp-validation-for="Status" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label for="author-search" class="form-label">Autor</label>
            @{
                ViewBag.FieldName = "AuthorId";
                ViewBag.DisplayText = Model?.Author?.FirstName + " " + Model?.Author?.LastName;
                ViewBag.Placeholder = "Pretraži autore...";
                ViewBag.SearchUrl = "/Authors/Search";
            }
            <partial name="_AutocompleteDropdown" />
            <span asp-validation-for="AuthorId" class="text-danger small"></span>
        </div>

        <div class="form-group mb-3">
            <label for="publisher-search" class="form-label">Izdavač</label>
            @{
                ViewBag.FieldName = "PublisherId";
                ViewBag.DisplayText = Model?.Publisher?.Name;
                ViewBag.Placeholder = "Pretraži izdavače...";
                ViewBag.SearchUrl = "/Publishers/Search";
            }
            <partial name="_AutocompleteDropdown" />
            <span asp-validation-for="PublisherId" class="text-danger small"></span>
        </div>

        <div class="form-group">
            <button type="submit" class="btn btn-primary">Spremi</button>
            <a asp-action="Index" class="btn btn-secondary">Odustani</a>
        </div>
    </form>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
    <script>
        $(document).ready(function () {
            const form = document.querySelector('.needs-validation');
            
            form.addEventListener('submit', function (e) {
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    </script>
}
```

## Output 3 — Create/Edit Views

### `Authors/Create.cshtml` i `Authors/Edit.cshtml`

```html
@{
    ViewData["Title"] = Model?.Id > 0 ? "Ažuriranje Autora" : "Dodaj Autora";
}

<div class="container mt-4">
    <partial name="_CreateOrEdit" model="Model" />
</div>
```

## Output 4 — Index View s AJAX Pretragom

### `Authors/Index.cshtml`

```html
@model IReadOnlyList<Author>

@{
    ViewData["Title"] = "Autori";
}

<div class="container mt-4">
    <div class="row mb-4">
        <div class="col-md-6">
            <h2>Popis Autora</h2>
        </div>
        <div class="col-md-6 text-end">
            <a asp-action="Create" class="btn btn-primary">+ Dodaj Autora</a>
        </div>
    </div>

    <div class="row mb-4">
        <div class="col-md-6">
            <input type="text" 
                   id="search-input" 
                   class="form-control" 
                   placeholder="Pretraži autore..." />
        </div>
    </div>

    <div id="results-container">
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Ime</th>
                    <th>Prezime</th>
                    <th>Nacionalnost</th>
                    <th>Akcije</th>
                </tr>
            </thead>
            <tbody id="authors-table">
                @foreach (var author in Model)
                {
                    <tr data-author-id="@author.Id" class="author-row">
                        <td>@author.FirstName</td>
                        <td>@author.LastName</td>
                        <td>@author.Nationality</td>
                        <td>
                            <a asp-action="Details" asp-route-id="@author.Id" class="btn btn-sm btn-info">Detalji</a>
                            <a asp-action="Edit" asp-route-id="@author.Id" class="btn btn-sm btn-warning">Uredi</a>
                            <button type="button" 
                                    class="btn btn-sm btn-danger delete-btn" 
                                    data-id="@author.Id"
                                    data-name="@author.FirstName @author.LastName">
                                Obriši
                            </button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

@section Scripts {
    <script>
        $(document).ready(function () {
            const $searchInput = $("#search-input");
            const $authorsTable = $("#authors-table");
            let searchTimeout;
            
            // AJAX pretraga
            $searchInput.on("keyup", function () {
                const query = $(this).val().trim();
                
                clearTimeout(searchTimeout);
                
                searchTimeout = setTimeout(function () {
                    $.ajax({
                        url: "/Authors/Search",
                        type: "GET",
                        data: { query: query },
                        success: function (data) {
                            $authorsTable.empty();
                            
                            if (!data || data.length === 0) {
                                $authorsTable.html('<tr><td colspan="4" class="text-center">Nema rezultata</td></tr>');
                                return;
                            }
                            
                            data.forEach(function (author) {
                                const row = `<tr data-author-id="${author.id}" class="author-row">
                                    <td>${author.firstName}</td>
                                    <td>${author.lastName}</td>
                                    <td>${author.nationality}</td>
                                    <td>
                                        <a href="/Authors/${author.id}" class="btn btn-sm btn-info">Detalji</a>
                                        <a href="/Authors/Edit/${author.id}" class="btn btn-sm btn-warning">Uredi</a>
                                        <button type="button" class="btn btn-sm btn-danger delete-btn" data-id="${author.id}" data-name="${author.firstName} ${author.lastName}">Obriši</button>
                                    </td>
                                </tr>`;
                                $authorsTable.append(row);
                            });
                            
                            // Re-bind delete buttons
                            bindDeleteButtons();
                        }
                    });
                }, 300);
            });
            
            function bindDeleteButtons() {
                $(".delete-btn").on("click", function () {
                    const $btn = $(this);
                    const id = $btn.data("id");
                    const name = $btn.data("name");
                    
                    if (confirm(`Jeste li sigurni da želite obrisati "${name}"?`)) {
                        $.ajax({
                            url: `/Authors/Delete/${id}`,
                            type: "POST",
                            success: function (response) {
                                if (response.success) {
                                    // Fade out animation
                                    $(`tr[data-author-id="${id}"]`).fadeOut(300, function () {
                                        $(this).remove();
                                    });
                                } else {
                                    alert(response.message || "Greška pri brisanju");
                                }
                            },
                            error: function () {
                                alert("Greška pri komunikaciji sa serverom");
                            }
                        });
                    }
                });
            }
            
            // Inicijalna bind
            bindDeleteButtons();
        });
    </script>
}
```

## Kontroler Search Akcija (Potrebna za Autocomplete)

```csharp
[HttpGet]
[Route("search")]
public async Task<IActionResult> Search(string query)
{
    var results = await _repository.SearchAsync(query);
    
    var data = results.Select(a => new 
    { 
        id = a.Id, 
        text = $"{a.FirstName} {a.LastName}"
    }).ToList();
    
    return Json(data);
}
```

## Važne Napomene

1. **Partial Views**: `_AutocompleteDropdown.cshtml` i `_DateTimeControl.cshtml` trebaju biti u `Views/Shared/`
2. **DateTime Format**: Trebam koristiti `CultureInfo.CurrentCulture` za detektiranje jezika
3. **Validacija**: `asp-validation-for` je obavezna za svako polje
4. **AJAX**: `SearchAsync` metoda trebam u kontroleru za autocomplete
5. **Soft Delete**: Delete akcija vraća JSON s success/message
6. **CSS**: Trebam CSS za autocomplete rezultate i validacijske stilove

## Verifikacija

- [ ] Create/Edit forme se prikazuju
- [ ] Datumska kontrola radi za beide datume
- [ ] Autocomplete dropdown šalje AJAX zahtjev
- [ ] Validacijske poruke se prikazuju
- [ ] Index pretraga radi bez page reloada
- [ ] Delete dugme prikazuje modal potvrde
- [ ] Fade-out animacija radi pri brisanju
