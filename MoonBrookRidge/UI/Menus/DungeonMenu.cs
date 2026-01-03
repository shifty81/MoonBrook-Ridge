using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Dungeons;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Menu for displaying dungeon map, current floor, and room navigation
/// </summary>
public class DungeonMenu
{
    private SpriteFont _font;
    private Texture2D _pixelTexture;
    private bool _isOpen;
    private int _selectedRoomIndex;
    private DungeonSystem _dungeonSystem;
    
    // Colors
    private readonly Color _bgColor = new Color(20, 20, 30, 240);
    private readonly Color _headerColor = new Color(80, 40, 120);
    private readonly Color _textColor = Color.White;
    private readonly Color _highlightColor = new Color(200, 150, 255);
    private readonly Color _roomClearedColor = new Color(50, 150, 50);
    private readonly Color _roomActiveColor = new Color(200, 150, 50);
    private readonly Color _roomLockedColor = new Color(100, 100, 100);
    
    // Layout constants
    private const int PADDING = 20;
    private const int ROOM_SIZE = 40;
    private const int ROOM_SPACING = 10;
    
    public bool IsOpen => _isOpen;
    
    public DungeonMenu(SpriteFont font, Texture2D pixelTexture, DungeonSystem dungeonSystem)
    {
        _font = font;
        _pixelTexture = pixelTexture;
        _dungeonSystem = dungeonSystem;
        _isOpen = false;
        _selectedRoomIndex = 0;
    }
    
    public void Toggle()
    {
        _isOpen = !_isOpen;
        if (_isOpen)
        {
            _selectedRoomIndex = 0;
        }
    }
    
    public void Close()
    {
        _isOpen = false;
    }
    
