using CatalogoApi.Context;
using CatalogoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.ApiEndpoints;

public static class ProdutosEndpoints
{
    public static void MapProdutosEndpoints(this WebApplication app)
    {
        app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
        {
            db.Produtos.Add(produto);
            await db.SaveChangesAsync();

            return Results.Created($"/produtos/{produto.ProdutoId}", produto);
        })
            .Produces<Produto>(StatusCodes.Status201Created)
            .WithName("CriarNovoProduto")
            .WithTags("Produtos");

        app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync())
            .Produces<List<Produto>>(StatusCodes.Status200OK)
            .WithTags("Produtos");

        app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Produtos.FindAsync(id)
                is Produto produto ? Results.Ok(produto) : Results.NotFound("Produto não encontrado");
        })
            .Produces<Produto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Produtos");

        app.MapPut("/produtos", async (int produtoId, string produtoNome, AppDbContext db) =>
        {
            var produtoDb = db.Produtos.SingleOrDefault(s => s.ProdutoId == produtoId);

            if (produtoDb is null) return Results.NotFound();

            produtoDb.Nome = produtoNome;

            await db.SaveChangesAsync();
            return Results.Ok(produtoDb);
        })
            .Produces<Produto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("AtualizaNomeProduto")
            .WithTags("Produtos");

        app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) =>
        {
            if (produto.ProdutoId != id)
            {
                return Results.BadRequest("Ids não conferem");
            }

            var produtoDb = await db.Produtos.FindAsync(id);

            if (produtoDb is null) return Results.NotFound("Produto não encontrado");

            produtoDb.Nome = produto.Nome;
            produtoDb.Descricao = produto.Descricao;
            produtoDb.Preco = produto.Preco;
            produtoDb.DataCompra = produto.DataCompra;
            produtoDb.Estoque = produto.Estoque;
            produtoDb.Imagem = produto.Imagem;
            produtoDb.CategoriaId = produto.CategoriaId;

            await db.SaveChangesAsync();
            return Results.Ok(produtoDb);
        })
            .Produces<Produto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("AtualizaProduto")
            .WithTags("Produtos");

        app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            var produtoDb = await db.Produtos.FindAsync(id);

            if (produtoDb is null) return Results.NotFound("Produto não encontrado");

            db.Produtos.Remove(produtoDb);
            await db.SaveChangesAsync();
            return Results.Ok(produtoDb);
        })
            .Produces<Produto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeletaProduto")
            .WithTags("Produtos");

        app.MapGet("/produtos/nome/{criterio}", (string criterio, AppDbContext db) =>
        {
            var produtosSelecionados = db.Produtos.Where(x => x.Nome
                                        .ToLower().Contains(criterio.ToLower()))
                                        .ToList();

            return produtosSelecionados.Count > 0
                ? Results.Ok(produtosSelecionados)
                : Results.NotFound(Array.Empty<Produto>());
        })
            .Produces<List<Produto>>(StatusCodes.Status200OK)
            .WithName("ProdutosPorCriterio")
            .WithTags("Produtos");

        app.MapGet("/produtos/pagina", async (int numeroPagina, int tamanhoPagina, AppDbContext db) =>
        await db.Produtos
                        .OrderBy(p => p.ProdutoId)
                        .Skip((numeroPagina - 1) * tamanhoPagina)
                        .Take(tamanhoPagina)
                        .ToListAsync())
            .Produces<List<Produto>>(StatusCodes.Status200OK)
            .WithName("ProdutosPorPagina")
            .WithTags("Produtos");
    }
}
