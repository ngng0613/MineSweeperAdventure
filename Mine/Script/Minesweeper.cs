using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minesweeper : MonoBehaviour
{
    [SerializeField]
    private int _rows = 1;

    [SerializeField]
    private int _columns = 1;

    [SerializeField]
    private int MineCount = 1;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private Cell _cellPrefab = null;

    Cell[,] _cells;

    void Start()
    {
        var parent = _gridLayoutGroup.gameObject.transform;
        if (_columns < _rows)
        {
            _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayoutGroup.constraintCount = _columns;
        }
        else
        {
            _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _gridLayoutGroup.constraintCount = _rows;
        }

        _cells = new Cell[_rows, _columns];
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                _cells[r, c] = cell;
            }
        }

        for (var i = 0; i < MineCount; i++)
        {
            var r = Random.Range(0, _rows);
            var c = Random.Range(0, _columns);
            var cell = _cells[r, c];
            cell.CellState = CellState.Mine;
        }
    }

    /*
    private int GetMineCount(int r, int c)
    {
        var count = 0;
        var top = r - 1;
        var bottom = r + 1;
        var left = c - 1;
        var right = c + 1;
​
	if (top >= 0)
        {
            if (left >= 0 && _cells[top, left].IsMine) { count++; }
            if (_cells[top, c].IsMine) { count++; }
            if (right < Columns && _cells[top, right].IsMine) { count++; }
        }
        if (left >= 0 && _cells[r, left].IsMine) { count++; }
        if (right < Columns && _cells[r, right].IsMine) { count++; }
        if (bottom < Rows)
        {
            if (left >= 0 && _cells[bottom, left].IsMine) { count++; }
            if (_cells[bottom, c].IsMine) { count++; }
            if (right < Columns && _cells[bottom, right].IsMine) { count++; }
        }
​
	return count;
    }
    */
}