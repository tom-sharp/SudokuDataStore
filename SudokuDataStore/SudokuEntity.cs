using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.DataStore
{
	public class SudokuEntity
	{
		[Key]
		public string PuzzleId { get; set; }        // each puzzle stored, should be unik and work as id

		public int Clues { get; set; }

		public int Category { get; set; }  // level 1-5 ( super easy, easy, medium, hard, super hard )

		public int Validated { get; set; }  // 0 = not validated, 1 = validated, -1 = Invalid / usolvable / multi-solution

		public int Difficulty { get; set; }	// backtrack counter - indicate difficulty solvabel through logic if 0

	}
}

