using Assignment.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.DataBase
{
    public class BarcodeDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public BarcodeDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<BarCode>().Wait();
        }

        public Task<List<BarCode>> GetBarCodesAsync()
        {
            return _database.Table<BarCode>().ToListAsync();
        }

        public Task<BarCode?> GetBarCodeAsync(int id)
        {
            return _database.Table<BarCode>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveBarCodeAsync(BarCode barCode)
        {
            if (barCode.Id != 0)
                return _database.UpdateAsync(barCode);
            else
                return _database.InsertAsync(barCode);
        }

        public Task<int> DeleteBarCodeAsync(BarCode barCode)
        {
            return _database.DeleteAsync(barCode);
        }

        public Task<bool> IsCodeExistsAsync(string code)
        {
            return _database.Table<BarCode>()
                            .Where(b => b.Code == code)
                            .FirstOrDefaultAsync()
                            .ContinueWith(task => task.Result != null);
        }

        public Task<int> SaveBarCodesAsync(List<BarCode> barCodes)
        {
            return _database.InsertAllAsync(barCodes);
        }

    }
}
