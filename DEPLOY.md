# 🚀 Deploy na Azure - Upute

## Preduvjeti

- Azure subscription (besplatan Azure Free Tier dostatan)
- Docker (za lokalno testiranje)
- Docker Hub račun (za image repository)
- GitHub Actions (već uključen u GitHub)

---

## 🐳 Lokalno testiranje sa Docker

### 1. Build Docker image

```bash
docker build -t book-review-app:latest .
```

### 2. Pokreni sa docker-compose

```bash
docker-compose up -d
```

Aplikacija će biti dostupna na `http://localhost:8080`

### 3. Provjeri logs

```bash
docker-compose logs -f app
```

### 4. Zaustavi kontejner

```bash
docker-compose down
```

---

## ☁️ Deploy na Azure Web App

### 1. Kreiraj Azure Web App

```bash
# Login u Azure
az login

# Kreiraj Resource Group
az group create --name BookReviewRG --location eastus

# Kreiraj App Service Plan
az appservice plan create \
  --name BookReviewPlan \
  --resource-group BookReviewRG \
  --sku B1 \
  --is-linux

# Kreiraj Web App
az webapp create \
  --resource-group BookReviewRG \
  --plan BookReviewPlan \
  --name book-review-app-<unique-suffix> \
  --runtime "DOTNETCORE:9.0"
```

### 2. Konfigurira GitHub Secrets

U GitHub repository → Settings → Secrets, dodaj:

```
DOCKER_USERNAME = tvoj-docker-hub-username
DOCKER_PASSWORD = tvoj-docker-hub-token
AZURE_APP_NAME = book-review-app-<unique-suffix>
AZURE_PUBLISH_PROFILE = <download sa Azure portala>
```

**Kako preuzeti Publish Profile:**
1. Otvori Azure Portal
2. Pronađi Web App
3. Download publish profile gumb (obično gore desno)
4. Kopiraj sadržaj datoteke u GitHubSecret

### 3. Postavi Environment Variables u Azure

```bash
az webapp config appsettings set \
  --resource-group BookReviewRG \
  --name book-review-app-<unique-suffix> \
  --settings \
  ASPNETCORE_ENVIRONMENT=Production \
  Anthropic__ApiKey=<tvoj-anthropic-api-key> \
  Authentication__Google__ClientId=<google-client-id> \
  Authentication__Google__ClientSecret=<google-client-secret>
```

### 4. Database Storage

SQLite baza će se koristiti sa `/app/data/` volumenom. Azure će čuvati Logs folder na `/app/Logs/`.

---

## 📊 CI/CD Pipeline (GitHub Actions)

Kada pushaš na `main` branch, GitHub Actions će:

1. ✅ Checkout kod
2. ✅ Setup .NET 9
3. ✅ Restore dependencies
4. ✅ Build aplikaciju
5. ✅ Pokrenuti testove (156+ testova)
6. ✅ Publish aplikaciju
7. ✅ Build Docker image
8. ✅ Push na Docker Hub
9. ✅ Deploy na Azure Web App

**Status:** Vidi Actions tab na GitHubu

---

## 🔍 Monitoring i Logs

### Azure Portal Logs
```bash
az webapp log tail \
  --resource-group BookReviewRG \
  --name book-review-app-<unique-suffix>
```

### Lokalni Logs (Docker)
```bash
docker-compose logs -f app | grep "Logs/app-"
```

---

## 📈 Azure DevOps Pipeline (Alternativa)

Ako koristiš Azure DevOps umjesto GitHub Actions:

```bash
az devops configure --defaults organization=https://dev.azure.com/<org> project=<project>
az pipelines create --name "Book Review Deploy" --repository <repo-url> --branch main --file .azure/deploy.yml
```

---

## ⚠️ Troubleshooting

### Docker build fails
```bash
docker build --no-cache -t book-review-app:latest .
```

### Port conflicts
```bash
docker-compose down -v  # Uključi volume cleanup
```

### Database migration issues
```bash
# Prisilna migracija
dotnet ef database drop --force
dotnet ef database update
```

### Azure deployment timeouts
- Provjeri App Service Plan SKU (minimalno B1)
- Provjeri Network security groups
- Provjeri Application Insights za greške

---

## 💾 Backup Database

SQLite baza je u `/app/data/catalog.db`

```bash
# Backup
docker-compose exec app cp /app/data/catalog.db /app/data/catalog.backup.db

# Ili sa Azure
az webapp deployment source config-zip \
  --resource-group BookReviewRG \
  --name book-review-app-<suffix> \
  --src backup.zip
```

---

## 🎯 Checklist Pre Produkcije

- [ ] Kreiraj Azure Web App
- [ ] Postavi GitHub Secrets
- [ ] Testiraj lokalno sa docker-compose
- [ ] Dodaj Anthropic API key
- [ ] Dodaj Google OAuth credentials
- [ ] Testiraj CI/CD pipeline
- [ ] Provjeri Logs nakon prvog deploita
- [ ] Testiraj production aplikaciju
- [ ] Setup monitoring i alerts

---

## 🔑 Dodatni Resursi

- [Azure App Service Docs](https://learn.microsoft.com/en-us/azure/app-service/)
- [Docker ASP.NET 9 Guide](https://github.com/dotnet/dotnet-docker)
- [GitHub Actions](https://docs.github.com/en/actions)
- [Azure DevOps Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/)

---

## 🆘 Help

Ako nešto ne valja:
1. Provjeri GitHub Actions logs
2. Provjeri Azure Application Insights
3. Provjeri Docker Compose logs
4. Čitaj error poruke u konsoli

