# Contributing to VersePress

Thank you for your interest in contributing to VersePress! This document provides guidelines and instructions for contributing.

## Code of Conduct

Please read and follow our [Code of Conduct](CODE_OF_CONDUCT.md).

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](https://github.com/yourusername/versepress/issues)
2. If not, create a new issue with:
   - Clear title and description
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots if applicable
   - Environment details (OS, .NET version, browser)

### Suggesting Features

1. Check existing feature requests
2. Create a new issue with:
   - Clear description of the feature
   - Use cases and benefits
   - Possible implementation approach

### Pull Requests

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**
   - Follow the coding standards
   - Write tests for new features
   - Update documentation

4. **Commit your changes**
   ```bash
   git commit -m "Add: Brief description of changes"
   ```
   
   Commit message format:
   - `Add:` for new features
   - `Fix:` for bug fixes
   - `Update:` for updates to existing features
   - `Refactor:` for code refactoring
   - `Docs:` for documentation changes

5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create a Pull Request**
   - Provide a clear description
   - Reference related issues
   - Include screenshots for UI changes

## Development Setup

See [PROJECT_SETUP.md](PROJECT_SETUP.md) for detailed setup instructions.

## Coding Standards

### C# Guidelines
- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods small and focused
- Use async/await for I/O operations

### JavaScript Guidelines
- Use ES6+ features
- Follow consistent naming conventions
- Add JSDoc comments for functions
- Use strict mode
- Handle errors appropriately

### CSS Guidelines
- Use CSS variables for theming
- Follow BEM naming convention
- Keep selectors specific but not overly complex
- Use mobile-first approach

## Testing

- Write unit tests for new features
- Ensure all tests pass before submitting PR
- Aim for 80%+ code coverage
- Test both English and Arabic content

```bash
dotnet test
```

## Documentation

- Update README.md if adding new features
- Add/update XML comments for public APIs
- Update relevant documentation in `/docs`
- Include code examples where appropriate

## Review Process

1. Automated checks must pass (build, tests, linting)
2. Code review by maintainers
3. Address feedback and make requested changes
4. Approval and merge by maintainers

## Questions?

Feel free to ask questions by:
- Opening an issue
- Joining our discussions
- Contacting maintainers

Thank you for contributing to VersePress! 🎉
