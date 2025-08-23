# General Guidelines

- Write comments and documentation to explain complex logic. Comments should explain why something is done, not just what is done.
- Put documentation in the `docs` folder, regardless of its format (Markdown, HTML, etc.).
- Prefer markdown for documentation files to take advantage of its simplicity and readability. 
  - If other formats are more suitable for specific content (charts, diagrams, complex tables, etc.), they may be used.

# Coding Guidelines

- Prioritize code readability and maintainability.
- Follow the SOLID principles of object-oriented design.
- Keep methods small and focused on a single responsibility.
- Use meaningful names for variables, methods, and classes.
- Use expression-bodied members for simple properties and methods to improve readability.
- Use `var` for local variable declarations when the type is obvious from the right-hand side of the assignment. Otherwise, use explicit types.
- Use `async` and `await` for asynchronous programming to improve responsiveness and scalability.
- Use `nameof` operator instead of hard-coded strings for member names.
- Use `string interpolation` instead of `string.Format` for better readability.
- Use `null-coalescing operator` (`??`) and `null-conditional operator` (`?.`) to simplify null checks.
- Use `switch` expressions for complex conditional logic to improve readability.
- Use `record` types for immutable data structures and DTOs.
- Use `readonly` aggressively for fields that should not change after initialization. It can always be removed later.
- Use `const` for compile-time constants and `readonly` for runtime constants.
- Use `IEnumerable<T>` for method return types when the caller does not need to modify the collection. Use `List<T>` or other concrete types when modification is required.
- Use `is` pattern matching for type checks and casting to improve readability.
- Use `using` statements for disposable resources to ensure proper cleanup.
- Append `()` on new object instantiations when there are no parameters.
- Avoid using `this.` unless necessary for disambiguation.

# Unit Testing Guidelines

- Use the `Xunit` testing framework for unit tests.
- Use the `Moq` library for mocking dependencies in tests.
- Use the `Shouldly` library for assertions in tests.

- Each test method should be decorated with the `[Fact]` attribute for simple tests or `[Theory]` with `[InlineData]` for parameterized tests.
- Group related tests into test classes that correspond to the class or functionality being tested. Each test class should be named after the class it tests, with a `_Tests` suffix (e.g., `UserService_Tests` for testing `UserService`).
- Use descriptive names for test methods that clearly indicate what is being tested and the expected outcome. Underscore `_` can be used to separate different parts of the name for better readability.

- Tests should be structured in one of the two following ways:
  - Arrange, Act, Assert (AAA) pattern should be followed for unit testsin each test method to structure the code clearly.
  - Given, When, Then pattern can be used for better readability in complex scenarios, particularly in behavior-driven development (BDD) style tests, or integration tests.
- Use Shouldly assertions to verify the expected outcomes. 
  - For example, use `result.ShouldBe(expectedValue)` instead of `Assert.Equal(expectedValue, result)`.
- Multiple assertions in a single test are okay as long as they are testing the same behavior. Each test should ideally verify one specific behavior or outcome.
- Ensure that tests are isolated and do not depend on external systems or state. Use mocking to simulate interactions with dependencies.
- Avoid using hard-coded values in tests. Instead, use constants or variables to improve maintainability. Values should not be shared between test classes unless they are in a common base class.
- When mocking dependencies with Moq, set up the mock behavior clearly and verify interactions where necessary.
- When testing for exceptions, use the `Should.Throw<TException>` method to assert that the expected exception is thrown.
- Aim for high code coverage, but prioritize meaningful tests that validate the behavior of the code over achieving a specific coverage percentage.
- Regularly review and refactor tests to maintain clarity and effectiveness as the codebase evolves.
- Ensure that tests are fast and efficient. Avoid long-running operations or unnecessary delays in test execution
- Use comments sparingly in tests. The test code itself should be clear enough to convey its purpose without extensive comments.

**Important: All tests must use the Shouldly and Moq libraries.**

Example Test

```csharp
[Fact]
public void Should_Return_All_Registered_Providers()
{
    // Arrange
    var mockProvider1 = new Mock<IMailingListProvider>();
    mockProvider1.Setup(p => p.ProviderName).Returns("Provider1");

    var mockProvider2 = new Mock<IMailingListProvider>();
    mockProvider2.Setup(p => p.ProviderName).Returns("Provider2");

    var serviceCollection = new ServiceCollection();
    serviceCollection.AddTransient(_ => mockProvider1.Object);
    serviceCollection.AddTransient(_ => mockProvider2.Object);

    var serviceProvider = serviceCollection.BuildServiceProvider();

    var discoveryService = new MailingListProviderDiscoveryService(serviceProvider);

    // Act
    var providers = discoveryService.GetAvailableProviders();

    // Assert
    providers.ShouldContain("Provider1");
    providers.ShouldContain("Provider2");
    providers.Count.ShouldBe(2);
}
```

# Integration Testing Guidelines

Integration tests can be placed in abstract base classes if they share common setup or utility methods, or if there are multiple implementations of an interface or service to be tested. For example, many services depend on a repository interface. You can create an abstract test class that defines the tests the repository should pass. Then create concrete test classes for each implementation of the repository interface in the appropriate test project. For example, define the tests in the `RepositoryTests` class in the `DomainTests` project. Then in an `EntityFrameworkCoreTests` project, inherit from the `RepositoryTests` class to reuse the tests. Do the same for `MongoDb`, etc.

```csharp
public abstract class RepositoryTests
{
    protected abstract IRepository<User> GetRepository();

    [Fact]
    public void Add_Should_Add_User()
    {
        // Arrange
        var repository = GetRepository();
        var user = new User { Id = 1, Name = "Test" };

        // Act
        repository.Add(user);
        var result = repository.GetById(1);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test");
    }

    // Add more shared tests here...
}
```

Entity Framework Core Tests

```csharp
public class EfRepositoryTests : RepositoryTests
{
    protected override IRepository<User> GetRepository()
        => new EfRepository<User>(/* dbContext setup */);
}
```

MongoDb Tests

```csharp
public class MongoDbRepositoryTests : RepositoryTests
{
    protected override IRepository<User> GetRepository()
        => new MongoDbRepository<User>(/* mongoClient setup */);
}
```


Example Test:

