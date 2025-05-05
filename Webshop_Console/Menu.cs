using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Visual;

namespace Webshop_Console;

internal class Menu
{
    string _title;
    List<MenuOption> _options;
    bool _isRunning;
    int _selectedRow;
    int _selectedColumn;
    ConsoleColor _titleColor;
    ConsoleColor _normalColor;
    ConsoleColor _highlightColor;
    int _windowWidth;
    int _minBoxWidth = 16;
    int _boxHeight = 3;
    int _defaultMaxItemsPerRow = 4;
    int _minItemsPerRow = 2;

    public Menu(string title)
    {
        _title = title;
        _options = new List<MenuOption>();
        _isRunning = false;
        _selectedRow = 0;
        _selectedColumn = 0;
        _titleColor = ConsoleColor.Red;
        _normalColor = ConsoleColor.White;
        _highlightColor = ConsoleColor.Yellow;
    }

    public void AddOption(string text, Action action) => _options.Add(new MenuOption(text, action));

    public void SetColors(ConsoleColor titleColor, ConsoleColor normalColor, ConsoleColor highlightColor)
    {
        _titleColor = titleColor;
        _normalColor = normalColor;
        _highlightColor = highlightColor;
    }

    public void SetBoxDimensions(int minWidth, int height)
    {
        _minBoxWidth = Math.Max(10, minWidth);
        _boxHeight = Math.Max(3, height);
    }

    int CalculateItemsPerRow()
    {
        int spaceNeededPerItem = _minBoxWidth + 2;
        int maxPossibleItems = Math.Max(1, _windowWidth /  spaceNeededPerItem);

        int itemsPerRow = Math.Min(maxPossibleItems, _defaultMaxItemsPerRow);
        itemsPerRow = Math.Max(_minItemsPerRow, itemsPerRow);

        if (_windowWidth < _minBoxWidth + 4)
            itemsPerRow = 1;

        return itemsPerRow;
    }

    int CalculateBoxWidth(int itemsPerRow)
    {
        int totalSpaceForBoxes = _windowWidth - (itemsPerRow * 2) - 4;
        int calculatedWidth = totalSpaceForBoxes / itemsPerRow;

        return Math.Max(_minBoxWidth, calculatedWidth);
    }

    void DrawBox(int x, int y, int boxWidth, string text, bool isSelected)
    {
        if (x < 0 || y < 0 || x + boxWidth >= _windowWidth)
            return;

        ConsoleColor borderColor = isSelected ? _highlightColor : _normalColor;
        ConsoleColor textColor = isSelected ? _highlightColor : _normalColor;

        try
        {
            // SPARA POSITION
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = borderColor;
            Console.Write($"┌{new string('─', boxWidth - 2)}┐");

            for (int i = 1; i < _boxHeight - 1; i++)
            {
                Console.SetCursorPosition(x, y + 1);
                Console.Write("|");
                Console.SetCursorPosition(x + boxWidth - 1, y + 1);
                Console.Write("|");
            }

            Console.SetCursorPosition(x, y + _boxHeight - 1);
            Console.Write($"└{new string('─', boxWidth - 2)}┘");

            //Om texten är längre än själva lådan.
            if (text.Length > boxWidth - 4)
                text = text.Substring(0, boxWidth - 7) + "...";

            int textX = x + (boxWidth - text.Length) / 2;
            int textY = y + (_boxHeight / 2);

            Console.SetCursorPosition(textX, textY);
            Console.ForegroundColor = textColor;
            Console.Write(text);

            Console.SetCursorPosition(oldX, oldY);

        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Error");
        }
    }

    void HandleInput()
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        int maxItemsPerRow = CalculateItemsPerRow();
        int totalRows = (int)Math.Ceiling((double)_options.Count / maxItemsPerRow);
        int itemsInLastRow = _options.Count % maxItemsPerRow;
        if (itemsInLastRow == 0 && _options.Count > 0)
            itemsInLastRow = maxItemsPerRow;

        int maxColoumsInCurrentRow = (_selectedRow == totalRows - 1) ? itemsInLastRow : maxItemsPerRow;

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:

