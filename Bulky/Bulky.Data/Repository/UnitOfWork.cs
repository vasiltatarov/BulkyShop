﻿namespace Bulky.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.CategoryRepository = new CategoryRepository(dbContext);
        this.ProductRepository = new ProductRepository(dbContext);
        this.CompanyRepository = new CompanyRepository(dbContext);
    }

    public ICategoryRepository CategoryRepository { get; private set; }

    public IProductRepository ProductRepository { get; private set; }

    public ICompanyRepository CompanyRepository { get; private set; }

    public void Save()
    {
        this.dbContext.SaveChanges();
    }
}