    public void Update(KeyboardState keyboardState, KeyboardState previousKeyboardState, 
                      MouseState mouseState, MouseState previousMouseState)
    {
        if (!_isOpen) return;
        
        var dungeon = _dungeonSystem.ActiveDungeon;
        if (dungeon == null) return;
        
        var currentFloorRooms = dungeon.GetCurrentFloor();
        if (currentFloorRooms.Count == 0) return;
        
        // Keyboard navigation
        if (keyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right))
        {
            _selectedRoomIndex = Math.Min(_selectedRoomIndex + 1, currentFloorRooms.Count - 1);
        }
        else if (keyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left))
        {
            _selectedRoomIndex = Math.Max(_selectedRoomIndex - 1, 0);
        }
        
        // Close menu with ESC
        if (keyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
        {
            Close();
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!_isOpen) return;
        
        var dungeon = _dungeonSystem.ActiveDungeon;
        if (dungeon == null) return;
        
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        // Menu dimensions
        int menuWidth = 600;
        int menuHeight = 500;
        int menuX = (screenWidth - menuWidth) / 2;
        int menuY = (screenHeight - menuHeight) / 2;
        
        // Background
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(menuX, menuY, menuWidth, menuHeight), 
            _bgColor);
        
        // Header
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(menuX, menuY, menuWidth, 50), 
            _headerColor);
        
        string dungeonName = GetDungeonName(dungeon.Type);
        string headerText = $"{dungeonName} - Floor {dungeon.CurrentFloor}/{dungeon.TotalFloors}";
        Vector2 headerSize = _font.MeasureString(headerText);
        spriteBatch.DrawString(_font, headerText, 
            new Vector2(menuX + menuWidth / 2 - headerSize.X / 2, menuY + 15), 
            _textColor);
        
        // Dungeon info
        int yPos = menuY + 70;
        spriteBatch.DrawString(_font, $"Difficulty: {dungeon.Difficulty}", 
            new Vector2(menuX + PADDING, yPos), _textColor);
        
        // Progress indicator
        int totalRooms = 0;
        int clearedRooms = 0;
        foreach (var room in dungeon.GetCurrentFloor())
        {
            if (room.Type == RoomType.Combat || room.Type == RoomType.Boss)
            {
                totalRooms++;
                if (room.IsCleared) clearedRooms++;
            }
        }
        
        yPos += 30;
        spriteBatch.DrawString(_font, $"Progress: {clearedRooms}/{totalRooms} rooms cleared", 
            new Vector2(menuX + PADDING, yPos), _textColor);
        
        // Floor map
        yPos += 50;
        DrawFloorMap(spriteBatch, dungeon, menuX + menuWidth / 2, yPos);
        
        // Room details for selected room
        var currentFloorRooms = dungeon.GetCurrentFloor();
        if (_selectedRoomIndex < currentFloorRooms.Count)
        {
            var selectedRoom = currentFloorRooms[_selectedRoomIndex];
            DrawRoomDetails(spriteBatch, selectedRoom, menuX, menuY + menuHeight - 120, menuWidth);
        }
        
        // Instructions
        string instructions = "Arrow Keys: Navigate | ESC: Close";
        Vector2 instructionsSize = _font.MeasureString(instructions);
        spriteBatch.DrawString(_font, instructions, 
            new Vector2(menuX + menuWidth / 2 - instructionsSize.X / 2, menuY + menuHeight - 30), 
            new Color(180, 180, 180));
    }
    
    private void DrawFloorMap(SpriteBatch spriteBatch, Dungeon dungeon, int centerX, int startY)
    {
        var rooms = dungeon.GetCurrentFloor();
        if (rooms.Count == 0) return;
        
        // Calculate total width needed
        int totalWidth = rooms.Count * (ROOM_SIZE + ROOM_SPACING) - ROOM_SPACING;
        int startX = centerX - totalWidth / 2;
        
        // Draw rooms horizontally
        for (int i = 0; i < rooms.Count; i++)
        {
            var room = rooms[i];
            int roomX = startX + i * (ROOM_SIZE + ROOM_SPACING);
            int roomY = startY;
            
            // Determine room color
            Color roomColor;
            if (room.IsCleared)
            {
                roomColor = _roomClearedColor;
            }
            else if (i == _selectedRoomIndex)
            {
                roomColor = _highlightColor;
            }
            else if (i > 0 && !rooms[i - 1].IsCleared)
            {
                roomColor = _roomLockedColor; // Locked until previous room cleared
            }
            else
            {
                roomColor = _roomActiveColor;
            }
            
            // Draw room box
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(roomX, roomY, ROOM_SIZE, ROOM_SIZE), 
                roomColor);
            
            // Draw room border
            DrawBorder(spriteBatch, roomX, roomY, ROOM_SIZE, ROOM_SIZE, 2, Color.Black);
            
            // Draw room icon/type
            string roomIcon = GetRoomIcon(room.Type);
            Vector2 iconSize = _font.MeasureString(roomIcon);
            spriteBatch.DrawString(_font, roomIcon, 
                new Vector2(roomX + ROOM_SIZE / 2 - iconSize.X / 2, roomY + ROOM_SIZE / 2 - iconSize.Y / 2), 
                Color.White);
            
            // Draw connection line to next room (except last room)
            if (i < rooms.Count - 1)
            {
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(roomX + ROOM_SIZE, roomY + ROOM_SIZE / 2 - 2, ROOM_SPACING, 4), 
                    Color.Gray);
            }
        }
    }
    
    private void DrawRoomDetails(SpriteBatch spriteBatch, DungeonRoom room, int x, int y, int width)
    {
        // Details box
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(x + PADDING, y, width - PADDING * 2, 100), 
            new Color(40, 40, 50, 200));
        
        int textX = x + PADDING + 10;
        int textY = y + 10;
        
        // Room type
        string roomName = GetRoomName(room.Type);
        spriteBatch.DrawString(_font, $"Room: {roomName}", 
            new Vector2(textX, textY), _highlightColor);
        
        textY += 25;
        
        // Room status
        string status = room.IsCleared ? "[X] Cleared" : "[!] Active";
        Color statusColor = room.IsCleared ? _roomClearedColor : Color.Orange;
        spriteBatch.DrawString(_font, status, 
            new Vector2(textX, textY), statusColor);
        
        textY += 25;
        
        // Room contents
        if (room.Type == RoomType.Combat || room.Type == RoomType.Boss)
        {
            int enemyCount = room.Enemies.Count;
            int aliveCount = room.Enemies.FindAll(e => !e.IsDead).Count;
            spriteBatch.DrawString(_font, $"Enemies: {aliveCount}/{enemyCount}", 
                new Vector2(textX, textY), _textColor);
        }
        else if (room.Type == RoomType.Treasure)
        {
            int unopenedChests = room.Chests.FindAll(c => !c.IsOpened).Count;
            spriteBatch.DrawString(_font, $"Chests: {unopenedChests} unopened", 
                new Vector2(textX, textY), _textColor);
        }
        else if (room.Type == RoomType.Exit)
        {
            spriteBatch.DrawString(_font, "Exit to next floor", 
                new Vector2(textX, textY), _textColor);
        }
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color)
    {
        // Top
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, thickness), color);
        // Bottom
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + height - thickness, width, thickness), color);
        // Left
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, thickness, height), color);
        // Right
        spriteBatch.Draw(_pixelTexture, new Rectangle(x + width - thickness, y, thickness, height), color);
    }
    
    private string GetDungeonName(DungeonType type)
    {
        return type switch
        {
            DungeonType.SlimeCave => "Slime Cave",
            DungeonType.SkeletonCrypt => "Skeleton Crypt",
            DungeonType.SpiderNest => "Spider Nest",
            DungeonType.GoblinWarrens => "Goblin Warrens",
            DungeonType.HauntedManor => "Haunted Manor",
            DungeonType.DragonLair => "Dragon Lair",
            DungeonType.DemonRealm => "Demon Realm",
            DungeonType.AncientRuins => "Ancient Ruins",
            _ => "Unknown Dungeon"
        };
    }
    
    private string GetRoomIcon(RoomType type)
    {
        return type switch
        {
            RoomType.Entrance => ">",
            RoomType.Combat => "!",
            RoomType.Treasure => "*",
            RoomType.Boss => "X",
            RoomType.Puzzle => "?",
            RoomType.Shop => "$",
            RoomType.Shrine => "+",
            RoomType.Exit => "v",
            _ => "#"
        };
    }
    
    private string GetRoomName(RoomType type)
    {
        return type switch
        {
            RoomType.Entrance => "Entrance",
            RoomType.Combat => "Combat Room",
            RoomType.Treasure => "Treasure Room",
            RoomType.Boss => "Boss Room",
            RoomType.Puzzle => "Puzzle Room",
            RoomType.Shop => "Shop",
            RoomType.Shrine => "Shrine",
            RoomType.Exit => "Exit/Stairs",
            _ => "Unknown"
        };
    }
}
