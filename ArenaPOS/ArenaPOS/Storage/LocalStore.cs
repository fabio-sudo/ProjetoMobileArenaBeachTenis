using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using ArenaPOS.Models;
using Tab = ArenaPOS.Models.Tab;

namespace ArenaPOS.Storage
{
    public class LocalStore
    {
        private SQLiteAsyncConnection _db;

        public async Task Init()
        {
            if (_db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "ArenaPOSData.db");
            _db = new SQLiteAsyncConnection(databasePath);

            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Product>();
            await _db.CreateTableAsync<Sale>();
            await _db.CreateTableAsync<SaleItem>();
            await _db.CreateTableAsync<Tab>();
            await _db.CreateTableAsync<TabItem>();
            await _db.CreateTableAsync<Reservation>();
            await _db.CreateTableAsync<CashRegister>();
            await _db.CreateTableAsync<Payment>();
        }

        public SQLiteAsyncConnection Connection => _db;
    }
}
