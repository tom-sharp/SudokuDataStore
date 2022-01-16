using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Sudoku.Puzzle;
using Syslib;
using Syslib.Db.Csv;

namespace Sudoku.DataStore
{
	public class SudokuDb : ISudokuDb {

		SudokuDbContext db = null;

		public SudokuDb(SudokuDbContext ctx) {
			this.db = ctx;
		}

		public async Task<bool> AddAsync(SudokuEntity puzzle) {
			if ((puzzle == null) || (puzzle.PuzzleId == null)) return false;
			var pz = new SudokuPuzzle(puzzle.PuzzleId);
			var pzstr = pz.GetPuzzle();			// uniformed puzzle string
			var p = await db.SudokuPuzzles.AsNoTracking().FirstOrDefaultAsync(p1 => p1.PuzzleId == pzstr);
			if (p == null) {
				puzzle.PuzzleId = pzstr;
				puzzle.Clues = pz.GetNumberCount();
				db.SudokuPuzzles.Add(puzzle);
				return true;
			}
			return false;
		}

		public async Task<bool> AddAsync(string puzzle) {
			if (puzzle == null) return false;
			var pz = new SudokuPuzzle(puzzle);
			var pzstr = pz.GetPuzzle();         // uniformed puzzle string
			var p = await db.SudokuPuzzles.AsNoTracking().FirstOrDefaultAsync(p => p.PuzzleId == pzstr);
			if (p == null) {
				p = new SudokuEntity() { PuzzleId = pzstr, Clues = pz.GetNumberCount() };
				db.SudokuPuzzles.Add(p);
				return true;
			}
			return false;
		}

		public async Task<bool> RemoveAsync(SudokuEntity puzzle) {
			if ((puzzle == null) || (puzzle.PuzzleId == null)) return false;
			var pz = new SudokuPuzzle(puzzle.PuzzleId);
			var pzstr = pz.GetPuzzle();         // uniformed puzzle string
			var p = await db.SudokuPuzzles.FirstOrDefaultAsync(p => p.PuzzleId == pzstr);
			if (p != null) {
				db.SudokuPuzzles.Remove(p);
				return true;
			}
			return false;
		}

		public async Task<bool> RemoveAsync(string puzzle) {
			if (puzzle == null) return false;
			var pz = new SudokuPuzzle(puzzle);
			var pzstr = pz.GetPuzzle();         // uniformed puzzle string
			var p = await db.SudokuPuzzles.FirstOrDefaultAsync(p => p.PuzzleId == pzstr);
			if (p != null) {
				db.SudokuPuzzles.Remove(p);
				return true;
			}
			return false;
		}

		public async Task<bool> UpdateAsync(SudokuEntity puzzle) {
			if ((puzzle == null) || puzzle.PuzzleId == null) return false;

			var pz = new SudokuPuzzle(puzzle.PuzzleId);
			var pzstr = pz.GetPuzzle();         // uniformed puzzle string in db
			var p = await db.SudokuPuzzles.AsNoTracking().FirstOrDefaultAsync(p => p.PuzzleId == pzstr);
			if (p != null) {
				puzzle.PuzzleId = pzstr;
				puzzle.Clues = pz.GetNumberCount();
				if (puzzle.Validated == 0) {
					// puzzle is not validated - check if it is in db and restore that validated data
					if (p.Validated != 0) {
						puzzle.Validated = p.Validated;
						puzzle.Difficulty = p.Difficulty;
					}
				}
				db.SudokuPuzzles.Update(puzzle);
				return true;
			}
			return false;
		}

		public async Task<SudokuEntity> GetPuzzleAsync(string puzzle) {
			if (puzzle == null) return null;
			var pz = new SudokuPuzzle(puzzle);
			var pzstr = pz.GetPuzzle();         // uniformed puzzle string in db
			return await db.SudokuPuzzles.AsNoTracking().FirstOrDefaultAsync(p => p.PuzzleId == pzstr);
		}

		public async Task<SudokuEntity> GetRandomPuzzleAsync(int category = 0) {
			List<SudokuEntity> puzzles = null;
			if (category == 0) puzzles = await db.SudokuPuzzles.AsNoTracking().ToListAsync();
			else puzzles = await db.SudokuPuzzles.AsNoTracking().Where(p => p.Category == category).ToListAsync();
			if (puzzles != null) {
				var cnt = puzzles.Count();
				if (cnt > 0) return puzzles.ElementAtOrDefault(new CRandom().RandomNumber(0, cnt - 1));
			}
			return null;
		}

		public async Task<IEnumerable<SudokuEntity>> GetPuzzlesAsync(int category = 0) {
			if (category == 0) return await db.SudokuPuzzles.AsNoTracking().ToListAsync();
			return await db.SudokuPuzzles.AsNoTracking().Where(p => p.Category == category).ToListAsync();
		}

		public async Task<int> Import(string filename) {
			if (!IsValid.FileName(filename)) return 0;
			CCsvDb csvdb = new CCsvDb();
			if ((csvdb.Load(filename) && (csvdb.IsMapped("puzzle")))) return await this.ImportCsv(csvdb);
			return 0;
		}

		public async Task<int> Export(string filename) {
			int count = 0;
			CCsvDb csvdb = new CCsvDb("Puzzle,Clues,Category,Difficulty,Validated");

			foreach (var p in await this.GetPuzzlesAsync()) {
				if (p.Validated == 1) csvdb.AddRecord($"{p.PuzzleId},{p.Clues},{p.Category},{p.Difficulty},Valid");
				else if (p.Validated == -1) csvdb.AddRecord($"{p.PuzzleId},{p.Clues},{p.Category},{p.Difficulty},Invalid");
				else csvdb.AddRecord($"{p.PuzzleId},{p.Clues},{p.Category},{p.Difficulty},");
				count++;
			}

			if (csvdb.Save(filename)) return count;

			return 0;
		}

		// Import Puzzles from CSV file and return number of added or updated puzzles
		async Task<int> ImportCsv(CCsvDb csv) {
			if (csv == null) return 0;
			int temp, counter = 0, failed = 0;
			string value;
			foreach (var r in csv) {

				var puzzle = new SudokuEntity();
				puzzle.PuzzleId = r.GetValue("puzzle");

				if (puzzle.PuzzleId != null) {

					value = r.GetValue("clues");
					if ((value != null) && (Int32.TryParse(value, out temp))) puzzle.Clues = temp;
					else puzzle.Clues = 0;

					value = r.GetValue("category");
					if ((value != null) && (Int32.TryParse(value, out temp))) puzzle.Category = temp;
					else puzzle.Category = 0;

					value = r.GetValue("difficulty");
					if ((value != null) && (Int32.TryParse(value, out temp))) puzzle.Difficulty = temp;
					else puzzle.Difficulty = 0;

					value = r.GetValue("validated");
					if (value == null) puzzle.Validated = 0;
					else if (value.ToLower() == "valid") puzzle.Validated = 1;
					else puzzle.Validated = -1;

					if (await this.AddAsync(puzzle)) counter++;
					else if (await this.UpdateAsync(puzzle)) counter++;
					else failed++;

				}
			}
			await this.SaveChangesAsync();
			return counter;
		}



		public async Task<int> SaveChangesAsync() {
			// ADD ERROR HANDLE FOR UPDATE ERROR
			return await this.db.SaveChangesAsync();
		}

		public void CreateDB() {
			this.db.Database.Migrate();
		}

		public void DeleteDB() {
			this.db.Database.EnsureDeleted();
		}
	}

}
