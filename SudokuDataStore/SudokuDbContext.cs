using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Sudoku.DataStore
{
	public class SudokuDbContext : DbContext {

		public SudokuDbContext(DbContextOptions<SudokuDbContext> ctxoption) : base(ctxoption) {
		}

		//protected override void OnModelCreating(ModelBuilder builder) {
		//	base.OnModelCreating(builder);
		//	builder.Entity<SudokuEntity>().Property(p => p.PuzzleId).ValueGeneratedOnAdd();
		//}


		public DbSet<SudokuEntity> SudokuPuzzles { get; set; }
	}
}