                if (_selectedRow > 0)
                {
                    _selectedRow--;
                    _selectedColumn = Math.Min(_selectedColumn, maxItemsPerRow - 1);
                }
                break;

            case ConsoleKey.DownArrow:

                if (_selectedRow < totalRows - 1)
                {
                    _selectedRow++;
                    _selectedColumn = Math.Min(_selectedColumn, ((_selectedRow == totalRows - 1) ? itemsInLastRow : maxItemsPerRow) - 1);
                }
                break;

            case ConsoleKey.LeftArrow:

                if(_selectedColumn > 0)
                    _selectedColumn--;
                else if (_selectedRow > 0)
                {
                    _selectedRow--;
                    int prevRowMaxColumns = (_selectedRow == totalRows - 1) ? itemsInLastRow : maxItemsPerRow;
                    _selectedColumn = prevRowMaxColumns;
                }
                break;

            case ConsoleKey.RightArrow:

                if (_selectedColumn < maxColoumsInCurrentRow - 1)
                    _selectedColumn++;
                else if(_selectedRow < totalRows - 1)
                {
                    _selectedRow++;
                    _selectedColumn = 0;
                }
                break;

            case ConsoleKey.Enter:

                        int selectedIndex = _selectedRow * maxItemsPerRow + _selectedColumn;
                        if (selectedIndex >= 0 && selectedIndex < _options.Count)
                            _options[selectedIndex].Action?.Invoke();
                        break;

                    case ConsoleKey.Escape:
                        _isRunning = false;
                        break;
                    }
    }

    public void Close()
    {
        _isRunning = false;
    }

    public void DrawMenu()
    {
        int maxItemsPerRow = CalculateItemsPerRow();
        Console.ForegroundColor = _titleColor;

        string titleText = $" {_title} ";
        int titleWidth = Math.Min(_windowWidth - 4, titleText.Length + 4);

        int titlePosition = (_windowWidth - titleWidth) / 2;
        titlePosition = Math.Max(0, titlePosition);

        Console.SetCursorPosition(titlePosition, 1);

        Console.Write($"╔{new string('═', titleWidth - 2)}╗");
        Console.SetCursorPosition(titlePosition, 2);

        int textPadding = (titleWidth - titleText.Length) / 2;
        Console.Write($"║{new string(' ', textPadding - 1)}{titleText}{new string(' ', titleWidth - 1 - textPadding - titleText.Length)}║");

        Console.SetCursorPosition(titlePosition, 3);
        Console.Write($"╚{new string('═', titleWidth - 2)}╝");

        Console.WriteLine();
        Console.WriteLine();

        int totalRow = (int)Math.Ceiling((double)_options.Count / maxItemsPerRow);
        int startY = 6;

        int boxWidth = CalculateBoxWidth(maxItemsPerRow);
        
        for (int i = 0; i < totalRow; i++)
        {
            int itemsInCurrentRow = Math.Min(maxItemsPerRow, _options.Count - (i * maxItemsPerRow));
            int totalRowWidth = itemsInCurrentRow * (boxWidth + 2);
            int startX = (_windowWidth - totalRowWidth) / 2;
            
            startX = Math.Max(0, startX);

            for (int y = 0; y < itemsInCurrentRow; y++)
            {
                int index = i * maxItemsPerRow + y;
                int boxX = startX + y * (boxWidth + 2);

                bool isSelected = (i == _selectedRow && y == _selectedColumn);

                DrawBox(boxX, startY + (i * (_boxHeight + 1)), boxWidth, _options[index].Text, isSelected);
            }

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, startY + (totalRow * (_boxHeight + 1)) + 1);
            Console.ForegroundColor = _normalColor;
            Console.WriteLine($"Use arrow keys to navigate | {Color.G("Enter")} to select | {Color.R("Esc")} to exit");

           
        }
    }

    public void Display()
    {
        _isRunning = true;
        _selectedRow = 0;
        _selectedColumn = 0;

        while (_isRunning)
        {
            try
            {
                _windowWidth = Console.WindowWidth;
                Console.Clear();
                DrawMenu();
                HandleInput();
            }
            catch (Exception e)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
