﻿namespace Bulky.Data.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }

    IProductRepository ProductRepository { get; }

    void Save();
}