using MyFirstWebApi.Exceptions;
using MyFirstWebApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var books = new List<Book>
{
    new Book { Id = 1, Name = "Mockingbird", Price = 499 },
    new Book { Id = 2, Name = "Way of kings", Price = 699 },
    new Book { Id = 3, Name = "Hobbit", Price = 599 }
};
var nextId = 1;

// Global felhanterare
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (NotFoundException ex)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (AlreadyLoanedException ex)
    {
        context.Response.StatusCode = 409;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Något gick fel.", details = ex.Message });
    }
});

// Hämtar alla böcker
app.MapGet("/books", () => Results.Ok(books));

// Hämtar en specifik bok
app.MapGet("/books/{id}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id)
        ?? throw new NotFoundException($"Bok med id {id} hittades inte.");

    return Results.Ok(book);
});

// Skapar en ny bok
app.MapPost("/books", (Book book) =>
{
    book.Id = nextId++;
    books.Add(book);
    return Results.Created($"/books/{book.Id}", book);
});

// Uppdaterar en bok
app.MapPut("/books/{id}", (int id, Book updatedBook) =>
{
    var book = books.FirstOrDefault(b => b.Id == id)
        ?? throw new NotFoundException($"Bok med id {id} hittades inte.");

    book.Name = updatedBook.Name;
    book.Price = updatedBook.Price;
    return Results.Ok(book);
});

// Låna en bok
app.MapPut("/books/{id}/loan", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id)
        ?? throw new NotFoundException($"Bok med id {id} hittades inte.");

    if (book.IsLoaned)
        throw new AlreadyLoanedException($"Bok med id {id} är redan utlånad.");

    book.IsLoaned = true;
    return Results.Ok(book);
});

// Lämna tillbaka en bok
app.MapPut("/books/{id}/return", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id)
        ?? throw new NotFoundException($"Bok med id {id} hittades inte.");

    if (!book.IsLoaned)
        throw new AlreadyLoanedException($"Bok med id {id} är inte utlånad.");

    book.IsLoaned = false;
    return Results.Ok(book);
});

// Ta bort en bok
app.MapDelete("/books/{id}", (int id) =>
{
    var book = books.FirstOrDefault(b => b.Id == id)
        ?? throw new NotFoundException($"Bok med id {id} hittades inte.");

    books.Remove(book);
    return Results.NoContent();
});

app.Run();
