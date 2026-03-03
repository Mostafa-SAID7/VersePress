# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

We take the security of VersePress seriously. If you discover a security vulnerability, please follow these steps:

### 1. Do Not Disclose Publicly

Please do not create a public GitHub issue for security vulnerabilities.

### 2. Report Privately

Send an email to: **security@versepress.com**

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

### 3. Response Timeline

- **Initial Response**: Within 48 hours
- **Status Update**: Within 7 days
- **Fix Timeline**: Depends on severity
  - Critical: 1-7 days
  - High: 7-14 days
  - Medium: 14-30 days
  - Low: 30-90 days

### 4. Disclosure Policy

- We will acknowledge your report within 48 hours
- We will provide regular updates on our progress
- We will notify you when the vulnerability is fixed
- We will credit you in the security advisory (unless you prefer to remain anonymous)

## Security Measures

### Authentication & Authorization
- ASP.NET Core Identity for user management
- Role-based access control (Admin, Author)
- Secure password requirements:
  - Minimum 8 characters
  - Requires digit, uppercase, lowercase, and special character
- Account lockout after 5 failed attempts (15-minute lockout)
- Cookie-based authentication with sliding expiration (14 days)

### Input Validation
- FluentValidation for all user inputs
- XSS protection with HTML encoding
- SQL injection protection with parameterized queries
- CSRF protection with anti-forgery tokens
- Input sanitization for all user-generated content

### Rate Limiting
- Contact form: 3 submissions per hour per IP address
- Prevents brute force attacks
- Prevents spam submissions

### Data Protection
- HTTPS enforcement in production
- Secure cookie settings (HttpOnly, Secure, SameSite)
- Password hashing with ASP.NET Core Identity (PBKDF2)
- Sensitive data never logged
- Environment variables for secrets in production

### Database Security
- Parameterized queries prevent SQL injection
- Soft delete pattern (data never truly deleted)
- Connection string encryption
- Principle of least privilege for database access

### Logging & Monitoring
- Serilog for structured logging
- Security events logged (login attempts, failures)
- No sensitive data in logs
- Log rotation and retention policies

### Dependencies
- Regular dependency updates
- Automated security scanning (GitHub Dependabot)
- Only trusted NuGet packages
- Minimal external dependencies

### Headers & CORS
- Security headers configured
- CORS policy for SignalR
- Content Security Policy (CSP)
- X-Frame-Options
- X-Content-Type-Options

## Best Practices for Deployment

### Production Checklist
- [ ] Use HTTPS only
- [ ] Set secure connection strings via environment variables
- [ ] Enable Application Insights or similar monitoring
- [ ] Configure firewall rules
- [ ] Use Azure Key Vault for secrets
- [ ] Enable Azure DDoS protection
- [ ] Configure backup and disaster recovery
- [ ] Set up alerts for security events
- [ ] Review and update dependencies regularly
- [ ] Perform security audits periodically

### Environment Variables

Never commit these to source control:
```
ConnectionStrings__DefaultConnection
EmailSettings__Username
EmailSettings__Password
APPLICATIONINSIGHTS_CONNECTION_STRING
```

### Secure Configuration

In production, use:
- Azure Key Vault for secrets
- Managed Identity for Azure resources
- Environment variables for configuration
- Encrypted connection strings

## Known Security Considerations

### Email Service
- SMTP credentials stored in configuration
- Use app-specific passwords for Gmail
- Consider using SendGrid or similar service in production

### Session Management
- Sessions expire after 14 days of inactivity
- Sliding expiration extends session on activity
- Logout clears all session data

### File Uploads
- Currently not implemented
- When implemented, will include:
  - File type validation
  - Size limits
  - Virus scanning
  - Secure storage

## Security Updates

We regularly update dependencies and apply security patches. Subscribe to our releases to stay informed about security updates.

## Compliance

VersePress follows:
- OWASP Top 10 security practices
- Microsoft Security Development Lifecycle (SDL)
- GDPR principles for data protection
- Accessibility standards (WCAG 2.1 Level AA)

## Contact

For security concerns: security@versepress.com
For general inquiries: support@versepress.com

## Acknowledgments

We appreciate responsible disclosure and will acknowledge security researchers who help improve VersePress security.
