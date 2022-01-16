using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.DataStore
{
	public interface ISudokuDb {

		public Task<bool> AddAsync(SudokuEntity puzzle);

		public Task<bool> AddAsync(string puzzle);

		public Task<bool> RemoveAsync(SudokuEntity puzzle);

		public Task<bool> RemoveAsync(string puzzle);

		public Task<bool> UpdateAsync(SudokuEntity puzzle);

		public Task<SudokuEntity> GetPuzzleAsync(string puzzle);

		public Task<SudokuEntity> GetRandomPuzzleAsync(int level = 0);

		public Task<IEnumerable<SudokuEntity>> GetPuzzlesAsync(int level);

		public Task<int> Import(string filename);

		public Task<int> Export(string filename);

		public Task<int> SaveChangesAsync();
	}

}

