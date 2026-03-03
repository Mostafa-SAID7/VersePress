# Deployment Guide

## Azure App Service Deployment

### Prerequisites
- Azure account
- Azure CLI installed
- GitHub repository

### Step 1: Create Azure Resources

```bash
# Login to Azure
az login

# Create resource group
az group create --name versepress-rg --location eastus

# Create App Service plan
az appservice plan create --name versepress-plan --resource-group versepress-rg --sku B1 --is-linux

# Create web app
az webapp create --name versepress-app --resource-group versepress-rg --plan versepress-plan --runtime "DOTNET|9.0"

# Create SQL Server
az sql server create --name versepress-sql --resource-group versepress-rg --location eastus --admin-user sqladmin --admin-password YourPassword123!

# Create SQL Database
az sql db create --resource-group versepress-rg --server versepress-sql --name VersePressDb --service-objective S0

# Configure firewall
az sql server firewall-rule create --resource-group versepress-rg --server versepress-sql --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```

### Step 2: Configure Application Settings

```bash
# Set connection string
az webapp config connection-string set --name versepress-app --resource-group versepress-rg --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:versepress-sql.database.windows.net,1433;Database=VersePressDb;User ID=sqladmin;Password=YourPassword123!;Encrypt=True;TrustServerCertificate=False;"

# Set application settings
az webapp config appsettings set --name versepress-app --resource-group versepress-rg --settings \
  ASPNETCORE_ENVIRONMENT=Production \
  EmailSettings__SmtpServer=smtp.gmail.com \
  EmailSettings__SmtpPort=587 \
  EmailSettings__SenderEmail=your-email@gmail.com \
  EmailSettings__Username=your-email@gmail.com \
  EmailSettings__Password=your-app-password
```

### Step 3: Configure GitHub Actions

1. Get publish profile:
```bash
az webapp deployment list-publishing-profiles --name versepress-app --resource-group versepress-rg --xml
```

2. Add secrets to GitHub repository:
   - Go to Settings > Secrets and variables > Actions
   - Add `AZURE_WEBAPP_PUBLISH_PROFILE` with the XML content
   - Add `AZURE_WEBAPP_NAME` with value `versepress-app`

3. Push to main branch to trigger deployment

### Step 4: Apply Database Migrations

```bash
# From local machine with connection to Azure SQL
dotnet ef database update --project src/VersePress.Infrastructure --startup-project src/VersePress.Web --connection "Server=tcp:versepress-sql.database.windows.net,1433;Database=VersePressDb;User ID=sqladmin;Password=YourPassword123!;"
```

## Manual Deployment

### Step 1: Publish Application

```bash
dotnet publish src/VersePress.Web/VersePress.Web.csproj -c Release -o ./publish
```

### Step 2: Deploy to Azure

```bash
az webapp deployment source config-zip --resource-group versepress-rg --name versepress-app --src publish.zip
```

## Docker Deployment

### Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/VersePress.Web/VersePress.Web.csproj", "src/VersePress.Web/"]
COPY ["src/VersePress.Application/VersePress.Application.csproj", "src/VersePress.Application/"]
COPY ["src/VersePress.Domain/VersePress.Domain.csproj", "src/VersePress.Domain/"]
COPY ["src/VersePress.Infrastructure/VersePress.Infrastructure.csproj", "src/VersePress.Infrastructure/"]
RUN dotnet restore "src/VersePress.Web/VersePress.Web.csproj"
COPY . .
WORKDIR "/src/src/VersePress.Web"
RUN dotnet build "VersePress.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VersePress.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VersePress.Web.dll"]
```

### Build and Run

```bash
docker build -t versepress:latest .
docker run -d -p 8080:80 --name versepress versepress:latest
```

## Environment Configuration

### Production Checklist

- [ ] Update connection string
- [ ] Configure email settings
- [ ] Set ASPNETCORE_ENVIRONMENT to Production
- [ ] Enable HTTPS
- [ ] Configure custom domain
- [ ] Set up SSL certificate
- [ ] Configure Application Insights
- [ ] Set up backup and recovery
- [ ] Configure scaling rules
- [ ] Review security settings
- [ ] Test all functionality

### Security Configuration

```bash
# Enable HTTPS only
az webapp update --name versepress-app --resource-group versepress-rg --https-only true

# Configure custom domain
az webapp config hostname add --webapp-name versepress-app --resource-group versepress-rg --hostname www.yourdomain.com

# Bind SSL certificate
az webapp config ssl bind --certificate-thumbprint <thumbprint> --ssl-type SNI --name versepress-app --resource-group versepress-rg
```

## Monitoring

### Application Insights

```bash
# Create Application Insights
az monitor app-insights component create --app versepress-insights --location eastus --resource-group versepress-rg

# Get instrumentation key
az monitor app-insights component show --app versepress-insights --resource-group versepress-rg --query instrumentationKey

# Configure in app
az webapp config appsettings set --name versepress-app --resource-group versepress-rg --settings APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=<key>"
```

## Scaling

### Manual Scaling

```bash
# Scale up (change plan)
az appservice plan update --name versepress-plan --resource-group versepress-rg --sku P1V2

# Scale out (add instances)
az appservice plan update --name versepress-plan --resource-group versepress-rg --number-of-workers 3
```

### Auto-scaling

```bash
# Enable autoscale
az monitor autoscale create --resource-group versepress-rg --resource versepress-plan --resource-type Microsoft.Web/serverfarms --name versepress-autoscale --min-count 1 --max-count 5 --count 2

# Add CPU rule
az monitor autoscale rule create --resource-group versepress-rg --autoscale-name versepress-autoscale --condition "Percentage CPU > 70 avg 5m" --scale out 1
```

## Backup and Recovery

### Configure Backup

```bash
# Create storage account
az storage account create --name versepressbackup --resource-group versepress-rg --location eastus --sku Standard_LRS

# Configure backup
az webapp config backup create --resource-group versepress-rg --webapp-name versepress-app --container-url <storage-url> --backup-name daily-backup
```

## Troubleshooting

### View Logs

```bash
# Stream logs
az webapp log tail --name versepress-app --resource-group versepress-rg

# Download logs
az webapp log download --name versepress-app --resource-group versepress-rg --log-file logs.zip
```

### Common Issues

**Issue**: Application won't start
- Check application logs
- Verify connection string
- Ensure migrations are applied

**Issue**: Database connection fails
- Check firewall rules
- Verify connection string
- Test connectivity

**Issue**: Performance issues
- Enable Application Insights
- Check resource utilization
- Consider scaling up/out

## Cost Optimization

- Use appropriate App Service plan (B1 for dev, P1V2+ for production)
- Enable auto-scaling to handle traffic spikes
- Use Azure SQL elastic pools for multiple databases
- Configure backup retention policies
- Monitor and optimize resource usage

## Post-Deployment

1. Verify application is running
2. Test all features
3. Check health endpoint: `https://your-app.azurewebsites.net/health`
4. Monitor Application Insights
5. Set up alerts for errors and performance
6. Configure custom domain and SSL
7. Update DNS records
8. Test from different locations
9. Perform load testing
10. Document any issues

## Support

For deployment issues:
- Check Azure documentation
- Review application logs
- Contact Azure support
- Open GitHub issue
