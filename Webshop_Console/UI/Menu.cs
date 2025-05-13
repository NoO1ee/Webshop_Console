using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Visual;

namespace Webshop_Console.UI;

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
    int _boxHeight = 6;
    int _defaultMaxItemsPerRow = 4;
    int _minItemsPerRow = 2;
    Action? _headerRenderer;

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

    public void SetHeader(Action header) => _headerRenderer = header;
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
    
    public static int CalculateBoxWidthFor(int itemsPerRow)
    {
        int totalSpace = Console.WindowWidth - itemsPerRow * 2 - 4;
        int w = totalSpace / itemsPerRow;
        return Math.Max(16, w);
    }

    public static void DrawBoxStatic(int x, int y, int boxW, string lines, bool sel)
    {
        var m = new Menu("");
        m._boxHeight = 6;
        m._normalColor = ConsoleColor.White;
        m._highlightColor = ConsoleColor.Yellow;
        m.DrawBox(x, y, boxW, lines, sel);
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
        int totalSpaceForBoxes = _windowWidth - itemsPerRow * 2 - 4;
        int calculatedWidth = totalSpaceForBoxes / itemsPerRow;

        return Math.Max(_minBoxWidth, calculatedWidth);
    }

    void DrawBox(int x, int y, int boxWidth, string text, bool isSelected)
    {
        var lines = text.Split('\n');

        var borderCol = isSelected ? _highlightColor : _normalColor;
        var textCol = borderCol;

        int oldX = Console.CursorLeft, oldY = Console.CursorTop;

        Console.ForegroundColor = borderCol;
        Console.SetCursorPosition(x, y);
        Console.Write('┌' + new string('─', boxWidth - 2) + '┐');

        for (int i = 0; i < _boxHeight - 2; i++)
        {
            Console.SetCursorPosition(x, y + 1 + i);
            Console.Write('│' + new string(' ', boxWidth - 2) + '│');
        }

        Console.SetCursorPosition(x, y + _boxHeight - 1);
        Console.Write('└' + new string('─', boxWidth - 2) + '┘');

        for (int i = 0; i < lines.Length && i < _boxHeight - 2; i++)
        {
            var line = lines[i].Length > boxWidth - 4 ? lines[i].Substring(0, boxWidth - 7) + "..." : lines[i];
            int tx = x + 1 + (boxWidth - 2 - line.Length) / 2;
            int ty = y + 1 + i;

            Console.SetCursorPosition(tx, ty);
            Console.ForegroundColor = textCol;
            Console.Write(line);
        }

        Console.SetCursorPosition(oldX, oldY);
    }

    void HandleInput()
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

        int maxItemsPerRow = CalculateItemsPerRow();
        int totalRows = (int)Math.Ceiling((double)_options.Count / maxItemsPerRow);
        int itemsInLastRow = _options.Count % maxItemsPerRow;
        if (itemsInLastRow == 0 && _options.Count > 0)
            itemsInLastRow = maxItemsPerRow;

        int maxColoumsInCurrentRow = _selectedRow == totalRows - 1 ? itemsInLastRow : maxItemsPerRow;

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
                    _selectedColumn = Math.Min(_selectedColumn, (_selectedRow == totalRows - 1 ? itemsInLastRow : maxItemsPerRow) - 1);
                }
                break;

            case ConsoleKey.LeftArrow:

                if(_selectedColumn > 0)
                    _selectedColumn--;
                else if (_selectedRow > 0)
                {
                    _selectedRow--;
                    int prevRowMaxColumns = _selectedRow == totalRows - 1 ? itemsInLastRow : maxItemsPerRow;
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
        _windowWidth = Console.WindowWidth;
        Console.Clear();
        _headerRenderer?.Invoke();
        Console.ForegroundColor = _titleColor;

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
            int itemsInCurrentRow = Math.Min(maxItemsPerRow, _options.Count - i * maxItemsPerRow);
            int totalRowWidth = itemsInCurrentRow * (boxWidth + 2);
            int startX = (_windowWidth - totalRowWidth) / 2;
            
            startX = Math.Max(0, startX);

            for (int y = 0; y < itemsInCurrentRow; y++)
            {
                int index = i * maxItemsPerRow + y;
                int boxX = startX + y * (boxWidth + 2);

                bool isSelected = i == _selectedRow && y == _selectedColumn;

                DrawBox(boxX, startY + i * (_boxHeight + 1), boxWidth, _options[index].Text, isSelected);
            }

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, startY + totalRow * (_boxHeight + 1) + 1);
            Console.ForegroundColor = _normalColor;
            Console.WriteLine($"Använd pilarna för att navigera | {Color.G("Enter")} gå vidare | {Color.R("Esc")} för att lämna");

           
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
                Console.WriteLine(e);
                Thread.Sleep(1000);
            }
        }
    }


    public static async Task ShowMenu(string consoleTitle, string menuTitle, (string optionText, Action action)[] options, ConsoleColor titleColor = ConsoleColor.DarkYellow, ConsoleColor textColor = ConsoleColor.White, ConsoleColor selectedColor = ConsoleColor.Red, int? minBoxWidth = null, int? boxHeight = null)
    {
        Console.Title = consoleTitle;

        var menu = new Menu(menuTitle);

        if(minBoxWidth.HasValue && boxHeight.HasValue)
            menu.SetBoxDimensions(minBoxWidth.Value, boxHeight.Value);

        foreach (var (text, action) in options)
            menu.AddOption(text, action);

        menu.SetColors(titleColor, textColor, selectedColor);
        menu.Display();
    }
}
