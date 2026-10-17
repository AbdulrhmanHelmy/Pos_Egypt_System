using HeCashier.Data;
using HeCashier.Models;
using HeCashier.Services;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

public sealed class CategoryAndUnitService(IDbContextFactory<HeCashierDbContext> factory) : ICategoryAndUnitService
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Categories.OrderBy(x => x.Name).AsNoTracking().ToListAsync();
    }

    public async Task<Category> AddCategoryAsync(string name)
    {
        var normalized = NormalizeName(name);
        await using var db = await factory.CreateDbContextAsync();
        if (await db.Categories.AnyAsync(x => x.Name == normalized))
            throw new InvalidOperationException("اسم القسم مستخدم بالفعل.");
        var category = new Category { Name = normalized };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(int id, string name)
    {
        var normalized = NormalizeName(name);
        await using var db = await factory.CreateDbContextAsync();
        var category = await db.Categories.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("القسم غير موجود.");
        if (await db.Categories.AnyAsync(x => x.Id != id && x.Name == normalized))
            throw new InvalidOperationException("اسم القسم مستخدم بالفعل.");
        category.Name = normalized;
        await db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        var category = await db.Categories.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("القسم غير موجود.");
        if (await db.Products.AnyAsync(x => x.CategoryId == id))
            throw new InvalidOperationException("لا يمكن حذف قسم مرتبط بأصناف.");
        db.Categories.Remove(category);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Unit>> GetUnitsAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Units.OrderBy(x => x.Name).AsNoTracking().ToListAsync();
    }

    public async Task<Unit> AddUnitAsync(string name)
    {
        var normalized = NormalizeName(name);
        await using var db = await factory.CreateDbContextAsync();
        if (await db.Units.AnyAsync(x => x.Name == normalized))
            throw new InvalidOperationException("اسم الوحدة مستخدم بالفعل.");
        var unit = new Unit { Name = normalized };
        db.Units.Add(unit);
        await db.SaveChangesAsync();
        return unit;
    }

    public async Task<Unit> UpdateUnitAsync(int id, string name)
    {
        var normalized = NormalizeName(name);
        await using var db = await factory.CreateDbContextAsync();
        var unit = await db.Units.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("الوحدة غير موجودة.");
        if (await db.Units.AnyAsync(x => x.Id != id && x.Name == normalized))
            throw new InvalidOperationException("اسم الوحدة مستخدم بالفعل.");
        unit.Name = normalized;
        await db.SaveChangesAsync();
        return unit;
    }

    public async Task DeleteUnitAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        var unit = await db.Units.SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException("الوحدة غير موجودة.");
        if (await db.Products.AnyAsync(x => x.UnitId == id))
            throw new InvalidOperationException("لا يمكن حذف وحدة مرتبطة بأصناف.");
        db.Units.Remove(unit);
        await db.SaveChangesAsync();
    }

    private static string NormalizeName(string name)
    {
        var normalized = name?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("الاسم مطلوب.", nameof(name));
        if (normalized.Length > 150)
            throw new ArgumentException("الاسم أطول من الحد المسموح.", nameof(name));
        return normalized;
    }
}