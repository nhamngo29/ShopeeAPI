﻿using Microsoft.EntityFrameworkCore.Storage;
using Shopee.Application.Common.Exceptions;
using Shopee.Domain.Interfaces.Repositories;
using Shopee.Domain.Interfaces.UnitOfWork;
using Shopee.Infrastructure.Repository;

namespace Shopee.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
        private bool _disposed;
        private readonly ApplicationDbContext _context;

        #region Repositories

        private Lazy<IProductRepository> _products;
        public IProductRepository Products => _products.Value;

        private Lazy<ICategoryRepository> _categories;
        public ICategoryRepository Categories => _categories.Value;

        private Lazy<IImageProductRepository> _imageProducts;
        public IImageProductRepository ImageProducts => _imageProducts.Value;

        private Lazy<ICartRepository> _cart;
        public ICartRepository Cart => _cart.Value;

        private Lazy<ICartItemRepository> _cartItem;
        public ICartItemRepository CartItem => _cartItem.Value;
        private Lazy<IUserRepository> _user;
        public IUserRepository User => _user.Value;

        private Lazy<IOrderRepository> _orders;
        public IOrderRepository Orders => _orders.Value;

        private Lazy<IOrderItemRepository> _orderItems;
        public IOrderItemRepository OrderItems=> _orderItems.Value;

        #endregion Repositories

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _context = (ApplicationDbContext)dbContext;
            _products = new Lazy<IProductRepository>(() => new ProductRepository(_context));
            _categories = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
            _imageProducts = new Lazy<IImageProductRepository>(() => new ImageProductRepository(_context));
            _cart = new Lazy<ICartRepository>(() => new CartRepository(_context));
            _cartItem = new Lazy<ICartItemRepository>(() => new CartItemRepository(_context));
            _user=new Lazy<IUserRepository>(() => new UserRepository(_context));
            _orders=new Lazy<IOrderRepository>(() => new OrderRepository(_context));
            _orderItems=new Lazy<IOrderItemRepository>(()=>new OrderItemRepository(_context));
        }

        // save changes
        public int SaveChanges() => _context.SaveChanges();

        public async Task<int> SaveChangesAsync(CancellationToken token) => await _context.SaveChangesAsync(token);

        // transaction
        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        // commit
        public void Commit()
        {
            if (_transaction == null)
            {
                throw TransactionException.TransactionNotCommitException();
            }
            try
            {
                _context.SaveChanges();
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task CommitAsync(CancellationToken token)
        {
            if (_transaction == null)
            {
                throw TransactionException.TransactionNotCommitException();
            }

            try
            {
                await _context.SaveChangesAsync(token);
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        // rollback
        public void Rollback()
        {
            if (_transaction == null)
            {
                throw TransactionException.TransactionNotCommitException();
            }

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw TransactionException.TransactionNotCommitException();
            }

            await _transaction.RollbackAsync();
            _transaction.Dispose();
            _transaction = null;
        }
        // dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // execute transaction
        public async Task ExecuteTransactionAsync(Action action, CancellationToken token)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                action();
                await _context.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                throw TransactionException.TransactionNotExecuteException(ex);
            }
        }

        public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken token)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await action();
                await _context.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(token);
                throw TransactionException.TransactionNotExecuteException(ex);
            }
        }
    }
}